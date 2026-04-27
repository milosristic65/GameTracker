using GameTrackerWPF.Enums;
using GameTrackerWPF.Models;
using GameTrackerWPF.MVVM;
using GameTrackerWPF.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using Wpf.Ui.Input;
using WPFUI = Wpf.Ui.Controls;

namespace GameTrackerWPF.ViewModels
{
    class GamesViewModel : ViewModelBase
    {
        public ObservableCollection<Game> Games { get; set; }
        public RelayCommand AddGameCommand { get; }
        public RelayCommand<Guid> RemoveGameCommand { get; }
        public RelayCommand ToggleSortOrderCommand { get; }

        private SortingMethod _selectedSorting = SortingMethod.Added;
        public SortingMethod SelectedSorting
        {
            get => _selectedSorting;
            set { _selectedSorting = value; OnPropertyChanged(); ApplySorting(); }
        }

        private bool _isAscending = false;
        public bool IsAscending
        {
            get => _isAscending;
            set { _isAscending = value; OnPropertyChanged(); ApplySorting(); }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set { _isRunning = value; OnPropertyChanged(); }
        }

        private readonly DispatcherTimer _processTimer = new DispatcherTimer();
        private readonly DispatcherTimer _playtimeTimer = new DispatcherTimer();
        private readonly GameStorageService _storage = new GameStorageService();

        public GamesViewModel()
        {
            // Setting commands
            AddGameCommand = new RelayCommand(execute => AddGame());
            RemoveGameCommand = new RelayCommand<Guid>(id => RemoveGame(id));
            ToggleSortOrderCommand = new RelayCommand(execute => IsAscending = !IsAscending);

            // Loading games
            var savedGames = _storage.Load();
            Games = new ObservableCollection<Game>(savedGames);
            ApplySorting();

            // Running processes tick
            _processTimer.Interval = TimeSpan.FromSeconds(3);
            _processTimer.Tick += CheckRunningProcesses;
            _processTimer.Start();

            // Playtime tick
            _playtimeTimer.Interval = TimeSpan.FromMinutes(1);
            _playtimeTimer.Tick += IncrementPlaytime;
            _playtimeTimer.Start();
        }

        private void AddGame()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Game Path";
            openFileDialog.Filter = "Executable Files (*.exe)|*.exe";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Check if the game is already in the list
                if (Games.Any(game => game.Path == filePath))
                {
                    var removeGameModal = new WPFUI.MessageBox
                    {
                        Title = "Warning!",
                        Content = "The game is already on the list!\nAre you sure you want to add it anyway?",
                        PrimaryButtonText = "Yes",
                        CloseButtonText = "No"
                    };

                    var removeGameResult = removeGameModal.ShowDialogAsync().GetAwaiter().GetResult();
                    if (removeGameResult != WPFUI.MessageBoxResult.Primary)
                    {
                        return;
                    }
                }

                Games.Add(new Game
                {
                    Title = System.IO.Path.GetFileNameWithoutExtension(filePath),
                    Path = filePath,
                });

                _storage.Save(Games.ToList());
            }
        }

        private void RemoveGame(Guid id)
        {
            if (id == Guid.Empty)
            {
                new WPFUI.MessageBox { Title = "Error!", Content = "Unexpected error occurred!", CloseButtonText = "OK" }.ShowDialogAsync();
                return;
            }

            var gameToRemove = Games.FirstOrDefault(game => game.Id == id);
            if (gameToRemove == null)
            {
                new WPFUI.MessageBox { Title = "Error!", Content = "Unexpected error occurred!", CloseButtonText = "OK" }.ShowDialogAsync();
                return;
            }

            var removeGameModal = new WPFUI.MessageBox
            {
                Title = "Remove Game",
                Content = $"Are you sure you want to remove {gameToRemove.Title}?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            var removeGameResult = removeGameModal.ShowDialogAsync().GetAwaiter().GetResult();
            if (removeGameResult == WPFUI.MessageBoxResult.Primary)
            {
                // Delete the cover if added
                string coverPath = gameToRemove.CoverPath;
                if (!string.IsNullOrEmpty(coverPath) && System.IO.File.Exists(coverPath))
                {
                    try
                    {
                        gameToRemove.CoverPath = "";
                        System.IO.File.Delete(coverPath);
                    }
                    catch (Exception ex)
                    {
                        new WPFUI.MessageBox { Title = "Error!", Content = $"Failed to delete cover image: {ex.Message}", CloseButtonText = "OK" }.ShowDialogAsync();
                        return;
                    }
                }

                Games.Remove(gameToRemove);
                _storage.Save(Games.ToList());
            }
        }

        private void ApplySorting()
        {
            IOrderedEnumerable<Game> sortedGames = _selectedSorting switch
            {
                SortingMethod.Added => _isAscending ? Games.OrderBy(game => game.DateAdded) : Games.OrderByDescending(game => game.DateAdded),
                SortingMethod.Alphabetical => _isAscending ? Games.OrderBy(game => game.Title) : Games.OrderByDescending(game => game.Title),
                SortingMethod.Playtime => _isAscending ? Games.OrderBy(game => game.Playtime) : Games.OrderByDescending(game => game.Playtime),
                _ => Games.OrderBy(game => game.DateAdded)
            };

            List<Game> sortedResult = sortedGames.ToList();
            Games.Clear();
            foreach (Game game in sortedResult)
            {
                Games.Add(game);
            }
        }


        private void CheckRunningProcesses(object? sender, EventArgs e)
        {
            Process[] processes = Process.GetProcesses();

            foreach (Game game in Games)
            {
                game.IsRunning = processes.Any(process => process.ProcessName.Equals(
                    Path.GetFileNameWithoutExtension(game.Path),
                    StringComparison.OrdinalIgnoreCase));
            }
        }

        private void IncrementPlaytime(object? sender, EventArgs e)
        {
            foreach (Game game in Games.Where(game => game.IsRunning))
            {
                game.Playtime++;
            }

            _storage.Save(Games.ToList());
        }

    }
}

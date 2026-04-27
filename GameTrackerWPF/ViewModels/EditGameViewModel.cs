using GameTrackerWPF.Enums;
using GameTrackerWPF.Models;
using GameTrackerWPF.MVVM;
using GameTrackerWPF.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using WPFUI = Wpf.Ui.Controls;

namespace GameTrackerWPF.ViewModels
{
    class EditGameViewModel : ViewModelBase
    {
        private readonly Game _originalGame;
        private readonly Game _copyGame;

        private readonly GameStorageService _storage = new GameStorageService();

        private ObservableCollection<Game> _games { get; set; }
        public ObservableCollection<Game> Games { get; set; }

        public uint Hours
        {
            get => _copyGame.Playtime / 60;
            set
            {
                _copyGame.Playtime = value * 60 + Minutes;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Minutes));
            }
        }

        public uint Minutes
        {
            get => _copyGame.Playtime % 60;
            set
            {
                _copyGame.Playtime = Hours * 60 + value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Hours));
            }
        }

        public string Title
        {
            get => _copyGame.Title;
            set { _copyGame.Title = value; OnPropertyChanged(); }
        }

        public uint Playtime
        {
            get => _copyGame.Playtime;
            set { _copyGame.Playtime = value; OnPropertyChanged(); }
        }

        public string GamePath
        {
            get => _copyGame.Path;
            set { _copyGame.Path = value; OnPropertyChanged(); }
        }

        public string CoverPath
        {
            get => _copyGame.CoverPath;
            set { _copyGame.CoverPath = value; OnPropertyChanged(); }
        }

        public RelayCommand BrowseGamePathCommand { get; }
        public RelayCommand BrowseCoverCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public event Action? SaveRequested;
        public event Action? CancelRequested;

        public EditGameViewModel(Game editedGame, ObservableCollection<Game> games)
        {
            _originalGame = editedGame;
            _copyGame = new Game(editedGame);
            _games = games;

            BrowseGamePathCommand = new RelayCommand(execute => BrowseGamePath());
            BrowseCoverCommand = new RelayCommand(execute => BrowseCover());
            SaveCommand = new RelayCommand(execute => Save());
            CancelCommand = new RelayCommand(execute => Cancel());
        }

        private void BrowseGamePath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Game Path";
            openFileDialog.Filter = "Executable Files (*.exe)|*.exe";

            if (openFileDialog.ShowDialog() == true)
            {
                GamePath = openFileDialog.FileName;
            }
        }

        private void BrowseCover()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Cover Image";
            openFileDialog.Filter = "Image Files (*.jpg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                CoverPath = openFileDialog.FileName;
            }
        }

        private void Save()
        {
            // Save the cover image
            if (!string.IsNullOrEmpty(_copyGame.CoverPath) && File.Exists(_copyGame.CoverPath))
            {
                string coversFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "GameTracker", "Covers");

                Directory.CreateDirectory(coversFolder);

                string extension = Path.GetExtension(_copyGame.CoverPath);
                string destination = Path.Combine(coversFolder, $"{_originalGame.Id}{extension}");

                // Delete the old cover if exists
                if (!string.IsNullOrEmpty(_originalGame.CoverPath) && File.Exists(_originalGame.CoverPath))
                {
                    try
                    {
                        File.Delete(_originalGame.CoverPath);
                    }
                    catch
                    {
                        new WPFUI.MessageBox { Title = "Error!", Content = "Failed to delete old cover!", CloseButtonText = "OK" }.ShowDialogAsync();
                        return;
                    }
                }

                // Only copy if source and destination are different
                if (!string.Equals(_copyGame.CoverPath, destination, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(_copyGame.CoverPath, destination, overwrite: true);
                    _copyGame.CoverPath = destination;
                }
            }


            // Save the values
            _originalGame.Title = _copyGame.Title;
            _originalGame.Path = _copyGame.Path;
            _originalGame.CoverPath = _copyGame.CoverPath;
            _originalGame.Playtime = _copyGame.Playtime;

            _storage.Save(_games.ToList());

            SaveRequested?.Invoke();
        }

        private void Cancel()
        {
            CancelRequested?.Invoke();
        }
    }
}

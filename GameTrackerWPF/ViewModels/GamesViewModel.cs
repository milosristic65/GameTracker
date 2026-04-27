using GameTrackerWPF.Models;
using GameTrackerWPF.MVVM;
using GameTrackerWPF.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using Wpf.Ui.Input;
using WPFUI = Wpf.Ui.Controls;

namespace GameTrackerWPF.ViewModels
{
    class GamesViewModel : ViewModelBase
    {
        public ObservableCollection<Game> Games { get; set; }
        public RelayCommand AddGameCommand { get; }
        public RelayCommand<Guid> RemoveGameCommand { get; }

        private readonly GameStorageService _storage = new();

        public GamesViewModel()
        {
            AddGameCommand = new RelayCommand(execute => AddGame());
            RemoveGameCommand = new RelayCommand<Guid>(id => RemoveGame(id));

            var savedGames = _storage.Load();
            Games = new ObservableCollection<Game>(savedGames);
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
                    new WPFUI.MessageBox { Title = "Warning", Content = "The game is already on the list!", CloseButtonText = "OK" }.ShowDialogAsync();
                    return;
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
                new WPFUI.MessageBox { Title = "Error", Content = "Unexpected error occurred!", CloseButtonText = "OK" }.ShowDialogAsync();
                return;
            }

            var gameToRemove = Games.FirstOrDefault(game => game.Id == id);
            if (gameToRemove == null)
            {
                new WPFUI.MessageBox { Title = "Error", Content = "Unexpected error occurred!", CloseButtonText = "OK" }.ShowDialogAsync();
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
                Games.Remove(gameToRemove);
                _storage.Save(Games.ToList());
            }
        }
    }
}

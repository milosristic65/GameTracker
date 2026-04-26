using GameTrackerWPF.Models;
using GameTrackerWPF.MVVM;
using GameTrackerWPF.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Wpf.Ui.Input;

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

            openFileDialog.Filter = "Executable (*.exe)|*.exe";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Check if the game is already in the list
                if (Games.Any(game => game.Path == filePath))
                {
                    MessageBox.Show("The game is already on the list!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Unexpected error occured!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var gameToRemove = Games.FirstOrDefault(game => game.Id == id);
            if (gameToRemove == null)
            {
                MessageBox.Show("Unexpected error occured!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var removeGameResult = MessageBox.Show($"Are you sure you want to remove {gameToRemove.Title}?", "Remove Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (removeGameResult == MessageBoxResult.Yes)
            {
                Games.Remove(gameToRemove);
                _storage.Save(Games.ToList());
            }
        }
    }
}

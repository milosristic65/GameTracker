using GameTrackerWPF.Models;
using GameTrackerWPF.ViewModels;
using System.Collections.ObjectModel;

namespace GameTrackerWPF.Views
{
    public partial class EditGameWindow
    {
        public EditGameWindow(Game game, ObservableCollection<Game> games)
        {
            InitializeComponent();
            var editGameViewModel = new EditGameViewModel(game, games);

            editGameViewModel.SaveRequested += () => DialogResult = true;
            editGameViewModel.CancelRequested += () => DialogResult = false;

            DataContext = editGameViewModel;
        }
    }
}

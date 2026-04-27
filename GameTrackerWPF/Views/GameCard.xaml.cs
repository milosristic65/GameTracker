using GameTrackerWPF.Models;
using GameTrackerWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Input;
using WPFUI = Wpf.Ui.Controls;

namespace GameTrackerWPF.Views
{
    public partial class GameCard : UserControl
    {
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(GameCard), new PropertyMetadata(Guid.Empty));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(GameCard), new PropertyMetadata(""));

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(GameCard), new PropertyMetadata(""));

        public static readonly DependencyProperty CoverImageProperty =
            DependencyProperty.Register("CoverImage", typeof(BitmapImage), typeof(GameCard));

        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register("IsRunning", typeof(bool), typeof(GameCard), new PropertyMetadata(false));

        public static readonly DependencyProperty PlaytimeProperty =
            DependencyProperty.Register("Playtime", typeof(uint), typeof(GameCard), new PropertyMetadata((uint)0));

        public Guid Id
        {
            get => (Guid)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public BitmapImage CoverImage
        {
            get => (BitmapImage)GetValue(CoverImageProperty);
            set => SetValue(CoverImageProperty, value);
        }

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public uint Playtime
        {
            get => (uint)GetValue(PlaytimeProperty);
            set => SetValue(PlaytimeProperty, value);
        }

        public GameCard()
        {
            InitializeComponent();
        }

        private void btnEditGame_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var gamesViewModel = window?.DataContext as GamesViewModel;
            var games = gamesViewModel?.Games;

            if (games == null)
            {
                new WPFUI.MessageBox { Title = "Error", Content = "Games data not found.", CloseButtonText = "OK" }.ShowDialogAsync();
                return;
            }

            var editedGame = games.FirstOrDefault(game => game.Id == Id);
            if (editedGame == null)
            {
                new WPFUI.MessageBox { Title = "Error", Content = "Game not found.", CloseButtonText = "OK" }.ShowDialogAsync();
                return;
            }

            var editWindow = new EditGameWindow(editedGame, games);
            editWindow.Owner = window;
            editWindow.ShowDialog();
        }
    }

}

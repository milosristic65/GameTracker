using GameTrackerWPF.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Wpf.Ui.Appearance;


namespace GameTrackerWPF
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            GamesViewModel gameCardViewModel = new GamesViewModel();
            DataContext = gameCardViewModel;
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            // Minimize to tray instead of closing
            e.Cancel = true;
            this.Hide();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
using GameTrackerWPF.ViewModels;
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
    }
}
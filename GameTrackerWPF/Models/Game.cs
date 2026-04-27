using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace GameTrackerWPF.Models
{
    public class Game : INotifyPropertyChanged
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        private string _path = "";
        public string Path
        {
            get => _path;
            set { _path = value; OnPropertyChanged(); }
        }

        private string _coverPath = "";
        public string CoverPath
        {
            get => _coverPath;
            set { _coverPath = value; OnPropertyChanged(); ReloadCover(); }
        }

        private BitmapImage? _coverImage;
        [JsonIgnore]
        public BitmapImage? CoverImage
        {
            get => _coverImage;
            set { _coverImage = value; OnPropertyChanged(); }
        }

        private DateTime _dateAdded = DateTime.Now;
        public DateTime DateAdded
        {
            get => _dateAdded;
            set { _dateAdded = value; OnPropertyChanged(); }
        }

        private string _title = "";
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private uint _playtime;
        public uint Playtime
        {
            get => _playtime;
            set { _playtime = value; OnPropertyChanged(); }
        }

        private bool _isRunning;
        [JsonIgnore]
        public bool IsRunning
        {
            get => _isRunning;
            set { _isRunning = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Game() { }

        public Game(Game game)
        {
            Title = game.Title;
            Path = game.Path;
            CoverPath = game.CoverPath;
            Playtime = game.Playtime;
            IsRunning = game.IsRunning;
            DateAdded = game.DateAdded;
            ReloadCover();
        }

        private void ReloadCover()
        {
            if (string.IsNullOrEmpty(CoverPath) || !File.Exists(CoverPath))
            {
                CoverImage = null;
                return;
            }

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(CoverPath, UriKind.Absolute);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            CoverImage = bitmapImage;
        }
    }
}
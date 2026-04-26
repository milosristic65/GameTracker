using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            set { _coverPath = value; OnPropertyChanged(); }
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
        }
    }
}
using GameTrackerWPF.Models;
using System.IO;
using System.Text.Json;

namespace GameTrackerWPF.Services
{
    public class GameStorageService
    {
        private readonly string _savePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GameTracker", "games.json");

        public List<Game> Load()
        {
            try
            {
                if (!File.Exists(_savePath)) return new List<Game>();

                string json = File.ReadAllText(_savePath);
                return JsonSerializer.Deserialize<List<Game>>(json) ?? new List<Game>();
            }
            catch
            {
                return new List<Game>();
            }
        }

        public void Save(List<Game> games)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_savePath)!);

                string json = JsonSerializer.Serialize(games, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_savePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
            }
        }
    }
}

using GameTrackerWPF.Models;
using System.IO;
using System.Text.Json;
using WPFUI = Wpf.Ui.Controls;

namespace GameTrackerWPF.Services
{
    public class SettingsService
    {
        private readonly string _savePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GameTracker", "settings.json");

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(_savePath)) return new AppSettings();

                string json = File.ReadAllText(_savePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_savePath)!);

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_savePath, json);
            }
            catch (Exception ex)
            {
                new WPFUI.MessageBox { Title = "Error!", Content = ex.Message, CloseButtonText = "OK" }.ShowDialogAsync();
            }
        }
    }
}

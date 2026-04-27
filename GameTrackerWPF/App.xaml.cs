using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Windows;

namespace GameTrackerWPF
{
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private Thread? _pipeThread;

        protected override void OnStartup(StartupEventArgs e)
        {
            new Mutex(true, "GameTracker_SingleInstance", out bool isNewInstance);

            // If another instance is already running, open that instance and terminate this one
            if (!isNewInstance)
            {
                NamedPipeClientStream client = new NamedPipeClientStream(".", "GameTracker_Pipe", PipeDirection.Out);
                try
                {
                    client.Connect(1000);
                    using var writer = new StreamWriter(client);
                    writer.WriteLine("SHOW");
                }
                catch { }

                client.Close();
                Shutdown();
                return;
            }

            _pipeThread = new Thread(ListenForSignals) { IsBackground = true };
            _pipeThread.Start();

            // Normal startup
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Show();
        }

        private void ListenForSignals()
        {
            while (true)
            {
                using var server = new NamedPipeServerStream("GameTracker_Pipe", PipeDirection.In);
                server.WaitForConnection();

                StreamReader reader = new StreamReader(server);
                string message = reader.ReadLine() ?? "";

                if (message == "SHOW")
                {
                    // Restore main window from another instance safely
                    Dispatcher.Invoke(() =>
                    {
                        if (MainWindow != null)
                        {
                            if (!MainWindow.IsVisible)
                            {
                                MainWindow.Show();
                            }
                            if (MainWindow.WindowState == WindowState.Minimized)
                            {
                                MainWindow.WindowState = WindowState.Normal;
                            }

                            MainWindow.Activate();
                            MainWindow.Focus();
                        }
                    });
                }

                reader.Close();
            }
        }
    }
}
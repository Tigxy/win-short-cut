using System;
using System.Windows;
using win_short_cut.DataClasses;
using win_short_cut.Pages;
using win_short_cut.Utils;

namespace win_short_cut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            // ensure that necessary directories exist
            System.IO.Directory.CreateDirectory(Globals.ApplicationDir);
            System.IO.Directory.CreateDirectory(Globals.ShortcutBinPath);

            // make shortcut files visible for command prompt
            if (!Utils.Environment.IsInPath(Globals.ShortcutBinPath))
                Utils.Environment.AddToPath(Globals.ShortcutBinPath);

            LoadSettings();
            LoadShortcuts();

            PageManager.Instance.RegisterPage("overview", new ShortcutsPage());
            PageManager.Instance.RegisterPage("edit", new ShortcutPage());

            PageManager.Instance.SwitchToPage("overview");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StoreSettings();
            StoreShortcuts();
        }

        private void LoadSettings()
        {
            (bool success, Settings? settings) = Utils.Serialization.LoadJson<Settings>(Globals.SettingsFile);

            if (success)
            {
                this.Width = settings!.WindowWidth;
                this.Height = settings!.WindowHeight;

                this.Top = settings!.WindowTop;
                this.Left = settings!.WindowLeft;
            }
            else
            {
                // set default startup location in case no previous settings are known
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void StoreSettings()
        {
            Utils.Serialization.StoreJson<Settings>(new Settings()
            {
                WindowHeight = this.Height,
                WindowWidth = this.Width,
                WindowLeft = this.Left,
                WindowTop = this.Top
            }, Globals.SettingsFile);
        }

        private void StoreShortcuts() => Utils.Serialization.Store<Shortcuts>(new Shortcuts(Globals.Shortcuts), Globals.ShortcutDataFile);
        private void LoadShortcuts()
        {
            (bool success, Shortcuts? data) = Utils.Serialization.Load<Shortcuts>(Globals.ShortcutDataFile);
            if (success)
                data!.ForEach(x => Globals.Shortcuts.Add(x));
        }

        private Shortcut CreateDummyShortcut()
        {
            return new Shortcut()
            {
                Name = "test",
                Description = "Some test shortcut",
                Containers = new() {
                    new CommandContainer() {
                        KeepOpenOnceDone = true,
                        Description = "one container",
                        Commands = new() {
                            new Command() { ExecutionString = "explorer.exe", PrintCommand = true}
                        }
                    },
                    new CommandContainer() {
                        KeepOpenOnceDone = true,
                        Description = "another container",
                        Commands = new() {
                            new Command() { ExecutionString = "explorer.exe", PrintCommand = true},
                            new Command() { ExecutionString = "explorer.exe", PrintCommand = true}
                        }
                    }
                }
            };
        }
    }
}

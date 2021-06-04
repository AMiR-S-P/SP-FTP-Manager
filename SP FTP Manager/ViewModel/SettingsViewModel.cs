using Microsoft.Win32;
using SP_FTP_Manager.Commands;
using SP_FTP_Manager.CustomControls;
using SP_FTP_Manager.Helper;
using SP_FTP_Manager.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace SP_FTP_Manager.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {

        private Settings settings = new Settings();

        public Settings Settings
        {
            get { return settings; }
            set { settings = value; OnPropertyChanged(); }
        }


        #region GeneralTab


        public AsyncRelayCommand<object> BrowseCommand { get; set; }
        public AsyncRelayCommand<object> OpenDownloadPathCommand { get; set; }

        private Task OnBrowse(object arg)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(folderBrowser.SelectedPath))
                {
                    Settings.DownloadPath = folderBrowser.SelectedPath;
                }
            }

            return Task.CompletedTask;
        }
        private Task OnOpenDownloadPath(object arg)
        {
            Process.Start(new ProcessStartInfo(App.Settings.DownloadPath) { UseShellExecute = true });
            return Task.CompletedTask;
        }
        #endregion



        #region Theme
        public AsyncRelayCommand<Color> SetAccentColorCommand { get; set; }
        public AsyncRelayCommand<Color> SetTextColorCommand { get; set; }


        private Task OnSetAccentColor(Color arg)
        {
            Settings.AccentColor = arg;
            return Task.CompletedTask;
        }
        private Task OnSetTextColor(Color arg)
        {
            Settings.TextColor = arg;
            return Task.CompletedTask;
        }

        #endregion

        #region About
        public AssemblyName Version
        {
            get
            {
                return Assembly.GetAssembly(typeof(SettingsViewModel)).GetName();
            }
        }

        public AsyncRelayCommand<object> LinkedInCommand { get; set; }
        public AsyncRelayCommand<object> GitHubCommand { get; set; }
        public AsyncRelayCommand<object> GoogleCommand { get; set; }

        private Task OnGitHub(object arg)
        {
            Process.Start(new ProcessStartInfo("https://github.com/AMiR-S-P") { UseShellExecute = true });

            return Task.CompletedTask;
        }

        private Task OnGoogle(object arg)
        {
            string uri = $"mailto:AmirSetvatiPaydar@Gmail.com?Subject=SPColorWheel&body=";
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });

            return Task.CompletedTask;
        }

        private Task OnLinkedIn(object arg)
        {
            Process.Start(new ProcessStartInfo("https://www.linkedin.com/in/amir-setvati-paydar-747657152/") { UseShellExecute = true });

            return Task.CompletedTask;
        }
        #endregion


        public AsyncRelayCommand<ChromeWindow> OkCommand { get; set; }
        public AsyncRelayCommand<object> ResetToDefaultCommand { get; set; }
        public AsyncRelayCommand<object> LoadSettings { get; set; }
        public SettingsViewModel()
        {
            OkCommand = new AsyncRelayCommand<ChromeWindow>(OnOk);
            ResetToDefaultCommand = new AsyncRelayCommand<object>(OnReset);
            LoadSettings = new AsyncRelayCommand<object>(OnLoadSettings);

            BrowseCommand = new AsyncRelayCommand<object>(OnBrowse);
            OpenDownloadPathCommand = new AsyncRelayCommand<object>(OnOpenDownloadPath);

            SetAccentColorCommand = new AsyncRelayCommand<Color>(OnSetAccentColor);
            SetTextColorCommand = new AsyncRelayCommand<Color>(OnSetTextColor);

            LinkedInCommand = new AsyncRelayCommand<object>(OnLinkedIn);
            GitHubCommand = new AsyncRelayCommand<object>(OnGitHub);
            GoogleCommand = new AsyncRelayCommand<object>(OnGoogle);


        }



        private async Task OnLoadSettings(object arg)
        {
            await Settings.LoadAsync();
            App.Current.Resources["BaseBackgroundColor"] =new SolidColorBrush( Colors.Yellow);
        }

        private async Task OnReset(object arg)
        {
            await Settings.ResetAsync();
        }
        
        private async Task OnOk(ChromeWindow arg)
        {
            App.Settings = Settings;
            await App.Settings.SaveAsync();
            await new WindowsService().CloseWindow(arg);
        }


    }
}

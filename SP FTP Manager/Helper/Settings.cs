using SP_FTP_Manager.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SP_FTP_Manager.Helper
{
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string pName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
        }

        //private static Settings _instance;

        //public static Settings Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            Task.Run(async () =>
        //            {
        //                _instance = new Settings();
        //                await _instance.LoadAsync();
        //            });
        //        }
        //        return _instance;
        //    }
        //}

        #region GeneralTab

        private string downloadPath;

        public string DownloadPath { get => downloadPath; set { downloadPath = value; OnPropertyChanged(); } }
        #endregion

        #region ConnectionTab

        private bool hasProxy;

        public bool HasProxy
        {
            get { return hasProxy; }
            set { hasProxy = value; OnPropertyChanged(); }
        }

        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; OnPropertyChanged(); }
        }

        private string port;

        public string Port
        {
            get { return port; }
            set { port = value; OnPropertyChanged(); }
        }

        #endregion

        #region Theme
        private bool isLight;

        public bool IsLight
        {
            get { return isLight; }
            set { isLight = value; OnPropertyChanged(); }
        }

        private Color accentColor;

        public Color AccentColor
        {
            get { return accentColor; }
            set
            {
                accentColor = value; OnPropertyChanged();
            }
        }
        private Color textColor;

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; OnPropertyChanged(); }
        }
        #endregion

        public Settings()
        {
            Task.Run(async () =>
            {
                await ResetAsync();
            });
        }

        public async Task LoadAsync()
        {
            if (File.Exists(App.SettingsPath))
            {
                using (StreamReader sr = new StreamReader(App.SettingsPath))
                {
                    string s = sr.ReadToEnd();
                    if (!string.IsNullOrEmpty(s))
                    {
                        var loadedSettings = System.Text.Json.JsonSerializer.Deserialize<Settings>(s);
                        this.DownloadPath = loadedSettings.DownloadPath;

                        this.AccentColor = loadedSettings.AccentColor;
                        this.TextColor = loadedSettings.TextColor;
                        this.IsLight = loadedSettings.IsLight;

                        this.HasProxy = loadedSettings.HasProxy;
                        this.Address = loadedSettings.Address;
                        this.Port = loadedSettings.Port;
                    }
                    else
                    {
                        await ResetAsync();
                    }
                }
            }
            else
            {
                await ResetAsync();
            }
        }

        public async Task SaveAsync()
        {
            if (!File.Exists(App.SettingsPath))
            {
                File.CreateText(App.SettingsPath).Close();
            }

            using (StreamWriter sr = new StreamWriter(App.SettingsPath))
            {
                await sr.WriteAsync(System.Text.Json.JsonSerializer.Serialize(this));
            }
        }

        public Task ResetAsync()
        {
            AccentColor = (Color)ColorConverter.ConvertFromString("#Ff669166");
            TextColor = System.Windows.Media.Colors.WhiteSmoke;
            IsLight = false;

            HasProxy = false;
            Address = "";
            Port = "";

            DownloadPath = App.DefaultDownloadPath;

            return Task.CompletedTask;
        }
    }
}

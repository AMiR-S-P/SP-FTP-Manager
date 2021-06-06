using Newtonsoft.Json;
using SP_FTP_Manager.Helper;
using SP_FTP_Manager.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SP_FTP_Manager
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SP FTP Manager");

        public static string SettingsPath = Path.Combine(AppDataFolder, "Settings.sps");
        public static string DefaultDownloadPath = "";
        public static string LoginInfoPath = Path.Combine(AppDataFolder, "LoginInfo.sps");

        //load settings
        public static Settings Settings { get; set; } = new Settings();
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }


            if (!File.Exists(SettingsPath))
            {
                File.CreateText(SettingsPath).Close();
            }

            if (!File.Exists(LoginInfoPath))
            {
                using (var file = File.CreateText(LoginInfoPath))
                {
                    file.Write(JsonConvert.SerializeObject(new LoginModel()));
                }
            }
            Startup += App_Startup;
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            await Settings.LoadAsync();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }
    }
}

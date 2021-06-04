using SP_FTP_Manager.Helper;
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
        public static string SettingsPath = Path.Combine(Environment.CurrentDirectory, "Settings.sps");
        public static string DefaultDownloadPath = Path.Combine(Environment.CurrentDirectory, "Downloads");

        //load settings
        public static Settings Settings { get; set; } = new Settings();
        public App()
        {
            Task.Run(async () =>
            {
                await Settings.LoadAsync();
            });
            if (!File.Exists(SettingsPath))
            {
                File.CreateText(SettingsPath);
            }

            if (Directory.Exists(DefaultDownloadPath))
            {
                Directory.CreateDirectory(DefaultDownloadPath);
            }
        }
    }
}

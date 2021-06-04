using SP_FTP_Manager.Commands;
using SP_FTP_Manager.CustomControls;
using SP_FTP_Manager.Services;
using SP_FTP_Manager.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SP_FTP_Manager.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy; set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }





        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string pName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
        }


        public AsyncRelayCommand<ChromeWindow> BackCommand { get; set; }
        public AsyncRelayCommand<ChromeWindow> SettingsCommand { get; set; }

        public BaseViewModel()
        {

            BackCommand = new AsyncRelayCommand<ChromeWindow>(OnBack);
            SettingsCommand = new AsyncRelayCommand<ChromeWindow>(OnSettings);
        }

        private async Task OnSettings(ChromeWindow arg)
        {
            WindowsService windowsService = new WindowsService();
            await windowsService.OpenDialog(typeof(SettingsPage), arg, new SettingsViewModel());
        }

        private async Task OnBack(ChromeWindow arg)
        {
            IWindowsService windowsService = new WindowsService();
            await windowsService.OpenNewWindow(typeof(LoginPage), arg, null);
        }



    }
}

using Newtonsoft.Json;
using SP_FTP_Manager.Commands;
using SP_FTP_Manager.CustomControls;
using SP_FTP_Manager.Helper;
using SP_FTP_Manager.Model;
using SP_FTP_Manager.Services;
using SP_FTP_Manager.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SP_FTP_Manager.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private LoginModel login = new LoginModel();
        private bool isLoginIn = true;
        private bool? isLoginSuccessful;
        private string message;

        public LoginModel Login { get => login; set => login = value; }
        public bool IsNotLoginIn { get => isLoginIn; set { isLoginIn = value; OnPropertyChanged(); } }
        public bool? IsLoginSuccessful { get => isLoginSuccessful; set { isLoginSuccessful = value; OnPropertyChanged(); } }
        public string Message { get => message; set { message = value; OnPropertyChanged(); } }
        public AsyncRelayCommand<ChromeWindow> SendOnlyCommand { get; set; }
        public AsyncRelayCommand<ChromeWindow> ManageCommand { get; set; }
        public LoginViewModel()
        {
            SendOnlyCommand = new AsyncRelayCommand<ChromeWindow>(OnSendOnly);
            ManageCommand = new AsyncRelayCommand<ChromeWindow>(OnManage);

            if (!File.Exists(App.LoginInfoPath))
            {
                Login = new LoginModel();
                using (var file = File.CreateText(App.LoginInfoPath))
                {
                    file.Write(JsonConvert.SerializeObject(Login));
                }
            }
            else
            {
                try
                {
                    using (var file = File.OpenText(App.LoginInfoPath))
                    {
                        Login = JsonConvert.DeserializeObject<LoginModel>(file.ReadToEnd())??new LoginModel();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private async Task OnManage(ChromeWindow arg)
        {
            IsNotLoginIn = false;

            if (await CheckInfos())
            {

                WindowsService windowsService = new WindowsService();

                await windowsService.OpenNewWindow(typeof(ManagePage), arg, new ManageViewModel(Login,false));
            }
            else
            {
                IsLoginSuccessful = false;
                IsNotLoginIn = true;
            }
        }


        private async Task OnSendOnly(ChromeWindow arg)
        {

            IsNotLoginIn = false;

            if (await CheckInfos())
            {

                WindowsService windowsService = new WindowsService();

                await windowsService.OpenNewWindow(typeof(SendOnlyPage), arg, new SendOnlyViewModel(Login,true));
            }
            else
            {
                IsLoginSuccessful = false;
                IsNotLoginIn = true;
            }


        }

        async Task<bool> CheckInfos()
        {
            try
            {
                WebRequest webRequest = FtpWebRequest.Create(Login.Server);
                webRequest.Credentials = new NetworkCredential(Login.Username, PasswordCipher.Decrypt(Login.Password));
                webRequest.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;

                FtpWebResponse response = await webRequest.GetResponseAsync() as FtpWebResponse;


                if (response.WelcomeMessage.Contains("230"))
                {
                    if (Login.Remember == true)
                    {
                        using (var file = new StreamWriter(App.LoginInfoPath))
                        {
                            file.WriteLine(JsonConvert.SerializeObject(Login));
                        }
                    }
                    else
                    {
                        File.Delete(App.LoginInfoPath);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }



    }
}

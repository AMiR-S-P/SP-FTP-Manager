using SP_FTP_Manager.CustomControls;
using SP_FTP_Manager.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SP_FTP_Manager.Services
{
    public class WindowsService : IWindowsService
    {
        public Task OpenChildWindow(Type window, ChromeWindow owner, object dataContext)
        {
            ChromeWindow win = (ChromeWindow)Activator.CreateInstance(window);
            if (dataContext != null)
            {
                win.DataContext = dataContext;
            }
            win.Owner = owner;
            win.Show();
            return Task.CompletedTask;
        }

        public Task OpenDialog(Type window, ChromeWindow owner, object dataContext)
        {
            ChromeWindow win = (ChromeWindow)Activator.CreateInstance(window);
            if (dataContext != null)
            {
                win.DataContext = dataContext;
            }
            win.Owner = owner;
            win.ShowDialog();
            return Task.CompletedTask;
        }

        public Task OpenInputBox(string title,ref string result)
        {
            InputBox inputBox = new InputBox();
            inputBox.Title = title;
            inputBox.ShowDialog();
            result = inputBox.Value;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Close last window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public Task OpenNewWindow(Type window, ChromeWindow lastWindow, object dataContext)
        {
            ChromeWindow win = (ChromeWindow)Activator.CreateInstance(window);
            if (dataContext != null)
            {
                win.DataContext = dataContext;
            }
            lastWindow.Close();
            win.Show();
            return Task.CompletedTask;
        }

        public Task CloseWindow(ChromeWindow window)
        {
            window?.Close();
            return Task.CompletedTask;
        }
    }
}

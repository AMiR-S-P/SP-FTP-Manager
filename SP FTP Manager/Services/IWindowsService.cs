using SP_FTP_Manager.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SP_FTP_Manager.Services
{
    public interface IWindowsService
    {
        Task OpenDialog(Type window, ChromeWindow owner, object dataContext);
        Task OpenChildWindow(Type window, ChromeWindow owner,object dataContext);
        Task OpenNewWindow(Type window, ChromeWindow lastWindow, object dataContext);
        Task OpenInputBox(string title ,ref string result);
        Task CloseWindow(ChromeWindow window);
    }
}

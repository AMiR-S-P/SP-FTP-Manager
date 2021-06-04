using SP_FTP_Manager.Commands;
using SP_FTP_Manager.CustomControls;
using SP_FTP_Manager.Enums;
using SP_FTP_Manager.Helper;
using SP_FTP_Manager.Model;
using SP_FTP_Manager.Services;
using SP_FTP_Manager.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SP_FTP_Manager.ViewModel
{
    public class SendOnlyViewModel : BaseFTPManageViewModel
    {

        public AsyncRelayCommand<object> AddCommand { get; set; }
        public AsyncRelayCommand<object> MakeDirectoryCommand { set; get; }
        public AsyncRelayCommand<object> AddFolderCommand { get; set; }


        public SendOnlyViewModel(LoginModel loginModel,bool isSendOnly):base(loginModel,isSendOnly)
        {
            AddCommand = new AsyncRelayCommand<object>(OnAdd);
            AddFolderCommand = new AsyncRelayCommand<object>(OnAddFolder);
            MakeDirectoryCommand = new AsyncRelayCommand<object>(OnMakeDirectory);
        }

     
        private async Task OnAddFolder(object arg)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                await EnumerateFolders(folderBrowser.SelectedPath, LoginModel.Server);
            }
        }

        async Task EnumerateFolders(string path, string ftpPath)
        {
            var directories = Directory.EnumerateDirectories(path);
            var files = Directory.EnumerateFiles(path);

            //upload files 
            foreach (var file in files)
            {
                Jobs.Add(new JobModel()
                {
                    LocalAddress = file,
                    Status = Enums.JobStatus.Pending,
                    Type = FtpType.File,
                    Name = file.Substring(file.LastIndexOf('\\') + 1),
                    FtpAddress = ftpPath,
                    Title = Enums.JobTitle.Upload,
                    Size = new FileInfo(file).OpenRead().Length
                });
            }

            //create directories
            foreach (var dir in directories)
            {
                Jobs.Add(new JobModel()
                {
                    LocalAddress = dir,
                    Status = Enums.JobStatus.Pending,
                    Type = FtpType.Directory,
                    Name = dir.Substring(dir.LastIndexOf('\\') + 1),
                    FtpAddress = ftpPath,
                    Title = Enums.JobTitle.MakeDirectory,
                });


                await EnumerateFolders(dir, ftpPath + "/" + dir.Substring(dir.LastIndexOf('\\') + 1));
            }
        }

        private Task OnMakeDirectory(object arg)
        {
            string newName = "";
            WindowsService windowsService = new WindowsService();
            windowsService.OpenInputBox("Folder Name",ref newName);

            if (!string.IsNullOrEmpty(newName))
            {
                Jobs.Add(new JobModel()
                {
                    Type = FtpType.Directory,
                    Title = Enums.JobTitle.MakeDirectory,
                    Name = newName,
                    FtpAddress = LoginModel.Server,
                });
            }
            return Task.CompletedTask;
        }

        private Task OnAdd(object arg)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    Jobs.Add(new JobModel()
                    {
                        LocalAddress = file,
                        Status = Enums.JobStatus.Pending,
                        Type = FtpType.File,
                        Name = file.Substring(file.LastIndexOf('/') + 1),
                        FtpAddress = LoginModel.Server,
                        Title = Enums.JobTitle.Upload,
                        Size = new FileInfo(file).OpenRead().Length
                    });
                }
            }

            return Task.CompletedTask;
        }
    }
}

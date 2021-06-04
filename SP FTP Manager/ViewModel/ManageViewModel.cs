using SP_FTP_Manager.Commands;
using SP_FTP_Manager.Enums;
using SP_FTP_Manager.Helper;
using SP_FTP_Manager.Model;
using SP_FTP_Manager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;

namespace SP_FTP_Manager.ViewModel
{
    public class ManageViewModel : BaseFTPManageViewModel
    {
        //private FTPModel ftpModel;

        //public FTPModel FtpModel
        //{
        //    get { return ftpModel; }
        //    set { ftpModel = value; OnPropertyChanged(); }
        //}
        public ObservableCollection<JobModel> AllJobs { get => allJobs; private set { allJobs = value;OnPropertyChanged(); } }

        private FTPMapModel selectedMap;
        private ObservableCollection<JobModel> allJobs;

        public FTPMapModel SelectedMap
        {
            get { return selectedMap; }
            set
            {
                selectedMap = value;
                OnPropertyChanged();
                AddFileCommand.OnCanExecuteChanged();
                AddFolderCommand.OnCanExecuteChanged();
                CreateFolderCommand.OnCanExecuteChanged();
                RenameCommand.OnCanExecuteChanged();
                DownloadCommand.OnCanExecuteChanged();
                DeleteCommand.OnCanExecuteChanged();
            }
        }

        public AsyncRelayCommand<object> RefreshCommand { get; set; }

        public AsyncRelayCommand<FTPMapModel> AddFileCommand { get; set; }
        public AsyncRelayCommand<FTPMapModel> AddFolderCommand { get; set; }
        public AsyncRelayCommand<FTPMapModel> CreateFolderCommand { get; set; }
        public AsyncRelayCommand<FTPMapModel> RenameCommand { get; set; }
        public AsyncRelayCommand<FTPMapModel> DownloadCommand { get; set; }
        public AsyncRelayCommand<FTPMapModel> DeleteCommand { get; set; }

        public AsyncRelayCommand<FTPMapModel> CopyNameCommand { get; set; }
        public AsyncRelayCommand<FTPMapModel> CopyPathCommand { get; set; }

        public AsyncRelayCommand<FTPMapModel> SetSelectedMapCommand { get; set; }
        public AsyncRelayCommand<object> FilterCommand { get; set; }
        public AsyncRelayCommand<object> FilterStatusCommand { get; set; }
        public ManageViewModel(LoginModel login, bool isSendOnly) : base(login, isSendOnly)
        {
            RefreshCommand = new AsyncRelayCommand<object>(OnRefresh);

            AddFileCommand = new AsyncRelayCommand<FTPMapModel>(OnAddFile, CanButtonsWork);
            AddFolderCommand = new AsyncRelayCommand<FTPMapModel>(OnAddFolder, CanButtonsWork);
            CreateFolderCommand = new AsyncRelayCommand<FTPMapModel>(OnCreateFolder, CanButtonsWork);
            RenameCommand = new AsyncRelayCommand<FTPMapModel>(OnRename, CanButtonsWork);
            DownloadCommand = new AsyncRelayCommand<FTPMapModel>(OnDownload, CanButtonsWork);
            DeleteCommand = new AsyncRelayCommand<FTPMapModel>(OnDelete, CanButtonsWork);

            CopyNameCommand = new AsyncRelayCommand<FTPMapModel>(OnCopyName);
            CopyPathCommand = new AsyncRelayCommand<FTPMapModel>(OnCopyPath);

            SetSelectedMapCommand = new AsyncRelayCommand<FTPMapModel>(OnSetSelectedMap);
            FilterCommand = new AsyncRelayCommand<object>(OnFilter);
            FilterStatusCommand = new AsyncRelayCommand<object>(OnStatusFilter);
        }

        private Task OnStatusFilter(object arg)
        {
            var values = arg as object[];
            ListCollectionView collection = values[0] as ListCollectionView;
            if (values[1] == null)
            {
                collection.Filter = null;
                return Task.CompletedTask;
            }
            JobStatus status = (JobStatus)values[1];
            switch (status)
            {
                case JobStatus.Failure:
                    {
                        collection.Filter = new Predicate<object>(x => (x as JobModel).Status == JobStatus.Failure);
                        break;
                    }
                case JobStatus.Success:
                    {
                        collection.Filter = new Predicate<object>(x => (x as JobModel).Status == JobStatus.Success);

                        break;
                    }
           
            }
            return Task.CompletedTask;
        }

        private Task OnFilter(object arg)
        {
            var values = arg as object[];
            ListCollectionView collection = values[0] as ListCollectionView;
            if(values[1]==null)
            {
                collection.Filter = null;
                return Task.CompletedTask;
            }
            JobTitle title = (JobTitle)values[1];
            switch (title)
            {
                case JobTitle.MakeDirectory:
                    {
                        collection.Filter = new Predicate<object>(x => (x as JobModel).Title == JobTitle.MakeDirectory);
                        break;
                    }
                case JobTitle.Delete:
                    {
                        collection.Filter = new Predicate<object>(x => (x as JobModel).Title == JobTitle.Delete);

                        break;
                    }
                case JobTitle.Upload:
                    {
                        collection.Filter = new Predicate<object>(x => (x as JobModel).Title == JobTitle.Upload);

                        break;
                    }
                case JobTitle.Download:
                    {
                        collection.Filter = new Predicate<object>(x => (x as JobModel).Title == JobTitle.Download);

                        break;
                    }
                case JobTitle.Rename:
                    {
                        collection.Filter = new Predicate<object>(x => (x as JobModel).Title == JobTitle.Rename);

                        break;
                    }
            }
            return Task.CompletedTask;
        }

        private async Task OnRefresh(object arg)
        {
            IsBusy = true;
            await FTP.RefreshMap();
            IsBusy = false;
        }

        private bool CanButtonsWork(object arg)
        {
            return SelectedMap != null;
        }

        private Task OnSetSelectedMap(FTPMapModel arg)
        {
            SelectedMap = arg;
            return Task.CompletedTask;
        }

        private Task OnCopyPath(FTPMapModel arg)
        {
            Clipboard.SetText(arg.Path);

            return Task.CompletedTask;
        }

        private Task OnCopyName(FTPMapModel arg)
        {
            Clipboard.SetText(arg.Name);

            return Task.CompletedTask;
        }

        private Task OnDelete(FTPMapModel arg)
        {
            Jobs.Add(new JobModel()
            {
                Type = arg.Type,
                Title = Enums.JobTitle.Delete,
                Name = arg.Name,
                FtpAddress = arg.Path,
                Status = Enums.JobStatus.Pending,
                FtpMap = arg
            });

            return Task.CompletedTask;
        }

        private Task OnDownload(FTPMapModel arg)
        {
            Jobs.Add(new JobModel()
            {
                Type = arg.Type,
                Title = Enums.JobTitle.Download,
                Name = arg.Name,
                FtpAddress = arg.Path,
                LocalAddress = App.DefaultDownloadPath,
                Status = Enums.JobStatus.Pending,
                Size = arg.Size??0,
                FtpMap = arg
            });

            return Task.CompletedTask;

        }

        private Task OnRename(FTPMapModel arg)
        {
            string newName = "";
            WindowsService windowsService = new WindowsService();
            windowsService.OpenInputBox("New Name", ref newName);

            if (!string.IsNullOrEmpty(newName))
            {

                Jobs.Add(new JobModel()
                {
                    Type = arg.Type,
                    Title = Enums.JobTitle.Rename,
                    Name = arg.Name,
                    FtpAddress = arg.Path,
                    Status = Enums.JobStatus.Pending,
                    NewName = newName,
                    FtpMap = arg,
                });
            }
            return Task.CompletedTask;
        }

        private Task OnCreateFolder(FTPMapModel arg)
        {
            string newName = "";
            WindowsService windowsService = new WindowsService();
            windowsService.OpenInputBox("Folder Name", ref newName);

            if (!string.IsNullOrEmpty(newName))
            {
                Jobs.Add(new JobModel()
                {
                    Type = FtpType.Directory,
                    Title = Enums.JobTitle.MakeDirectory,
                    Name = newName,
                    FtpAddress = arg.Type == FtpType.Directory ? arg.Path : arg.Parent?.Path,
                });
            }
            return Task.CompletedTask;
        }

        private async Task OnAddFolder(FTPMapModel arg)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                await EnumerateFolders(folderBrowser.SelectedPath, arg.Type == FtpType.Directory ? arg.Path : arg.Parent?.Path);
            }
        }

        private Task OnAddFile(FTPMapModel arg)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    string ftpPath = arg.Type == FtpType.Directory ? arg.Path : arg.Parent?.Path;
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
            }

            return Task.CompletedTask;
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
        //public ManageViewModel()
        //{
        //    AddFileCommand = new AsyncRelayCommand<object>(OnAddFile);
        //    AddFolderCommand = new AsyncRelayCommand<object>(OnAddFolder);
        //    CreateFolderCommand = new AsyncRelayCommand<object>(OnCreateFolder);
        //    RenameCommand = new AsyncRelayCommand<FTPMapModel>(OnRename);
        //    DownloadCommand = new AsyncRelayCommand<FTPMapModel>(OnDownload);
        //    DeleteCommand = new AsyncRelayCommand<FTPMapModel>(OnDelete);

        //    CopyNameCommand = new AsyncRelayCommand<FTPMapModel>(OnCopyName);
        //    CopyPathCommand = new AsyncRelayCommand<FTPMapModel>(OnCopyPath);
        //    //FtpModel = new FTPModel("gfdgfdgd", "gfdgdf", new byte[255]);

        //    LoginModel = new LoginModel()
        //    {
        //        Username =FTP.FTPModel.Username,
        //        Password = FTP.FTPModel.Password,
        //        Server = FTP.FTPModel.ServerBasePath
        //    };

        //    Jobs.Add(new JobModel() { Caption = "Caption", Percentage = 76, Status = Enums.JobStatus.Canceled, Type = FtpType.Directory, Title = Enums.JobTitle.MakeDirectory });
        //    Jobs.Add(new JobModel() { Caption = "Caption", Percentage = 23, Status = Enums.JobStatus.Failure, Type = FtpType.File, Title = Enums.JobTitle.Download });
        //    Jobs.Add(new JobModel() { Caption = "Caption", Percentage = 55, Status = Enums.JobStatus.Operating, Type = FtpType.Directory, Title = Enums.JobTitle.Download });
        //    Jobs.Add(new JobModel() { Caption = "Caption", Percentage = 10, Status = Enums.JobStatus.Pending, Type = FtpType.Directory, Title = Enums.JobTitle.Rename });
        //    Jobs.Add(new JobModel() { Caption = "Caption", Percentage = 0, Status = Enums.JobStatus.Retry, Type = FtpType.File, Title = Enums.JobTitle.Upload });
        //    Jobs.Add(new JobModel() { Caption = "Caption", Percentage = 100, Status = Enums.JobStatus.Success, Type = FtpType.Directory, Title = Enums.JobTitle.Delete });


        //    var map = new FTPMapModel()
        //    {
        //        Name = "map",
        //        CreateTime = DateTime.Now,
        //        Path = "c",
        //        Size = 2222324,
        //        Type = FtpType.Directory,
        //    };

        //    map.Children.Add(new FTPMapModel()
        //    {
        //        Name = "map",
        //        CreateTime = DateTime.Now,
        //        Path = "c",
        //        Size = 2222324,
        //        Type = FtpType.Directory,
        //    });
        //    map.Children.Add(new FTPMapModel()
        //    {
        //        Name = "map222",
        //        CreateTime = DateTime.Now,
        //        Path = "cvvv",
        //        Size = 2222324,
        //        Type = FtpType.File,
        //    });
        //    map.Children.Add(new FTPMapModel()
        //    {
        //        Name = "map222",
        //        CreateTime = DateTime.Now,
        //        Path = "cvvv",
        //        Size = 65464,
        //        Type = FtpType.File,
        //    });

        //    FTP.FTPModel.Map.Children.Add(map);
        //}
    }
}

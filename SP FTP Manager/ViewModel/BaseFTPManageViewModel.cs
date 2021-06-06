using SP_FTP_Manager.Commands;
using SP_FTP_Manager.Helper;
using SP_FTP_Manager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP_FTP_Manager.ViewModel
{
    public class BaseFTPManageViewModel : BaseViewModel
    {
        private ObservableCollection<Model.JobModel> jobs = new ObservableCollection<JobModel>();
        private bool isApplying = false;
        CancellationTokenSource JobCancellationTokenSource = new CancellationTokenSource();
        CancellationTokenSource AllJobsCancellationTokenSource = new CancellationTokenSource();
        private JobHandler JobHandler = new JobHandler();
        protected LoginModel LoginModel;
        private string message;

        private int filesToUploadCount;
        private int filesToDeleteCount;
        private int foldersToCreateCount;
        private int filesToDownloadCount;
        private int filesToRenameCount;


        public int FilesToUploadCount
        {
            get { return filesToUploadCount; }
            set { filesToUploadCount = value; OnPropertyChanged(); }
        }

        public int FoldersToCreateCount
        {
            get { return foldersToCreateCount; }
            set { foldersToCreateCount = value; OnPropertyChanged(); }
        }

        public int FilesToDeleteCount
        {
            get { return filesToDeleteCount; }
            set { filesToDeleteCount = value; OnPropertyChanged(); }
        }

        public int FilesToDownloadCount
        {
            get { return filesToDownloadCount; }
            set { filesToDownloadCount = value; OnPropertyChanged(); }
        }

        public int FilesToRenameCount
        {
            get { return filesToRenameCount; }
            set { filesToRenameCount = value; OnPropertyChanged(); }
        }

        public int FailedCount
        {
            get => failedCount;
            set { failedCount = value; OnPropertyChanged(); } 
        }
        private int successCount;

        public int SuccessCount
        {
            get { return successCount; }
            set { successCount = value; OnPropertyChanged(); }
        }

        public bool IsApplying
        {
            get => isApplying; set
            {
                isApplying = value;
                IsBusy = value;
                ApplyCommand.OnCanExecuteChanged();
                CancelCurrentJobCommand.OnCanExecuteChanged();
                OnPropertyChanged();
            }
        }
        private FTP ftp;
        private int failedCount;

        public FTP FTP
        {
            get { return ftp; }
            set { ftp = value; OnPropertyChanged(); }
        }

        public string Message { get => message; set { message = value; OnPropertyChanged(); } }


        public ObservableCollection<JobModel> Jobs { get => jobs; set => jobs = value; }


        public AsyncRelayCommand<JobModel> IgnoreCommand { get; set; }
        public AsyncRelayCommand<JobModel> DeleteJobCommand { get; set; }
        public AsyncRelayCommand<JobModel> RetryCommand { get; set; }
        public AsyncRelayCommand<JobModel> CancelCurrentJobCommand { get; set; }
        public AsyncRelayCommand<object> RetryAllCommand { get; set; }
        public AsyncRelayCommand<object> RetryAllFailedCommand { get; set; }
        public AsyncRelayCommand<object> ClearAllCommand { get; set; }
        public AsyncRelayCommand<object> ClearSuccessfulJobsCommand { get; set; }


        public AsyncRelayCommand<object> ApplyCommand { get; set; }

        //public BaseFTPManageViewModel()
        //{
        //    DeleteJobCommand = new AsyncRelayCommand<JobModel>(OnDeleteJob);
        //    RetryAllCommand = new AsyncRelayCommand<object>(OnRetryAll);
        //    RetryAllFailedCommand = new AsyncRelayCommand<object>(OnRetryAllFailed);
        //    RetryCommand = new AsyncRelayCommand<JobModel>(OnRetry);
        //    ClearAllCommand = new AsyncRelayCommand<object>(OnClearAll);
        //    ClearSuccessfulJobsCommand = new AsyncRelayCommand<object>(OnClear);
        //    IgnoreCommand = new AsyncRelayCommand<JobModel>(OnIgnore);
        //    CancelCurrentJobCommand = new AsyncRelayCommand<JobModel>(OnCancelCurrent, CanCancelCurrent);

        //    ApplyCommand = new AsyncRelayCommand<object>(OnApply, allowMultipleExecution: true);

        //    Jobs.CollectionChanged += Jobs_CollectionChanged;
        //}



        public BaseFTPManageViewModel(LoginModel loginModel, bool isSendOnly)
        {
            DeleteJobCommand = new AsyncRelayCommand<JobModel>(OnDeleteJob,CanExecute);
            RetryAllCommand = new AsyncRelayCommand<object>(OnRetryAll, CanExecute);
            RetryAllFailedCommand = new AsyncRelayCommand<object>(OnRetryAllFailed, CanExecute);
            RetryCommand = new AsyncRelayCommand<JobModel>(OnRetry, CanExecute);
            ClearAllCommand = new AsyncRelayCommand<object>(OnClearAll, CanExecute);
            ClearSuccessfulJobsCommand = new AsyncRelayCommand<object>(OnClearAllSuccesses, CanExecute);
            IgnoreCommand = new AsyncRelayCommand<JobModel>(OnIgnore, CanExecute);
            CancelCurrentJobCommand = new AsyncRelayCommand<JobModel>(OnCancelCurrent, CanCancelCurrent);

            ApplyCommand = new AsyncRelayCommand<object>(OnApply, allowMultipleExecution: true);

            //if (LoginModel == null)
            //{
            //    LoginModel = new LoginModel()
            //    {
            //        Server = "ftp://FTPTest.somee.com/www.FTPTest.somee.com",
            //        Username = "AmirSp",
            //        Password = PasswordCipher.Encrypt("Paydar123")
            //    };
            //}
            LoginModel = loginModel;
            FTP = new FTP(LoginModel.Server,
                          LoginModel.Username,
                          LoginModel.Password,
                          isSendOnly);
            Jobs.CollectionChanged += Jobs_CollectionChanged;
        }

        private bool CanExecute(object arg)
        {
            return !IsApplying;
        }

        private void Jobs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null && e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                switch ((e.NewItems[0] as JobModel).Title)
                {
                    case Enums.JobTitle.Delete:
                        {
                            FilesToDeleteCount++;
                            break;
                        }
                    case Enums.JobTitle.Download:
                        {
                            FilesToDownloadCount++;
                            break;
                        }
                    case Enums.JobTitle.MakeDirectory:
                        {
                            FoldersToCreateCount++;
                            break;
                        }
                    case Enums.JobTitle.Rename:
                        {
                            FilesToRenameCount++;
                            break;
                        }
                    case Enums.JobTitle.Upload:
                        {
                            FilesToUploadCount++;
                            break;
                        }
                }
            }
            else if (e?.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                switch ((e.OldItems[0] as JobModel).Title)
                {
                    case Enums.JobTitle.Delete:
                        {
                            FilesToDeleteCount--;
                            break;
                        }
                    case Enums.JobTitle.Download:
                        {
                            FilesToDownloadCount--;
                            break;
                        }
                    case Enums.JobTitle.MakeDirectory:
                        {
                            FoldersToCreateCount--;
                            break;
                        }
                    case Enums.JobTitle.Rename:
                        {
                            FilesToRenameCount--;
                            break;
                        }
                    case Enums.JobTitle.Upload:
                        {
                            FilesToUploadCount--;
                            break;
                        }
                }

                switch ((e.OldItems[0] as JobModel).Status)
                {
                    case Enums.JobStatus.Failure:
                        {
                            FailedCount--;
                            break;
                        }
                    case Enums.JobStatus.Success:
                        {
                            SuccessCount--;
                            break;
                        }
                }
            }
        }
        private async Task Cancel()
        {
            await JobHandler?.CancelJob();
            AllJobsCancellationTokenSource.Cancel();
            AllJobsCancellationTokenSource = new CancellationTokenSource();
            JobCancellationTokenSource = new CancellationTokenSource();
            IsApplying = false;
        }


        private async Task OnApply(object arg)
        {
            if (IsApplying)
            {
                await Cancel();
                //return;
            }

            IsApplying = true;
            foreach (var j in Jobs.Where(x => x.Status == Enums.JobStatus.Pending || x.Status == Enums.JobStatus.Retry))
            {
                if (AllJobsCancellationTokenSource.IsCancellationRequested)
                {
                    Message = "Canceled!";

                    IsApplying = false;
                    return;
                }
                JobHandler.CurrentJob = j;

                if (await JobHandler.Start(FTP, JobCancellationTokenSource))
                {
                    SuccessCount++;
                }
                else
                {
                    if (JobHandler.CurrentJob.Status == Enums.JobStatus.Failure)
                    {
                        FailedCount++;
                    }
                }
            }

            IsApplying = false;
        }
        private bool CanCancelCurrent(JobModel arg)
        {
            return IsApplying;
        }

        private Task OnCancelCurrent(JobModel arg)
        {
            JobCancellationTokenSource.Cancel();
            JobCancellationTokenSource = new CancellationTokenSource();
            return Task.CompletedTask;
        }

        private Task OnIgnore(JobModel arg)
        {
            if (!IsApplying)
            {
                arg.Status = Enums.JobStatus.Ignored;
            }
            return Task.CompletedTask;
        }

        private Task OnClearAllSuccesses(object arg)
        {
            if (!IsApplying)
            {
               
                var succesfullJobs = jobs.Where(x => x.Status == Enums.JobStatus.Success);
                foreach (var sj in succesfullJobs)
                {
                    jobs.Remove(sj);
                }
            }
            return Task.CompletedTask;
        }

        private Task OnClearAll(object arg)
        {
            if (!IsApplying)
            {
                FailedCount = SuccessCount = 0;
                FilesToDeleteCount = FilesToDownloadCount = FilesToRenameCount = FilesToUploadCount = FoldersToCreateCount = 0;
                jobs.Clear();
            }
            return Task.CompletedTask;
        }

        private Task OnRetry(JobModel arg)
        {
            arg.Status = Enums.JobStatus.Retry;
            return Task.CompletedTask;
        }

        private Task OnRetryAllFailed(object arg)
        {
            foreach (var job in Jobs.Where(x => x.Status == Enums.JobStatus.Failure))
            {
                job.Status = Enums.JobStatus.Retry;
            }
            return Task.CompletedTask;
        }

        private Task OnRetryAll(object arg)
        {
            foreach (var job in Jobs)
            {
                job.Status = Enums.JobStatus.Retry;
            }
            return Task.CompletedTask;
        }

        private Task OnDeleteJob(JobModel arg)
        {
            if (IsBusy) { return null; }
            jobs.Remove(arg);
            if (arg.Status == Enums.JobStatus.Success)
            {
                SuccessCount--;
            }
            else if (arg.Status == Enums.JobStatus.Failure)
            {
                FailedCount--;
            }


            return Task.CompletedTask;
        }

    }
}

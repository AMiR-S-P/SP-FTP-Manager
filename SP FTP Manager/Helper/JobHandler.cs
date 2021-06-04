using SP_FTP_Manager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SP_FTP_Manager.Enums;
using System.Diagnostics;

namespace SP_FTP_Manager.Helper
{
    public class JobHandler
    {
        CancellationTokenSource CancellationTokenSource;


        public JobModel CurrentJob { get; set; }

        public JobHandler(JobModel currentJob)
        {
            CurrentJob = currentJob;
        }
        public JobHandler()
        {

        }

        public Task CancelJob()
        {
            CancellationTokenSource.Cancel();
            CurrentJob.Status = JobStatus.Canceled;
            CurrentJob.IsNotDoingJob = true;

            return Task.CompletedTask;
        }

        public async Task<bool> Start(FTP ftp, CancellationTokenSource cancellationTokenSource)
        {
            CancellationTokenSource = cancellationTokenSource;
            switch (CurrentJob.Title)
            {
                case JobTitle.Download:
                    {
                        return await Download(ftp);
                    }
                case JobTitle.Upload:
                    {
                        return await Upload(ftp);
                    }
                case JobTitle.Rename:
                    {
                        return await Rename(ftp);
                    }
                case JobTitle.Delete:
                    {
                        return await Delete(ftp);
                    }
                case JobTitle.MakeDirectory:
                    {
                        return await MakeDirectory(ftp);
                    }
            }
            return false;
        }



        async Task<bool> Download(FTP ftp)
        {
            CurrentJob.IsNotDoingJob = false;
            if (string.IsNullOrEmpty(CurrentJob.FtpAddress))
            {
                throw new Exception($"Ftp Address cannot be empty!");
            }
            if (string.IsNullOrEmpty(CurrentJob.LocalAddress))
            {
                throw new Exception($"Local Address cannot be empty!");
            }

            try
            {
                CurrentJob.Status = JobStatus.Operating;
                if (CurrentJob.Type == FtpType.File)
                {
                    await foreach (var p in ftp.DownloadFile(CurrentJob.FtpMap, CurrentJob.FtpAddress, CurrentJob.LocalAddress, CurrentJob.Name, CancellationTokenSource.Token))
                    {
                        CurrentJob.Percentage = p;
                    }
                }
                else
                {
                    await foreach (var p in ftp.DownloadDirectory(CurrentJob.FtpMap, CurrentJob.FtpAddress, CurrentJob.LocalAddress, CurrentJob.Name, CancellationTokenSource.Token))
                    {
                        CurrentJob.Percentage = p;
                    }
                }
                CurrentJob.Status = JobStatus.Success;
                CurrentJob.Percentage = 100;
                return true;
            }
            catch (Exception ex)
            {
                if (CurrentJob.Percentage == -1)
                {
                    CurrentJob.Caption = "Canceled.";
                    CurrentJob.Status = JobStatus.Failure;
                    return false;
                }
                CurrentJob.Caption = $"Download failed: {ex.Message}.";

                CurrentJob.Status = JobStatus.Failure;
                return false;
            }
            finally
            {
                CurrentJob.IsNotDoingJob = true;
            }
        }
        async Task<bool> Upload(FTP ftp)
        {
            CurrentJob.IsNotDoingJob = false;

            if (string.IsNullOrEmpty(CurrentJob.FtpAddress))
            {
                throw new Exception($"Ftp Address cannot be empty!");
            }
            if (string.IsNullOrEmpty(CurrentJob.LocalAddress))
            {
                throw new Exception($"Local Address cannot be empty!");
            }

            try
            {
                CurrentJob.Status = JobStatus.Operating;
                await foreach (var p in ftp.UploadFile(CurrentJob.LocalAddress, CurrentJob.FtpAddress, CurrentJob.Name, CancellationTokenSource.Token))
                {
                    CurrentJob.Percentage = p;
                }
                CurrentJob.Status = JobStatus.Success;
                CurrentJob.Percentage = 100;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (CurrentJob.Percentage == -1)
                {
                    CurrentJob.Caption = "Canceled.";
                    CurrentJob.Status = JobStatus.Canceled;
                    return false;
                }

                CurrentJob.Caption = $"Upload failed: {ex.Message}.";
                CurrentJob.Status = JobStatus.Failure;
                return false;
            }

            finally
            {
                CurrentJob.IsNotDoingJob = true;

            }
        }
        async Task<bool> Rename(FTP ftp)
        {
            CurrentJob.IsNotDoingJob = false;

            if (string.IsNullOrEmpty(CurrentJob.FtpAddress))
            {
                throw new Exception($"Ftp Address cannot be empty!");
            }
            if (string.IsNullOrEmpty(CurrentJob.NewName))
            {
                throw new Exception($"New name cannot be empty!");
            }

            try
            {
                CurrentJob.Status = JobStatus.Operating;
                CurrentJob.Caption = $"New Name: {CurrentJob.NewName}.";

                await foreach (var p in ftp.RenameFile(CurrentJob.FtpAddress, CurrentJob.NewName, CancellationTokenSource.Token))
                {
                    CurrentJob.Percentage = p;
                }
                CurrentJob.Status = JobStatus.Success;
                CurrentJob.Percentage = 100;
                return true;
            }
            catch (Exception ex)
            {
                if (CurrentJob.Percentage == -1)
                {
                    CurrentJob.Caption = "Canceled.";
                    CurrentJob.Status = JobStatus.Failure;
                    return false;
                }
                CurrentJob.Status = JobStatus.Failure;
                return false;
            }
            finally
            {
                CurrentJob.IsNotDoingJob = true;

            }
        }
        async Task<bool> Delete(FTP ftp)
        {
            CurrentJob.IsNotDoingJob = false;

            if (string.IsNullOrEmpty(CurrentJob.FtpAddress))
            {
                throw new Exception($"Ftp Address cannot be empty!");
            }


            try
            {
                CurrentJob.Status = JobStatus.Operating;

                if (CurrentJob.Type == FtpType.Directory)
                {
                    await foreach (var p in ftp.DeleteDirectory(CurrentJob.FtpMap, CurrentJob.FtpAddress, CancellationTokenSource.Token))
                    {
                        CurrentJob.Percentage = p;
                    }
                }
                else if (CurrentJob.Type == FtpType.File)
                {
                    await foreach (var p in ftp.DeleteFile(CurrentJob.FtpMap, CurrentJob.FtpAddress, CancellationTokenSource.Token))
                    {
                        CurrentJob.Percentage = p;
                    }
                }
                CurrentJob.Status = JobStatus.Success;
                return true;
            }
            catch (Exception ex)
            {
                if (CurrentJob.Percentage == -1)
                {
                    CurrentJob.Caption = "Canceled.";
                    CurrentJob.Status = JobStatus.Failure;
                    return false;
                }

                CurrentJob.Status = JobStatus.Failure;
                return true;
            }
            finally
            {
                CurrentJob.Percentage = 100;
                CurrentJob.IsNotDoingJob = true;
            }
        }
        async Task<bool> MakeDirectory(FTP ftp)
        {
            CurrentJob.IsNotDoingJob = false;

            if (string.IsNullOrEmpty(CurrentJob.FtpAddress))
            {
                throw new Exception($"Ftp Address cannot be empty!");
            }
            if (string.IsNullOrEmpty(CurrentJob.Name))
            {
                throw new Exception($"Name cannot be empty!");
            }

            try
            {
               CurrentJob.Status = JobStatus.Operating;

                await foreach (var p in ftp.CreateDirectory(CurrentJob.FtpAddress, CurrentJob.Name, CancellationTokenSource.Token))
                {
                    CurrentJob.Percentage = p;
                }

                CurrentJob.Status = JobStatus.Success;
                CurrentJob.Percentage = 100;
                return true;

            }
            catch (Exception ex)
            {
                if (CurrentJob.Percentage == -1)
                {
                    CurrentJob.Caption = "Canceled.";
                    CurrentJob.Status = JobStatus.Failure;
                    return false;
                }


                CurrentJob.Status = JobStatus.Failure;
                CurrentJob.Caption = $"Creation failed: {ex.Message}.";
                return false;
            }
            finally
            {
                CurrentJob.IsNotDoingJob = true;
            }
        }
    }
}

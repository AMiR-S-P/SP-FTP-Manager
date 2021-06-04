using SP_FTP_Manager.Enums;
using SP_FTP_Manager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP_FTP_Manager.Helper
{
    public class FTP : INotifyPropertyChanged
    {
        public FTPModel FTPModel { get; private set; }
        //private NetworkCredential Credential { set; get; }
        private int filesUploadedCount;
        private int filesDownloadedCount;
        private int filesDeletedCount;
        private int foldersCreatedCount;
        private int filesRenamedCount;


        public int FilesUploadedCount
        {
            get { return filesUploadedCount; }
            private set { filesUploadedCount = value; OnPropertyChanged(); }
        }

        public int FoldersCreatedCount
        {
            get { return foldersCreatedCount; }
            private set { foldersCreatedCount = value; OnPropertyChanged(); }
        }

        public int FilesDownloadedCount
        {
            get { return filesDownloadedCount; }
            private set { filesDownloadedCount = value; OnPropertyChanged(); }
        }

        public int FilesDeletedCount
        {
            get { return filesDeletedCount; }
            set { filesDeletedCount = value; }
        }

        public int FilesRenamedCount
        {
            get { return filesRenamedCount; }
            set { filesRenamedCount = value; }
        }


        public bool IsSendoOnlyMode { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string pName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
        }
        public FTP(string serverBasePath, string username, byte[] password, bool isSendOnlyMode)
        {
            FTPModel = new FTPModel(serverBasePath, username, password);
            FTPModel.Map = new FTPMapModel()
            {
                Name = serverBasePath,
                Path = serverBasePath,
                Type = FtpType.Directory
            };
            IsSendoOnlyMode = isSendOnlyMode;
            //Credential = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password));
        }


        async Task<FtpWebRequest> InitFtpWebRequest(string filePath)
        {
            FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(filePath);
            ftp.Credentials = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password));
            ftp.ServicePoint.ConnectionLimit = 4;
            ftp.ConnectionGroupName = "SPFTPM";
            ftp.KeepAlive = true;

            if (App.Settings.HasProxy)
            {
                ftp.Proxy = new WebProxy(App.Settings.Address,int.Parse(App.Settings.Port));
            }

            return await Task.FromResult(ftp);
        }

        public async Task<bool> FileExistsOnlineAsync(string filePath)
        {
            try
            {
                //WebRequest ftp = FtpWebRequest.Create(filePath);
                //ftp.Credentials = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password));

                WebRequest ftp = await InitFtpWebRequest(filePath);
                ftp.Method = WebRequestMethods.Ftp.GetFileSize;


                var response = await ftp.GetResponseAsync();
                if (response.ContentLength > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        /// <summary>
        /// just check root 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool FileExists(string fileName)
        {
            return FTPModel.Map.Children.Any(x => x.Name == fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath">just folder name from root  (e.g  cs,lang,... Not www.test.com/test/cs) </param>
        /// <returns></returns>
        public bool DirectoryExistsInMap(string directoryPath)
        {
            return FTPModel.Map.Children.Any(x => x.Path == directoryPath);
        }
        //public async Task<bool> DirectoryExistsInServer(string directoryPath)
        //{
        //    WebRequest ftp = FtpWebRequest.Create(directoryPath);
        //    ftp.Credentials = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password));
        //    ftp.Method = WebRequestMethods.Ftp.GetDateTimestamp;

        //    FtpWebResponse response = await ftp.GetResponseAsync() as FtpWebResponse;
        //    if (response.StatusDescription.Contains("257"))
        //    {
        //        return true;
        //    } 
        //    return false;
        //}
        /// <summary>
        /// folder name ,search root only not deep
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        public bool DirectoryExistsName(string directoryName)
        {
            return FTPModel.Map.Children.Any(x => x.Name == directoryName);
        }
        public async Task RefreshMap()
        {
            try
            {
                FTPModel.IsNotListing = false;
                FTPModel.Map.Children.Clear();
                await FillMap(FTPModel.Map);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                FTPModel.IsNotListing = true;
            }
        }
        async Task FillMap(FTPMapModel map)
        {
            try
            {
                FtpWebRequest ftp = await InitFtpWebRequest(map.Path);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                var response = await ftp.GetResponseAsync();

                System.IO.Stream stream = response.GetResponseStream();
                System.IO.StreamReader streamReader = new System.IO.StreamReader(stream);


                while (!streamReader.EndOfStream)
                {
                    try
                    {
                        string fileString = await streamReader.ReadLineAsync();
                        
                        var parse = await DirectoryDetailsParser(fileString);

                        if (parse.Item2 == "<DIR>" || parse.Item2 == "d")//it's folder
                        {
                            var newMap = new FTPMapModel()
                            {
                                Name = parse.Item4,
                                CreateTime = parse.Item1,
                                Parent = map,
                                Path = Path.Combine(map.Path, parse.Item4).Replace("\\", "/"),
                                Size = null,
                                Type = FtpType.Directory

                            };

                            //get files that is in this folder
                            await FillMap(newMap);
                            map.Children.Add(newMap);
                        }
                        else if (string.IsNullOrEmpty(parse.Item2) || parse.Item2 == "-")//it's file
                        {
                            var newMap = new FTPMapModel()
                            {
                                Name = parse.Item4,
                                CreateTime = parse.Item1,
                                Parent = map,
                                Path = Path.Combine(map.Path, parse.Item4).Replace("\\", "/"),
                                Size = parse.Item3,
                                Type = FtpType.File
                            };
                            map.Children.Add(newMap);
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            catch (Exception ex)
            {
            }
        }
        async Task<(DateTime, string, long, string)> DirectoryDetailsParser(string detail)
        {
            await Task.Run(() => { });
            if (string.IsNullOrEmpty(detail))
            {
                throw new Exception("Value connot be null.");
            }

            System.Text.RegularExpressions.Regex regexWindows = new System.Text.RegularExpressions.Regex(@"([0-9]{2}-[0-9]{2}-[0-9]{2}[\s]+[0-9]{2}:[0-9]{2}[A|P]+[M])[\s]*([<DIR>]*)([\d]*)[\s]*([\S|\s]*[^""])");
            System.Text.RegularExpressions.Match matchWindows = regexWindows.Match(detail);

            if (matchWindows.Success)
            {

                DateTime time = DateTime.MinValue;
                //string dateTimeString = matchWindows.Groups[1].Value[matchWindows.Groups[1].Value.Length - 2] == 'A' ?
                //                        matchWindows.Groups[1].Value.Replace("AM", " AM") :
                //                        matchWindows.Groups[1].Value.Replace("PM", " PM");

                var s = DateTime.TryParseExact(matchWindows.Groups[1].Value,
                                              "MM-dd-yy hh:mmtt",
                                              new System.Globalization.CultureInfo("en-us"),
                                              System.Globalization.DateTimeStyles.AllowInnerWhite,
                                              out time);

                string dir = matchWindows.Groups[2].Value;
                long size = 0;
                if (string.IsNullOrEmpty(dir))
                {
                    size = long.Parse(matchWindows.Groups[3].Value);
                }
                string name = matchWindows.Groups[4].Value;

                return (time, dir, size, name);
            }


            System.Text.RegularExpressions.Regex regexLinux = new System.Text.RegularExpressions.Regex(@"(([d|-]?)[r|w|x|-]+)\s+\d*\s+[group|other|owner|0-9]*\s+[group|other|owner|0-9]*\s+[group|other|owner|0-9]*\s+([0-9]*)\s+(\w{3})\s+(\d{2})\s*([\d]{4})?\s*([0-9]{2}:[0-9]{2})?\s*(.*)");
            /*
            g1: permissions drwxr-xr-x
            g2: d or -
            g3:size
            g4:month
            g5:day
            g6:year
            g7:Time
            g8:fileName
            */
            System.Text.RegularExpressions.Match matchLinux = regexLinux.Match(detail);
            if (matchLinux.Success)
            {
                DateTime time = DateTime.MinValue;

                string year = string.IsNullOrEmpty(matchLinux.Groups[6].Value) ? DateTime.Now.Year.ToString() : matchLinux.Groups[6].Value;
                string createdTime = string.IsNullOrEmpty(matchLinux.Groups[7].Value) ? "00:00" : matchLinux.Groups[7].Value;
                string timeString = $"{matchLinux.Groups[4].Value} {matchLinux.Groups[5].Value} { year} { createdTime}";
                var s = DateTime.TryParseExact(timeString,
                                              "MMM dd yyyy HH:mm",
                                              new System.Globalization.CultureInfo("en-us"),
                                              System.Globalization.DateTimeStyles.AllowInnerWhite,
                                              out time);

                string dir = matchLinux.Groups[2].Value;
                long size = long.Parse(matchLinux.Groups[3].Value);

                string name = matchLinux.Groups[8].Value;

                return (time, dir, size, name);
            }

            return (DateTime.MinValue, "", long.MinValue, "");
        }
        async Task<long> GetSize(string ftpPath)
        {
            try
            {
                WebRequest request = FtpWebRequest.Create(ftpPath);
                request.Credentials = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password));
                request.Method = WebRequestMethods.Ftp.GetFileSize;


                var response = (FtpWebResponse)(await request.GetResponseAsync());

                if (response.StatusDescription.Contains("213"))
                {
                    return response.ContentLength;
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async IAsyncEnumerable<int> DownloadDirectory(FTPMapModel ftp, string ftpPath, string localPath, string directoryName, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(ftpPath))
            {
                throw new Exception($"Ftp Address cannot be empty!");
            }
            if (string.IsNullOrEmpty(localPath))
            {
                throw new Exception($"Local Address cannot be empty!");
            }

            var dir = Directory.CreateDirectory(Path.Combine(localPath, directoryName));

            var map = FTPModel[ftpPath];
            int index = 0, count = await map.GetAllChildrenCount();

            foreach (var c in map.Children)
            {
                yield return (int)(++index * 100 / count);
                if (c.Type == FtpType.File)
                {
                    await foreach (var f in DownloadFile(c, c.Path, dir.FullName, c.Name, cancellationToken))
                    {
                    };
                }
                else
                {
                    await foreach (var d in DownloadDirectory(c, c.Path, dir.FullName, dir.Name, cancellationToken))
                    {
                    };
                }

            }

        }
        public async IAsyncEnumerable<int> DownloadFile(FTPMapModel ftp, string ftpPath, string localPath, string fileName, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(ftpPath))
            {
                throw new Exception($"Ftp Address cannot be empty!");
            }
            if (string.IsNullOrEmpty(localPath))
            {
                throw new Exception($"Local Address cannot be empty!");
            }

            //WebRequest webRequest = FtpWebRequest.Create(ftpPath);
            //webRequest.Credentials = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password));
            FtpWebRequest webRequest = await InitFtpWebRequest(ftpPath);
            webRequest.Method = WebRequestMethods.Ftp.DownloadFile;


       
            var response = await webRequest.GetResponseAsync();
            using (var ftpStream = response.GetResponseStream())
            {
                using (var fs = new FileStream(Path.Combine(localPath, fileName), FileMode.Create))
                {
                    long length = ftp.Size ?? 0;

                    int index = 0, count = 1024;
                    byte[] bytes = new byte[count];

                    while (((count = await ftpStream.ReadAsync(bytes, 0, count)) > 0))
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            yield return -1;
                            throw new Exception();
                        }
                        await fs.WriteAsync(bytes, 0, count);

                        index += count;
                        int percent = (int)(index * 100 / length);
                        yield return percent;

                        bytes = new byte[count];
                    }
                }
                FilesDownloadedCount++;
            }
            yield return 100;

        }
        public async IAsyncEnumerable<int> UploadFile(string localPath, string ftpPath, string fileName, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            fileName = fileName.Replace("\\", "");

            FileInfo fileInfo = new FileInfo(localPath);
            
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath + "/" + fileInfo.Name);
            //request.Credentials = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password)); ;
            FtpWebRequest request = await InitFtpWebRequest(ftpPath + "/" + fileInfo.Name);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            using (var ftpStream = await request.GetRequestStreamAsync())
            {
                using (var sr = new FileStream(localPath, FileMode.Open))
                {

                    int index = 0, count = 512;
                    byte[] bytes = new byte[count];

                    while ((count = await sr.ReadAsync(bytes, 0, count)) > 0)
                    {
                        if (cancellation.IsCancellationRequested)
                        {
                            yield return -1;
                            throw new OperationCanceledException();
                        }
                        await ftpStream.WriteAsync(bytes, 0, count);
                        index += count;
                        int percent = (int)(index * 100 / sr.Length);

                        yield return percent;
                    }

                    if (!IsSendoOnlyMode)
                    {
                        FTPMapModel map = FTPModel.Map[ftpPath];
                        var child = map.Children.Where(x => x.Name == fileName).FirstOrDefault();
                        if (child != null)
                        {
                            FTPModel.Map[ftpPath].Children.Remove(child);
                        }

                        map.Children.Add(new FTPMapModel()
                        {
                            Name = fileName,
                            Parent = FTPModel.Map[ftpPath],
                            CreateTime = DateTime.Now,
                            Size = fileInfo.Length,
                            Path = $"{ftpPath}/{fileName}",
                            Type = FtpType.File
                        });
                    }
                    FilesUploadedCount++;
                }

            }
            yield return 100;

        }
        public async IAsyncEnumerable<int> RenameFile(string ftpPath, string newName, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            if (string.IsNullOrEmpty(ftpPath))
            {
                throw new Exception($"Ftp Address cannot be empty!");
            }
            if (string.IsNullOrEmpty(newName) || string.IsNullOrWhiteSpace(newName))
            {
                throw new Exception($"New name cannot be empty!");
            }



            yield return 20;

            if (cancellation.IsCancellationRequested)
            {
                yield return -1;
                throw new Exception();
            }
            //FtpWebRequest webRequest = (FtpWebRequest)FtpWebRequest.Create(ftpPath);
            //webRequest.Credentials = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password));
            FtpWebRequest webRequest = await InitFtpWebRequest(ftpPath);
            webRequest.Method = WebRequestMethods.Ftp.Rename;
            //webRequest.RenameTo = Path.Combine(ftpPath.Substring(0, ftpPath.LastIndexOf("/")), newName);
            webRequest.RenameTo = newName;

            yield return 50;

            using (var response = await webRequest.GetResponseAsync())
            {
                var ftpMap = FTPModel.Map[ftpPath];
                ftpMap.Name = newName;
                ftpMap.Path = Path.Combine(ftpPath.Substring(0, ftpPath.LastIndexOf("/")), newName).Replace("\\", "/");


                yield return 100;
            }
            FilesRenamedCount++;

        }
        public async IAsyncEnumerable<int> DeleteDirectory(FTPMapModel ftp, string directoryPath, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            var root = FTPModel.Map[directoryPath].Children.ToList();
            int childSize = root.Count(), index = 0;

            //yield return 10;

            foreach (FTPMapModel d in root)
            {
                if (d.Type == FtpType.File)
                {

                    await foreach (var p in DeleteFile(FTPModel.Map, d.Path, cancellation))
                    {
                    }
                }
                else
                {
                    await foreach (var p in DeleteDirectory(FTPModel.Map, d.Path, cancellation))
                    {

                    }
                }
                yield return (++index / childSize) * 100;
            }

            //WebRequest rmDirectory = FtpWebRequest.Create(directoryPath);
            //rmDirectory.Credentials = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password));
            WebRequest rmDirectory = await InitFtpWebRequest(directoryPath);
            rmDirectory.Method = WebRequestMethods.Ftp.RemoveDirectory;

            FtpWebResponse rmResponse = (FtpWebResponse)await rmDirectory.GetResponseAsync();
            if (rmResponse.StatusDescription.Contains("250"))
            {
                var map = FTPModel.Map[directoryPath];
                map.Parent.Children.Remove(map);
                yield return 100;
                FilesDeletedCount++;
            }


        }
        public async IAsyncEnumerable<int> DeleteFile(FTPMapModel ftp, string filePath, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            var fileSize = ftp.Size ?? 0;
            WebRequest request = await InitFtpWebRequest(filePath);

            request.Method = WebRequestMethods.Ftp.DeleteFile;
            yield return 20;
            if (cancellation.IsCancellationRequested)
            {
                yield return -1;
                throw new Exception();
            }
            var response = (FtpWebResponse)(await request.GetResponseAsync());

            yield return 50;

            if (response.StatusDescription.Contains("250"))
            {
                var map = FTPModel.Map[filePath];
                map.Parent.Children.Remove(map);
                yield return 100;
                FilesDeletedCount++;
            }
        }
        public async IAsyncEnumerable<int> CreateDirectory(string ftpPath, string directoryName, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            if (string.IsNullOrEmpty(ftpPath))
            {
                throw new Exception($"Ftp Address cannot be empty!");
            }
            if (string.IsNullOrEmpty(directoryName) || string.IsNullOrWhiteSpace(directoryName))
            {
                throw new Exception($"Directory name cannot be empty!");
            }
            string path = $"{ftpPath}/{directoryName}";


            if (cancellation.IsCancellationRequested)
            {
                yield return -1;
                throw new Exception();
            }

                //FtpWebRequest webRequest = (FtpWebRequest)FtpWebRequest.Create(path);
                //webRequest.Credentials = new NetworkCredential(FTPModel.Username, PasswordCipher.Decrypt(FTPModel.Password));

                WebRequest webRequest = await InitFtpWebRequest(path);
                webRequest.Method = WebRequestMethods.Ftp.MakeDirectory;


                var response = (FtpWebResponse)await webRequest.GetResponseAsync();

                FoldersCreatedCount++;

                if (!IsSendoOnlyMode)
                {
                    var map = FTPModel.Map[ftpPath];

                    map?.Children.Add(new FTPMapModel()
                    {
                        Name = directoryName,
                        CreateTime = DateTime.Now,
                        Parent = map,
                        Path = ftpPath + "/" + directoryName,
                        Size = -1,
                        Type = FtpType.Directory
                    });
                }
                yield return 100;     
        }

    }
}

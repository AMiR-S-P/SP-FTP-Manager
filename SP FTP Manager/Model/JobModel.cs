using SP_FTP_Manager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_FTP_Manager.Model
{
   public class JobModel:BaseModel
    {
        private JobStatus status= JobStatus.Pending;
        private JobTitle title;
        private int percentage;
        private string ftpAddress;
        private string localAddress;
        private long size;
        private string name;
        private string newName;
        private string caption;
        private string description;
        private FtpType type;
        private bool isNotDoingJob;
        private FTPMapModel ftpMap;

        public FTPMapModel FtpMap { get => ftpMap; set { ftpMap = value; OnPropertyChanged(); } }
        public JobStatus Status { get => status; set { status = value; OnPropertyChanged(); } }
        public JobTitle Title { get => title; set { title = value; OnPropertyChanged(); } }
        public int Percentage { get => percentage; set { percentage = value; OnPropertyChanged(); } }
        public string FtpAddress { get => ftpAddress; set { ftpAddress = value; OnPropertyChanged(); } }
        public string LocalAddress { get => localAddress; set { localAddress = value; OnPropertyChanged(); } }
        public long Size { get => size; set { size = value; OnPropertyChanged(); } }
        public string Name { get => name; set { name = value; OnPropertyChanged(); } }
        public string NewName { get => newName; set { newName = value; OnPropertyChanged(); } }
        public string Caption { get => caption; set { caption = value; OnPropertyChanged(); } }
        public string Description { get => description; set { description = value; OnPropertyChanged(); } }
        public FtpType Type { get => type; set { type = value; } }
        public bool IsNotDoingJob { get => isNotDoingJob; set { isNotDoingJob = value; OnPropertyChanged(); } }
    }
}

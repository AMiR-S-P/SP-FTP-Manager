using SP_FTP_Manager.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_FTP_Manager.Model
{
    public class FTPModel : BaseModel
    {
        private FTPMapModel map;
        private bool isNotListing;
        private int filesCount;
        private int foldersCount;
        private long totalSize;

        public FTPMapModel this[string path]
        {
            get
            {
                return Map[path];
            }
        }
        public string ServerBasePath { get; }
        public string Username { get; }
        public byte[] Password { get; }
        public bool IsNotListing
        {
            get => isNotListing;
            set
            {
                isNotListing = value;
                this.OnPropertyChanged();
            }
        }

        public FTPMapModel Map { get => map; set { map = value; OnPropertyChanged(); } }

        public FTPModel(string serverBasePath, string username, byte[] password)
        {
            ServerBasePath = serverBasePath;
            Username = username;
            Password = password;


            Map = new FTPMapModel()
            {
                CreateTime = DateTime.Now,
                Name = serverBasePath,
                Parent = null,
                Path = serverBasePath,
                Size = 0,
                Type = FtpType.Directory
            };
        }

       
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_FTP_Manager.Model
{
    public class LoginModel : BaseModel
    {
        private bool remember;
        private byte[] password=new byte[] { };
        private string username;
        private string server;

        public string Server { get => server; set { server = value; OnPropertyChanged(); } }
        public string Username { get => username; set { username = value; OnPropertyChanged(); } }

        [JsonIgnore]
        public byte[] Password { get => password; set { password = value; OnPropertyChanged(); } }

        public bool Remember { get => remember; set { remember = value; OnPropertyChanged(); } }
    }
}

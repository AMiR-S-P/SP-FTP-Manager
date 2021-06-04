using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_FTP_Manager.Enums
{
    public enum JobStatus { Success = 1, Failure = 2, Pending = 4, Operating = 8, Retry = 16,Canceled = 32,Ignored = 64 }

}

using SP_FTP_Manager.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_FTP_Manager.Model
{

    public class FTPMapModel:BaseModel, IOrderedEnumerable<FTPMapModel>, IComparable
    {
        private string name;
        private ObservableCollection<FTPMapModel> children = new ObservableCollection<FTPMapModel>();
        private string path;
        public FTPMapModel this[string path]
        {
            get
            {
                if (path.StartsWith(Name))
                {
                    var p = path.Remove(0, Name.Count()).TrimStart('/');

                    if (string.IsNullOrEmpty(p))
                    {
                        return this;
                    }
                    else
                    {
                        foreach (var f in Children)
                        {
                            var val = f[p];
                            if (val != null)
                            {
                                return val;
                            }

                        }
                    }
                }

                return null;
            }
        }
        public FtpType Type { get; set; }
        public string Name { get => name; set { name = value; OnPropertyChanged(); } }
        public long? Size { get; set; }
        public string Path { get => path; set { path = value; OnPropertyChanged(); } }
        public DateTime CreateTime { set; get; }
        public FTPMapModel Parent { get; set; }
        public ObservableCollection<FTPMapModel> Children { get => children; set { children = value; OnPropertyChanged(); } }


        public IOrderedEnumerable<FTPMapModel> CreateOrderedEnumerable<TKey>(Func<FTPMapModel, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            return Children.OrderBy(keySelector);
        }

        public IEnumerator GetEnumerator()
        {
            List<FTPMapModel> list = new List<FTPMapModel>();
            list.Add(this);
            return list.GetEnumerator();
        }

        IEnumerator<FTPMapModel> IEnumerable<FTPMapModel>.GetEnumerator()
        {
            List<FTPMapModel> list = new List<FTPMapModel>();
            list.Add(this);
            return list.GetEnumerator();
        }
        public async Task<int> GetAllChildrenCount()
        {
            int count = 0;

            foreach(var c in Children)
            {
                if(c.Type == FtpType.File)
                {
                    count++;
                }
                else
                {
                    count += await c.GetAllChildrenCount();
                }
            }
            return count;
        }
        public int CompareTo(object obj)
        {
            FTPMapModel o = obj as FTPMapModel;

            if (Type == FtpType.Directory)
            {
                if (o.Type == FtpType.Directory)
                {
                    return Name.CompareTo(o.Name);
                }
                else if (o.Type == FtpType.File)
                {
                    return -1;
                }
            }
            else if (Type == FtpType.File)
            {
                if (o.Type == FtpType.File)
                {
                    return Name.CompareTo(o.Name);
                }
                else if (o.Type == FtpType.Directory)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}

//using SP_FTP_Manager.Model;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;
//using System.Windows.Data;

//namespace SP_FTP_Manager.Helper
//{
//    public class FilterableObservableCollection<T> : ObservableCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged,IEnumerable<T>
//    {
//        private Func<T, bool> filter;
//        ObservableCollection<T> list=new ObservableCollection<T>();



//        public Func<T, bool> Filter { get => filter; set { filter = value; OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move)); } }

//        //public event NotifyCollectionChangedEventHandler CollectionChanged;
//        public new event PropertyChangedEventHandler PropertyChanged;

//        void OnPropertyChanged([CallerMemberName] string pName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
//        }

//        //void OnCollectionChanged(NotifyCollectionChangedAction action, T? value, int? index)
//        //{
//        //    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, value, index));
//        //}
//        //void OnCollectionChanged(NotifyCollectionChangedAction action)
//        //{
//        //    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
//        //}

//        protected override void InsertItem(int index, T item)
//        {
//            base.InsertItem(index, item);
//            list.Add(item);

//        }

//        protected override void ClearItems()
//        {
//            base.ClearItems();
//            list.Clear();
//        }
//        protected override void RemoveItem(int index)
//        {
//            T removedItem = this[index];
//            base.RemoveItem(index);
//            list.RemoveAt(index);
//        }
//        protected override void SetItem(int index, T item)
//        {
//            base.SetItem(index, item);
//            list[index] = item;
//        }

//        public new IEnumerator<T> GetEnumerator()
//        {
//            if (Filter != null)
//            {
//                var bb =list.Where(Filter).GetEnumerator();
//                return bb;
//            }
//            return base.Items.GetEnumerator();
//        }

//        IEnumerator<T> IEnumerable<T>.GetEnumerator()
//        {
//            if (Filter != null)
//            {
//                return list.Where(Filter).GetEnumerator();
//            }
//            return base.Items.GetEnumerator();
//        }
//    }

//    public class test 
//    {
//        public test()
//        {
//            FilterableObservableCollection<FTPModel> vs = new FilterableObservableCollection<FTPModel>();
//            vs.Filter = new Func<FTPModel, bool>(x => x.IsNotListing == true);

//            //new ListView().ItemsSource = vs.Filter;
//        }

     
//    }
//}

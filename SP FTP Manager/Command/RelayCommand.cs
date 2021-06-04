using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace  SP_FTP_Manager.Commands
{
    public class RelayCommand<T> : ICommand
    {
        bool _Async = false;
        Action<T> _ExecuteMethod;
        Func<object, bool> _CanExecuteMethod;

        public RelayCommand(Action<T> executeMethod, bool isAsync = false)
        {
            _ExecuteMethod = executeMethod;
            _Async = isAsync;
        }
        public RelayCommand(Action<T> executeMethod, Func<object, bool> canExecuteMethod, bool isAsync = false)
        {
            _ExecuteMethod = executeMethod;
            _CanExecuteMethod = canExecuteMethod;
            _Async = isAsync;
        }

        public event EventHandler CanExecuteChanged;
        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Execute(object obj = null)
        {
            if (_Async)
            {
                if (_ExecuteMethod != null)
                {
                    System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker();
                    bw.DoWork += (s, e) =>
                    {
                        _ExecuteMethod((T)obj);
                    };
                    bw.RunWorkerAsync();
                }
            }
            else
            {
                _ExecuteMethod?.Invoke((T)obj);
            }
        }
        public bool CanExecute(object obj = null)
        {
            if (_CanExecuteMethod != null)
            {
                return _CanExecuteMethod(obj);
            }
            if (_ExecuteMethod != null)
            {
                return true;
            }
            return false;
        }
    }
}

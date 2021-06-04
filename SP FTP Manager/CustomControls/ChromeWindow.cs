using SP_FTP_Manager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SP_FTP_Manager.CustomControls
{
    public class ChromeWindow : Window
    {
        public bool CanMaximize
        {
            get { return (bool)GetValue(CanMaximizeProperty); }
            set { SetValue(CanMaximizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanMaximize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanMaximizeProperty =
            DependencyProperty.Register("CanMaximize", typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));



        public bool CanMinimize
        {
            get { return (bool)GetValue(CanMinimizeProperty); }
            set { SetValue(CanMinimizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanMinimize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.Register("CanMinimize", typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));


        /// <summary>
        /// except close ,maximize ,minimize 
        /// </summary>
        public ObservableCollection<Button> RightToolBarButtons
        {
            get { return (ObservableCollection<Button>)GetValue(RightToolBarButtonsProperty); }
            set { SetValue(RightToolBarButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolBarButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightToolBarButtonsProperty =
            DependencyProperty.Register("RightToolBarButtons", typeof(ObservableCollection<Button>), typeof(ChromeWindow), new PropertyMetadata(new ObservableCollection<Button>()));



        public ObservableCollection<Button> LeftToolBarButtons
        {
            get { return (ObservableCollection<Button>)GetValue(LeftToolBarButtonsProperty); }
            set { SetValue(LeftToolBarButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftToolBarButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftToolBarButtonsProperty =
            DependencyProperty.Register("LeftToolBarButtons", typeof(ObservableCollection<Button>), typeof(ChromeWindow), new PropertyMetadata(new ObservableCollection<Button>()));



        public AsyncRelayCommand<ChromeWindow> CloseCommand { get; set; }
        public AsyncRelayCommand<ChromeWindow> MinimizeCommand { get; set; }
        public AsyncRelayCommand<ChromeWindow> MaximizeCommand { get; set; }
        public ChromeWindow()
        {
            LeftToolBarButtons = new ObservableCollection<Button>();
            RightToolBarButtons = new ObservableCollection<Button>();

            CloseCommand = new AsyncRelayCommand<ChromeWindow>(OnClose);
            MinimizeCommand = new AsyncRelayCommand<ChromeWindow>(OnMinimize);
            MaximizeCommand = new AsyncRelayCommand<ChromeWindow>(OnMaximaize);
            
        }
        
        private Task OnMaximaize(ChromeWindow arg)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
            return Task.CompletedTask;
        }

        private Task OnMinimize(ChromeWindow arg)
        {
            this.WindowState = WindowState.Minimized;
            return Task.CompletedTask;
        }

        private Task OnClose(ChromeWindow arg)
        {
            this.Close();
            return Task.CompletedTask;
        }

    }
}

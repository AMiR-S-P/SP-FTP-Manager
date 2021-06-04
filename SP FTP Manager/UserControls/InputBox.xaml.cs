using SP_FTP_Manager.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SP_FTP_Manager.UserControls
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : ChromeWindow
    {
        public string Value { get; set; }

        public InputBox()
        {
            InitializeComponent();
            valueTxtBox.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Value = valueTxtBox.Text;
            this.Close();
        }

       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void valueTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(valueTxtBox.Text))
            {
                okBtn.IsEnabled = false;
            }
            else
            {
                okBtn.IsEnabled = true;
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Value = "";
            this.Close();
        }

    }
}

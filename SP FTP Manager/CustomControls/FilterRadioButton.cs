using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SP_FTP_Manager.CustomControls
{
    public class FilterRadioButton:RadioButton
    {


        public string  Title
        {
            get { return (string )GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string ), typeof(FilterRadioButton), new PropertyMetadata(""));
        
        static FilterRadioButton()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterRadioButton), new PropertyMetadata());
        }
    }
}

using SP_FTP_Manager.Helper;
using SP_FTP_Manager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SP_FTP_Manager.Behaviors
{
public    class PasswordEncryptorBehavior:Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }
        private void AssociatedObject_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            (AssociatedObject.DataContext as LoginViewModel).Login.Password = PasswordCipher.Encrypt(AssociatedObject.Password);
        }
    }
}

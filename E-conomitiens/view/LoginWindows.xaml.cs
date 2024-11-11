using E_conomitiens.viewModel;
using E_conomitiens.model;
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


    namespace E_conomitiens.view
    {
        /// <summary>
        /// Interaction logic for LoginWindow.xaml
        /// </summary>
        public partial class LoginWindows : Window
        {
            public LoginWindows()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;      
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
            {
                if (DataContext is LoginViewModel viewModel)
                {
                    viewModel.Password = passwordBox.Password;
                }
            }



    }
    }

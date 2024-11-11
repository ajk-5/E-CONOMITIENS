using E_conomitiens.viewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using E_conomitiens.model;


namespace E_conomitiens.view
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();


            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            emailBox.TextChanged += TextBox_TextChanged;
        }
        public void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegistrationViewModel viewModel)
            {
                User.Password = passwordBox.Password;
            }
        }
        public void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is RegistrationViewModel viewModel)
            {
                User.Email = emailBox.Text;
            }

        }

    }
}
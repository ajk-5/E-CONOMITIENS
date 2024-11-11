using System.Windows;
using E_conomitiens.model;
using E_conomitiens.viewModel;
using System.Windows.Controls;

namespace E_conomitiens.view
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
            DataContext = new RegistrationViewModel();

            // Password and confirm password binding
            passwordBox0.PasswordChanged += PasswordBox_PasswordChanged;
            confirmPasswordBox0.PasswordChanged += ConfirmPasswordBox_PasswordChanged;

            // Role selection binding
            RoleComboBox.ItemsSource = Enum.GetValues(typeof(Role));
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegistrationViewModel viewModel)
            {
                viewModel.Password = passwordBox0.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegistrationViewModel viewModel)
            {
                viewModel.ConfirmPassword = confirmPasswordBox0.Password;
            }
        }
    }
}
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using MySql.Data.MySqlClient;
using E_conomitiens.model;

namespace E_conomitiens.viewModel
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _email;
        private string _firstName;
        private string _lastName;
        private Role _role;

        public string UserName
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }
        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }
        }
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; OnPropertyChanged(); }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; OnPropertyChanged(); }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { _confirmPassword = value; OnPropertyChanged(); }
        }
        public Role Role
        {
            get { return _role; }
            set { _role = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }

        public RegistrationViewModel()
        {
            RegisterCommand = new RelayCommand(Register, CanRegister);
        }

        private bool CanRegister(object parameter)
        {
            return !string.IsNullOrEmpty(UserName) &&
                   !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(FirstName) &&
                   !string.IsNullOrEmpty(LastName) &&
                   !string.IsNullOrEmpty(Password) &&
                   Password == ConfirmPassword;
        }

        private void Register(object parameter)
        {
            string insertQuery = "INSERT INTO USER (EMAIL, FIRST_NAME, LAST_NAME, USER_NAME, PASSWORD, ROLE) VALUES (@Email, @FirstName, @LastName, @UserName, @Password, @Role);";
            string connectionString = "server=localhost;database=economitiens;user=root;password=;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@FirstName", FirstName);
                    cmd.Parameters.AddWithValue("@LastName", LastName);
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.Parameters.AddWithValue("@Role", Role.ToString());

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("User registered successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


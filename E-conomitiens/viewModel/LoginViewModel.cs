using E_conomitiens.model;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using E_conomitiens.Utilities;
using E_conomitiens.view;
namespace E_conomitiens.viewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _email;
        private string _password;

        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login, CanLogin);
            
        }

        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
        }

   
            private void Login(object parameter)
            {
                string selectQuery = "SELECT * FROM USER WHERE EMAIL = @email AND PASSWORD = @Password;";
                string connectionString = "server=localhost;database=economitiens;user=root;password=;";

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(selectQuery, conn);
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.Parameters.AddWithValue("@Password", Password);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            MessageBox.Show("Login successful!");
                        // Additional logic after successful login
                        User user = new User
                        {
                            email = reader["EMAIL"].ToString(),
                            firstName = reader["FIRST_NAME"].ToString(),
                            lastName = reader["LAST_NAME"].ToString(),
                            userName = reader["USER_NAME"].ToString(),
                            password = reader["PASSWORD"].ToString(),
                            role = (Role)Enum.Parse(typeof(Role), reader["Role"].ToString())
                        };

                        // Set the user session
                        UserSession.Instance.SetUser(user);
                        // Navigate to the main window
                        //Application.Current.Dispatcher.Invoke(() =>
                        {
                            //MainWindow mainWindow = new MainWindow();
                            //mainWindow.Show();
                            //Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is LoginWindows)?.Close();
                        }//);
                    }
                        else
                        {
                            MessageBox.Show("Invalid username or password.");
                        }

                        reader.Close();
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        
        


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
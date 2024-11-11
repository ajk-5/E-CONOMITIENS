using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_conomitiens.viewModel
{
    public class AuthenticationService
    {
        private string _loggedInUser;

        public bool Login(string username, string password)
        {
            // Replace with actual user validation logic
            if (ValidateUser(username, password))
            {
                _loggedInUser = username;
                return true;
            }
            return false;
        }

        private bool ValidateUser(string username, string password)
        {
            // Example validation logic (replace with real database validation)
            return username == "user" && password == "password";
        }

        public bool IsUserLoggedIn()
        {
            return _loggedInUser != null;
        }

        public void Logout()
        {
            _loggedInUser = null;
        }

        public string GetLoggedInUser()
        {
            return _loggedInUser;
        }
    }
}

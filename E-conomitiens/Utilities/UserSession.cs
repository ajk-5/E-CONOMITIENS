using E_conomitiens.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_conomitiens.Utilities
{
    public class UserSession
    {
        private static UserSession _instance;
        public User LoggedInUser { get; private set; }

        private UserSession() { }

        public static UserSession Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UserSession();
                return _instance;
            }
        }

        public void SetUser(User user)
        {
            LoggedInUser = user;
        }

        public void Clear()
        {
            LoggedInUser = null;
        }
    }
}


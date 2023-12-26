using HMS.Data;
using KEA_Final1.Controllers;

namespace HMS.DTO
{
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public enum Role 
        {
            Admin = 0,
            Doctor = 1,
            Patient = 2
        }

        /// <summary>
        /// Returns false is credentials are suspicious.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CheckUserCredentials(Account user)
        {
            if (user.Username.Length != 10 ||
                string.IsNullOrEmpty(user.Password) ||
                SqlInjectionPrevention.CheckString(user.Password) ||
                SqlInjectionPrevention.CheckString(user.Username) ||
                int.TryParse(user.Username, out int parsed) == false || parsed == 0) return false;
            else
                return true;
        }
    }
}

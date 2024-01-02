using HMS.Data;
using KEA_Final1.Controllers;
using System.Globalization;

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
                user.Username == "0000000000" ||
                string.IsNullOrEmpty(user.Password) ||
                user.Password.Length > 30 ||
                SqlInjectionPrevention.CheckString(user.Password) ||
                SqlInjectionPrevention.CheckString(user.Username) ||
                !CheckValidDateOnCPR(user))
                return true;
            else
                return false;
        }


        public bool CheckValidDateOnCPR(Account user)
        {
            if (user.Username == string.Empty || user.Username == null || user.Username.Length != 10) return false;

            string stringedDate = string.Empty;

            stringedDate += user.Username.Substring(0, 2) + "-";
            stringedDate += user.Username.Substring(2, 2) + "-";
            stringedDate += "19" + user.Username.Substring(4, 2);

            bool dateValid = DateTime.TryParseExact(stringedDate, @"dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

            return dateValid;
        }
    }
}

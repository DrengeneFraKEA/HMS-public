using HMS.Data;
using HMS.DTO;
using HMS.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace KEA_Final1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly JwtTokenGenerator jwtTokenGenerator;

        public LoginController(IConfiguration configuration)
        {
            jwtTokenGenerator = new JwtTokenGenerator(configuration);
        }
        [AllowAnonymous]
        [HttpPost]
        public string LogIn([FromBody]Account user) 
        {
            if (user.CheckUserCredentials(user) == false) return null; // Suspicious credentials provided.

            Database.MySQLContext mysql = new Database.MySQLContext();

            mysql.Db.Open();

            using var command = new MySqlCommand($"SELECT * FROM accounts where username = {user.Username} and password = {user.Password};", mysql.Db);
            using var reader = command.ExecuteReader();
            bool userExists = reader.HasRows;
            
            mysql.Db.Close();

            if (userExists)
            {
                return jwtTokenGenerator.GenerateToken(user.Username, "User");
            }
            


            return string.Empty;
        }


        [HttpPost]
        [Route("register")]
        public bool? Register([FromBody] Account user) 
        {
            if (user.CheckUserCredentials(user) == false) return null; // Suspicious credentials provided.

            using(MySqlCommand cmd = new MySqlCommand()) 
            {
                Database.MySQLContext mysql = new Database.MySQLContext();
                cmd.CommandText = $"SELECT * FROM accounts where username = {user.Username}";
                cmd.Connection = mysql.Db;

                mysql.Db.Open();

                using (var reader = cmd.ExecuteReader()) 
                {
                    if (reader.HasRows)
                    {
                        // User already exists.
                        mysql.Db.Close();
                        return false;
                    }
                }

                cmd.CommandText = $"SELECT * FROM persondata WHERE cpr = {user.Username}";

                bool shouldInsertIntoPersondata;
                using (var reader = cmd.ExecuteReader()) shouldInsertIntoPersondata = reader.HasRows;
                

                if (shouldInsertIntoPersondata)
                {
                    // User exists in persondata (table of all cprs in denmark) and the user can be registered in hms.
                    cmd.CommandText = $"INSERT INTO accounts (username, password) VALUES ({user.Username}, {user.Password})";
                    cmd.ExecuteReader();
                }
                else
                {
                    // Add into persondata (because we don't actually have all cprs in denmark).
                    cmd.CommandText = $"INSERT INTO persondata (cpr) VALUES ({user.Username})";
                    using (var reader = cmd.ExecuteReader()) { };
                    
                    cmd.CommandText = $"INSERT INTO accounts (username, password) VALUES ({user.Username}, {user.Password})";
                    cmd.ExecuteReader();
                }
                
                mysql.Db.Close();
                return true;
            }
        }
    }
}

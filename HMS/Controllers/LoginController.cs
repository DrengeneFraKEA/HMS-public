using HMS.Data;
using HMS.DTO;
using HMS.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using MySqlConnector;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            BrowserStorage bs = null;

            using (MySqlCommand cmd = new MySqlCommand()) 
            {
                Database.MySQLContext mysql = new Database.MySQLContext();
                mysql.Db.Open();

                cmd.CommandText = $"SELECT * FROM accounts a JOIN persondata p on p.cpr = a.username WHERE username = {user.Username} and password = {user.Password};";
                cmd.Connection = mysql.Db;
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    bs = new BrowserStorage()
                    {
                        UserId = reader.GetInt32("id"),
                        Role = reader.GetString("role"),
                        Token = jwtTokenGenerator.GenerateToken(user.Username, reader.GetString("role"))
                    };
                }

                mysql.Db.Close();
            }

            if (bs != null)
                return JsonSerializer.Serialize(bs);
            
            return string.Empty;
        }


        [HttpPost]
        [Route("register")]
        public bool? Register([FromBody] Account user) 
        {
            if (user.CheckUserCredentials(user) == false) return null; // Suspicious credentials provided.
            Database.MySQLContext mysql = new Database.MySQLContext();
            using (MySqlCommand cmd = new MySqlCommand()) 
            {
                //Database.MySQLContext mysql = new Database.MySQLContext();
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
                    using (var transaction = mysql.Db.BeginTransaction())
                    {
                        try
                        {
                            cmd.Transaction = transaction;

                            // Add into persondata (because we don't actually have all cprs in denmark).
                            cmd.CommandText = $"INSERT INTO persondata (first_name, last_name, contact_number, cpr, address) VALUES ('John','Smith',12345678,{user.Username}, 'Guldbergsgade 29N')";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = $"INSERT INTO accounts (username, password, role) VALUES ({user.Username}, {user.Password}, 'patient')";
                            cmd.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                
                mysql.Db.Close();
                return true;
            }
        }
    }
}

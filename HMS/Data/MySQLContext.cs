using MySqlConnector;

namespace HMS.Data
{
    public class MySQLContext
    {
        public MySqlConnection Db { get; set; }



        public MySQLContext() 
        {
            this.Db = new MySqlConnection("Server=hospitalmanagementsystem1.mysql.database.azure.com; User ID=hospitalmanagementsystem; Password=hms1234!; Database=HMS");

        }
    }
}

using MySqlConnector;

namespace HMS.Data
{
    public class MySQLContext
    {
        public MySqlConnection Db { get; set; }



        public MySQLContext() 
        {
            this.Db = new MySqlConnection("Server=hospitalmanagementsystem.mysql.database.azure.com; User ID=HospitalManagementSystemAdmin; Password=hms1234!; Database=HMS");

        }
    }
}

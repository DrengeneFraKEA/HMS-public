using MongoDB.Driver;
using MongoDB.Bson;
using MySqlConnector;
using System.Runtime.CompilerServices;

namespace HMS.Data
{
    public static class Database 
    {
        public static int SelectedDatabase { get; set; } = 0;

        public class MySQLContext
        {
            public MySqlConnection Db { get; set; }
            public MySQLContext() 
            {
                this.Db = new MySqlConnection("Server=hospitalmanagementsystem1.mysql.database.azure.com; User ID=hospitalmanagementsystem; Password=hms1234!; Database=HMS");
            }
        }

        public class MongoDbContext
        {
            public string ConnectionString { get; set; }
            public MongoClientSettings Settings { get; set; }
            public MongoDbContext() 
            {
                this.ConnectionString = "mongodb+srv://hms_admin:vGaQGWA85GqRofPf@hms.eigi1od.mongodb.net/?retryWrites=true&w=majority";
                this.Settings = MongoClientSettings.FromConnectionString(ConnectionString);
                this.Settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            }
        }

        public class GraphQlContext 
        {
            public GraphQlContext() 
            {
                // Do something..
            }
        }
    }


}

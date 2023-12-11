using MongoDB.Driver;
using MySqlConnector;
using Neo4j.Driver;

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
            public IDriver Neo4jDriver { get; set; }
            public GraphQlContext() 
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var neo4jUri = new Uri("neo4j+s://07a963f9.databases.neo4j.io");

                this.Neo4jDriver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic("neo4j", "QlofL24P9k4fIbxjfnj7rwr5dvtZP4eahW7zem7vu-s"));
                
            }

            public void Dispose()
            {
                Neo4jDriver?.Dispose();
            }
        }
    }


}

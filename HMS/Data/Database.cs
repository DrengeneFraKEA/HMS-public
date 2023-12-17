using MongoDB.Driver;
using MySqlConnector;
using Neo4j.Driver;

namespace HMS.Data
{
    public static class Database 
    {
        public static int SelectedDatabase { get; set; } = 0;
        public enum MySqlAccountType 
        {
            ReadOnly = 0,
            WriteOnly = 1,
            ReadWrite = 2,
            FullAdmin = 3
        }

        public class MySQLContext
        {
            public MySqlConnection Db { get; set; }
            public MySQLContext(MySqlAccountType type) 
            {
                switch (type)
                {
                    case MySqlAccountType.ReadOnly:
                        this.Db = new MySqlConnection("Server=hospitalmanagementsystem1.mysql.database.azure.com; User ID=hmsread; Password=hms1234!; Database=HMS");
                        break;
                    case MySqlAccountType.WriteOnly:
                        this.Db = new MySqlConnection("Server=hospitalmanagementsystem1.mysql.database.azure.com; User ID=hmswrite; Password=hms1234!; Database=HMS");
                        break;
                    case MySqlAccountType.ReadWrite:
                        this.Db = new MySqlConnection("Server=hospitalmanagementsystem1.mysql.database.azure.com; User ID=hmsreadwrite; Password=hms1234!; Database=HMS");
                        break;
                    case MySqlAccountType.FullAdmin:
                        this.Db = new MySqlConnection("Server=hospitalmanagementsystem1.mysql.database.azure.com; User ID=hospitalmanagementsystem; Password=hms1234!; Database=HMS");
                        break;
                }
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
                var neo4jConnection = config.GetSection("Neo4jSettings:Neo4jConnection").Value;
                var neo4jUser = config.GetSection("Neo4jSettings:Neo4jUser").Value;
                var neo4jPassword = config.GetSection("Neo4jSettings:Neo4jPassword").Value;


                var neo4jUri = new Uri(neo4jConnection);

                this.Neo4jDriver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword));
                
            }

            public void Dispose()
            {
                Neo4jDriver?.Dispose();
            }
        }
    }


}

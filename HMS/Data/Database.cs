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
                IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
                
                switch (type)
                {
                    case MySqlAccountType.ReadOnly:
                        string mysql_read = Environment.GetEnvironmentVariable("MYSQL_READ");
                        this.Db = new MySqlConnection(mysql_read);
                        break;
                    case MySqlAccountType.WriteOnly:
                        this.Db = new MySqlConnection(config.GetSection("MySql:MySqlWrite").Value);
                        break;
                    case MySqlAccountType.ReadWrite:
                        this.Db = new MySqlConnection(config.GetSection("MySql:MySqlReadWrite").Value);
                        break;
                    case MySqlAccountType.FullAdmin:
                        this.Db = new MySqlConnection(config.GetSection("MySql:MySqlAdmin").Value);
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
                IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

                this.ConnectionString = config.GetSection("MongoDB:Connection").Value;
                this.Settings = MongoClientSettings.FromConnectionString(ConnectionString);
                this.Settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            }
        }

        public class GraphQlContext 
        {
            public IDriver Neo4jDriver { get; set; }
            public GraphQlContext() 
            {
                //var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
                var neo4jConnection = Environment.GetEnvironmentVariable("NEO4J_CONN");
                var neo4jUser = Environment.GetEnvironmentVariable("NEO4J_USER");
                var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");


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

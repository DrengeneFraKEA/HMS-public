﻿using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MySqlConnector;
using Neo4j.Driver;
using System.Data.Common;

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
            public bool UseLocalHost = true;
            public string DbName = "hms3";
            public MySQLContext(MySqlAccountType type) 
            {
                IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
                string cs = "MySql";
                if (UseLocalHost) 
                {
                    cs = "MySqlLocalHost";
                    this.DbName = "hms";
                }

                switch (type)
                {
                    case MySqlAccountType.ReadOnly:
                        var mysql_read = Environment.GetEnvironmentVariable($"MYSQL_READ");
                        if (mysql_read.IsNullOrEmpty()) this.Db = new MySqlConnection(config.GetSection($"{cs}:MySqlRead").Value);
                        else this.Db = new MySqlConnection(mysql_read);
                        break;
                    case MySqlAccountType.WriteOnly:
                        var mysql_write = Environment.GetEnvironmentVariable($"MYSQL_WRITE");
                        if (mysql_write.IsNullOrEmpty()) this.Db = new MySqlConnection(config.GetSection($"{cs}:MySqlWrite").Value);
                        else this.Db = new MySqlConnection(mysql_write);
                        break;
                    case MySqlAccountType.ReadWrite:
                        var mysql_readwrite = Environment.GetEnvironmentVariable($"MYSQL_READWRITE");
                        if (mysql_readwrite.IsNullOrEmpty()) this.Db = new MySqlConnection(config.GetSection($"{cs}:MySqlReadWrite").Value);
                        else this.Db = new MySqlConnection(mysql_readwrite);
                        break;
                    case MySqlAccountType.FullAdmin:
                        var mysql_admin = Environment.GetEnvironmentVariable($"MYSQL_ADMIN");
                        if (mysql_admin.IsNullOrEmpty()) this.Db = new MySqlConnection(config.GetSection($"{cs}:MySqlAdmin").Value);
                        else this.Db = new MySqlConnection(mysql_admin);
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

                this.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONN");
                if (this.ConnectionString == null) this.ConnectionString = config.GetSection("MongoDB:Connection").Value;
                this.Settings = MongoClientSettings.FromConnectionString(ConnectionString);
                this.Settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            }
        }

        public class GraphQlContext 
        {
            public IDriver Neo4jDriver { get; set; }
            public GraphQlContext() 
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

                var neo4jConnection = Environment.GetEnvironmentVariable("NEO4J_CONN");
                if (neo4jConnection.IsNullOrEmpty()) neo4jConnection = config.GetSection("Neo4jSettings:Neo4jConnection").Value;
                
                var neo4jUser = Environment.GetEnvironmentVariable("NEO4J_USER");
                if (neo4jUser.IsNullOrEmpty()) neo4jUser = config.GetSection("Neo4jSettings:Neo4jUser").Value;

                var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");
                if (neo4jPassword.IsNullOrEmpty()) neo4jPassword = config.GetSection("Neo4jSettings:Neo4jPassword").Value;

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

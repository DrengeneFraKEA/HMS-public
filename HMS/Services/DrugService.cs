using HMS.Data;
using HMS.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MySqlConnector;
using System.Text.Json;

namespace HMS.Services;

public class DrugService
{

    public string GetDrugs()
    {

        switch (Database.SelectedDatabase)
        {
            case 0:
                // Hent med mysql..

                var drugs = new List<Drug>();
                Database.MySQLContext mysql = new Database.MySQLContext();

                mysql.Db.Open();

                var command = new MySqlCommand("SELECT * FROM drug;", mysql.Db);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var drug = new Drug()
                    {
                        DrugId = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        SideEffects = reader.GetString("sideeffects"),

                    };
                    drugs.Add(drug);
                }

                mysql.Db.Close();

                return JsonSerializer.Serialize(drugs);

                //break;
            case 1:
                // Hent med mongo db..

                Database.MongoDbContext mdbc = new Database.MongoDbContext();
                MongoClient mc = new MongoClient(mdbc.ConnectionString);

                var result = mc.GetDatabase("HMS").RunCommand<BsonDocument>(new BsonDocument("ping", 1));

                return result.ToString();
            case 2:

                // Hent med graphql
                break;
        }

        return string.Empty;
    }

}


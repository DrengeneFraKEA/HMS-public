using HMS.Data;
using HMS.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MySqlConnector;
using System.Net;
using System;
using System.Text.Json;
using MongoDB.Bson.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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

    /// <summary>
    /// Uses external api: https://lhncbc.nlm.nih.gov/RxNav/APIs/RxNormAPIs.html
    /// </summary>
    /// <param name="drugsearch"></param>
    /// <returns></returns>
    public string GetDrugByName(string drugsearch) 
    {
        List<DTO.Drug> drugs = new List<DTO.Drug>();

        using (HttpClient client = new HttpClient())
        {
            string result = client.GetStringAsync($"https://rxnav.nlm.nih.gov/REST/drugs.json?name={drugsearch}").Result;
            
            if (result != string.Empty) 
            {
                JObject jsonObject = JObject.Parse(result);

                JArray conceptProperties = (JArray)jsonObject?["drugGroup"]?["conceptGroup"]?[1]?["conceptProperties"];
                if (conceptProperties == null) return JsonSerializer.Serialize(drugs); // No results found. Return empty list.

                foreach (var conceptProperty in conceptProperties)
                {
                    DTO.Drug drug = new DTO.Drug()
                    {
                        Id = (int)conceptProperty["rxcui"],
                        Name = (string)conceptProperty["synonym"]
                    };

                    if (drug.Name != string.Empty)
                        drugs.Add(drug);
                }
            }
        }

        return JsonSerializer.Serialize(drugs);
    }

    public string GetDrugsWithPrescriptions()
    {
        var drug_prescriptions = new List<Drug_Prescription>();
        Database.MySQLContext mysql = new Database.MySQLContext();
        mysql.Db.Open();

        using var command = new MySqlCommand("SELECT * FROM drug_prescription;", mysql.Db);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var drug_prescription = new Drug_Prescription()
            {
                Name = reader.GetString("name"),
                Sideeffects = reader.GetString("sideeffects"),
                Dosage = reader.GetString("dosage"),
                Instructions = reader.GetString("instructions"),
            };
            drug_prescriptions.Add(drug_prescription);
        }
        mysql.Db.Close();

        return JsonSerializer.Serialize(drug_prescriptions);
    }
}


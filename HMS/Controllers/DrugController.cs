using HMS.Data;
using Microsoft.AspNetCore.Mvc;
using HMS.Models;

using MySqlConnector;
using Microsoft.Extensions.Options;

namespace HMS.Controllers;

[ApiController]
[Route("[controller]")]
public class DrugController : ControllerBase
{
    [HttpGet("drugs")]
    public IEnumerable<Drug> GetDrugs()
    {
        var drugs = new List<Drug>();
        MySQLContext mysql = new MySQLContext();

        mysql.Db.Open();

        using var command = new MySqlCommand("SELECT * FROM drug;", mysql.Db);
        using var reader = command.ExecuteReader();
        
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

        return drugs;
    }

    [HttpGet("drug_prescriptions")]
    public IEnumerable<Drug_Prescription> GetDrugsWithPrescriptions()
    {
        var drug_prescriptions = new List<Drug_Prescription>();
        MySQLContext mysql = new MySQLContext();
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

        return drug_prescriptions;
    }

}
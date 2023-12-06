using HMS.Data;
using Microsoft.AspNetCore.Mvc;
using HMS.Models;

using MySqlConnector;
using Microsoft.Extensions.Options;
using HMS.Services;

namespace HMS.Controllers;

[ApiController]
[Route("[controller]")]
public class DrugController : ControllerBase
{
    private readonly DrugService drugService;
    public DrugController(DrugService service)
    {
        drugService = service;
    }

    [HttpGet("drugs")]
    public string GetDrugs()
    {
       var drugs = drugService.GetDrugs();

        return drugs;
    }

    [HttpGet("drug_prescriptions")]
    public IEnumerable<Drug_Prescription> GetDrugsWithPrescriptions()
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

        return drug_prescriptions;
    }

}
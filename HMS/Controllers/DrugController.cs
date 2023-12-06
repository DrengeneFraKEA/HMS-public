using HMS.Data;
using Microsoft.AspNetCore.Mvc;
using HMS.Models;

using MySqlConnector;
using Microsoft.Extensions.Options;
using HMS.Services;
using Microsoft.AspNetCore.Authorization;

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
        return drugService.GetDrugs();
    }

    [Authorize]
    [HttpGet("{drugsearch}")]
    public string GetDrugByName(string drugsearch) 
    {
        return drugService.GetDrugByName(drugsearch);
    }

    [HttpGet("drug_prescriptions")]
    public string GetDrugsWithPrescriptions()
    {
        return drugService.GetDrugsWithPrescriptions();
    }
}
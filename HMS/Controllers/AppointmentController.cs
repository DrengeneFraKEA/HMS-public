using HMS.Data;
using Microsoft.AspNetCore.Mvc;
using HMS.Models;
using MySqlConnector;
using Microsoft.Extensions.Options;
using System.Data;
using HMS.DTO;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using HMS.Utils;
using MongoDB.Driver;
using MongoDB.Bson;
using HMS.Services;

namespace HMS.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentController : ControllerBase
{

    private readonly AppointmentService appointmentService;
    public AppointmentController(AppointmentService service)
    {
        appointmentService = service;
    }

    [Authorize]
    [HttpGet]
    public string GetAppointments()
    {

        var appointments = appointmentService.GetAppointments();

        return appointments;
       
    }

    [Authorize]
    [HttpGet("patient/{patientId}")]
    public string GetAppointmentsByPatientId(int patientId)
    {
        return appointmentService.GetAppointmentsByPatientId(patientId);
    }


    [HttpPost]
    public IActionResult CreateAppointment([FromBody]Models.Appointment appointment)
    {
        try 
        {
            appointmentService.CreateAppointment(appointment);
            return Ok("Appointment created successfully");
        }
        catch (Exception ex) 
        {
            return BadRequest("Failed to create appointment "+ex.Message);
        }
    }


    [HttpPut]
    public IActionResult UpdateAppointment([FromBody]Models.Appointment appointment)
    {
        try
        {
            appointmentService.UpdateAppointment(appointment);
            return Ok("Appointment updated successfully");
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to update appointment " + ex.Message);
        }
    }


    [HttpDelete("delete/{appointmentId}")]
    public IActionResult DeleteAppointment(int appointmentId)
    {
        try
        {
            appointmentService.DeleteAppointment(appointmentId);
            return Ok("Appointment deleted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to delete appointment " + ex.Message);
        }
    }

    [HttpGet("Test")]
    public string Test() 
    {
        switch (Database.SelectedDatabase) 
        {
            case 0:
                // Hent med mysql..

                Database.MySQLContext mysql = new Database.MySQLContext();

                break;
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
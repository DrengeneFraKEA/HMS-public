using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
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
    [HttpGet("{id}")]
    public string GetAppointmentbyId(string id)
    {
        return JsonSerializer.Serialize(appointmentService.GetAppointmentById(id));
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

    [Authorize]
    [HttpGet("{id}/start/{start}/end/{end}")]
    public IActionResult UpdateAppointment(string id, string start, string end)
    {
        try
        {
            Models.Appointment appointment = appointmentService.GetAppointmentById(id);
            if (appointment == null) return BadRequest("Appointment with id doesn't exist.");

            appointment.AppointmentDate = DateTime.Parse(start);
            appointment.AppointmentDateEnd = DateTime.Parse(end);

            appointmentService.UpdateAppointment(appointment);
            return Ok("Appointment updated successfully");
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to update appointment " + ex.Message);
        }
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteAppointment(string id)
    {
        try
        {
            appointmentService.DeleteAppointment(int.Parse(id));
            return Ok($"Appointment with id {id} deleted");
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to delete appointment " + ex.Message);
        }
    }
}
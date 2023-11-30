using HMS.Data;
using Microsoft.AspNetCore.Mvc;
using HMS.Models;
using MySqlConnector;
using Microsoft.Extensions.Options;
using System.Data;
using HMS.DTO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace HMS.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentController : ControllerBase
{
    [HttpGet]
    public string GetAppointments()
    {
        var appointments = new List<DTO.Appointment>();
        MySQLContext mysql = new MySQLContext();

        mysql.Db.Open();

        using var command = new MySqlCommand("SELECT * FROM appointment;", mysql.Db);
        using var reader = command.ExecuteReader();
        int id = 0;
        
        while (reader.Read())
        {
            var appointment = new DTO.Appointment()
            {
                Id = id,
                Place = "Guldbergsgade 29N", // Don't mind this for now.
                Start = reader.GetDateTime("appointment_date"),
                End = reader.GetDateTime("appointment_date_end"),
            };

            id++;
            appointments.Add(appointment);
        }

        mysql.Db.Close();

        return JsonSerializer.Serialize(appointments);
    }

    [HttpPost]
    public IActionResult CreateAppointment([FromBody]Models.Appointment appointment)
    {
        MySQLContext mysql = new MySQLContext();

        mysql.Db.Open();
        using var command = new MySqlCommand("CreateAppointment", mysql.Db);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("a_patient_id", appointment.PatientId);
        command.Parameters.AddWithValue("a_doctor_id", appointment.DoctorId);
        command.Parameters.AddWithValue("a_department_id", appointment.DepartmentId);
        command.Parameters.AddWithValue("a_hospital_id", appointment.HospitalId);
        command.Parameters.AddWithValue("a_appointment_date", appointment.AppointmentDate);
        command.Parameters.AddWithValue("a_appointment_date_end", appointment.AppointmentDateEnd);

        try 
        {
            command.ExecuteNonQuery();
            mysql.Db.Close();
            return Ok("Appointment created successfully");
        }
        catch (Exception ex) 
        {
            mysql.Db.Close();
            return BadRequest("Failed to create appointment "+ex.Message);
        }
    }


    [HttpPut]
    public IActionResult UpdateAppointment([FromBody]Models.Appointment appointment)
    {
        MySQLContext mysql = new MySQLContext();

        mysql.Db.Open();
        using var command = new MySqlCommand("UpdateAppointment", mysql.Db);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("a_appointment_id", appointment.AppointmentId);
        command.Parameters.AddWithValue("a_patient_id", appointment.PatientId);
        command.Parameters.AddWithValue("a_doctor_id", appointment.DoctorId);
        command.Parameters.AddWithValue("a_department_id", appointment.DepartmentId);
        command.Parameters.AddWithValue("a_hospital_id", appointment.HospitalId);
        command.Parameters.AddWithValue("a_appointment_date", appointment.AppointmentDate);
        command.Parameters.AddWithValue("a_appointment_date_end", appointment.AppointmentDateEnd);

        try
        {
            command.ExecuteNonQuery();
            mysql.Db.Close();
            return Ok("Appointment updated successfully");
        }
        catch (Exception ex)
        {
            mysql.Db.Close();
            return BadRequest("Failed to update appointment " + ex.Message);
        }
    }


    [HttpDelete("{appointmentId}")]
    public IActionResult DeleteAppointment(int appointmentId)
    {
        MySQLContext mysql = new MySQLContext();

        mysql.Db.Open();
        using var command = new MySqlCommand("DELETE FROM hms.appointment WHERE id = @appointmentId;", mysql.Db);
        command.Parameters.AddWithValue("@appointmentId", appointmentId);

        try
        {
            command.ExecuteNonQuery();
            mysql.Db.Close();    
            return Ok("Appointment deleted successfully");
        }
        catch (Exception ex)
        {
            mysql.Db.Close();
            return BadRequest("Failed to delete appointment " + ex.Message);
        }
    }
}
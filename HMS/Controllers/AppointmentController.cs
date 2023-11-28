using HMS.Data;
using Microsoft.AspNetCore.Mvc;
using HMS.Models;
using MySqlConnector;
using Microsoft.Extensions.Options;
using System.Data;

namespace HMS.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentController : ControllerBase
{
    [HttpGet]
    public IEnumerable<Appointment> GetAppointments()
    {
        var appointments = new List<Appointment>();
        MySQLContext mysql = new MySQLContext();

        mysql.Db.Open();

        using var command = new MySqlCommand("SELECT * FROM appointment;", mysql.Db);
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var appointment = new Appointment()
            {
                AppointmentId = reader.GetInt32("id"),
                PatientId = reader.GetInt32("patient_id"),
                ClinicId = reader.IsDBNull("clinic_id") ? 0 : reader.GetInt32("clinic_id"),
                DoctorId = reader.GetInt32("doctor_id"),
                DepartmentId = reader.GetInt32("department_id"),
                HospitalId = reader.GetInt32("hospital_id"),
                AppointmentDateId = reader.GetDateTime("appointment_date"),

            };
            appointments.Add(appointment);
        }

        mysql.Db.Close();

        return appointments;
    }

    [HttpPost]
    public IActionResult CreateAppointment([FromBody]Appointment appointment)
    {
        MySQLContext mysql = new MySQLContext();

        mysql.Db.Open();
        using var command = new MySqlCommand("CreateAppointment", mysql.Db);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("a_patient_id", appointment.PatientId);
        command.Parameters.AddWithValue("a_doctor_id", appointment.DoctorId);
        command.Parameters.AddWithValue("a_department_id", appointment.DepartmentId);
        command.Parameters.AddWithValue("a_hospital_id", appointment.HospitalId);
        command.Parameters.AddWithValue("a_appointment_date", appointment.AppointmentDateId);

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
}
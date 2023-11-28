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
                AppointmentDate = reader.GetDateTime("appointment_date"),
                AppointmentDateEnd = reader.GetDateTime("appointment_date_end"),

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
    public IActionResult UpdateAppointment([FromBody]Appointment appointment)
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
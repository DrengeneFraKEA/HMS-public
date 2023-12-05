using HMS.Data;
using HMS.DTO;
using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using System.Text.Json;

namespace HMS.Services
{
    public class AppointmentService
    {

        public string GetAppointments()
        {

            var appointments = new List<DTO.Appointment>();
            Database.MySQLContext mysql = new Database.MySQLContext();

            mysql.Db.Open();

            using var command = new MySqlCommand("SELECT * FROM appointment a JOIN hospital h on h.id = a.hospital_id;", mysql.Db);
            using var reader = command.ExecuteReader();

        
            while (reader.Read())
            {
                var appointment = new DTO.Appointment()
                {
                    Id = reader.GetInt32("id"),
                    Place = reader.GetString("name"),
                    Start = reader.GetDateTime("appointment_date"),
                    End = reader.GetDateTime("appointment_date_end"),
                };

                appointments.Add(appointment);
            }

            mysql.Db.Close();

            return JsonSerializer.Serialize(appointments);
        }

        public string GetAppointmentsByPatientId(int patientId)
        {
            var appointments = new List<DTO.Appointment>();
            Database.MySQLContext mysql = new Database.MySQLContext();

            mysql.Db.Open();
            using var command = new MySqlCommand("SELECT * FROM hms.appointment a JOIN hospital h on h.id = a.hospital_id WHERE patient_id = @patientId;", mysql.Db);
            command.Parameters.AddWithValue("@patientId", patientId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var appointment = new DTO.Appointment()
                {
                    Id = reader.GetInt32("id"),
                    Place = reader.GetString("name"),
                    Start = reader.GetDateTime("appointment_date"),
                    End = reader.GetDateTime("appointment_date_end"),
                };

                appointments.Add(appointment);
            }

            mysql.Db.Close();
            return JsonSerializer.Serialize(appointments);
        }

        public void CreateAppointment(Models.Appointment appointment)
        {
            Database.MySQLContext mysql = new Database.MySQLContext();

            mysql.Db.Open();
            using var command = new MySqlCommand("CreateAppointment", mysql.Db);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("a_patient_id", appointment.PatientId);
            command.Parameters.AddWithValue("a_doctor_id", appointment.DoctorId);
            command.Parameters.AddWithValue("a_department_id", appointment.DepartmentId);
            command.Parameters.AddWithValue("a_hospital_id", appointment.HospitalId);
            command.Parameters.AddWithValue("a_appointment_date", appointment.AppointmentDate);
            command.Parameters.AddWithValue("a_appointment_date_end", appointment.AppointmentDateEnd);
            
            command.ExecuteNonQuery();
            mysql.Db.Close();
        }

        public void UpdateAppointment(Models.Appointment appointment)
        {
            Database.MySQLContext mysql = new Database.MySQLContext();
      
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
           
            command.ExecuteNonQuery();
            mysql.Db.Close();
           
        }

        public void DeleteAppointment(int appointmentId)
        {
            Database.MySQLContext mysql = new Database.MySQLContext();

            mysql.Db.Open();
            using var command = new MySqlCommand("DELETE FROM hms.appointment WHERE id = @appointmentId;", mysql.Db);
            command.Parameters.AddWithValue("@appointmentId", appointmentId);

            command.ExecuteNonQuery();
            mysql.Db.Close();
        }
    }
}

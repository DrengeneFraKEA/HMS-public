using HMS.Data;
using HMS.DTO;
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

            var appointments = new List<Appointment>();
            Database.MySQLContext mysql = new Database.MySQLContext();

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

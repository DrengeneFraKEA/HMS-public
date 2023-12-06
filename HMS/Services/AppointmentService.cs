﻿using HMS.Data;
using HMS.DTO;
using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MySqlConnector;
using System.Data;
using System.Text.Json;

namespace HMS.Services
{
    public class AppointmentService
    {

        public string GetAppointments()
        {

            switch (Database.SelectedDatabase)
            {
                case 0:
                    // Hent med mysql..
                    
                    var appointments = new List<DTO.Appointment>();
                    Database.MySQLContext mysql = new Database.MySQLContext();

                    mysql.Db.Open();

                    var command = new MySqlCommand("SELECT * FROM appointment a JOIN hospital h on h.id = a.hospital_id;", mysql.Db);
                    var reader = command.ExecuteReader();


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
                    
                //break;
                case 1:
                    // Hent med mongo db..

                    Database.MongoDbContext mdbc = new Database.MongoDbContext();
                    MongoClient mc = new MongoClient(mdbc.ConnectionString);

                    var database = mc.GetDatabase("HMS");
                    var appointmentCollection = database.GetCollection<Models.Appointment>("appointments");
                    var filter = Builders<Models.Appointment>.Filter.Empty;
                    //var filter = Builders<Models.Appointment>.Filter.Gt(a => a.AppointmentId, 0);
                    var documents = appointmentCollection.Find(filter).ToList();

                    var result = documents.Select(appointment => new DTO.Appointment
                    {
                        Id = appointment.AppointmentId,
                        Place = appointment.Clinic?.Name != null ? appointment.Clinic.Name : appointment.Hospital.Name,
                        Start = appointment.AppointmentDate,
                        End = appointment.AppointmentDateEnd
                    }).ToList(); 

                    return JsonSerializer.Serialize(result);
                case 2:

                    // Hent med graphql
                    break;
            }

            return string.Empty;
        }

        public string GetAppointmentsByPatientId(int patientId)
        {

            switch (Database.SelectedDatabase)
            {
                case 0:
                    // Hent med mysql..

                    var appointments = new List<DTO.Appointment>();
                    Database.MySQLContext mysql = new Database.MySQLContext();

                    mysql.Db.Open();
                    var command = new MySqlCommand("SELECT * FROM hms.appointment a JOIN hospital h on h.id = a.hospital_id WHERE patient_id = @patientId;", mysql.Db);
                    command.Parameters.AddWithValue("@patientId", patientId);
                    var reader = command.ExecuteReader();

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

                    //break;

                case 1:
                    // Hent med mongo db..
                    Database.MongoDbContext mdbc = new Database.MongoDbContext();
                    MongoClient mc = new MongoClient(mdbc.ConnectionString);

                    var database = mc.GetDatabase("HMS");
                    var appointmentCollection = database.GetCollection<Models.Appointment>("appointments");

                    var filter = Builders<Models.Appointment>.Filter.Eq(a => a.Patient.PatientId, patientId) & Builders<Models.Appointment>.Filter.Gt(a => a.AppointmentId, 0);
                    var documents = appointmentCollection.Find(filter).ToList();

                    var result = documents.Select(appointment => new DTO.Appointment
                    {
                        Id = appointment.AppointmentId,
                        Place = appointment.Clinic?.Name != "" ? appointment.Clinic.Name : appointment.Hospital.Name,
                        Start = appointment.AppointmentDate,
                        End = appointment.AppointmentDateEnd
                    }).ToList();
                    return JsonSerializer.Serialize(result);

                case 2:

                    // Hent med graphql
                    break;
            }

            return string.Empty;
        }

        public void CreateAppointment(Models.Appointment appointment)
        {

            switch (Database.SelectedDatabase)
            {
                case 0:
                    // Hent med mysql..
                    Database.MySQLContext mysql = new Database.MySQLContext();

                    mysql.Db.Open();
                    var command = new MySqlCommand("CreateAppointment", mysql.Db);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("a_patient_id", appointment.PatientId);
                    command.Parameters.AddWithValue("a_doctor_id", appointment.DoctorId);
                    command.Parameters.AddWithValue("a_department_id", appointment.DepartmentId);
                    command.Parameters.AddWithValue("a_hospital_id", appointment.HospitalId);
                    command.Parameters.AddWithValue("a_appointment_date", appointment.AppointmentDate);
                    command.Parameters.AddWithValue("a_appointment_date_end", appointment.AppointmentDateEnd);

                    command.ExecuteNonQuery();
                    mysql.Db.Close();
                    break;
                case 1:
                    // Hent med mongo db..
                    Database.MongoDbContext mdbc = new Database.MongoDbContext();
                    MongoClient mc = new MongoClient(mdbc.ConnectionString);

                    var database = mc.GetDatabase("HMS");
                    var appointmentCollection = database.GetCollection<Models.Appointment>("appointments");

                    
                    appointmentCollection.InsertOne(appointment);
                    break;

                case 2:

                    // Hent med graphql
                    break;
            }
        }

        public void UpdateAppointment(Models.Appointment appointment)
        {

            switch (Database.SelectedDatabase)
            {
                case 0:
                    // Hent med mysql..
                    Database.MySQLContext mysql = new Database.MySQLContext();

                    mysql.Db.Open();
                    var command = new MySqlCommand("UpdateAppointment", mysql.Db);
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
                    break;
                case 1:
                    // Hent med mongo db..
                    Database.MongoDbContext mdbc = new Database.MongoDbContext();
                    MongoClient mc = new MongoClient(mdbc.ConnectionString);

                    var database = mc.GetDatabase("HMS");
                    var appointmentCollection = database.GetCollection<Models.Appointment>("appointments");

                    var filter = Builders<Models.Appointment>.Filter.Eq(a => a.AppointmentId, appointment.AppointmentId);
                    var update = Builders<Models.Appointment>.Update
                        .Set(a => a.PatientId, appointment.PatientId)
                        .Set(a => a.DoctorId, appointment.DoctorId)
                        .Set(a => a.DepartmentId, appointment.DepartmentId)
                        .Set(a => a.Clinic, appointment.Clinic) 
                        .Unset(a => a.HospitalId) 
                        .Set(a => a.AppointmentDate, appointment.AppointmentDate)
                        .Set(a => a.AppointmentDateEnd, appointment.AppointmentDateEnd);

                    appointmentCollection.UpdateOne(filter, update);

                    break;

                case 2:

                    // Hent med graphql
                    break;
            }
           
        }

        public void DeleteAppointment(int appointmentId)
        {

            switch (Database.SelectedDatabase)
            {
                case 0:
                    // Hent med mysql..
                    Database.MySQLContext mysql = new Database.MySQLContext();

                    mysql.Db.Open();
                    var command = new MySqlCommand("DELETE FROM hms.appointment WHERE id = @appointmentId;", mysql.Db);
                    command.Parameters.AddWithValue("@appointmentId", appointmentId);

                    command.ExecuteNonQuery();
                    mysql.Db.Close();
                    break;
                case 1:
                    // Hent med mongo db..
                    Database.MongoDbContext mdbc = new Database.MongoDbContext();
                    MongoClient mc = new MongoClient(mdbc.ConnectionString);

                    var database = mc.GetDatabase("HMS");
                    var appointmentCollection = database.GetCollection<Models.Appointment>("appointments");

                    var filter = Builders<Models.Appointment>.Filter.Eq(a => a.AppointmentId, appointmentId);

                    appointmentCollection.DeleteOne(filter);
                    break;

                case 2:

                    // Hent med graphql
                    break;
            }
        }
    }
}

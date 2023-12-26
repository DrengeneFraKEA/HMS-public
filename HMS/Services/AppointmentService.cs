using HMS.Data;
using MongoDB.Driver;
using MySqlConnector;
using Neo4j.Driver;
using System.Data;
using System.Globalization;
using System.Text.Json;

namespace HMS.Services
{
    public class AppointmentService
    {
        public Models.Appointment GetAppointmentById(string id)
        {
            Models.Appointment appointment = null;

            // Hent med mysql.
            Database.MySQLContext mysql = new Database.MySQLContext(Database.MySqlAccountType.ReadOnly);

            mysql.Db.Open();

            var command = new MySqlCommand($"SELECT * FROM appointment where id = {id};", mysql.Db);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                appointment = new Models.Appointment()
                {
                    AppointmentId = reader.GetInt32("id"),
                    PatientId = reader.GetInt32("patient_id"),
                    DoctorId = reader.GetInt32("doctor_id"),
                    DepartmentId = reader.GetInt32("department_id"),
                    HospitalId = reader.GetInt32("hospital_id"),
                    AppointmentDate = reader.GetDateTime("appointment_date"),
                    AppointmentDateEnd = reader.GetDateTime("appointment_date_end")
                };
            }

            mysql.Db.Close();

            return appointment;
        }

        public string GetAppointmentsByPatientId(int patientId)
        {
            List<DTO.Appointment> appointments = new List<DTO.Appointment>();

            switch (Database.SelectedDatabase)
            {
                case 0:
                    // Hent med mysql.
                    Database.MySQLContext mysql = new Database.MySQLContext(Database.MySqlAccountType.ReadOnly);

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
                    break;
                case 1:
                    // Hent med mongo db..
                    Database.MongoDbContext mdbc = new Database.MongoDbContext();
                    MongoClient mc = new MongoClient(mdbc.ConnectionString);

                    var database = mc.GetDatabase("HMS");
                    var appointmentCollection = database.GetCollection<Models.Appointment>("appointments");

                    var filter = Builders<Models.Appointment>.Filter.Eq(a => a.Patient.PatientId, patientId);
                    var documents = appointmentCollection.Find(filter).ToList();

                    var result = documents.Select(appointment => new DTO.Appointment
                    {
                        Id = appointment.AppointmentId,
                        Place = appointment.Clinic?.Name != null ? appointment.Clinic.Name : appointment.Hospital.Name,
                        Start = appointment.AppointmentDate,
                        End = appointment.AppointmentDateEnd
                    }).ToList();

                    foreach (var element in result) appointments.Add(element);
                    break;
                case 2:

                    // Hent med graphql
                    Database.GraphQlContext gdbc = new Database.GraphQlContext();
                    var session = gdbc.Neo4jDriver.Session();
                    var allAppointments = session.ExecuteRead(tx =>
                    {
                        var res = tx.Run("match (p:Patient {patient_id: " + patientId + "})-[:SCHEDULED_FOR] ->(a) return a");

                        appointments = res.Select(record =>
                        {
                            var node = record["a"].As<INode>();
                            var props = node.Properties;
                            IFormatProvider format = new CultureInfo("da-DK");
                            return new DTO.Appointment
                            {
                                Id = int.Parse(props["appointment_id"].ToString()),
                                Place = props["place"].ToString(),
                                Start = DateTime.Parse(props["appointment_date"].ToString(), format),
                                End = DateTime.Parse(props["appointment_date_end"].ToString(), format)
                            };
                            
                        }).ToList();

                        return appointments;
                    });
                break;
            }

            return JsonSerializer.Serialize(appointments);
        }

        public void CreateAppointment(Models.Appointment appointment)
        {
            // MySql
            Database.MySQLContext mysql = new Database.MySQLContext(Database.MySqlAccountType.ReadWrite);

            mysql.Db.Open();
            var command = new MySqlCommand("CreateAppointment", mysql.Db);
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

            // MongoDB
            Database.MongoDbContext mdbc = new Database.MongoDbContext();
            MongoClient mc = new MongoClient(mdbc.ConnectionString);

            var database = mc.GetDatabase("HMS");
            var appointmentCollection = database.GetCollection<Models.Appointment>("appointments");


            appointmentCollection.InsertOne(appointment);

            // Neo4j
            Database.GraphQlContext gdbc = new Database.GraphQlContext();
            var session = gdbc.Neo4jDriver.Session();
            string place = appointment.Clinic?.Name != null ? appointment.Clinic.Name : appointment.Hospital.Name;
            var createAppointment = session.ExecuteWrite(tx => 
            {
                var res = tx.Run($@"CREATE (a:Appointment {{ 
                    appointment_id: {int.Parse(appointment.AppointmentId.ToString())}, 
                    appointment_date: '{DateTime.Parse(appointment.AppointmentDate.ToString())}', 
                    appointment_date_end: '{DateTime.Parse(appointment.AppointmentDateEnd.ToString())}', 
                    place: '{place}'
                        }}) 
                    WITH a 
                    MATCH (p:Patient {{patient_id: {appointment.Patient.PatientId}}}) 
                    MATCH (d:Doctor {{doctor_id: {appointment.DoctorId}}}) 
                    CREATE (p)-[:SCHEDULED_FOR]->(a)<-[:SCHEDULED_FOR]-(d)");

                return res;
            });
        }

        public void UpdateAppointment(Models.Appointment appointment)
        {
            // MySql
            Database.MySQLContext mysql = new Database.MySQLContext(Database.MySqlAccountType.ReadWrite);

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

            // MongoDB
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

            // Neo4j
            Database.GraphQlContext gdbc = new Database.GraphQlContext();
            var session = gdbc.Neo4jDriver.Session();

            var updateAppointment = session.ExecuteWrite(tx =>
            {
                var res = tx.Run($@"MATCH (a:Appointment 
                {{ 
                appointment_id: {int.Parse(appointment.AppointmentId.ToString())}
                }}) 
                SET a.appointment_date = '{DateTime.Parse(appointment.AppointmentDate.ToString())}', 
                a.appointment_date_end = '{DateTime.Parse(appointment.AppointmentDateEnd.ToString())}'");

                return res;
            });
        }

        public void DeleteAppointment(int appointmentId)
        {
            // Hent med mysql..
            Database.MySQLContext mysql = new Database.MySQLContext(Database.MySqlAccountType.FullAdmin);

            mysql.Db.Open();
            var command = new MySqlCommand("DELETE FROM hms.appointment WHERE id = @appointmentId;", mysql.Db);
            command.Parameters.AddWithValue("@appointmentId", appointmentId);

            command.ExecuteNonQuery();
            mysql.Db.Close();

            // Hent med mongo db..
            Database.MongoDbContext mdbc = new Database.MongoDbContext();
            MongoClient mc = new MongoClient(mdbc.ConnectionString);

            var database = mc.GetDatabase("HMS");
            var appointmentCollection = database.GetCollection<Models.Appointment>("appointments");

            var filter = Builders<Models.Appointment>.Filter.Eq(a => a.AppointmentId, appointmentId);

            appointmentCollection.DeleteOne(filter);

            // Hent med graphql
            Database.GraphQlContext gdbc = new Database.GraphQlContext();
            var session = gdbc.Neo4jDriver.Session();

            var deleteAppointment = session.ExecuteWrite(tx =>
            {
                var res = tx.Run($@"MATCH (a:Appointment {{ appointment_id: {int.Parse(appointmentId.ToString())}}})
                                DETACH DELETE a ");

                return res;
            });
        }
    }
}
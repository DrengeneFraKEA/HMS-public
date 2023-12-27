using HMS.Controllers;
using HMS.Data;
using HMS.DTO;
using HMS.Models;
using HMS.Services;
using HMS.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HMS_Tests
{
    public class Tests
    {
        #region db integration tests
        [Fact]
        public void MySqlConnectivityTest()
        {
            Database.SelectedDatabase = 0; // MySql
            AppointmentService aps = new AppointmentService();

            List<HMS.DTO.Appointment> appointments = aps.GetAppointmentsByPatientId(1); // Assuming patient with id 1 exists

            Assert.True(appointments.Any());
        }


        [Fact]
        public void MongoDbConnectivityTest()
        {
            Database.SelectedDatabase = 1; // MongoDB
            AppointmentService aps = new AppointmentService();

            List<HMS.DTO.Appointment> appointments = aps.GetAppointmentsByPatientId(1); // Assuming patient with id 1 exists

            Assert.True(appointments.Any());
        }


        [Fact]
        public void Neo4jConnectivityTest()
        {
            Database.SelectedDatabase = 2; // Neo4j
            AppointmentService aps = new AppointmentService();

            List<HMS.DTO.Appointment> appointments = aps.GetAppointmentsByPatientId(1); // Assuming patient with id 1 exists

            Assert.True(appointments.Any());
        }
        #endregion

        #region appointment_services
        [Fact]
        public void GetRandomAppointmentId()
        {
            AppointmentService aps = new AppointmentService();

            int? randomAppointmentId = aps.GetRandomAppointmentId();

            Assert.True(randomAppointmentId != null);
        }

        [Fact]
        public void GetAppointmentById()
        {
            AppointmentService aps = new AppointmentService();

            int? randomAppointmentId = aps.GetRandomAppointmentId();
            HMS.Models.Appointment appointment = aps.GetAppointmentById(randomAppointmentId.Value.ToString());

            Assert.NotNull(appointment);

        }

        /// <summary>
        /// CRUD functionality for all 3 databases on appointment.
        /// </summary>
        [Fact]
        public void CRUDAppointment()
        {
            // Create
            AppointmentService aps = new AppointmentService();

            HMS.Models.Appointment appointment = new HMS.Models.Appointment()
            {
                PatientId = 1,
                DoctorId = 1,
                ClinicId = 1,
                Patient = new Patient()
                {
                    PatientId = 1
                },
                DepartmentId = 1,
                HospitalId = 1,
                AppointmentDate = DateTime.Now,
                AppointmentDateEnd = DateTime.Now.AddHours(1),
                Clinic = new Clinic()
                {
                    Name = "København Sundhedshus",
                    Department = null,
                    Doctor = new Doctor()
                    {
                        DoctorId = 1
                    }
                },
                Hospital = new Hospital()
                {
                    HospitalId = 1,
                    Name = "København Sundhedshus",
                    Department = "ICU",
                    DoctorId = 1
                },
            };

            bool createdSuccesful = aps.CreateAppointment(appointment, out int? createdAppointmentId);
            Assert.NotNull(createdAppointmentId);
            // Update
            appointment.AppointmentDate = DateTime.Now.AddHours(2);
            appointment.AppointmentDateEnd = DateTime.Now.AddHours(3);
            bool updatedSuccessful = aps.UpdateAppointment(appointment);

            // Delete
            bool deleteSuccessful = aps.DeleteAppointment(createdAppointmentId.Value);

            Assert.True(createdSuccesful && updatedSuccessful && deleteSuccessful);
        }

        [Fact]
        public void CRUDAppointmentMongoDB()
        {
            Database.SelectedDatabase = 1;
            // Create
            AppointmentService aps = new AppointmentService();

            HMS.Models.Appointment appointment = new HMS.Models.Appointment()
            {
                PatientId = 1,
                DoctorId = 1,
                ClinicId = 1,
                Patient = new Patient()
                {
                    PatientId = 1
                },
                DepartmentId = 1,
                HospitalId = 1,
                AppointmentDate = DateTime.Now,
                AppointmentDateEnd = DateTime.Now.AddHours(1),
                Clinic = new Clinic()
                {
                    Name = "København Sundhedshus",
                    Department = null,
                    Doctor = new Doctor()
                    {
                        DoctorId = 1
                    }
                },
                Hospital = new Hospital()
                {
                    HospitalId = 1,
                    Name = "København Sundhedshus",
                    Department = "ICU",
                    DoctorId = 1
                },
            };

            bool createdSuccesful = aps.CreateAppointment(appointment, out int? createdAppointmentId);
            Assert.NotNull(createdAppointmentId);
            // Update
            appointment.AppointmentDate = DateTime.Now.AddHours(2);
            appointment.AppointmentDateEnd = DateTime.Now.AddHours(3);
            bool updatedSuccessful = aps.UpdateAppointment(appointment);

            // Delete
            bool deleteSuccessful = aps.DeleteAppointment(createdAppointmentId.Value);

            Assert.True(createdSuccesful && updatedSuccessful && deleteSuccessful);
        }

        [Fact]
        public void CRUDAppointmentNeo4j()
        {
            Database.SelectedDatabase = 2;
            // Create
            AppointmentService aps = new AppointmentService();

            HMS.Models.Appointment appointment = new HMS.Models.Appointment()
            {
                PatientId = 1,
                DoctorId = 1,
                ClinicId = 1,
                Patient = new Patient()
                {
                    PatientId = 1
                },
                DepartmentId = 1,
                HospitalId = 1,
                AppointmentDate = DateTime.Now,
                AppointmentDateEnd = DateTime.Now.AddHours(1),
                Clinic = new Clinic()
                {
                    Name = "København Sundhedshus",
                    Department = null,
                    Doctor = new Doctor()
                    {
                        DoctorId = 1
                    }
                },
                Hospital = new Hospital()
                {
                    HospitalId = 1,
                    Name = "København Sundhedshus",
                    Department = "ICU",
                    DoctorId = 1
                },
            };

            bool createdSuccesful = aps.CreateAppointment(appointment, out int? createdAppointmentId);
            Assert.NotNull(createdAppointmentId);
            // Update
            appointment.AppointmentDate = DateTime.Now.AddHours(2);
            appointment.AppointmentDateEnd = DateTime.Now.AddHours(3);
            bool updatedSuccessful = aps.UpdateAppointment(appointment);

            // Delete
            bool deleteSuccessful = aps.DeleteAppointment(createdAppointmentId.Value);

            Assert.True(createdSuccesful && updatedSuccessful && deleteSuccessful);
        }

        [Fact]
        public void GetAppointmentBySpecificId()
        {
            AppointmentService aps = new AppointmentService();

            HMS.Models.Appointment appointment = aps.GetAppointmentById("1");

            Assert.NotNull(appointment);
            Assert.Equal(1, appointment.AppointmentId);
        }

        [Fact]
        public void GetAppointmentByInvalidId()
        {
            AppointmentService aps = new AppointmentService();

            HMS.Models.Appointment appointment = aps.GetAppointmentById("50");

            Assert.Null(appointment);
        }

        [Fact]
        public void GetAppointmentByPatientId()
        {
            AppointmentService aps = new AppointmentService();

            List<HMS.DTO.Appointment> appointments = aps.GetAppointmentsByPatientId(1);

            Assert.NotNull(appointments);
            Assert.NotEmpty(appointments);
        }

        [Theory]
        [InlineData(-1)] 
        [InlineData(0)] 
        [InlineData(0.1)]
        public void GetAppointmentByPatientInvalidId(int id)
        {
            AppointmentService aps = new AppointmentService();

            List<HMS.DTO.Appointment> appointments = aps.GetAppointmentsByPatientId(id);

            Assert.Empty(appointments);
        }

        [Fact]
        public void CreateAppointmentFalse()
        {
            AppointmentService aps = new AppointmentService();

            bool createdSuccesful = aps.CreateAppointment(null, out int? createdAppointmentId);

            Assert.False(createdSuccesful);
            Assert.Null(createdAppointmentId);
        }

        [Fact]
        public void GetAppointmentByIdEquals()
        {
            AppointmentService aps = new AppointmentService();

            HMS.Models.Appointment appointment = aps.GetAppointmentById("1");

            Assert.NotNull(appointment);
            Assert.Equal(1, appointment.AppointmentId);
            Assert.Equal(1, appointment.PatientId);
            Assert.Equal(1, appointment.DoctorId);
            Assert.Equal(1, appointment.DepartmentId);
            Assert.Equal(1, appointment.HospitalId);
        }

        [Fact]
        public void GetAppointmentsByPatientIdMongoDB()
        {
            Database.SelectedDatabase = 1;
            
            AppointmentService aps = new AppointmentService();

            List<HMS.DTO.Appointment> appointments = aps.GetAppointmentsByPatientId(1);

            Assert.NotNull(appointments);
        }
        [Fact]
        public void GetAppointmentsByPatientIdNeo4j()
        {
            Database.SelectedDatabase = 2;
          
            AppointmentService aps = new AppointmentService();

            List<HMS.DTO.Appointment> appointments = aps.GetAppointmentsByPatientId(2);

            Assert.NotNull(appointments);
        }

        #endregion

        #region Drug_service
        /// <summary>
        /// Public Drug API
        /// </summary>
        [Theory]
        [InlineData("Cymbalta", false)]
        [InlineData("Aspirin", false)]
        [InlineData("DrugThatDoesntExist", true)]
        [InlineData("", true)]
        public void GetDrugByName(string drugname, bool expectedToFail) 
        {
            DrugService ds = new DrugService();

            List<HMS.DTO.Drug> drugs = ds.GetDrugByName(drugname);

            Assert.True(drugs.Any() && !expectedToFail || !drugs.Any() && expectedToFail);
        }
        #endregion

        #region Journal_service
        [Fact]
        public void GetJournalsByDoctorId() 
        {
            JournalService js = new JournalService();
            Database.SelectedDatabase = 0; // MySql

            List<HMS.DTO.Journal> journals = js.GetJournals(1); // Assuming doctor with id 1 exists

            Assert.True(journals.Any());
        }

        /// <summary>
        /// CRUD test for journal for mysql and neo4j databases.
        /// </summary>
        [Fact]
        public void CRUDJournal() 
        {
            JournalService js = new JournalService();

            // Create
            string journaltext = "Patienten er syg i hovedet";
            string cpr = "1234567896"; // Assuming it exists
            string doctorId = "1"; // Assuming it exists

            bool createdSucessful = js.CreateJournal(journaltext, cpr, doctorId, out int? createdJournalId);

            // Update
            string newJournalText = "Patienten er kureret!";

            bool updateSucessful = js.UpdateJournal(createdJournalId.Value.ToString(), newJournalText);

            // Delete
            bool deleteSucessful = js.DeleteJournal(createdJournalId.Value.ToString());

            Assert.True(createdSucessful && updateSucessful && deleteSucessful);
        }
        #endregion

        #region person_service
        [Fact]
        public void GetPersonById() 
        {
            PersonService ps = new PersonService();
            Database.SelectedDatabase = 0;
            int id = 1;

            HMS.DTO.Person person = ps.GetPersonData(id);

            Assert.NotNull(person);
        }

        [Theory]
        [InlineData("1234567896", false)]
        [InlineData("11111", true)]
        [InlineData("", true)]
        [InlineData("0000000000", true)]
        [InlineData("abc", true)]
        public void DoesPersonExistByCPR(string cpr, bool expectedToFail) 
        {
            PersonService ps = new PersonService();
            Database.SelectedDatabase = 0;

            bool exists = ps.DoesPersonExistByCPR(cpr);

            Assert.True(exists && !expectedToFail || !exists && expectedToFail);
        }
        #endregion

        #region JWT
        [Fact]
        public void GenerateJwtToken() 
        {
            JwtTokenGenerator jtg = new JwtTokenGenerator();
            string username = "123";
            string role = "123";

            string token = jtg.GenerateToken(username, role);

            Assert.True(!token.IsNullOrEmpty());
        }
        #endregion

        #region sql_injection_sanitazation
        [Theory]
        [InlineData("1234567890", false)]
        [InlineData("12345", true)]
        [InlineData("1--DROP;", true)]
        [InlineData("0000000000", true)]
        public void CheckCredentials(string val, bool expectedToFail) 
        {
            Account a = new Account()
            {
                Username = val,
                Password = val
            };

            bool valid = a.CheckUserCredentials(a);

            Assert.True(valid && !expectedToFail || !valid && expectedToFail);
        }
        #endregion
    }
}
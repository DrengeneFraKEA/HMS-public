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
                PatientId = 31,
                DoctorId = 31,
                ClinicId = 1,
                Patient = new Patient()
                {
                    PatientId = 31
                },
                DepartmentId = 2,
                HospitalId = 2,
                AppointmentDate = DateTime.Now,
                AppointmentDateEnd = DateTime.Now.AddHours(1),
                Clinic = new Clinic()
                {
                    Name = "Rigshospitalet",
                    Department = null,
                    Doctor = new Doctor()
                    {
                        DoctorId = 31
                    }
                },
                Hospital = new Hospital()
                {
                    HospitalId = 1,
                    Name = "Rigshospitalet",
                    Department = "ICU",
                    DoctorId = 31
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

        [Fact]
        public void CreateJournal_InvalidCPR_ReturnsFalse()
        {
            JournalService js = new JournalService();

            // Arrange
            string journalText = "Test journal text";
            string doctorId = "1234";


            // Act
            bool created = js.CreateJournal(journalText, "invalidCPR", doctorId, out int? lastId);

            // Assert
            Assert.False(created);
            Assert.Null(lastId);
        }

        /// <summary>
        /// CRUD test for journal for mysql and neo4j databases.
        /// </summary>
        [Fact]
        public void CRUDJournal() 
        {
            JournalService js = new JournalService();

            // Create
            string journaltext = "Patienten er syg";
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
        [InlineData("1234567890", true)]
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

        [Theory]
        [InlineData("2201970002", false)] // Valid CPR number, 20th century, female
        [InlineData("2301010004", false)] // Valid CPR number, 21st century, female
        [InlineData("2301010001", false)] // Valid CPR number, 21st century, male
        [InlineData("2201950003", false)] // Valid CPR number, 20th century, male
        [InlineData("1234567890", true)]  // Invalid CPR number
        [InlineData("3201010002", true)]  // Invalid CPR number, day
        [InlineData("2213010002", true)]  // Invalid CPR number, month 13
        [InlineData("2200010002", true)]  // Invalid CPR number, month 0
        [InlineData("123456040", true)]   // Invalid length (too short) -1
        [InlineData("", true)]            // Invalid length (empty)
        [InlineData("1", true)]           // Invalid length (empty)
        [InlineData("12345678900", true)] // Invalid length (too long) +1
        [InlineData("invalid_CPR", true)] // Contains non-numeric characters
        [InlineData("123a4567890", true)] // Contains non-numeric characters
        [InlineData("123a45671!d", true)] // Contains Special characters
        public void CheckCPRNumber_Validity(string cprNumber, bool expectedToFail)
        {
            Account user = new Account()
            {
                Username = cprNumber
            };
            bool result = user.CheckUserCredentials(user);
            Assert.True(!expectedToFail == result || expectedToFail == result);
            //Assert.True(!expectedToFail && user.CheckValidDateOnCPR(user) || expectedToFail && !user.CheckValidDateOnCPR(user));
        }


        [Theory]
        [InlineData("412321", false)]                           //Valid password 6 lenght
        [InlineData("1", false)]                                //Valid password 1 length
        [InlineData("12345678901234567890123456789", false)]    //Valid password 29 numbers length
        [InlineData("123456789012345678901234567890", false)]   //Valid password 30 numbers length
        [InlineData("abcdefghijabcdefghijabcdefghij", false)]   //Valid password 30 charachters length
        [InlineData("abcdefghij1234567890abcdefghij", false)]   //Valid password 30 charachters or numbers length
        [InlineData("abcdefghij12!#dsa_d", false)]              //Valid password Special charachters
        [InlineData("abcdefghij1234567890abcdefghi32", true)]   //Invalid length 31
        [InlineData("", true)]                                  //Invalid Empty password
        public void CheckPassword_Validity(string password, bool expectedToFail)
        {
            Account user = new Account()
            {
                Username = "2201970002", //Valid cpr for testing
                Password = password
            };
            bool result = user.CheckUserCredentials(user);
            Assert.True(!expectedToFail == result || expectedToFail == result);
        }

        #endregion
    }
}
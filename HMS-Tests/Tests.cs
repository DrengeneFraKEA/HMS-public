using HMS.Controllers;
using HMS.Data;
using HMS.DTO;
using HMS.Models;
using HMS.Services;

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

            int? randomAppointmentId = aps.GetRandomAppointmentId();

            Assert.True(randomAppointmentId != null);
        }

        [Fact]
        public void MongoDbConnectivityTest()
        {
            Database.SelectedDatabase = 1; // MongoDB
            AppointmentService aps = new AppointmentService();

            int? randomAppointmentId = aps.GetRandomAppointmentId();

            Assert.True(randomAppointmentId != null);
        }

        [Fact]
        public void Neo4jConnectivityTest()
        {
            Database.SelectedDatabase = 2; // Neo4j
            AppointmentService aps = new AppointmentService();

            int? randomAppointmentId = aps.GetRandomAppointmentId();

            Assert.True(randomAppointmentId != null);
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
                    Name = "K�benhavn Sundhedshus",
                    Department = null,
                    Doctor = new Doctor()
                    {
                        DoctorId = 1
                    }
                },
                Hospital = new Hospital()
                {
                    HospitalId = 1,
                    Name = "K�benhavn Sundhedshus",
                    Department = "ICU",
                    DoctorId = 1
                },
            };

            bool createdSuccesful = aps.CreateAppointment(appointment, out int? createdAppointmentId);

            // Update
            appointment.AppointmentDate = DateTime.Now.AddHours(2);
            appointment.AppointmentDateEnd = DateTime.Now.AddHours(3);
            bool updatedSuccessful = aps.UpdateAppointment(appointment);

            // Delete
            bool deleteSuccessful = aps.DeleteAppointment(createdAppointmentId.Value);

            Assert.True(createdSuccesful && updatedSuccessful && deleteSuccessful);
        }
        #endregion
    }
}
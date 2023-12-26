using HMS.Controllers;
using HMS.Data;
using HMS.DTO;
using HMS.Services;

namespace HMS_Tests
{
    public class Tests
    {
        /*
        [Fact]
        public void MySqlConnectivityTest()
        {
            Database.SelectedDatabase = 0; // MySql
            AppointmentService aps = new AppointmentService();

            string jsonAppointments = aps.GetAppointmentsByPatientId(1); // assuming patient by id 1 exists

            Assert.True(jsonAppointments != "");
        }
        
        [Fact]
        public void MongoDbConnectivityTest()
        {
            Database.SelectedDatabase = 1; // MongoDB
            AppointmentService aps = new AppointmentService();

            string jsonAppointments = aps.GetAppointmentsByPatientId(1); // assuming patient by id 1 exists

            Assert.True(jsonAppointments != "");
        }
        */

        [Fact]
        public void Neo4jConnectivityTest()
        {
            Database.SelectedDatabase = 2; // Neo4j
            AppointmentService aps = new AppointmentService();

            string jsonAppointments = aps.GetAppointmentsByPatientId(1); // assuming patient by id 1 exists

            Assert.True(jsonAppointments != "");
        }
       
        [Fact]
        public void GithubSecretTest()
        {
            string secret = Environment.GetEnvironmentVariable("MYSECRET");

            Assert.True(secret == "secret exposed 2");
        }
    }
}
using HMS.Controllers;
using HMS.Data;
using HMS.DTO;
using HMS.Services;
using System.Text;

namespace HMS_Tests
{
    public class Tests
    {

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


        [Fact]
        public void Neo4jConnectivityTest()
        {
            Database.SelectedDatabase = 2; // Neo4j
            AppointmentService aps = new AppointmentService();

            string jsonAppointments = aps.GetAppointmentsByPatientId(1); // assuming patient by id 1 exists

            Assert.True(jsonAppointments != "");
        }

        [Fact]
        public void RegisterAndLogin()
        {
            // Register


            // Login
            using (HttpClient client = new HttpClient())
            {
                var jsonBody = "{\"username\":\"1234567890\",\"password\":\"1234\"}";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = client.PostAsync("http://localhost:44402/login", content).Result;
            }
        }

    }
}
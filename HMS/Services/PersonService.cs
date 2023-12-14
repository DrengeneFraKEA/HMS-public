using HMS.Data;
using HMS.DTO;
using MySqlConnector;
using System.Text.Json;

namespace HMS.Services
{
    public class PersonService
    {
        public string GetPersonData(int id) 
        {
            DTO.Person person = new Person();
            switch (Database.SelectedDatabase) 
            {
                case 0:
                    Database.MySQLContext mysql = new Database.MySQLContext();
                    mysql.Db.Open();

                    var command = new MySqlCommand($"SELECT * FROM persondata WHERE id = {id};", mysql.Db);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        person = new Person()
                        {
                            Firstname = reader.GetString("first_name"),
                            Lastname = reader.GetString("last_name"),
                            CPR = reader.GetString("cpr"),
                            Address = reader.GetString("address"),
                            Phonenumber = reader.GetInt32("contact_number")
                        };
                    }

                    mysql.Db.Close();
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }

            return JsonSerializer.Serialize(person);
        }

        public bool DoesPersonExistByCPR(string cpr)
        {
            bool legit = cpr.Length == 10 && int.TryParse(cpr, out _);
            if (!legit) return false;

            bool exists = false;

            switch (Database.SelectedDatabase)
            {
                case 0:
                    Database.MySQLContext mysql = new Database.MySQLContext();

                    mysql.Db.Open();

                    var command = new MySqlCommand($"SELECT * FROM personData WHERE cpr = {cpr};", mysql.Db);
                    var reader = command.ExecuteReader();
                    exists = reader.HasRows;

                    mysql.Db.Close();
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }

            return exists;
        }
    }
}

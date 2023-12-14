using HMS.Data;
using HMS.Models;
using MySqlConnector;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;

namespace HMS.Services
{
    public class JournalService
    {
        public string GetJournals(int doctorid) 
        {
            List<DTO.Journal> journals = new List<DTO.Journal>();

            switch (Database.SelectedDatabase) 
            {
                case 0:
                    Database.MySQLContext mysql = new Database.MySQLContext();

                    mysql.Db.Open();

                    var command = new MySqlCommand($"SELECT * FROM journal WHERE doctor_id = {doctorid};", mysql.Db);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DTO.Journal journal = new DTO.Journal()
                        {
                            Id = reader.GetInt32("id"),
                            Note = reader.GetString("journalnotes"),
                            CreatedOn = reader.GetDateTime("created_on"),
                            ModifiedOn = reader.GetDateTime("modified_on"),
                            CPR = reader.GetString("cpr")
                        };

                        journals.Add(journal);
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }

            return JsonSerializer.Serialize(journals);
        }

        public bool CreateJournal(string journaltext, string cpr, string doctorid) 
        {
            bool legit = cpr.Length == 10 && int.TryParse(cpr, out _);
            if (!legit) return false;

            // MySQL
            DateTime dt = DateTime.Now;
            string now = dt.ToString("yyyy-MM-dd HH:mm:ss");
            Database.MySQLContext mysql = new Database.MySQLContext();

            mysql.Db.Open();

            var command = new MySqlCommand($"INSERT INTO journal (doctor_id, journalnotes, created_on, modified_on, cpr) " +
                $"VALUES ('{doctorid}', '{journaltext}', '{now}', '{now}', '{cpr}')", mysql.Db);

            command.ExecuteReader();

            // MongoDB


            // GraphQL

            return true;
        }

        public bool DeleteJournal(string journalid) 
        {
            // MySQL
            Database.MySQLContext mysql = new Database.MySQLContext();

            mysql.Db.Open();

            var command = new MySqlCommand($"DELETE FROM journal WHERE id = {journalid}", mysql.Db);

            command.ExecuteReader();

            mysql.Db.Close();

            // MongoDB


            // GraphQL

            return true;
        }

        public bool UpdateJournal(string journalid, string newjournaltext) 
        {
            // MySQL
            DateTime dt = DateTime.Now;
            string now = dt.ToString("yyyy-MM-dd HH:mm:ss");

            Database.MySQLContext mysql = new Database.MySQLContext();

            mysql.Db.Open();

            var command = new MySqlCommand($"UPDATE journal SET journalnotes = '{newjournaltext}', modified_on = '{now}' " +
                $"WHERE id = {journalid}", mysql.Db);

            command.ExecuteReader();

            mysql.Db.Close();

            // MongoDB


            // GraphQL

            return true;
        }
    }
}

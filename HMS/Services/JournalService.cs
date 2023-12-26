using HMS.Data;
using HMS.DTO;
using HMS.Models;
using MySqlConnector;
using Neo4j.Driver;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;

namespace HMS.Services
{
    public class JournalService
    {
        public List<DTO.Journal> GetJournals(int doctorid) 
        {
            List<DTO.Journal> journals = new List<DTO.Journal>();

            switch (Database.SelectedDatabase)
            {
                case 0:
                    Database.MySQLContext mysql = new Database.MySQLContext(Database.MySqlAccountType.ReadOnly);

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

            return journals;
        }

        public bool CreateJournal(string journaltext, string cpr, string doctorid, out int? lastId) 
        {
            bool legit = cpr.Length == 10 && int.TryParse(cpr, out _);
            if (!legit) 
            {
                lastId = null;
                return false;
            }

            // MySQL
            DateTime dt = DateTime.Now;
            string now = dt.ToString("yyyy-MM-dd HH:mm:ss");
            Database.MySQLContext mysql = new Database.MySQLContext(Database.MySqlAccountType.ReadWrite);

            mysql.Db.Open();

            var command = new MySqlCommand($"INSERT INTO journal (doctor_id, journalnotes, created_on, modified_on, cpr) " +
                $"VALUES ('{doctorid}', '{journaltext}', '{now}', '{now}', '{cpr}')", mysql.Db);

            command.ExecuteReader();

            int lastInsertedId = int.Parse(command.LastInsertedId.ToString());
            lastId = lastInsertedId;

            mysql.Db.Close();

            // MongoDB
            // WIP..

            // GraphQL
            Database.GraphQlContext gdbc = new Database.GraphQlContext();
            var session = gdbc.Neo4jDriver.Session();
            
            var createJournal = session.ExecuteWrite(tx =>
            {
                var res = tx.Run($@"CREATE (j:Journal {{ 
                    journal_id: {lastInsertedId}, 
                    doctor_id: '{doctorid}', 
                    journalnotes: '{journaltext}', 
                    created_on: '{now}',
                    modified_on: '{now}',
                    cpr: '{cpr}'
                        }}) 
                    WITH j 
                    MATCH (p:Patient {{cpr: {cpr}}}) 
                    MATCH (d:Doctor {{doctor_id: {doctorid}}}) 
                    CREATE (p)-[:ON_BEHALF_OF]->(j)<-[:WRITES]-(d)");

                return res;
            });

            return true;
        }

        public bool DeleteJournal(string journalid) 
        {
            // MySQL
            Database.MySQLContext mysql = new Database.MySQLContext(Database.MySqlAccountType.FullAdmin);

            mysql.Db.Open();

            var command = new MySqlCommand($"DELETE FROM journal WHERE id = {journalid}", mysql.Db);

            command.ExecuteReader();

            mysql.Db.Close();

            // MongoDB


            // GraphQL
            Database.GraphQlContext gdbc = new Database.GraphQlContext();
            var session = gdbc.Neo4jDriver.Session();

            var deleteAppointment = session.ExecuteWrite(tx =>
            {
                var res = tx.Run($@"MATCH (j:Journal {{ journal_id: {journalid}}})
                                DETACH DELETE j ");

                return res;
            });
            return true;
        }

        public bool UpdateJournal(string journalid, string newjournaltext) 
        {
            // MySQL
            DateTime dt = DateTime.Now;
            string now = dt.ToString("yyyy-MM-dd HH:mm:ss");

            Database.MySQLContext mysql = new Database.MySQLContext(Database.MySqlAccountType.ReadWrite);

            mysql.Db.Open();

            var command = new MySqlCommand($"UPDATE journal SET journalnotes = '{newjournaltext}', modified_on = '{now}' " +
                $"WHERE id = {journalid}", mysql.Db);

            command.ExecuteReader();

            mysql.Db.Close();

            // MongoDB


            // GraphQL
            Database.GraphQlContext gdbc = new Database.GraphQlContext();
            var session = gdbc.Neo4jDriver.Session();

            var updateJournal = session.ExecuteWrite(tx =>
            {
                var res = tx.Run($@"MATCH (j:Journal 
                {{ 
                journal_id: {journalid}
                }}) 
                SET j.journalnotes = '{newjournaltext}', 
                j.modified_on = '{now}'");

                return res;
            });
            return true;
        }
    }
}

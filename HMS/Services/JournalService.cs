using HMS.Data;
using HMS.DTO;
using HMS.Models;
using MongoDB.Driver;
using MySqlConnector;
using Neo4j.Driver;
using System.Globalization;
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
                    // MongoDB
                    Database.MongoDbContext mdbc = new Database.MongoDbContext();
                    MongoClient mc = new MongoClient(mdbc.ConnectionString);

                    var database = mc.GetDatabase("HMS");
                    var journalCollection = database.GetCollection<Models.Journal>("journals");

                    var filter = Builders<Models.Journal>.Filter.Eq(a => a.DoctorId, doctorid);
                    var documents = journalCollection.Find(filter).ToList();

                    var result = documents.Select(journal => new DTO.Journal
                    {
                        Id = journal.JournalId,
                        Note = journal.JournalNote,
                        CreatedOn = journal.CreatedOn,
                        ModifiedOn = journal.ModifiedOn,
                        CPR = journal.CPR
                    }).ToList();

                    foreach (var element in result) journals.Add(element);
                    break;
                case 2:
                    // Hent med graphql
                    Database.GraphQlContext gdbc = new Database.GraphQlContext();
                    var session = gdbc.Neo4jDriver.Session();
                    var allJournals = session.ExecuteRead(tx =>
                    {
                        var res = tx.Run("match (j:Journal {doctor_id: " + doctorid + "}) return j");

                        journals = res.Select(record =>
                        {
                            var node = record["j"].As<INode>();
                            var props = node.Properties;
                            IFormatProvider format = new CultureInfo("da-DK");
                            return new DTO.Journal
                            {
                                Id = int.Parse(props["journal_id"].ToString()),
                                Note = props["journalnotes"].ToString(),
                                CreatedOn = DateTime.Parse(props["created_on"].ToString(), format),
                                ModifiedOn = DateTime.Parse(props["modified_on"].ToString(), format),
                                CPR = props["cpr"].ToString()
                            };

                        }).ToList();

                        return journals;
                    });
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
            Database.MongoDbContext mdbc = new Database.MongoDbContext();
            MongoClient mc = new MongoClient(mdbc.ConnectionString);

            var database = mc.GetDatabase("HMS");
            var journalCollection = database.GetCollection<Models.Journal>("journals");

            Models.Journal journal = new Models.Journal()
            {
                JournalId = lastInsertedId,
                DoctorId = int.Parse(doctorid),
                JournalNote = journaltext,
                CreatedOn = DateTime.Parse(now),
                ModifiedOn = DateTime.Parse(now),
                CPR = cpr
            };

            journalCollection.InsertOne(journal);

            // GraphQL
            Database.GraphQlContext gdbc = new Database.GraphQlContext();
            var session = gdbc.Neo4jDriver.Session();
            
            var createJournal = session.ExecuteWrite(tx =>
            {
                var res = tx.Run($@"CREATE (j:Journal {{ 
                    journal_id: {lastInsertedId}, 
                    doctor_id: {doctorid}, 
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
            Database.MongoDbContext mdbc = new Database.MongoDbContext();
            MongoClient mc = new MongoClient(mdbc.ConnectionString);

            var database = mc.GetDatabase("HMS");
            var journalCollection = database.GetCollection<Models.Journal>("journals");
            int jid = int.Parse(journalid);

            var filter = Builders<Models.Journal>.Filter.Eq(j => j.JournalId, jid);

            journalCollection.DeleteOne(filter);

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
            Database.MongoDbContext mdbc = new Database.MongoDbContext();
            MongoClient mc = new MongoClient(mdbc.ConnectionString);
            var database = mc.GetDatabase("HMS");
            var journalCollection = database.GetCollection<Models.Journal>("journals");
            int jid = int.Parse(journalid);

            var filter = Builders<Models.Journal>.Filter.Eq(j => j.JournalId, jid);
            var update = Builders<Models.Journal>.Update
                .Set(j => j.JournalNote, newjournaltext)
                .Set(j => j.ModifiedOn, dt);

            journalCollection.UpdateOne(filter, update);

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

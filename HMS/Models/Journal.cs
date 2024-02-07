using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HMS.Models;

[BsonIgnoreExtraElements]
public class Journal
{
    [BsonElement("_id")]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    [BsonElement("journalid")]
    public int JournalId { get; set; }
    [BsonElement("doctorid")]
    public int DoctorId { get; set; }
    [BsonElement("journalnote")]
    public string? JournalNote { get; set; }
    [BsonElement("createdon")]
    public DateTime? CreatedOn { get; set; }
    [BsonElement("modifiedon")]
    public DateTime? ModifiedOn { get; set; }
    [BsonElement("cpr")]
    public string CPR { get; set; }
}
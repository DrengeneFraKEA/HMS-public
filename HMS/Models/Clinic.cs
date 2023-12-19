

using MongoDB.Bson.Serialization.Attributes;

namespace HMS.Models;
[BsonIgnoreExtraElements]
public class Clinic
{
    [BsonElement("id")]
    public int ClinicId { get; set; }
    public int DepartmenttId { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("doctor")]
    public Doctor Doctor { get; set; }

    [BsonElement("department")]
    public string? Department { get; set; }
}
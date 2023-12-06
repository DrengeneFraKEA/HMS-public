using MongoDB.Bson.Serialization.Attributes;

namespace HMS.Models;

[BsonIgnoreExtraElements]
public class Hospital
{
    [BsonElement("hospital_id")]
    public int HospitalId { get; set; }
    public string? Name { get; set; }
    

}
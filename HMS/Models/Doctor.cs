using MongoDB.Bson.Serialization.Attributes;

namespace HMS.Models;

public class Doctor
{
    [BsonElement("id")]
    public int DoctorId { get; set; }
    public int PersonDataId { get; set; }
    
}
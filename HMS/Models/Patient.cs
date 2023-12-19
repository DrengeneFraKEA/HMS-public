using MongoDB.Bson.Serialization.Attributes;

namespace HMS.Models;
[BsonIgnoreExtraElements]
public class Patient
{
    [BsonElement("id")]
    public int PatientId { get; set; }
    public int ClinicId { get; set; }
    public int PersonDataId { get; set; }

    [BsonElement("appointments")]
    public List<Appointment>? Appointments { get; set; }
}
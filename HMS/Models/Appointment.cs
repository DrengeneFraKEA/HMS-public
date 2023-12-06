

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HMS.Models;
[BsonIgnoreExtraElements]
public class Appointment
{


    [BsonElement("_id")]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("appointment_id")]
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int? ClinicId { get; set; }
    public int DoctorId { get; set; }
    public int DepartmentId { get; set; }
    public int? HospitalId { get; set; }

    [BsonElement("appointment_date")]
    public DateTime AppointmentDate { get; set; }

    [BsonElement("appointment_date_end")]
    public DateTime AppointmentDateEnd { get; set; }

    [BsonElement("patient")]
    public Patient Patient { get; set; }

    [BsonElement("clinic")]
    public Clinic Clinic { get; set; }




}
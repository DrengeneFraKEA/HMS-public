

namespace HMS.Models;

public class Appointment
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int ClinicId { get; set; }
    public int DoctorId { get; set; }
    public int DepartmentId { get; set; }
    public int HospitalId { get; set; }
    public DateTime AppointmentDateId { get; set; }

}
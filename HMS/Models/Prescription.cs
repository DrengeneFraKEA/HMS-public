namespace HMS.Models;

public class Prescription
{
    public int PrescriptionId { get; set; }
    public int DrugId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string? Dosage { get; set; }
    public string? Instructions { get; set; }

}
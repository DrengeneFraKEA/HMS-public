namespace HMS.Models;

public class Rating
{
    public int RatingId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; } 
    public string? Title { get; set; }
    public string? Text { get; set; }
    public int RatingAverage { get; set; }
}
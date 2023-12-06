namespace HMS.DTO
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string Place { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}

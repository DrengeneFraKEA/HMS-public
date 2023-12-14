namespace HMS.DTO
{
    public class Journal
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CPR { get; set; }
    }
}

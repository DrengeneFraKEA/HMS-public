using System.ComponentModel.DataAnnotations;

namespace HMS.Models;

public class PersonData
{
    public int Id { get; set; }
    public string? First_Name { get; set; }
    public string? Last_Name { get; set; }
    public string? Email {  get; set; }
    public int Contact_Number { get; set; }
    public char CPR { get; set; }
    public string? Address { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace HMS.Models;

public class PersonData
{
    public int PersonDataId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int ContactNumber { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(10)]
    public char CPR { get; set; }

    public string? Address { get; set; }

}
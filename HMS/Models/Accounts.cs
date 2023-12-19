namespace HMS.Models;

public class Accounts
{
    public string? CPR { get; set; }
    public string? Password { get; set; } 
}

    public enum Role { 
        Admin, Doctor, Patient
     }
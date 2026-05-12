using System.ComponentModel.DataAnnotations;

namespace SerwisSamochodowy.Models;

public class Users
{
    [Key]
    public int IdUser { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public string ApiToken { get; set; }

    public bool IsAdmin { get; set; }
}
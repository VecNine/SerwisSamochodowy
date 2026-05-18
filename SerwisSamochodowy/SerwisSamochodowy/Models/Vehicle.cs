using System.ComponentModel.DataAnnotations;

namespace SerwisSamochodowy.Models;

public class Vehicle
{
    [Key]
    public int IdVehicle { get; set; }

    [Required]
    public string Model { get; set; }

    [Required]
    public int Year { get; set; }
    [Required]
    public string OwnerName { get; set; }
    [Required]
    public string OwnerSurname { get; set; }
    [Required]
    public int OwnerPhone { get; set; }
    
    public List<Repairment> Repairments { get; set; } = new();
}
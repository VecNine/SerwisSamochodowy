using System.ComponentModel.DataAnnotations;

namespace SerwisSamochodowy.Models;

public class Mechanic
{
    [Key]
    public int IdMechanic { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Surname { get; set; }

    [Required]
    public decimal Salary { get; set; }
    
    public List<Repairment> Repairments { get; set; } = new();
}
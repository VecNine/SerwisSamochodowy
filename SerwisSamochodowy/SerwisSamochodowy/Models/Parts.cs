using System.ComponentModel.DataAnnotations;

namespace SerwisSamochodowy.Models;

public class Parts
{
    [Key]
    public int IdPart { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public decimal Cost { get; set; }
    
    public List<RepairmentParts> RepairmentParts { get; set; } = new();
}
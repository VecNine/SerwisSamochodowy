using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Dodaj ten using!

namespace SerwisSamochodowy.Models;

public class Repairment
{
    [Key]
    public int IdRepairment { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public decimal ServicePrice { get; set; }

    // --- KLUCZE OBCE ---
    public int IdVehicle { get; set; }
    
    [ForeignKey("IdVehicle")]
    public Vehicle? Vehicle { get; set; }

    public int IdMechanic { get; set; }
    
    [ForeignKey("IdMechanic")] 
    public Mechanic? Mechanic { get; set; }
    
    public List<RepairmentParts> RepairmentParts { get; set; } = new();
}
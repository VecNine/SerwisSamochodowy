using System.ComponentModel.DataAnnotations;

namespace SerwisSamochodowy.Models;

public class Repairment
{
    [Key]
    public int IdRepairment { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public decimal ServicePrice { get; set; }

    public int IdVehicle { get; set; }
    public Vehicle Vehicle { get; set; }

    public int IdMechanic { get; set; }
    public Mechanic Mechanic { get; set; }
    
    public List<RepairmentParts> RepairmentParts { get; set; } = new();
}
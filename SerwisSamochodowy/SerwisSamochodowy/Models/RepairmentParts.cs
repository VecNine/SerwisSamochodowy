namespace SerwisSamochodowy.Models;

public class RepairmentParts
{
    public int IdRepairment { get; set; }
    public Repairment Repairment { get; set; }
    public int IdPart { get; set; }
    public Parts Part { get; set; }

    public int Quantity { get; set; }
}
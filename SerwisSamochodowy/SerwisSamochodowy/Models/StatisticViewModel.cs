namespace SerwisSamochodowy.Models;

public class StatisticsViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public decimal TotalServiceRevenue { get; set; }
    public decimal TotalPartsValue { get; set; }
    public decimal TotalMechanicSalaries { get; set; }
    public decimal FinalProfit => TotalServiceRevenue - TotalMechanicSalaries;
    
    public List<MechanicPerformanceDetail> MechanicStats { get; set; } = new();
    
    public List<TopVehicleStats> TopVehicles { get; set; } = new();
}

public class MechanicPerformanceDetail
{
    public string FullName { get; set; }
    public int MonthsWorkedInRange { get; set; }
    public decimal SalaryCosts { get; set; }
    public int RepairCount { get; set; }
    public decimal RevenueGenerated { get; set; }
}

public class TopVehicleStats
{
    public int VehicleId { get; set; }
    public string OwnerFullName { get; set; }
    public string VehicleModel { get; set; }
    public int RepairCount { get; set; }
    public decimal TotalSpent { get; set; }
}
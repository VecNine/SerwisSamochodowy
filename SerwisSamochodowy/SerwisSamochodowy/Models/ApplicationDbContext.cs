using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SerwisSamochodowy.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Mechanic> Mechanics { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Parts> Parts { get; set; }
    public DbSet<Repairment> Repairments { get; set; }
    public DbSet<RepairmentParts> RepairmentParts { get; set; }
    public DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<RepairmentParts>()
            .HasKey(rp => new { rp.IdRepairment, rp.IdPart });
        
        modelBuilder.Entity<RepairmentParts>()
            .HasOne(rp => rp.Repairment)
            .WithMany(r => r.RepairmentParts)
            .HasForeignKey(rp => rp.IdRepairment); 
        
        modelBuilder.Entity<RepairmentParts>()
            .HasOne(rp => rp.Part)
            .WithMany(p => p.RepairmentParts)
            .HasForeignKey(rp => rp.IdPart);

        modelBuilder.Entity<Repairment>()
            .HasOne(r => r.Vehicle)
            .WithMany(v => v.Repairments)
            .HasForeignKey(r => r.IdVehicle);
        
        modelBuilder.Entity<Repairment>()
            .HasOne(r => r.Mechanic)
            .WithMany(m => m.Repairments)
            .HasForeignKey(r => r.IdMechanic);
    }
    
    public static string GetMd5Hash(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = MD5.HashData(inputBytes);
        return Convert.ToHexString(hashBytes).ToLower();
    }
}
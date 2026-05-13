using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerwisSamochodowy.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mechanics",
                columns: table => new
                {
                    IdMechanic = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Surname = table.Column<string>(type: "TEXT", nullable: false),
                    Salary = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mechanics", x => x.IdMechanic);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    IdPart = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Cost = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.IdPart);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    ApiToken = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    IdVehicle = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.IdVehicle);
                });

            migrationBuilder.CreateTable(
                name: "Repairments",
                columns: table => new
                {
                    IdRepairment = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ServicePrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    IdVehicle = table.Column<int>(type: "INTEGER", nullable: false),
                    VehicleIdVehicle = table.Column<int>(type: "INTEGER", nullable: false),
                    IdMechanic = table.Column<int>(type: "INTEGER", nullable: false),
                    MechanicIdMechanic = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repairments", x => x.IdRepairment);
                    table.ForeignKey(
                        name: "FK_Repairments_Mechanics_MechanicIdMechanic",
                        column: x => x.MechanicIdMechanic,
                        principalTable: "Mechanics",
                        principalColumn: "IdMechanic",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repairments_Vehicles_VehicleIdVehicle",
                        column: x => x.VehicleIdVehicle,
                        principalTable: "Vehicles",
                        principalColumn: "IdVehicle",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RepairmentParts",
                columns: table => new
                {
                    IdRepairment = table.Column<int>(type: "INTEGER", nullable: false),
                    IdPart = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairmentParts", x => new { x.IdRepairment, x.IdPart });
                    table.ForeignKey(
                        name: "FK_RepairmentParts_Parts_IdPart",
                        column: x => x.IdPart,
                        principalTable: "Parts",
                        principalColumn: "IdPart",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepairmentParts_Repairments_IdRepairment",
                        column: x => x.IdRepairment,
                        principalTable: "Repairments",
                        principalColumn: "IdRepairment",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepairmentParts_IdPart",
                table: "RepairmentParts",
                column: "IdPart");

            migrationBuilder.CreateIndex(
                name: "IX_Repairments_MechanicIdMechanic",
                table: "Repairments",
                column: "MechanicIdMechanic");

            migrationBuilder.CreateIndex(
                name: "IX_Repairments_VehicleIdVehicle",
                table: "Repairments",
                column: "VehicleIdVehicle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepairmentParts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "Repairments");

            migrationBuilder.DropTable(
                name: "Mechanics");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}

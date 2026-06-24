using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NurseSchedulingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToNurse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Nurses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Nurses");
        }
    }
}

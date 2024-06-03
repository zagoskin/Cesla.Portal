using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cesla.Portal.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLogicalDeleteForEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                schema: "cesla",
                table: "Employees",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                schema: "cesla",
                table: "Employees");
        }
    }
}

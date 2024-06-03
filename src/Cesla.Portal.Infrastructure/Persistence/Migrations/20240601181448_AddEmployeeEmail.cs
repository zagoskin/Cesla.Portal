using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cesla.Portal.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "cesla",
                table: "Employees",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "cesla",
                table: "Employees");
        }
    }
}

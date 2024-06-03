using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cesla.Portal.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cesla");

            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "cesla",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    CompanyName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    FantasyName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Cnpj = table.Column<string>(type: "varchar(18)", maxLength: 18, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    AddressLine = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    State = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Country = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies",
                schema: "cesla");
        }
    }
}

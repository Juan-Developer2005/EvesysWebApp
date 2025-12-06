using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvesysWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRolCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RolId", "Nombre" },
                values: new object[] { 3, "Cliente" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RolId",
                keyValue: 3);
        }
    }
}

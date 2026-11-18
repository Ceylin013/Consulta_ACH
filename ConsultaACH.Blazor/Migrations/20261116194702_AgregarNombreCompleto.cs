using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaACH.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNombreCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NombreConmpleto",
                table: "AspNetUsers",
                newName: "NombreCompleto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NombreCompleto",
                table: "AspNetUsers",
                newName: "NombreConmpleto");
        }
    }
}

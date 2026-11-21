using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaACH.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarActivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "activo",
                table: "AspNetUsers",
                newName: "Activo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "AspNetUsers",
                newName: "activo");
        }
    }
}

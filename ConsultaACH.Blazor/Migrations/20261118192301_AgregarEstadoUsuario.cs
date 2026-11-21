using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaACH.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activo",
                table: "AspNetUsers");
        }
    }
}

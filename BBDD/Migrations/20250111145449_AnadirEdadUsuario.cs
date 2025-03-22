using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDD.Migrations
{
    /// <inheritdoc />
    public partial class AnadirEdadUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "edad",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "edad",
                table: "Usuarios");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyect.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarBaseDeDatosGre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Abonos__IdEstado__778AC167",
                table: "Abonos");

            migrationBuilder.DropTable(
                name: "EstadosAbonos");

            migrationBuilder.DropIndex(
                name: "IX_Abonos_IdEstadoAbono",
                table: "Abonos");

            migrationBuilder.DropColumn(
                name: "IdEstadoAbono",
                table: "Abonos");

            migrationBuilder.AlterColumn<string>(
                name: "Contraseña",
                table: "Usuarios",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Contraseña",
                table: "Usuarios",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdEstadoAbono",
                table: "Abonos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EstadosAbonos",
                columns: table => new
                {
                    IdEstadoAbono = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EstadosA__6956405AAAC400AF", x => x.IdEstadoAbono);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abonos_IdEstadoAbono",
                table: "Abonos",
                column: "IdEstadoAbono");

            migrationBuilder.AddForeignKey(
                name: "FK__Abonos__IdEstado__778AC167",
                table: "Abonos",
                column: "IdEstadoAbono",
                principalTable: "EstadosAbonos",
                principalColumn: "IdEstadoAbono");
        }
    }
}

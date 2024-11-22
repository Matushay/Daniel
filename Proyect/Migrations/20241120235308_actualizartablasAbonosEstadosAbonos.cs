using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyect.Migrations
{
    /// <inheritdoc />
    public partial class actualizartablasAbonosEstadosAbonos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Abonos__IdReserv__75A278F5",
                table: "Abonos");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Abonos__A4693DA78D83026D",
                table: "Abonos");

            migrationBuilder.DropColumn(
                name: "CantidadAbono",
                table: "Abonos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Abonos");

            migrationBuilder.DropColumn(
                name: "IVA",
                table: "Abonos");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Abonos",
                newName: "ValorAbono");

            migrationBuilder.RenameColumn(
                name: "Subtotal",
                table: "Abonos",
                newName: "Pendiente");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInicio",
                table: "Reservas",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFin",
                table: "Reservas",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "IdEstadoAbono",
                table: "Abonos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Abonos__A4693DA7FFF9E5A1",
                table: "Abonos",
                column: "IdAbono");

            migrationBuilder.CreateTable(
                name: "EstadosAbonos",
                columns: table => new
                {
                    IdEstadoAbono = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
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

            migrationBuilder.AddForeignKey(
                name: "FK__Abonos__IdReserv__76969D2E",
                table: "Abonos",
                column: "IdReserva",
                principalTable: "Reservas",
                principalColumn: "IdReserva");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Abonos__IdEstado__778AC167",
                table: "Abonos");

            migrationBuilder.DropForeignKey(
                name: "FK__Abonos__IdReserv__76969D2E",
                table: "Abonos");

            migrationBuilder.DropTable(
                name: "EstadosAbonos");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Abonos__A4693DA7FFF9E5A1",
                table: "Abonos");

            migrationBuilder.DropIndex(
                name: "IX_Abonos_IdEstadoAbono",
                table: "Abonos");

            migrationBuilder.DropColumn(
                name: "IdEstadoAbono",
                table: "Abonos");

            migrationBuilder.RenameColumn(
                name: "ValorAbono",
                table: "Abonos",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "Pendiente",
                table: "Abonos",
                newName: "Subtotal");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaInicio",
                table: "Reservas",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaFin",
                table: "Reservas",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "CantidadAbono",
                table: "Abonos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "Abonos",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IVA",
                table: "Abonos",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Abonos__A4693DA78D83026D",
                table: "Abonos",
                column: "IdAbono");

            migrationBuilder.AddForeignKey(
                name: "FK__Abonos__IdReserv__75A278F5",
                table: "Abonos",
                column: "IdReserva",
                principalTable: "Reservas",
                principalColumn: "IdReserva");
        }
    }
}

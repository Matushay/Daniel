using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyect.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDocumento = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Documento = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Direccion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Celular = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    CorreoElectronico = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Clientes__D5946642560A07B1", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "EstadoReserva",
                columns: table => new
                {
                    IdEstadoReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estados = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EstadoRe__3E654CA85050FB3E", x => x.IdEstadoReserva);
                });

            migrationBuilder.CreateTable(
                name: "MetodoPago",
                columns: table => new
                {
                    IdMetodoPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MetodoPa__6F49A9BECCCBF33A", x => x.IdMetodoPago);
                });

            migrationBuilder.CreateTable(
                name: "Paquetes",
                columns: table => new
                {
                    IdPaquete = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Paquetes__DE278F8B25567580", x => x.IdPaquete);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    IdPermiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePermiso = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permisos__0D626EC81BF47727", x => x.IdPermiso);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRol = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__2A49584C0F208FF6", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    IdServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Servicio__2DCCF9A27DE1C009", x => x.IdServicio);
                });

            migrationBuilder.CreateTable(
                name: "TipoHabitaciones",
                columns: table => new
                {
                    IdTipoHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoHabi__AB75E87CF22C5C7B", x => x.IdTipoHabitacion);
                });

            migrationBuilder.CreateTable(
                name: "TipoMueble",
                columns: table => new
                {
                    IdTipoMueble = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoMueb__C451A954F856429B", x => x.IdTipoMueble);
                });

            migrationBuilder.CreateTable(
                name: "RolesPermisos",
                columns: table => new
                {
                    IdRolPermiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRol = table.Column<int>(type: "int", nullable: true),
                    IdPermiso = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RolesPer__0CC73B1B6304D448", x => x.IdRolPermiso);
                    table.ForeignKey(
                        name: "FK__RolesPerm__IdPer__3D5E1FD2",
                        column: x => x.IdPermiso,
                        principalTable: "Permisos",
                        principalColumn: "IdPermiso");
                    table.ForeignKey(
                        name: "FK__RolesPerm__IdRol__3C69FB99",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDocumento = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Documento = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Celular = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Direccion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    CorreoElectronico = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Contraseña = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IdRol = table.Column<int>(type: "int", nullable: false),
                    CodigoRestablecimiento = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__5B65BF9703073B92", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK__Usuarios__IdRol__4316F928",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol");
                });

            migrationBuilder.CreateTable(
                name: "PaquetesServicios",
                columns: table => new
                {
                    IdPaqueteServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaquete = table.Column<int>(type: "int", nullable: true),
                    IdServicio = table.Column<int>(type: "int", nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Paquetes__DE5C2BC22DECE05D", x => x.IdPaqueteServicio);
                    table.ForeignKey(
                        name: "FK__PaquetesS__IdPaq__6383C8BA",
                        column: x => x.IdPaquete,
                        principalTable: "Paquetes",
                        principalColumn: "IdPaquete");
                    table.ForeignKey(
                        name: "FK__PaquetesS__IdSer__6477ECF3",
                        column: x => x.IdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio");
                });

            migrationBuilder.CreateTable(
                name: "Habitaciones",
                columns: table => new
                {
                    IdHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IdTipoHabitacion = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitaci__8BBBF9018C946199", x => x.IdHabitacion);
                    table.ForeignKey(
                        name: "FK__Habitacio__IdTip__534D60F1",
                        column: x => x.IdTipoHabitacion,
                        principalTable: "TipoHabitaciones",
                        principalColumn: "IdTipoHabitacion");
                });

            migrationBuilder.CreateTable(
                name: "Muebles",
                columns: table => new
                {
                    IdMueble = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    IdTipoMueble = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Muebles__829D15E8C1304F0A", x => x.IdMueble);
                    table.ForeignKey(
                        name: "FK__Muebles__IdTipoM__4D94879B",
                        column: x => x.IdTipoMueble,
                        principalTable: "TipoMueble",
                        principalColumn: "IdTipoMueble");
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    IdReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaReserva = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IVA = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdPaquete = table.Column<int>(type: "int", nullable: false),
                    IdEstadoReserva = table.Column<int>(type: "int", nullable: false),
                    IdMetodoPago = table.Column<int>(type: "int", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reservas__0E49C69D5897E3BE", x => x.IdReserva);
                    table.ForeignKey(
                        name: "FK_Reservas_Usuarios_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                    table.ForeignKey(
                        name: "FK__Reservas__IdClie__6D0D32F4",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente");
                    table.ForeignKey(
                        name: "FK__Reservas__IdEsta__6FE99F9F",
                        column: x => x.IdEstadoReserva,
                        principalTable: "EstadoReserva",
                        principalColumn: "IdEstadoReserva");
                    table.ForeignKey(
                        name: "FK__Reservas__IdMeto__70DDC3D8",
                        column: x => x.IdMetodoPago,
                        principalTable: "MetodoPago",
                        principalColumn: "IdMetodoPago");
                    table.ForeignKey(
                        name: "FK__Reservas__IdPaqu__6EF57B66",
                        column: x => x.IdPaquete,
                        principalTable: "Paquetes",
                        principalColumn: "IdPaquete");
                });

            migrationBuilder.CreateTable(
                name: "PaquetesHabitaciones",
                columns: table => new
                {
                    IdPaqueteHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaquete = table.Column<int>(type: "int", nullable: true),
                    IdHabitacion = table.Column<int>(type: "int", nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Paquetes__C0EF4AF019115212", x => x.IdPaqueteHabitacion);
                    table.ForeignKey(
                        name: "FK__PaquetesH__IdHab__60A75C0F",
                        column: x => x.IdHabitacion,
                        principalTable: "Habitaciones",
                        principalColumn: "IdHabitacion");
                    table.ForeignKey(
                        name: "FK__PaquetesH__IdPaq__5FB337D6",
                        column: x => x.IdPaquete,
                        principalTable: "Paquetes",
                        principalColumn: "IdPaquete");
                });

            migrationBuilder.CreateTable(
                name: "HabitacionMueble",
                columns: table => new
                {
                    IdHabitacionMueble = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdHabitacion = table.Column<int>(type: "int", nullable: false),
                    IdMueble = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitaci__402E049DD27D6B9C", x => x.IdHabitacionMueble);
                    table.ForeignKey(
                        name: "FK__Habitacio__IdHab__5629CD9C",
                        column: x => x.IdHabitacion,
                        principalTable: "Habitaciones",
                        principalColumn: "IdHabitacion");
                    table.ForeignKey(
                        name: "FK__Habitacio__IdMue__571DF1D5",
                        column: x => x.IdMueble,
                        principalTable: "Muebles",
                        principalColumn: "IdMueble");
                });

            migrationBuilder.CreateTable(
                name: "Abonos",
                columns: table => new
                {
                    IdAbono = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdReserva = table.Column<int>(type: "int", nullable: false),
                    FechaAbono = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Valordeuda = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Pendiente = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorAbono = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Porcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Comprobante = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Anulado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Abonos__A4693DA7FFF9E5A1", x => x.IdAbono);
                    table.ForeignKey(
                        name: "FK__Abonos__IdReserv__76969D2E",
                        column: x => x.IdReserva,
                        principalTable: "Reservas",
                        principalColumn: "IdReserva");
                });

            migrationBuilder.CreateTable(
                name: "DetalleHabitaciones",
                columns: table => new
                {
                    IdDetalleHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdReserva = table.Column<int>(type: "int", nullable: false),
                    IdHabitacion = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetalleH__15D58EF948F23CFC", x => x.IdDetalleHabitacion);
                    table.ForeignKey(
                        name: "FK__DetalleHa__IdHab__03F0984C",
                        column: x => x.IdHabitacion,
                        principalTable: "Habitaciones",
                        principalColumn: "IdHabitacion");
                    table.ForeignKey(
                        name: "FK__DetalleHa__IdRes__02FC7413",
                        column: x => x.IdReserva,
                        principalTable: "Reservas",
                        principalColumn: "IdReserva");
                });

            migrationBuilder.CreateTable(
                name: "DetallePaquetes",
                columns: table => new
                {
                    IdDetallePaquete = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdReserva = table.Column<int>(type: "int", nullable: false),
                    IdPaquete = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetalleP__4DD779D7BABEF416", x => x.IdDetallePaquete);
                    table.ForeignKey(
                        name: "FK__DetallePa__IdPaq__7F2BE32F",
                        column: x => x.IdPaquete,
                        principalTable: "Paquetes",
                        principalColumn: "IdPaquete");
                    table.ForeignKey(
                        name: "FK__DetallePa__IdRes__7E37BEF6",
                        column: x => x.IdReserva,
                        principalTable: "Reservas",
                        principalColumn: "IdReserva");
                });

            migrationBuilder.CreateTable(
                name: "DetalleServicios",
                columns: table => new
                {
                    IdDetalleServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdReserva = table.Column<int>(type: "int", nullable: false),
                    IdServicio = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetalleS__0BFF94E69DEE454C", x => x.IdDetalleServicio);
                    table.ForeignKey(
                        name: "FK__DetalleSe__IdRes__797309D9",
                        column: x => x.IdReserva,
                        principalTable: "Reservas",
                        principalColumn: "IdReserva");
                    table.ForeignKey(
                        name: "FK__DetalleSe__IdSer__7A672E12",
                        column: x => x.IdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abonos_IdReserva",
                table: "Abonos",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "UQ__Clientes__531402F3A819F6C7",
                table: "Clientes",
                column: "CorreoElectronico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetalleHabitaciones_IdHabitacion",
                table: "DetalleHabitaciones",
                column: "IdHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleHabitaciones_IdReserva",
                table: "DetalleHabitaciones",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_DetallePaquetes_IdPaquete",
                table: "DetallePaquetes",
                column: "IdPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_DetallePaquetes_IdReserva",
                table: "DetallePaquetes",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleServicios_IdReserva",
                table: "DetalleServicios",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleServicios_IdServicio",
                table: "DetalleServicios",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Habitaciones_IdTipoHabitacion",
                table: "Habitaciones",
                column: "IdTipoHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionMueble_IdHabitacion",
                table: "HabitacionMueble",
                column: "IdHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionMueble_IdMueble",
                table: "HabitacionMueble",
                column: "IdMueble");

            migrationBuilder.CreateIndex(
                name: "IX_Muebles_IdTipoMueble",
                table: "Muebles",
                column: "IdTipoMueble");

            migrationBuilder.CreateIndex(
                name: "IX_PaquetesHabitaciones_IdHabitacion",
                table: "PaquetesHabitaciones",
                column: "IdHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_PaquetesHabitaciones_IdPaquete",
                table: "PaquetesHabitaciones",
                column: "IdPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_PaquetesServicios_IdPaquete",
                table: "PaquetesServicios",
                column: "IdPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_PaquetesServicios_IdServicio",
                table: "PaquetesServicios",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdCliente",
                table: "Reservas",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdEstadoReserva",
                table: "Reservas",
                column: "IdEstadoReserva");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdMetodoPago",
                table: "Reservas",
                column: "IdMetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdPaquete",
                table: "Reservas",
                column: "IdPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioIdUsuario",
                table: "Reservas",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_RolesPermisos_IdPermiso",
                table: "RolesPermisos",
                column: "IdPermiso");

            migrationBuilder.CreateIndex(
                name: "IX_RolesPermisos_IdRol",
                table: "RolesPermisos",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "UQ__Usuarios__531402F36AFB6096",
                table: "Usuarios",
                column: "CorreoElectronico",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abonos");

            migrationBuilder.DropTable(
                name: "DetalleHabitaciones");

            migrationBuilder.DropTable(
                name: "DetallePaquetes");

            migrationBuilder.DropTable(
                name: "DetalleServicios");

            migrationBuilder.DropTable(
                name: "HabitacionMueble");

            migrationBuilder.DropTable(
                name: "PaquetesHabitaciones");

            migrationBuilder.DropTable(
                name: "PaquetesServicios");

            migrationBuilder.DropTable(
                name: "RolesPermisos");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Muebles");

            migrationBuilder.DropTable(
                name: "Habitaciones");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "EstadoReserva");

            migrationBuilder.DropTable(
                name: "MetodoPago");

            migrationBuilder.DropTable(
                name: "Paquetes");

            migrationBuilder.DropTable(
                name: "TipoMueble");

            migrationBuilder.DropTable(
                name: "TipoHabitaciones");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}

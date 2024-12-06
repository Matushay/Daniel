using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Proyect.Models;

public partial class ProyectContext : DbContext
{
    public ProyectContext()
    {
    }

    public ProyectContext(DbContextOptions<ProyectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Abono> Abonos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetalleHabitacione> DetalleHabitaciones { get; set; }

    public virtual DbSet<DetallePaquete> DetallePaquetes { get; set; }

    public virtual DbSet<DetalleServicio> DetalleServicios { get; set; }

    public virtual DbSet<EstadoReserva> EstadoReservas { get; set; }

    public virtual DbSet<HabitacionMueble> HabitacionMuebles { get; set; }

    public virtual DbSet<Habitacione> Habitaciones { get; set; }

    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    public virtual DbSet<Mueble> Muebles { get; set; }

    public virtual DbSet<Paquete> Paquetes { get; set; }

    public virtual DbSet<PaquetesHabitacione> PaquetesHabitaciones { get; set; }

    public virtual DbSet<PaquetesServicio> PaquetesServicios { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesPermiso> RolesPermisos { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<TipoHabitacione> TipoHabitaciones { get; set; }

    public virtual DbSet<TipoMueble> TipoMuebles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Initial Catalog=Proyect;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abono>(entity =>
        {
            entity.HasKey(e => e.IdAbono).HasName("PK__Abonos__A4693DA7FFF9E5A1");

            entity.Property(e => e.FechaAbono)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Pendiente).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Porcentaje).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ValorAbono).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Valordeuda).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Abonos__IdReserv__76969D2E");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Clientes__D5946642560A07B1");

            entity.HasIndex(e => e.CorreoElectronico, "UQ__Clientes__531402F3A819F6C7").IsUnique();

            entity.Property(e => e.Apellido)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Celular)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.CorreoElectronico)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Documento)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TipoDocumento)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DetalleHabitacione>(entity =>
        {
            entity.HasKey(e => e.IdDetalleHabitacion).HasName("PK__DetalleH__15D58EF948F23CFC");

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdHabitacionNavigation).WithMany(p => p.DetalleHabitaciones)
                .HasForeignKey(d => d.IdHabitacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleHa__IdHab__03F0984C");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.DetalleHabitaciones)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleHa__IdRes__02FC7413");
        });

        modelBuilder.Entity<DetallePaquete>(entity =>
        {
            entity.HasKey(e => e.IdDetallePaquete).HasName("PK__DetalleP__4DD779D7BABEF416");

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.DetallePaquetes)
                .HasForeignKey(d => d.IdPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetallePa__IdPaq__7F2BE32F");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.DetallePaquetes)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetallePa__IdRes__7E37BEF6");
        });

        modelBuilder.Entity<DetalleServicio>(entity =>
        {
            entity.HasKey(e => e.IdDetalleServicio).HasName("PK__DetalleS__0BFF94E69DEE454C");

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.DetalleServicios)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleSe__IdRes__797309D9");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.DetalleServicios)
                .HasForeignKey(d => d.IdServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleSe__IdSer__7A672E12");

        });

        modelBuilder.Entity<EstadoReserva>(entity =>
        {
            entity.HasKey(e => e.IdEstadoReserva).HasName("PK__EstadoRe__3E654CA85050FB3E");

            entity.ToTable("EstadoReserva");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estados)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HabitacionMueble>(entity =>
        {
            entity.HasKey(e => e.IdHabitacionMueble).HasName("PK__Habitaci__402E049DD27D6B9C");

            entity.ToTable("HabitacionMueble");

            entity.HasOne(d => d.IdHabitacionNavigation).WithMany(p => p.HabitacionMuebles)
                .HasForeignKey(d => d.IdHabitacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Habitacio__IdHab__5629CD9C");

            entity.HasOne(d => d.IdMuebleNavigation).WithMany(p => p.HabitacionMuebles)
                .HasForeignKey(d => d.IdMueble)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Habitacio__IdMue__571DF1D5");
        });

        modelBuilder.Entity<Habitacione>(entity =>
        {
            entity.HasKey(e => e.IdHabitacion).HasName("PK__Habitaci__8BBBF9018C946199");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdTipoHabitacionNavigation).WithMany(p => p.Habitaciones)
                .HasForeignKey(d => d.IdTipoHabitacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Habitacio__IdTip__534D60F1");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__MetodoPa__6F49A9BECCCBF33A");

            entity.ToTable("MetodoPago");

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Mueble>(entity =>
        {
            entity.HasKey(e => e.IdMueble).HasName("PK__Muebles__829D15E8C1304F0A");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTipoMuebleNavigation).WithMany(p => p.Muebles)
                .HasForeignKey(d => d.IdTipoMueble)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Muebles__IdTipoM__4D94879B");
        });

        modelBuilder.Entity<Paquete>(entity =>
        {
            entity.HasKey(e => e.IdPaquete).HasName("PK__Paquetes__DE278F8B25567580");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<PaquetesHabitacione>(entity =>
        {
            entity.HasKey(e => e.IdPaqueteHabitacion).HasName("PK__Paquetes__C0EF4AF019115212");

            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdHabitacionNavigation).WithMany(p => p.PaquetesHabitaciones)
                .HasForeignKey(d => d.IdHabitacion)
                .HasConstraintName("FK__PaquetesH__IdHab__60A75C0F");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.PaquetesHabitaciones)
                .HasForeignKey(d => d.IdPaquete)
                .HasConstraintName("FK__PaquetesH__IdPaq__5FB337D6");
        });

        modelBuilder.Entity<PaquetesServicio>(entity =>
        {
            entity.HasKey(e => e.IdPaqueteServicio).HasName("PK__Paquetes__DE5C2BC22DECE05D");

            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.PaquetesServicios)
                .HasForeignKey(d => d.IdPaquete)
                .HasConstraintName("FK__PaquetesS__IdPaq__6383C8BA");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.PaquetesServicios)
                .HasForeignKey(d => d.IdServicio)
                .HasConstraintName("FK__PaquetesS__IdSer__6477ECF3");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PK__Permisos__0D626EC81BF47727");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            //entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.NombrePermiso)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__Reservas__0E49C69D5897E3BE");

            entity.Property(e => e.Descuento).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.FechaReserva)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("IVA");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservas__IdClie__6D0D32F4");

            entity.HasOne(d => d.IdEstadoReservaNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdEstadoReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservas__IdEsta__6FE99F9F");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservas__IdMeto__70DDC3D8");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservas__IdPaqu__6EF57B66");

        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__2A49584C0F208FF6");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreRol)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Estado).HasDefaultValue(true);       
        });

        modelBuilder.Entity<RolesPermiso>(entity =>
        {
            entity.HasKey(e => e.IdRolPermiso).HasName("PK__RolesPer__0CC73B1B6304D448");

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.IdPermiso)
                .HasConstraintName("FK__RolesPerm__IdPer__3D5E1FD2");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__RolesPerm__IdRol__3C69FB99");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.IdServicio).HasName("PK__Servicio__2DCCF9A27DE1C009");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Estado).HasDefaultValue(true);

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

        });


        modelBuilder.Entity<TipoHabitacione>(entity =>
        {
            entity.HasKey(e => e.IdTipoHabitacion).HasName("PK__TipoHabi__AB75E87CF22C5C7B");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoMueble>(entity =>
        {
            entity.HasKey(e => e.IdTipoMueble).HasName("PK__TipoMueb__C451A954F856429B");

            entity.ToTable("TipoMueble");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__5B65BF9703073B92");

            entity.HasIndex(e => e.CorreoElectronico, "UQ__Usuarios__531402F36AFB6096").IsUnique();

            entity.Property(e => e.Apellido)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Celular)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Contraseña)
               .HasMaxLength(30)
               .IsUnicode(false);
            entity.Property(e => e.CorreoElectronico)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Documento)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TipoDocumento)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CodigoRestablecimiento)
            .HasMaxLength(200)
            .IsUnicode(false)
            .IsRequired(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__IdRol__4316F928");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

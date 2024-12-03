using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models
{
    public partial class Reserva
    {
        public int IdReserva { get; set; }

        public DateTime FechaReserva { get; set; } = DateTime.Now;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Iva { get; set; }

        public decimal Total { get; set; }

        public decimal Descuento { get; set; }

        public int IdCliente { get; set; }

        public int IdPaquete { get; set; }

        public int IdEstadoReserva { get; set; }

        public int IdMetodoPago { get; set; }

        public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

        public virtual ICollection<DetalleHabitacione> DetalleHabitaciones { get; set; } = new List<DetalleHabitacione>();

        public virtual ICollection<DetallePaquete> DetallePaquetes { get; set; } = new List<DetallePaquete>();

        public virtual ICollection<DetalleServicio> DetalleServicios { get; set; } = new List<DetalleServicio>();

        [Display(Name = "Documento del Cliente")]
        public virtual Cliente IdClienteNavigation { get; set; }

        [Display(Name = "Estado de la Reserva")]
        public virtual EstadoReserva IdEstadoReservaNavigation { get; set; }

        [Display(Name = "Método de Pago")]
        public virtual MetodoPago IdMetodoPagoNavigation { get; set; }

        [Display(Name = "Nombre del Paquete")]
        public virtual Paquete IdPaqueteNavigation { get; set; }



    }
}

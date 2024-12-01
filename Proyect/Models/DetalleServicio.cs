using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class DetalleServicio
{
    public int IdDetalleServicio { get; set; }

    public int IdReserva { get; set; }

    public int IdServicio { get; set; }

    public int Cantidad { get; set; }

    public decimal Precio { get; set; }

    public bool Estado { get; set; }

    public virtual Reserva IdReservaNavigation { get; set; }

    public virtual Servicio IdServicioNavigation { get; set; }

}

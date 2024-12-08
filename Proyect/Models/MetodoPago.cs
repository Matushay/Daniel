using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class MetodoPago
{
    public int IdMetodoPago { get; set; }

    public string Nombre { get; set; }

    public bool Estado { get; set; } = true;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

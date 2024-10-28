using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class Paquete
{
    public int IdPaquete { get; set; }

    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public decimal Precio { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<DetallePaquete> DetallePaquetes { get; set; } = new List<DetallePaquete>();

    public virtual ICollection<PaquetesHabitacione> PaquetesHabitaciones { get; set; } = new List<PaquetesHabitacione>();

    public virtual ICollection<PaquetesServicio> PaquetesServicios { get; set; } = new List<PaquetesServicio>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

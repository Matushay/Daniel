using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class PaquetesHabitacione
{
    public int IdPaqueteHabitacion { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdHabitacion { get; set; }

    public decimal? Precio { get; set; }

    public virtual Habitacione IdHabitacionNavigation { get; set; }

    public virtual Paquete IdPaqueteNavigation { get; set; }
}

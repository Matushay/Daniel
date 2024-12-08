using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class HabitacionMueble
{
    public int IdHabitacionMueble { get; set; }

    public int IdHabitacion { get; set; }

    public int IdMueble { get; set; }

    public int Cantidad { get; set; }

    public virtual Habitacione IdHabitacionNavigation { get; set; }

    public virtual Mueble IdMuebleNavigation { get; set; }
}

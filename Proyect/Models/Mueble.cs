using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class Mueble
{
    public int IdMueble { get; set; }

    public string Nombre { get; set; }

    public int IdTipoMueble { get; set; }

    public string Descripcion { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<HabitacionMueble> HabitacionMuebles { get; set; } = new List<HabitacionMueble>();

    public virtual TipoMueble IdTipoMuebleNavigation { get; set; }
}

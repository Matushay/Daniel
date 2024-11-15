using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class Mueble
{
    public int IdMueble { get; set; }

    public string Nombre { get; set; }

    public int IdTipoMueble { get; set; }

    public string Descripcion { get; set; }

    public int Cantidad { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<HabitacionMueble> HabitacionMuebles { get; set; } = new List<HabitacionMueble>();

    [Display(Name = "Tipo de Mueble")]
    public virtual TipoMueble IdTipoMuebleNavigation { get; set; }
}

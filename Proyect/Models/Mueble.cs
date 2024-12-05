using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class Mueble
{
    public int IdMueble { get; set; }

    public string Nombre { get; set; }

    [Display(Name ="Tipo de Mueble")]
    public int IdTipoMueble { get; set; }

    [Required(ErrorMessage = "La descripción es requerida.")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage ="La cantidad es requerida")]
    public int Cantidad { get; set; }

    public bool Estado { get; set; } = true;

    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public virtual ICollection<HabitacionMueble> HabitacionMuebles { get; set; } = new List<HabitacionMueble>();

    [Display(Name = "Tipo de Mueble")]
    public virtual TipoMueble IdTipoMuebleNavigation { get; set; }
}

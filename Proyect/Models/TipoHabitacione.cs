using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Proyect.Models;

public partial class TipoHabitacione
{
    public int IdTipoHabitacion { get; set; }

    public string Nombre { get; set; }

    [Required(ErrorMessage = "La descripción es requerida.")]
    public string Descripcion { get; set; }

    public int Capacidad { get; set; }

    public virtual ICollection<Habitacione> Habitaciones { get; set; } = new List<Habitacione>();
}

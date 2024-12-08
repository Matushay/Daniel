using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class Habitacione
{
    public int IdHabitacion { get; set; }

    public string Nombre { get; set; }

    [Display(Name ="Tipo de Habitación")]
    public int IdTipoHabitacion { get; set; }

    public bool Estado { get; set; } = true;

    [Required(ErrorMessage = "La descripción es requerida.")]
    public string Descripcion { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Precio { get; set; }

    public int Cantidad { get; set; }

    public virtual ICollection<DetalleHabitacione> DetalleHabitaciones { get; set; } = new List<DetalleHabitacione>();

    public virtual ICollection<HabitacionMueble> HabitacionMuebles { get; set; } = new List<HabitacionMueble>();

    [Display (Name ="Tipo de Habitacion")]
    public virtual TipoHabitacione IdTipoHabitacionNavigation { get; set; }

    public virtual ICollection<PaquetesHabitacione> PaquetesHabitaciones { get; set; } = new List<PaquetesHabitacione>();
}

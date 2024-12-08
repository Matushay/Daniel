using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class PaquetesHabitacione
{
    public int IdPaqueteHabitacion { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdHabitacion { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal? Precio { get; set; }

    public virtual Habitacione IdHabitacionNavigation { get; set; }

    public virtual Paquete IdPaqueteNavigation { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class PaquetesServicio
{
    public int IdPaqueteServicio { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdServicio { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal? Precio { get; set; }

    public virtual Paquete IdPaqueteNavigation { get; set; }

    public virtual Servicio IdServicioNavigation { get; set; }
}

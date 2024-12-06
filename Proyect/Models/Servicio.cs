using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class Servicio
{
    public int IdServicio { get; set; }

    public string Nombre { get; set; }

    public bool Estado { get; set; } = true;

    [Required(ErrorMessage = "La descripción es requerida.")]
    public string Descripcion { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Precio { get; set; } 

    public virtual ICollection<DetalleServicio> DetalleServicios { get; set; } = new List<DetalleServicio>();

    public virtual ICollection<PaquetesServicio> PaquetesServicios { get; set; } = new List<PaquetesServicio>();

}

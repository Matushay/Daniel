using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class TipoMueble
{
    public int IdTipoMueble { get; set; }

    public string Nombre { get; set; }

    [Required(ErrorMessage = "La descripción es requerida.")]
    public string Descripcion { get; set; }

    public virtual ICollection<Mueble> Muebles { get; set; } = new List<Mueble>();
}

using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class TipoMueble
{
    public int IdTipoMueble { get; set; }

    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public virtual ICollection<Mueble> Muebles { get; set; } = new List<Mueble>();
}

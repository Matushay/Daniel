using System;
using System.Collections.Generic;

namespace Proyect.Models
{
    public partial class EstadosAbono
    {
        public int IdEstadoAbono { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    }
}

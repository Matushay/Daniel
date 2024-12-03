using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string TipoDocumento { get; set; }

    public string Documento { get; set; }

    public string Nombre { get; set; }

    public string Apellido { get; set; }

    public string Celular { get; set; }

    public string Direccion { get; set; }

    public string CorreoElectronico { get; set; }

    public bool Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public int IdRol { get; set; }

    public virtual Role IdRolNavigation { get; set; }

}

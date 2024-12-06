using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class Permiso
{
    public int IdPermiso { get; set; }

    [Display(Name = "Nombre del permiso")]
    public string NombrePermiso { get; set; }

    public string Descripcion { get; set; }

    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();
}

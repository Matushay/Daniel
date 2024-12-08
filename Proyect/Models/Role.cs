using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models; 

public partial class Role
{
    public int IdRol { get; set; }

    [Display (Name ="Rol")]
    public string NombreRol { get; set; }

    public string Descripcion { get; set; }
    public bool Estado { get; set; } = true;

    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();
  
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

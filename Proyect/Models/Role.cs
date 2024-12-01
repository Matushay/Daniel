using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class Role
{
    public int IdRol { get; set; }

    public string NombreRol { get; set; }

    public string Descripcion { get; set; }

    //public bool Protegido { get; set; } // Nueva propiedad para marcar roles protegidos

    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();
  
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyect.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    [Display(Name = "Tipo Documento")]
    public string TipoDocumento { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public string Documento { get; set; }

    public string Nombre { get; set; }

    public string Apellido { get; set; }

    public string Celular { get; set; }

    public string Direccion { get; set; }

    [Display(Name = "Correo Electronico")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    public string CorreoElectronico { get; set; }

    [NotMapped]
    public bool EsEdicion { get; set; } // Indicador de si es edición

    // Validación condicional
    public bool EsCorreoRequerido => !EsEdicion;

    public bool Estado { get; set; } = true;

    public string Contraseña { get; set; }


    [Display(Name = "Fecha de Creación")]
    public DateTime? FechaCreacion { get; set; }

    public int IdRol { get; set; }

    [Display(Name = "Rol")]
    public virtual Role IdRolNavigation { get; set; }

    public string CodigoRestablecimiento { get; set; } = null;
    
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    [Display(Name = "Tipo Documento")]
    public string TipoDocumento { get; set; }

    public string Documento { get; set; }

    public string Nombre { get; set; }

    public string Apellido { get; set; }

    public string Celular { get; set; }

    public string Direccion { get; set; }

    [Display(Name = "Correo Electronico")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    public string CorreoElectronico { get; set; }

    public bool Estado { get; set; }

    //[Required(ErrorMessage = "La contraseña es obligatoria.")]
    //[RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$", ErrorMessage = "La contraseña debe comenzar con una letra mayúscula, contener al menos un número, un carácter especial y tener al menos 8 caracteres.")]
    public string Contraseña { get; set; }

    [Display(Name = "Fecha de Creación")]
    public DateTime? FechaCreacion { get; set; }

    public int IdRol { get; set; }

    [Display(Name = "Rol")]
    public virtual Role IdRolNavigation { get; set; }

    public string CodigoRestablecimiento { get; set; } = null;
    
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyect.Models;

public partial class Cliente
{
    [Display(Name = "Id Cliente")]
    public int IdCliente { get; set; }

    [Display(Name = "Tipo Documento")]
    public string TipoDocumento { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public string Documento { get; set; }

    public string Nombre { get; set; }

    public string Apellido { get; set; }

    public string Direccion { get; set; }

    public string Celular { get; set; }

    [Display(Name = "Correo Electronico")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    public string CorreoElectronico { get; set; }

    public bool Estado { get; set; } = true;
     
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyect.Models;

public partial class Abono
{
    public int IdAbono { get; set; }

    public int IdReserva { get; set; }

    public DateTime FechaAbono { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El valor de la deuda no puede ser negativo.")]
    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Valordeuda { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Pendiente { get; set; }

    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal ValorAbono { get; set; }

    public decimal Porcentaje { get; set; }

    public byte[]? Comprobante { get; set; }

    public bool Anulado { get; set; }

    public virtual Reserva IdReservaNavigation { get; set; } = null!;
}
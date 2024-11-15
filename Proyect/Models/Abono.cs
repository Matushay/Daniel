using System;
using System.Collections.Generic;

namespace Proyect.Models;

public partial class Abono
{
    public int IdAbono { get; set; }
    public int IdReserva { get; set; }
    public DateTime FechaAbono { get; set; }
    public decimal Valordeuda { get; set; }
    public decimal Porcentaje { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }

    // Cambiado a byte[] para almacenar la imagen directamente
    public byte[] Comprobante { get; set; }

    public int? CantidadAbono { get; set; }
    public bool Estado { get; set; }
    public virtual Reserva IdReservaNavigation { get; set; }
}
<<<<<<< HEAD


internal class DisplayAttribute : Attribute
{
    public string Name { get; set; }
}
=======
>>>>>>> Daniel

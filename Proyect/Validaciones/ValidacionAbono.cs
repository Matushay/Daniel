using FluentValidation;
using Proyect.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Proyect.Validaciones
{
    public class ValidacionAbono : AbstractValidator<Abono>
    {
        private readonly ProyectContext _context;

        public ValidacionAbono(ProyectContext context)
        {
            _context = context;

            // Validación para el primer abono: debe ser al menos el 50% de la deuda
            RuleFor(a => a.ValorAbono)
                .GreaterThanOrEqualTo(a => a.Valordeuda * 0.5m)
                .When(a => AbonoEsPrimerAbono(a.IdReserva)) // Condición cuando es el primer abono
                .WithMessage("El primer abono debe ser al menos el 50% del valor de la deuda.");

            // Validación de que no se supere el valor de la deuda acumulando los abonos
            RuleFor(a => a.ValorAbono)
                .Must((abono, valorAbono) => NoSuperarDeudaTotal(abono.IdReserva, valorAbono, abono.IdAbono))
                .WithMessage("El valor del abono no puede hacer que el total supere el valor de la deuda.");
        }

        // Método para verificar si el abono es el primer abono
        private bool AbonoEsPrimerAbono(int idReserva)
        {
            // Verifica si es el primer abono
            return !_context.Abonos.Any(a => a.IdReserva == idReserva); // No hay abonos previos
        }

        // Método para verificar si el total acumulado no supera la deuda
        private bool NoSuperarDeudaTotal(int idReserva, decimal valorAbono, int idAbono)
        {
            // Sumar todos los abonos anteriores (excluyendo el actual) y no anulados
            var totalAbonado = _context.Abonos
                .Where(a => a.IdReserva == idReserva && a.IdAbono != idAbono && !a.Anulado)
                .Sum(a => a.ValorAbono);

            var reserva = _context.Reservas.FirstOrDefault(r => r.IdReserva == idReserva);
            if (reserva == null)
                return false; // No hay reserva asociada, lo cual es un error.

            var deudaTotal = reserva.Total;

            // Verifica que no se supere el valor de la deuda
            return totalAbonado + valorAbono <= deudaTotal;
        }
    }
}

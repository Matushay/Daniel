using FluentValidation;
using Proyect.Models;

namespace Proyect.Validaciones
{
    public class ValidacionReserva : AbstractValidator<Reserva>
    {
        public ValidacionReserva()
        {
            RuleFor(r => r.FechaFin)
                .GreaterThanOrEqualTo(r => r.FechaInicio)
                .WithMessage("La fecha de fin debe ser igual o posterior a la fecha de inicio.");
        }
    }
}

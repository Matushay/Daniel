using FluentValidation;
using Proyect.Models;

namespace Proyect.Validaciones
{
    public class ValidacionReserva : AbstractValidator<Reserva>
    {
        public ValidacionReserva() 
        {
            //RuleFor(x => x.FechaInicio)
            //    .NotEmpty().WithMessage("La fecha de inicio es obligatoria.")
            //    .Must(date => date >= DateOnly.FromDateTime(DateTime.Now)).WithMessage("La fecha de inicio no puede ser en el pasado.");

            RuleFor(x => x.FechaFin)
                .NotEmpty().WithMessage("La fecha de fin es obligatoria.")
                .GreaterThan(x => x.FechaInicio).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");

            RuleFor(x => x.Subtotal)
                .GreaterThan(0).WithMessage("El subtotal debe ser mayor que 0.");

            RuleFor(x => x.Iva)
                .GreaterThan(0).WithMessage("El IVA debe ser mayor que 0.");

            RuleFor(x => x.Total)
                .GreaterThan(0).WithMessage("El total debe ser mayor que 0.");

            RuleFor(x => x.Descuento)
                .GreaterThanOrEqualTo(0).WithMessage("El descuento no puede ser negativo.");

            RuleFor(x => x.NoPersonas)
                .GreaterThan(0).WithMessage("El número de personas debe ser mayor que 0.");

            RuleFor(x => x.IdCliente)
                .GreaterThan(0).WithMessage("Debe seleccionar un cliente.");

            RuleFor(x => x.IdUsuario)
                .GreaterThan(0).WithMessage("Debe seleccionar un usuario.");

            RuleFor(x => x.IdEstadoReserva)
                .NotNull().WithMessage("El estado es obligatorio.");

            RuleFor(x => x.IdMetodoPago)
                .GreaterThan(0).WithMessage("Debe seleccionar un método de pago.");
        }
    }
}

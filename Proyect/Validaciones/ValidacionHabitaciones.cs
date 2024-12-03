using FluentValidation;
using Proyect.Models;
using System.Text.RegularExpressions;

namespace Proyect.Validaciones
{
    public class ValidacionHabitaciones : AbstractValidator<Habitacione>
    {
        public ValidacionHabitaciones()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .Length(3, 30).WithMessage("El nombre debe tener entre 3 y 30 caracteres.");

            RuleFor(x => x.IdTipoHabitacion)
                .GreaterThan(0).WithMessage("Debe seleccionar un tipo de habitación.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede tener más de 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.Precio)
             .NotEmpty().WithMessage("el precio es obligatorio")
             .GreaterThan(0).WithMessage("El precio debe ser mayor que 0.");
        }
    }
}

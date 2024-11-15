using FluentValidation;
using Proyect.Models;

namespace Proyect.Validaciones
{
    public class ValidacionTipoHabitacion : AbstractValidator<TipoHabitacione>
    {
        public ValidacionTipoHabitacion() 
        {
            RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede tener más de 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.Capacidad)
                .GreaterThan(0).WithMessage("La capacidad debe ser mayor que 0.");
        }
    }
}

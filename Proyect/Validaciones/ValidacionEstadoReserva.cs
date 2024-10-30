using Proyect.Models;
using FluentValidation;

namespace Proyect.Validaciones 
{
    public class ValidacionEstadoReserva : AbstractValidator<EstadoReserva>
    {
        public ValidacionEstadoReserva()
        {
            RuleFor(x => x.Estados)
           .NotEmpty().WithMessage("El nombre del Estado es obligatorio.")
           .Length(5, 20).WithMessage("El nombre del Estado debe tener entre 5 y 20 caracteres.");

            RuleFor(x => x.Descripcion)
               .MaximumLength(200).WithMessage("La descripción no puede tener más de 200 caracteres.")
               .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }
}

using FluentValidation;
using Proyect.Models;

namespace Proyect.Validaciones
{
    public class ValidacionMetodoPago : AbstractValidator<MetodoPago>
    {
        public ValidacionMetodoPago() 
        {
            RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .Length(3, 20).WithMessage("El nombre debe tener entre 3 y 20 caracteres.");
        }
    }
}

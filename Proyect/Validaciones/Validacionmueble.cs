using FluentValidation;
using Proyect.Models;

namespace Proyect.Validaciones
{
    public class Validacionmueble : AbstractValidator<Mueble>
    {
        public Validacionmueble() 
        {
            RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres.");

            RuleFor(x => x.IdTipoMueble)
                .NotEmpty().WithMessage("El tipo de Mueble es obligatorio.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede tener más de 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descripcion)); 
        }
    }
}

using Proyect.Models;
using FluentValidation;

namespace Proyect.Validaciones
{
    public class ValidacionPaquetes : AbstractValidator<Paquete>
    {
        public ValidacionPaquetes()
        {

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .Length(5, 30).WithMessage("El nombre debe tener entre 5 y 30 caracteres.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es obligatoria")
                .MaximumLength(200).WithMessage("La descripción no puede tener más de 200 caracteres.");

            RuleFor(x => x.Precio)
                .NotEmpty().WithMessage("el precio es obligatorio")
                .GreaterThan(0).WithMessage("El precio debe ser mayor que 0.");

        }
    }
}

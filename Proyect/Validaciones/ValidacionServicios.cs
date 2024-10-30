using Proyect.Models;
using FluentValidation;

namespace Proyect.Validaciones
{
    public class ValidacionServicios : AbstractValidator<Servicio>
    {
        public ValidacionServicios()
        {
            RuleFor(servicio => servicio.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .Length(3, 50).WithMessage("El nombre debe tener entre 3 y 50 caracteres.");

            RuleFor(servicio => servicio.Estado)
           .NotNull().WithMessage("El estado es obligatorio.")  // Valida que no sea nulo (si el bool es nullable, es decir, bool?)
           .Must(estado => estado == true || estado == false).WithMessage("El estado debe ser 'Activo' o 'Inactivo'.");

            RuleFor(servicio => servicio.Descripcion)
                .MaximumLength(30).WithMessage("La descripción no puede tener más de 30 caracteres.");

            RuleFor(servicio => servicio.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a cero.");
        }
    }
}

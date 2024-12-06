using FluentValidation;
using Proyect.Models;
using System.Text.RegularExpressions;

namespace Proyect.Validaciones
{
    public class ValidacionTipoHabitacion : AbstractValidator<TipoHabitacione>
    {
        public ValidacionTipoHabitacion() 
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .Length(3, 20).WithMessage("El nombre debe tener entre 3 y 20 caracteres.")
                .Must(nombre => Regex.IsMatch(nombre, @"^[a-zA-Z\s]+$")).WithMessage("El nombre solo puede contener letras");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es obligatoria")
                .MaximumLength(200).WithMessage("La descripción no puede tener más de 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.Capacidad)
                .NotEmpty().WithMessage("Capacidad es obligatoria")
                .GreaterThan(0).WithMessage("La capacidad debe ser mayor que 0.");
        }
    }
}

using FluentValidation;
using Proyect.Models;

namespace Proyect.Validaciones
{
    public class ValidacionUsuario : AbstractValidator<Usuario>
    {
        public ValidacionUsuario()
        {
            RuleFor(x => x.TipoDocumento)
                .NotEmpty().WithMessage("El tipo de documento es obligatorio.");

            RuleFor(x => x.Documento)
                .NotEmpty().WithMessage("El documento es obligatorio.")
                .Matches(@"^\d+$").WithMessage("El documento solo puede contener números.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .Length(3, 30).WithMessage("El nombre debe tener entre 3 y 30 caracteres.");

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .Length(3, 30).WithMessage("El apellido debe tener entre 3 y 30 caracteres.");

            RuleFor(x => x.Celular)
                .Matches(@"^\d{10}$").WithMessage("El celular debe tener exactamente 10 dígitos.")
                .When(x => !string.IsNullOrEmpty(x.Celular));

            RuleFor(x => x.Direccion)
                .MaximumLength(100).WithMessage("La dirección no puede tener más de 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Direccion));

            RuleFor(x => x.CorreoElectronico)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.");

            RuleFor(x => x.IdRol)
                .GreaterThan(0).WithMessage("Debe seleccionar un rol.");
        }
    }
}

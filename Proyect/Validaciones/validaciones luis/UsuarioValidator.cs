using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;

namespace Proyect.Validaciones.ValidacionesLuis
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        private readonly ProyectContext _context;

        public UsuarioValidator(ProyectContext context)
        {
            _context = context;

            // Validación de TipoDocumento
            RuleFor(u => u.TipoDocumento)
                .NotEmpty().WithMessage("El tipo de documento es obligatorio.")
                .MaximumLength(20).WithMessage("El tipo de documento no debe exceder los 20 caracteres.");

            // Validación de Documento (Sin asincrónico)
            RuleFor(u => u.Documento)
                .NotEmpty().WithMessage("El documento es obligatorio.");
                //.Must((usuario, documento) => !DocumentoExistente(documento)) // Validación sin asincrónico
                //.WithMessage("El documento ya existe.");

            // Validación de Nombre
            RuleFor(u => u.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no debe exceder los 50 caracteres.");

            // Validación de Apellido
            RuleFor(u => u.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(50).WithMessage("El apellido no debe exceder los 50 caracteres.");

            // Validación de Celular (si tiene un formato específico)
            RuleFor(u => u.Celular)
                .Matches(@"^\d{10}$").WithMessage("El número de celular debe ser un número de 10 dígitos.");

            // Validación de Dirección
            RuleFor(u => u.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(100).WithMessage("La dirección no debe exceder los 100 caracteres.");

            // Validación de Correo Electrónico
            RuleFor(u => u.CorreoElectronico)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

            // Validación de Estado
            RuleFor(u => u.Estado)
                .NotNull().WithMessage("El estado debe ser verdadero o falso.");

            // Validación de IdRol (sin asincrónico)
            RuleFor(u => u.IdRol)
                .Must((usuario, idRol) => RolExistente(idRol)) // Validación sin asincrónico
                .WithMessage("El rol seleccionado no existe.");
        }

        // Método para verificar que el documento no exista ya en la base de datos
        private bool DocumentoExistente(string documento)
        {
            return !_context.Usuarios.Any(u => u.Documento == documento);
        }

        // Método para verificar si el rol existe en la base de datos
        private bool RolExistente(int idRol)
        {
            return _context.Roles.Any(r => r.IdRol == idRol);
        }
    }
}
    
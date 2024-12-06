using FluentValidation;
using Proyect.Models;
using System.Linq;

namespace Proyect.Validaciones.ValidacionesLuis
{
    public class PermisoValidator : AbstractValidator<Permiso>
    {
        private readonly ProyectContext _context;

        public PermisoValidator(ProyectContext context)
        {
            _context = context;

            RuleFor(p => p.NombrePermiso)
                .NotEmpty().WithMessage("El nombre del permiso es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre del permiso no debe exceder los 50 caracteres.")
                //.Matches("^[a-zA-Z0-9_]*$").WithMessage("El nombre del permiso solo puede contener letras, números y guiones bajos.")
                .Must((permiso, nombre) => NombreUnico(nombre, permiso.IdPermiso))
                .WithMessage("El nombre del permiso ya existe."); // Validación síncrona
            //Validaciones descripcion 
            RuleFor(p => p.Descripcion)
                .NotEmpty().WithMessage("La descripción del permiso es obligatoria.")
                .MaximumLength(200).WithMessage("La descripción no debe exceder los 200 caracteres.");
        }

        // Método síncrono para validar que el nombre del permiso sea único, excluyendo el permiso actual
        private bool NombreUnico(string nombre, int id)
        {
            return !_context.Permisos.Any(p => p.NombrePermiso == nombre && p.IdPermiso != id);
        }
    }
}

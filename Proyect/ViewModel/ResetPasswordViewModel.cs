using System.ComponentModel.DataAnnotations;

namespace Proyect.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$", ErrorMessage = "La contraseña debe comenzar con una letra mayúscula, contener al menos un número, un carácter especial y tener al menos 8 caracteres.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación de la contraseña no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

}

//using Proyect.Models;
//using FluentValidation;

//namespace Proyect.Validaciones
//{
//    public class ValidacionAbono : AbstractValidator<Abono>
//    {
//        public ValidacionAbono()
//        {
//            // Validación: Valor del abono no puede ser mayor al pendiente
//            RuleFor(x => x.ValorAbono)
//                .LessThanOrEqualTo(x => x.Pendiente)
//                .WithMessage("El valor del abono no puede ser mayor al saldo pendiente.");

//            // Validación: Debe incluir un comprobante (imagen)
//            RuleFor(x => x.Comprobante)
//                .NotEmpty()
//                .WithMessage("Debe adjuntar una imagen del comprobante.");
//        }
//    }
//}

namespace Proyect.Models
{
    public class FranjaHoraria
    {
        public int IdFranjaHoraria { get; set; }

        public DateTime HoraInicio { get; set; } // Hora de inicio de la franja
        public DateTime HoraFin { get; set; }   // Hora de fin de la franja

        public int IdServicio { get; set; } // Relación con Servicio

        public int Capacidad { get; set; } // Capacidad máxima para esta franja

        public virtual Servicio IdServicioNavigation { get; set; } // Navegación al servicio

    }
}

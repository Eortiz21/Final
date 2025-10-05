using Primera.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primera.Models
{
    public class Pago
    {
        [Key]
        public int Id_Pago { get; set; }

        [ForeignKey("Ticket")]
        public int Id_Ticket { get; set; }
        public Ticket? Ticket { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoPago { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        [StringLength(50)]
        public string MetodoPago { get; set; }

        [Required]
        [StringLength(30)]
        public string EstadoPago { get; set; }

        /// <summary>
        /// Calcula automáticamente el monto según la tarifa y las horas de permanencia
        /// </summary>
        public void CalcularMonto()
        {
            if (Ticket == null || Ticket.Tarifa == null)
                throw new Exception("El ticket o la tarifa no están asignados.");

            // Redondear la hora de entrada hacia abajo
            DateTime entrada = new DateTime(
                Ticket.Fecha_hora_entrada.Year,
                Ticket.Fecha_hora_entrada.Month,
                Ticket.Fecha_hora_entrada.Day,
                Ticket.Fecha_hora_entrada.Hour, 0, 0
            );

            DateTime salida = FechaPago;

            double horas = (salida - entrada).TotalHours;
            if (horas < 0) horas = 0;

            // Siempre redondea hacia arriba
            int horasRedondeadas = (int)Math.Ceiling(horas);

            MontoPago = horasRedondeadas * Ticket.Tarifa.Monto;
        }
    }
}
//             }    
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Primera.Models
{
    public class Ticket
    {
        [Key]
        public int Id_Ticket { get; set; }

        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(20)]
        public string NoPlaca { get; set; } = string.Empty;

        public Vehiculo? Vehiculo { get; set; }

        [Required(ErrorMessage = "El espacio es obligatorio.")]
        public int Id_Espacio { get; set; }
        public EspacioEstacionamiento? EspacioEstacionamiento { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria.")]
        public DateTime Fecha_hora_entrada { get; set; }

        [Required(ErrorMessage = "La tarifa es obligatoria.")]
        public int Id_Tarifa { get; set; }
        public Tarifa? Tarifa { get; set; }

        // ✅ Nuevo campo Estado
        [Required(ErrorMessage = "El estado del ticket es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Estado del Ticket")]
        public string Estado { get; set; } = "Activo"; // valores posibles: Activo, Cerrado, Cancelado, etc.

        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primera.Models
{
    public class Ticket
    {
        [Key]
        public int Id_Ticket { get; set; }

        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(20)]
        public string NoPlaca { get; set; } = string.Empty;

        // Relación con Vehiculo usando NoPlaca como FK
        [ForeignKey(nameof(NoPlaca))]
        public Vehiculo? Vehiculo { get; set; }   // 🔥 Ya no required, se carga después

        [Required(ErrorMessage = "El espacio es obligatorio.")]
        public int Id_Espacio { get; set; }

        [ForeignKey(nameof(Id_Espacio))]
        public EspacioEstacionamiento? EspacioEstacionamiento { get; set; }  // 🔥 Ya no required

        [Required(ErrorMessage = "La fecha de entrada es obligatoria.")]
        public DateTime Fecha_hora_entrada { get; set; }

        public DateTime? Fecha_hora_salida { get; set; }

        [Required(ErrorMessage = "La tarifa es obligatoria.")]
        public int Id_Tarifa { get; set; }

        [ForeignKey(nameof(Id_Tarifa))]
        public Tarifa? Tarifa { get; set; }   // 🔥 Ya no required

        [Required(ErrorMessage = "El pago total es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PagoTotal { get; set; }

        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
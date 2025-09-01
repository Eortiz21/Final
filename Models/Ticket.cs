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

        // FK a Vehiculo usando NoPlaca
        [Required]
        [StringLength(20)]
        [ForeignKey("Vehiculo")]
        public string NoPlaca { get; set; }
        public Vehiculo Vehiculo { get; set; }

        // FK a EspacioEstacionamiento
        [ForeignKey("EspacioEstacionamiento")]
        public int Id_Espacio { get; set; }
        public EspacioEstacionamiento EspacioEstacionamiento { get; set; }

        [Required]
        public DateTime Fecha_hora_entrada { get; set; }

        public DateTime? Fecha_hora_salida { get; set; }

        // FK a Tarifa
        [ForeignKey("Tarifa")]
        public int Id_Tarifa { get; set; }
        public Tarifa Tarifa { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PagoTotal { get; set; }

        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}

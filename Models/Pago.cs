using System;
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
            public Ticket Ticket { get; set; }

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
        }
    }

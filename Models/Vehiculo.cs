using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Primera.Models
{
    public class Vehiculo
    {
        [Key]
        [Required]
        [StringLength(20)]
        public string NoPlaca { get; set; }   // PK

        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50)]
        public string Marca { get; set; }

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(30)]
        public string Color { get; set; }

        // Relación con Cliente (1 Cliente -> Muchos Vehículos)
        [Required]
        public int Id_Cliente { get; set; }
        public Cliente Cliente { get; set; }

        // Relación con TipoVehiculo (1 TipoVehiculo -> Muchos Vehículos)
        [Required]
        public int Id_Tipo { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }

        // Relación con Ticket (1 Vehículo -> Muchos Tickets)
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}

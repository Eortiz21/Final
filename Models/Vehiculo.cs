using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primera.Models
{
    public class Vehiculo
    {
        [Key]
        [Required]
        [StringLength(20)]
        public string NoPlaca { get; set; }   // PK según el diagrama

        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50)]
        public string Marca { get; set; }

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(30)]
        public string Color { get; set; }

        // Relación con Cliente
        [Required]
        public int Id_Cliente { get; set; }
        public Cliente Cliente { get; set; }

        // Relación con TipoVehiculo
        public ICollection<TipoVehiculo> TiposVehiculo { get; set; } = new List<TipoVehiculo>();

        // Relación con Ticket
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}

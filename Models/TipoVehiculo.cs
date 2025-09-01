using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primera.Models
{
    public class TipoVehiculo
    {
        [Key]
        public int Id_Tipo { get; set; }   // PK

        [Required]
        [StringLength(20)]
        public string NoPlaca { get; set; }   // FK hacia Vehiculo
        public Vehiculo Vehiculo { get; set; }

        [Required]
        public int Id_Tarifa { get; set; }   // FK hacia Tarifa
        public Tarifa Tarifa { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }
    }
}

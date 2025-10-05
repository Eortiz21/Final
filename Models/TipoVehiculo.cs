using System.ComponentModel.DataAnnotations;

namespace Primera.Models
{
    public class TipoVehiculo
    {
        [Key]
        public int Id_Tipo { get; set; }   // PK

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;  // Nombre o descripción del tipo de vehículo

        [Required]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;  // Marca del vehículo (ej. Toyota, Nissan)

        [Required]
        [StringLength(50)]
        public string Tamano { get; set; } = string.Empty;  // Tamaño (ej. pequeño, mediano, grande)

        [Range(1, 20)]
        public int Ejes { get; set; }  // Cantidad de ejes (ej. 2, 3, 4)

        // Relación con Vehiculo (1 TipoVehiculo -> Muchos Vehiculos)
        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}

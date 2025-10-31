using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Primera.Models
{
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    public class Vehiculo
    {
        [Key]
        [StringLength(20)]
        [Required(ErrorMessage = "La placa es obligatoria")]
        public string NoPlaca { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50)]
        public string Marca { get; set; }

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(30)]
        public string Color { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20)]
        public string Estado { get; set; } = "En parqueo"; // valor por defecto


        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        public int Id_Cliente { get; set; }

        [ValidateNever]   // 👈 evita que el binder intente validar Cliente
        public Cliente Cliente { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de vehículo")]
        public int Id_Tipo { get; set; }

        [ValidateNever]   // 👈 evita que el binder intente validar TipoVehiculo
        public TipoVehiculo TipoVehiculo { get; set; }
    }

}

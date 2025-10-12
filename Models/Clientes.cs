using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Primera.Models
{
    public class Cliente
    {
        [Key]
        public int Id_Cliente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El DPI/Pasaporte es obligatorio")]
        [StringLength(100)]
        // 👇 Esta validación remota solo se aplicará en Create (no bloquea Edit)
        [Remote(action: "VerificarDocumento", controller: "Clientes", AdditionalFields = nameof(Id_Cliente), ErrorMessage = "El número de documento ya existe")]
        public string NumeroDocumentacion { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Número de teléfono no válido")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200)]
        public string Direccion { get; set; }

        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}

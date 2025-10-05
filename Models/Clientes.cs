using Primera.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Primera.Models
{
    using Microsoft.AspNetCore.Mvc;

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
        [Remote(action: "VerificarDocumento", controller: "Clientes", ErrorMessage = "El número de documento ya existe")]
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
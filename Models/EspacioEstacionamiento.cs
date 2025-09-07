using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Primera.Models
{
    public class EspacioEstacionamiento
    {
        [Key]
        public int Id_Espacio { get; set; }

        [Required]
        [StringLength(20)]
        public string No_Espacio { get; set; }

        [Required]
        [StringLength(20)]
        public string Nivel { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoEspacio { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}

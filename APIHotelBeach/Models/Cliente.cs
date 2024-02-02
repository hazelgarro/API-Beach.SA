using System.ComponentModel.DataAnnotations;

namespace APIHotelBeach.Models
{
    public class Cliente
    {

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        [MinLength(8)]
        public string Cedula { get; set; }

        [Required]
        [MinLength(5)]
        public string TipoCedula { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(15)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(50)]
        public string Direccion { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

    }
}

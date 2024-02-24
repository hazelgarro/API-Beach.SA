using System.ComponentModel.DataAnnotations;

namespace APIHotelBeach.Models
{
    public class Restablecer
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar la contraseña enviada por email")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe ingresar su nueva contraseña")]
        public string NuevoPassword { get; set; }

        [Required(ErrorMessage = "Confirme su nueva contraseña")]
        public string Confirmar { get; set; }
    }
}

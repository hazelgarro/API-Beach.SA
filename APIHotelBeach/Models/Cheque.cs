using System.ComponentModel.DataAnnotations;

namespace APIHotelBeach.Models
{
    public class Cheque
    {
        public int NumeroCheque { get; set; }

        public string NombreBanco { get; set; }

        public int IdReservacion { get; set; }
    }
}

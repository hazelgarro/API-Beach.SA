namespace APIHotelBeach.Models
{
    public class ReservaPDFCheque
    {
        public int Id { get; set; }

        public string CedulaCliente { get; set; }

        public string Email { get; set; }

        public string NombreCompleto { get; set; }

        public int IdPaquete { get; set; }

        public string TipoPago { get; set; }

        public DateOnly FechaReserva { get; set; }

        public int Duracion { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Descuento { get; set; }

        public decimal MontoTotal { get; set; }

        public decimal Adelanto { get; set; }

        public decimal MontoMensualidad { get; set; }

        public int NumeroCheque { get; set; }

        public string NombreBanco { get; set; }
    }
}

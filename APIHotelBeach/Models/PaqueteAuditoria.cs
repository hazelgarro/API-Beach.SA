namespace APIHotelBeach.Models
{
    public class PaqueteAuditoria
    {
        public string Accion { get; set; }

        public DateTime FechaCambio { get; set; }

        public int ID { get; set; }

        public string NombrePaquete { get; set; }

        public decimal Precio { get; set; }

        public decimal PorcentajePrima { get; set; }

        public int LimiteMeses { get; set; }

        public DateTime FechaRegistro { get; set; }

        public char Estado { get; set; }
    }
}

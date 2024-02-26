namespace APIHotelBeach.Models
{
    public class EmpleadoAuditoria
    {
        public string Accion { get; set; }

        public DateTime FechaCambio { get; set; }

        public int ID { get; set; }

        public string NombreCompleto { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int TipoUsuario { get; set; }

        public DateTime FechaRegistro { get; set; }

        public char Estado { get; set; }
    }
}

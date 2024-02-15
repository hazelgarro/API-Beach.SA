using APIHotelBeach.Context;
using APIHotelBeach.Models;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIHotelBeach.Controllers
{
    public class ReservacionesController : Controller
    {

        private readonly DbContextHotel _context;

        /// <summary>
        /// Constructor con parámetros
        /// </summary>
        /// <param name="pContext"></param>
        public ReservacionesController(DbContextHotel pContext)
        {
            _context = pContext;
        }


        //***   MÉTODOS     CRUD ***

        //lista reservaciones
        [HttpGet("ListaReservas")]
        public async Task<List<Reservacion>> ListaReservas()
        {
            var listReservas = await _context.Reservaciones.ToListAsync();

            return listReservas;
        }

        

        //***   MÉTODOS     EMAIL   ***
        private bool EnviarEmail(Reservacion reservacion, Cliente usuario)
        {
            string mensaje = "";

            try
            {
                bool enviado = false;

                EmailReservacion email = new EmailReservacion();

                email.EnviarPDF(usuario, reservacion);

                enviado = true;

                return enviado;
            }
            catch (Exception e)
            {
                mensaje = "Error al enviar el correo " + e.Message;

                return false;
            }
        }
    }
}

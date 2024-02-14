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

        [HttpGet("ListaReservas")]
        public async Task<List<Reservacion>> ListaReservas()
        {
            var listReservas = await _context.Reservaciones.ToListAsync();

            return listReservas;
        }
    }
}

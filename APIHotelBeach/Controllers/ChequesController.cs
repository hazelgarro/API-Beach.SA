using APIHotelBeach.Context;
using APIHotelBeach.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIHotelBeach.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChequesController : Controller
    {
        private readonly DbContextHotel _context;

        public ChequesController(DbContextHotel pContext)
        {
            _context = pContext;
        }

        //[Authorize]
        [HttpGet("Listado")]
        public async Task<List<Cheque>> Listado()
        {
            var list = await _context.Cheques.ToListAsync();

            return list;
        }

        //[Authorize]
        [HttpGet("Consultar")]
        public async Task<Cheque> Consultar(int Id)
        {
            var temp = await _context.Cheques.FirstOrDefaultAsync(c => c.IdReservacion == Id);
            return temp;
        }

        //[Authorize]
        [HttpPost("Agregar")]
        public string Agregar(Cheque pCheque)
        {
            string mensaje = "";

            try
            {
                _context.Cheques.Add(pCheque);
                _context.SaveChanges();
                mensaje = "Cheque registrado correctamente";
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message + " " + ex.InnerException.ToString;
            }
            return mensaje;
        }

        //[Authorize]
        [HttpPut("Modificar")]
        public string Modificar(Cheque pCheque)
        {
            string mensaje = "";
            try
            {
                _context.Cheques.Update(pCheque);
                _context.SaveChanges();
                mensaje = "Cheque modificado correctamente";
            }
            catch (Exception ex)
            {
                mensaje = "Error " + ex.Message + " " + ex.InnerException.ToString;
            }
            return mensaje;
        }

        //[Authorize]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int Id)
        {
            string mensaje = "";
            try
            {
                var temp = await _context.Cheques.FirstOrDefaultAsync(c => c.NumeroCheque == Id);
                if (temp == null)
                {
                    mensaje = "No existe ningun cheque con el numero " + Id;
                }
                else
                {
                    _context.Cheques.Remove(temp);
                    await _context.SaveChangesAsync();
                    mensaje = $"Cheque con el numero {temp.NumeroCheque}, eliminado correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error " + ex.Message + " " + ex.InnerException.ToString;
            }
            return mensaje;
        }
    }
}

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
        public async Task<string> Agregar(Cheque pCheque)
        {
            string mensaje = "";
            bool chequeEncontrado = false;

            try
            {
                var cheques = await _context.Cheques.ToListAsync();

                foreach (var item in cheques)
                {
                    if (!chequeEncontrado)
                    {
                        if (item.NumeroCheque == pCheque.NumeroCheque)
                        {
                            chequeEncontrado = true;
                        }
                    }
                }

                if (chequeEncontrado)
                {
                    mensaje = "El cheque con ese numero ya se encuentra registrado.";
                }
                else
                {
                    _context.Cheques.Add(pCheque);
                    await _context.SaveChangesAsync();
                    mensaje = "Cheque registrado correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message + " " + ex.InnerException.ToString;
            }

            return mensaje;
        }

        //[Authorize]
        [HttpPut("Modificar")]
        public async Task<string> Modificar(Cheque pCheque)
        {
            string mensaje = "";
            bool chequeEncontrado = false;

            try
            {
                var cheques = await _context.Cheques.ToListAsync();

                foreach (var item in cheques)
                {
                    if (!chequeEncontrado)
                    {
                        if (item.NumeroCheque == pCheque.NumeroCheque)
                        {
                            if (item.IdReservacion != pCheque.IdReservacion)
                            {
                                chequeEncontrado = true;
                            }
                        }
                    }
                }

                if (chequeEncontrado)
                {
                    mensaje = "El cheque con ese numero ya se encuentra registrado.";
                }
                else
                {
                    _context.Cheques.Update(pCheque);
                    await _context.SaveChangesAsync();
                    mensaje = "Cheque modificado correctamente";
                }
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

using APIHotelBeach.Context;
using APIHotelBeach.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Identity.Client.AppConfig;

namespace APIHotelBeach.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaquetesController : Controller
    {
        private readonly DbContextHotel _context;

        public PaquetesController(DbContextHotel pContext)
        {
            _context = pContext;
        }//end PaquetesController

        //[Authorize] 
        [HttpGet("Listado")]
        public async Task<List<Paquete>> Listado()
        {
            var list = await _context.Paquetes.ToListAsync();
            if (list == null)
            {
                return new List<Paquete>();
            }
            else
            {
                return list;
            }//end if/else
        }//end Listado

        //[Authorize]
        [HttpGet("Consultar")]
        public async Task<Paquete> Consultar(int ID)
        {
            var temp = await _context.Paquetes.FirstOrDefaultAsync(p => p.ID == ID);
            return temp;
        }//end Consultar

        ////[Authorize]
        //[HttpGet("Consultar")]
        //public async Task<Paquete> Consultar(string nombrePaquete)
        //{
        //    var temp = await _context.Paquetes.FirstOrDefaultAsync(p => p.NombrePaquete == nombrePaquete);
        //    return temp;
        //}//end Consultar

        //[Authorize]
        [HttpPost("Agregar")]
        public string Agregar(Paquete paquete)
        {
            string msj = "";
            var users = _context.Paquetes.ToList();

            try
            {
                _context.Paquetes.Add(paquete);
                _context.SaveChanges();
                msj = "Paquete registrado correctamente";
            }
            catch (Exception ex)
            {
                msj = "Error: " + ex.Message + " " + ex.InnerException.ToString;
            }
            return msj;
        }//end Agregar

        //[Authorize]
        [HttpPut("Modificar")]
        public string Modificar(Paquete paquete)
        {
            string msj = "";
            try
            {
                _context.Paquetes.Update(paquete);
                _context.SaveChanges();
                msj = "Paquete modificado correctamente";
            }
            catch (Exception ex)
            {
                msj = "Error " + ex.Message + " " + ex.InnerException.ToString;
            }
            return msj;
        }//end modificar

        //[Authorize]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int ID)
        {
            string msj = "";
            try
            {
                var temp = await _context.Paquetes.FirstOrDefaultAsync(p => p.ID == ID);
                if (temp == null)
                {
                    msj = "No existe ningun paquete con el ID " + ID;
                }
                else
                {
                    _context.Paquetes.Remove(temp);
                    await _context.SaveChangesAsync();
                    msj = $"Paquete con el ID {temp.ID}, eliminado correctamente";
                }
            }
            catch (Exception ex)
            {
                msj = "Error " + ex.Message + " " + ex.InnerException.ToString;
            }
            return msj;
        }//end Eliminar

    }//end class
}//end namespace

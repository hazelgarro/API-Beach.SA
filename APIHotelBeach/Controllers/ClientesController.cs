using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIHotelBeach.Models;
using APIHotelBeach.Context;

namespace APIHotelBeach.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ClientesController : Controller
    {

        private readonly DbContextHotel _context;

        public ClientesController(DbContextHotel pContext)
        {
            _context = pContext;
        }
    //metodo para buscar por cedula
        [HttpGet("Buscar")]
        public async Task<Cliente> GetClient(string cedula)
        {
            var temp = await _context.Clientes.FirstOrDefaultAsync(c => c.Cedula == cedula);
            return temp;

        }

        //[Authorize]
        [HttpPut("Modificar")]
        public string Modificar(Cliente cliente)
        {
            string msj = "";
            try
            {
                _context.Clientes.Update(cliente);
                _context.SaveChanges();
                msj = "Cliente modificado correctamente";
            }
            catch (Exception ex)
            {
                msj = "Error " + ex.Message + " " + ex.InnerException.ToString;
            }
            return msj;
        }//end modificar

    }//end class
}//end namespace

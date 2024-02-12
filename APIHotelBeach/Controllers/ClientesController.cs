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
            var temp = await _context.Clientes.FirstOrDefaultAsync(x => x.Cedula == cedula);
            return temp;

        }

    }
}

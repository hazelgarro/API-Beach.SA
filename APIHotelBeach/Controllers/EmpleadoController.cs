using APIHotelBeach.Context;
using APIHotelBeach.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIHotelBeach.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmpleadoController : Controller
    {
        private readonly DbContextHotel _context;

        public EmpleadoController(DbContextHotel pContext)
        {
            _context = pContext;
        }//end EmpleadoController


        //[Authorize] 
        [HttpGet("Listado")]
        public async Task<List<Empleado>> Listado()
        {
            var list = await _context.Empleados.ToListAsync();
            if (list == null)
            {
                return new List<Empleado>();
            }
            else
            {
                return list;
            }//end if/else
        }//end Listado

        //[Authorize]
        [HttpGet("Consultar")]
        public async Task<Empleado> Consultar(int ID)
        {
            var temp = await _context.Empleados.FirstOrDefaultAsync(e => e.ID == ID);
            return temp;
        }//end Consultar

        [HttpPost("Agregar")]
        public string Agregar(Empleado empleado)
        {
            string msj = "";
            var users = _context.Empleados.ToList();

            try
            {
                _context.Empleados.Add(empleado);
                _context.SaveChanges();
                msj = "Empleado registrado correctamente";
            }
            catch (Exception ex)
            {
                msj = "Error: " + ex.Message + " " + ex.InnerException.ToString;
            }
            return msj;
        }//end Agregar

        //[Authorize]
        [HttpPut("Modificar")]
        public string Modificar(Empleado empleado)
        {
            string msj = "";
            try
            {
                _context.Empleados.Update(empleado);
                _context.SaveChanges();
                msj = "Empleado modificado correctamente";
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
                var temp = await _context.Empleados.FirstOrDefaultAsync(e => e.ID == ID);
                if (temp == null)
                {
                    msj = "No existe ningun empleado con el ID " + ID;
                }
                else
                {
                    _context.Empleados.Remove(temp);
                    await _context.SaveChangesAsync();
                    msj = $"Empleado con el ID {temp.ID}, eliminado correctamente";
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

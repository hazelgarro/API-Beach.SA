using APIHotelBeach.Context;
using APIHotelBeach.Models;
using APIHotelBeach.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIHotelBeach.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmpleadosController : Controller
    {
        private readonly DbContextHotel _context;
        private readonly IAutorizacionServicesEmpleado autorizacionService;

        public EmpleadosController(DbContextHotel pContext, IAutorizacionServicesEmpleado autorizacionService)
        {
            _context = pContext;
            this.autorizacionService = autorizacionService;

        }//end EmpleadoController


        [Authorize] 
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

        [HttpGet("EmpleadoLogin")]
        public async Task<ActionResult<object>> DatosEmpleado(string email)
        {
            var empleado = await _context.Empleados
                .Where(e => e.Email == email)
                .Select(e => new { TipoUsuario = e.TipoUsuario })
                .FirstOrDefaultAsync();

            if (empleado == null)
            {
                return NotFound();
            }
            else
            {
                return empleado;
            }
        }

        [Authorize]
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

                //Proceso de auditoria al agregar empleado
                EmpleadoAuditoria empleadoAuditoria = new EmpleadoAuditoria();

                empleadoAuditoria.Accion = "AGREGADO";
                empleadoAuditoria.FechaCambio = DateTime.Now;
                empleadoAuditoria.ID = empleado.ID;
                empleadoAuditoria.NombreCompleto = empleado.NombreCompleto;
                empleadoAuditoria.Email = empleado.Email;
                empleadoAuditoria.Password = empleado.Password;
                empleadoAuditoria.TipoUsuario = empleado.TipoUsuario;
                empleadoAuditoria.FechaRegistro = empleado.FechaRegistro;
                empleadoAuditoria.Estado = empleado.Estado;

                _context.Empleados_Auditoria.Add(empleadoAuditoria);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = "Error: " + ex.Message + " " + ex.InnerException.ToString();
            }
            return msj;
        }//end Agregar

        [Authorize]
        [HttpPut("Modificar")]
        public string Modificar(Empleado empleado)
        {
            string msj = "";
            try
            {
                var tempEmpleado = _context.Empleados.AsNoTracking().FirstOrDefault(f => f.ID == empleado.ID);

                _context.Empleados.Update(empleado);
                _context.SaveChanges();
                msj = "Empleado modificado correctamente";

                //Proceso de auditoria al modificar empleado
                EmpleadoAuditoria empleadoAuditoria = new EmpleadoAuditoria();

                empleadoAuditoria.Accion = "MODIFICADO";
                empleadoAuditoria.FechaCambio = DateTime.Now;
                empleadoAuditoria.ID = tempEmpleado.ID;
                empleadoAuditoria.NombreCompleto = tempEmpleado.NombreCompleto;
                empleadoAuditoria.Email = tempEmpleado.Email;
                empleadoAuditoria.Password = tempEmpleado.Password;
                empleadoAuditoria.TipoUsuario = tempEmpleado.TipoUsuario;
                empleadoAuditoria.FechaRegistro = tempEmpleado.FechaRegistro;
                empleadoAuditoria.Estado = tempEmpleado.Estado;

                _context.Empleados_Auditoria.Add(empleadoAuditoria);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = "Error " + ex.Message + " " + ex.InnerException.ToString();
            }
            return msj;
        }//end modificar

        [Authorize]
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

                    //Proceso de auditoria al eliminar empleado
                    EmpleadoAuditoria empleadoAuditoria = new EmpleadoAuditoria();

                    empleadoAuditoria.Accion = "ELIMINADO";
                    empleadoAuditoria.FechaCambio = DateTime.Now;
                    empleadoAuditoria.ID = temp.ID;
                    empleadoAuditoria.NombreCompleto = temp.NombreCompleto;
                    empleadoAuditoria.Email = temp.Email;
                    empleadoAuditoria.Password = temp.Password;
                    empleadoAuditoria.TipoUsuario = temp.TipoUsuario;
                    empleadoAuditoria.FechaRegistro = temp.FechaRegistro;
                    empleadoAuditoria.Estado = temp.Estado;

                    _context.Empleados_Auditoria.Add(empleadoAuditoria);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                msj = "Error " + ex.Message + " " + ex.InnerException.ToString();
            }
            return msj;
        }//end Eliminar


        //***   MÉTODOS  AUTENTICACION    ****

        //validar email y password
        [HttpPost]
        [Route("AutenticarPW")]
        public async Task<IActionResult> AutenticarPW(string email, string password)
        {
            var temp = await _context.Empleados.FirstOrDefaultAsync(u => (u.Email.Equals(email)) && (u.Password.Equals(password)));

            if (temp == null)
            {
                return Unauthorized();
            }
            else
            {
                var autorizado = await autorizacionService.DevolverTokenEmpleado(temp);

                if (autorizado == null)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(autorizado);
                }
            }
        }

    }//end class
}//end namespace

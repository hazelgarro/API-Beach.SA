using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIHotelBeach.Models;
using APIHotelBeach.Context;
using iText.Commons.Actions.Contexts;
using APIHotelBeach.Services;

namespace APIHotelBeach.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ClientesController : Controller
    {

        private readonly DbContextHotel _context;
        private readonly IAutorizacionServicesCliente autorizacionService;

        public ClientesController(DbContextHotel pContext, IAutorizacionServicesCliente autorizacionService)
        {
            _context = pContext;
            this.autorizacionService = autorizacionService;
        }

        //***   MÉTODOS  CRUD   ***

        //[Authorize]
        [HttpGet("Listado")]
        public async Task<List<Cliente>> Index()
        {
            var list = await _context.Clientes.ToListAsync();

            if (list == null)
            {
                return new List<Cliente>();
            }
            else
            {
                return list;
            }
        }

        //Registrar cliente
        //[Äuthorize]
        [HttpPost("CrearCuenta")]
        public string CrearCuenta(Cliente cliente)
        {
            string mensaje = "";

            cliente.Password = GenerarPassword();

            if (cliente != null)
            {
                _context.Clientes.Add(cliente);

                try
                {
                    _context.SaveChanges();

                    if (EnviarEmail(cliente))
                    {
                        mensaje = "Contraseña enviada por correo";
                    }
                    else
                    {
                        mensaje = "Cuenta creada pero su contraseña no fue enviada por email";
                    }

                    return mensaje;
                }
                catch (Exception e)
                {
                    mensaje = "Error " + e.Message;
                }
            }

            return mensaje;
        }

        //metodo para buscar por cedula
        //[Authorize]
        [HttpGet("Buscar")]
        public async Task<Cliente> GetClient(string cedula)
        {
            var temp = await _context.Clientes.FirstOrDefaultAsync(c => c.Cedula == cedula);
            return temp;

        }

        //Modificar cliente
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

        //eliminar cliente
        //[Authorize]
        [HttpDelete("EliminarCliente")]
        public async Task<string> Eliminar(string vCedula)
        {
            string mensaje = "No se ha podido eliminar el cliente";
            try
            {
                var data = await _context.Clientes.FirstOrDefaultAsync(f => f.Cedula == vCedula);

                if (data != null)
                {
                    _context.Clientes.Remove(data);
                    _context.SaveChanges();

                    mensaje = "Cliente " + data.NombreCompleto + " eliminado correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
            }

            return mensaje;
        }



        //***   MÉTODOS  CONTRASEÑA    ****

        //Generar contraseña automáticamente
        private string GenerarPassword()
        {
            Random random = new Random();
            string password = string.Empty;

            password = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            return new string(Enumerable.Repeat(password, 12).Select(p => p[random.Next(p.Length)]).ToArray());
        }

        //Restablecer contraseña
        [HttpPost("Restablecer")]
        public async Task<string> Restablecer(string email, [Bind] Restablecer pRestablecer)
        {
            string mensaje = "";

            if (!string.IsNullOrEmpty(email) && pRestablecer != null)
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(u => u.Email.Equals(email));

                if (cliente != null)
                {
                    if (cliente.Password.Equals(pRestablecer.Password))
                    {
                        if (pRestablecer.NuevoPassword.Equals(pRestablecer.Confirmar))
                        {
                            cliente.Password = pRestablecer.Confirmar;
                            cliente.Restablecer = 1;

                            _context.Clientes.Update(cliente);
                            await _context.SaveChangesAsync();

                            return mensaje = "Se ha restablecido la contraseña exitosamente";
                        }
                        else
                        {
                            mensaje = "La confirmación de la contraseña es incorrecta";
                        }
                    }
                    else
                    {
                        mensaje = "La contraseña es incorrecta";
                    }
                }
                else
                {
                    mensaje = "No existe el usuario a restablecer la contraseña";
                }
            }
            else
            {
                mensaje = "Datos incorrectos";
            }

            return mensaje;
        }


        //***   MÉTODOS  AUTENTICACION    ****

        //validar email y password
        [HttpPost]
        [Route("AutenticarPW")]
        public async Task<IActionResult> AutenticarPW(string email, string password)
        {
            var temp = await _context.Clientes.FirstOrDefaultAsync(u => (u.Email.Equals(email)) && (u.Password.Equals(password)));

            if (temp == null)
            {
                return Unauthorized();
            }
            else
            {
                var autorizado = await autorizacionService.DevolverToken(temp);

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


        //***   MÉTODOS     EMAIL   ***
        private bool EnviarEmail(Cliente temp)
        {
            string mensaje = "";

            try
            {
                bool enviado = false;

                EmailCuenta email = new EmailCuenta();

                email.Enviar(temp);

                enviado = true;

                return enviado;
            }
            catch (Exception e)
            {
                mensaje = "Error al enviar el correo " + e.Message;

                return false;
            }
        }

    }//end class
}//end namespace

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIHotelBeach.Models;
using APIHotelBeach.Context;
using iText.Commons.Actions.Contexts;
using APIHotelBeach.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using static APIHotelBeach.Models.Cliente;

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

                    //Proceso de auditoria al agregar cliente
                    ClienteAuditoria clienteAuditoria = new ClienteAuditoria();

                    clienteAuditoria.Accion = "AGREGADO";
                    clienteAuditoria.FechaCambio = DateTime.Now;
                    clienteAuditoria.Cedula = cliente.Cedula;
                    clienteAuditoria.TipoCedula = cliente.TipoCedula;
                    clienteAuditoria.NombreCompleto = cliente.NombreCompleto;
                    clienteAuditoria.Telefono = cliente.Telefono;
                    clienteAuditoria.Direccion = cliente.Direccion;
                    clienteAuditoria.Email = cliente.Email;
                    clienteAuditoria.Password = cliente.Password;
                    clienteAuditoria.TipoUsuario = cliente.TipoUsuario;
                    clienteAuditoria.Restablecer = cliente.Restablecer;
                    clienteAuditoria.FechaRegistro = cliente.FechaRegistro;
                    clienteAuditoria.Estado = cliente.Estado;

                    _context.Clientes_Auditoria.Add(clienteAuditoria);
                    _context.SaveChanges();
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


        [HttpGet("BuscarCorreo")]
        public async Task<Cliente> GetClientCorreo(string email)
        {
            var temp = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
            return temp;

        }

        //Modificar cliente
        //[Authorize]
        [HttpPut("Modificar")]
        public string Modificar(Cliente cliente)
        {
            string msj = "";
            Cliente vCliente = new Cliente();

            try
            {
                var tempCliente = _context.Clientes.AsNoTracking().FirstOrDefault(f => f.Cedula == cliente.Cedula);
                vCliente = tempCliente;

                _context.Clientes.Update(cliente);
                _context.SaveChanges();
                msj = "Cliente modificado correctamente";

                //Proceso de auditoria al modificar cliente
                ClienteAuditoria clienteAuditoria = new ClienteAuditoria();

                clienteAuditoria.Accion = "MODIFICADO";
                clienteAuditoria.FechaCambio = DateTime.Now;
                clienteAuditoria.Cedula = vCliente.Cedula;
                clienteAuditoria.TipoCedula = vCliente.TipoCedula;
                clienteAuditoria.NombreCompleto = vCliente.NombreCompleto;
                clienteAuditoria.Telefono = vCliente.Telefono;
                clienteAuditoria.Direccion = vCliente.Direccion;
                clienteAuditoria.Email = vCliente.Email;
                clienteAuditoria.Password = vCliente.Password;
                clienteAuditoria.TipoUsuario = vCliente.TipoUsuario;
                clienteAuditoria.Restablecer = vCliente.Restablecer;
                clienteAuditoria.FechaRegistro = vCliente.FechaRegistro;
                clienteAuditoria.Estado = vCliente.Estado;

                _context.Clientes_Auditoria.Add(clienteAuditoria);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = "Error " + ex.Message + " " + ex.InnerException.ToString();
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

                    //Proceso de auditoria al eliminar cliente
                    ClienteAuditoria clienteAuditoria = new ClienteAuditoria();

                    clienteAuditoria.Accion = "ELIMINADO";
                    clienteAuditoria.FechaCambio = DateTime.Now;
                    clienteAuditoria.Cedula = data.Cedula;
                    clienteAuditoria.TipoCedula = data.TipoCedula;
                    clienteAuditoria.NombreCompleto = data.NombreCompleto;
                    clienteAuditoria.Telefono = data.Telefono;
                    clienteAuditoria.Direccion = data.Direccion;
                    clienteAuditoria.Email = data.Email;
                    clienteAuditoria.Password = data.Password;
                    clienteAuditoria.TipoUsuario = data.TipoUsuario;
                    clienteAuditoria.Restablecer = data.Restablecer;
                    clienteAuditoria.FechaRegistro = data.FechaRegistro;
                    clienteAuditoria.Estado = data.Estado;

                    _context.Clientes_Auditoria.Add(clienteAuditoria);
                    await _context.SaveChangesAsync();
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

        [HttpPost("Login")]
        public async Task<string> Login([Bind] LoginDto loginDto)
        {
            string mensaje = "";

            var temp = ValidarUsuario(loginDto);

            if (temp != null)
            {
                bool restablecer = VerificarRestablecer(temp);

                if (restablecer)
                {
                    mensaje = "Debe restablecer contraseña";
                }
                else
                {
                    var userClaims = new List<Claim>() { new Claim(ClaimTypes.Name, temp.Email) };

                    var grandIdentity = new ClaimsIdentity(userClaims, "User Identity");

                    var userPrincipal = new ClaimsPrincipal(new[] { grandIdentity });

                    await HttpContext.SignInAsync(userPrincipal);

                    mensaje = "Ha iniciado sesión";
                }
            }
            else
            {
                mensaje = "Error. Usuario o contraseña incorrectos";
            }

            return mensaje;
        }



        private Cliente ValidarUsuario(LoginDto loginDto)
        {
            Cliente autorizado = null;

            var user = _context.Clientes.FirstOrDefault(u => u.Email == loginDto.Email);

            if (user != null && user.Password.Equals(loginDto.Password))
            {
                autorizado = user;
            }

            return autorizado;
        }

        private bool VerificarRestablecer(Cliente temp)
        {
            bool verificado = false;

            var user = _context.Clientes.FirstOrDefault(u => u.Email == temp.Email);

            if (user != null)
            {
                if (user.Restablecer == 0)
                {
                    verificado = true;
                }
            }
            return verificado;
        }
    

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

                //email.Enviar(temp);

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

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

        //***   MÉTODOS  CRUD   ***

        //Registrar cliente
        //[Äuthorize]
        [HttpPost("CrearCuenta")]
        public string CrearCuenta(Cliente cliente)
        {
            string mensaje = "";

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

        //Verificar si el cliente ha restablecido contraseña
        private bool VerificarRestablecer(Cliente temp)
        {
            bool verificado = false;

            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == temp.Email);

            if (cliente != null)
            {
                if (cliente.Restablecer == 0)
                {
                    verificado = true;
                }
            }

            return verificado;
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

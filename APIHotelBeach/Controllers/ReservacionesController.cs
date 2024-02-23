using APIHotelBeach.Context;
using APIHotelBeach.Models;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace APIHotelBeach.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservacionesController : Controller
    {

        private readonly DbContextHotel _context;

        /// <summary>
        /// Constructor con parámetros
        /// </summary>
        /// <param name="pContext"></param>
        public ReservacionesController(DbContextHotel pContext)
        {
            _context = pContext;
        }

        //***   MÉTODOS     CRUD ***

        //lista de todas las reservaciones
        [HttpGet("ListaReservas")]
        public async Task<List<Reservacion>> ListaReservas()
        {
            var listReservas = await _context.Reservaciones.ToListAsync();

            return listReservas;
        }

        //lista de reservaciones de un cliente especifico
        [HttpGet("ListaReservasCliente")]
        public async Task<List<Reservacion>> ListaReservasCliente(string cedula)
        {
            var listReservas = await _context.Reservaciones.ToListAsync();

            List<Reservacion> listReservacionesCliente = new List<Reservacion>();

            foreach (var reservacion in listReservas)
            {
                if (reservacion.CedulaCliente.Equals(cedula))
                {
                    listReservacionesCliente.Add(reservacion);
                }
            }

            return listReservacionesCliente;
        }

        //Agregar
        [HttpPost("Agregar")]
        public async Task<string> Agregar(Reservacion pReserva)
        {
            string mensaje = "";
            decimal montoConDescuento = 0;
            decimal porcAdelanto = 0;
            int meses = 0;
            bool clienteEncontrado = false;
            bool paqueteEncontrado = false;
            string emailGuardar = " ";
            string nombreCliente = " ";

            try
            {
                pReserva.Id = 0;

                var clientes = await _context.Clientes.ToListAsync();

                foreach (var item in clientes)
                {
                    if (!clienteEncontrado)
                    {
                        if (pReserva.CedulaCliente.Equals(item.Cedula))
                        {
                            clienteEncontrado = true;
                            emailGuardar = item.Email;
                            nombreCliente = item.NombreCompleto;
                        }
                    }

                }

                var paquetes = await _context.Paquetes.ToListAsync();

                foreach (var item in paquetes)
                {
                    if (!paqueteEncontrado)
                    {
                        if (pReserva.IdPaquete.Equals(item.ID))
                        {
                            paqueteEncontrado = true;
                            pReserva.Subtotal = item.Precio * pReserva.Duracion;
                            pReserva.Impuesto = (pReserva.Subtotal * 13) / 100;
                            porcAdelanto = item.PorcentajePrima;
                            meses = item.LimiteMeses;

                            if (pReserva.TipoPago.Equals("Efectivo"))
                            {
                                if (pReserva.Duracion >= 3 && pReserva.Duracion <= 6)
                                {
                                    pReserva.Descuento = (pReserva.Subtotal * 10) / 100;
                                }
                                else
                                {
                                    if (pReserva.Duracion >= 7 && pReserva.Duracion <= 9)
                                    {
                                        pReserva.Descuento = (pReserva.Subtotal * 15) / 100;
                                    }
                                    else
                                    {
                                        if (pReserva.Duracion >= 10 && pReserva.Duracion <= 12)
                                        {
                                            pReserva.Descuento = (pReserva.Subtotal * 20) / 100;
                                        }
                                        else
                                        {
                                            if (pReserva.Duracion >= 13)
                                            {
                                                pReserva.Descuento = (pReserva.Subtotal * 25) / 100;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                pReserva.Descuento = 0;
                            }
                        }
                    }
                }

                if (pReserva.Descuento > 0)
                {
                    montoConDescuento = pReserva.Subtotal - pReserva.Descuento;
                    pReserva.MontoTotal = montoConDescuento + pReserva.Impuesto;
                }
                else
                {
                    pReserva.MontoTotal = pReserva.Subtotal + pReserva.Impuesto;
                }

                pReserva.Adelanto = (pReserva.MontoTotal * porcAdelanto) / 100;

                pReserva.MontoMensualidad = (pReserva.MontoTotal - pReserva.Adelanto) / meses;

                if (clienteEncontrado)
                {
                    ReservaClienteEmail reservaClienteEmail = new ReservaClienteEmail();

                    reservaClienteEmail.CedulaCliente = pReserva.CedulaCliente;
                    reservaClienteEmail.Email = emailGuardar;
                    reservaClienteEmail.NombreCompleto = nombreCliente;
                    reservaClienteEmail.IdPaquete = pReserva.IdPaquete;
                    reservaClienteEmail.TipoPago = pReserva.TipoPago;
                    reservaClienteEmail.FechaReserva = pReserva.FechaReserva;
                    reservaClienteEmail.Duracion = pReserva.Duracion;
                    reservaClienteEmail.Subtotal = pReserva.Subtotal;
                    reservaClienteEmail.Impuesto = pReserva.Impuesto;
                    reservaClienteEmail.Descuento = pReserva.Descuento;
                    reservaClienteEmail.MontoTotal = pReserva.MontoTotal;
                    reservaClienteEmail.Adelanto = pReserva.Adelanto;
                    reservaClienteEmail.MontoMensualidad = pReserva.MontoMensualidad;

                    if (paqueteEncontrado)
                    {
                        _context.Reservaciones.Add(pReserva);
                        await _context.SaveChangesAsync();

                        if (EnviarEmail(reservaClienteEmail))
                        {
                            mensaje = "Reservacion agregada correctamente.";
                        }
                        else
                        {
                            mensaje = "Reservacion agregada correctamente, pero el email no pudo ser enviado.";
                        }
                    }
                    else
                    {
                        mensaje = "Paquete no encontrado";
                    }
                }
                else
                {
                    mensaje = "Cliente no encontrado";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message + ", informacion: " + ex.InnerException;
            }

            return mensaje;
        }

        //Editar
        //[Authorize]
        [HttpPut("Editar")]
        public async Task<string> Editar(Reservacion pReserva)
        {
            string mensaje = "";
            bool clienteEncontrado = false;
            bool paqueteEncontrado = false;
            decimal porcAdelanto = 0;
            decimal montoConDescuento = 0;
            int meses = 0;

            try
            {
                var clientes = await _context.Clientes.ToListAsync();
                clienteEncontrado = clientes.Any(c => c.Cedula == pReserva.CedulaCliente);
                
                if(clienteEncontrado)
                {
                    var paquetes = await _context.Paquetes.ToListAsync();
                    var paquete = paquetes.FirstOrDefault(p => p.ID == pReserva.IdPaquete);

                    if (paquete != null)
                    {
                        paqueteEncontrado = true;
                        pReserva.Subtotal = paquete.Precio * pReserva.Duracion;
                        pReserva.Impuesto = (pReserva.Subtotal * 13) / 100;
                        porcAdelanto = paquete.PorcentajePrima;
                        meses = paquete.LimiteMeses;

                        if (pReserva.TipoPago.Equals("Efectivo"))
                        {
                            switch (pReserva.Duracion)
                            {
                                case >= 3 and <= 6:
                                    pReserva.Descuento = (pReserva.Subtotal * 10) / 100;
                                    break;
                                case >= 7 and <= 9:
                                    pReserva.Descuento = (pReserva.Subtotal * 15) / 100;
                                    break;
                                case >= 10 and <= 12:
                                    pReserva.Descuento = (pReserva.Subtotal * 20) / 100;
                                    break;
                                case >= 13:
                                    pReserva.Descuento = (pReserva.Subtotal * 25) / 100;
                                    break;
                                default:
                                    pReserva.Descuento = 0;
                                    break;
                            }
                        }

                        if (pReserva.Descuento > 0)
                        {
                            montoConDescuento = pReserva.Subtotal - pReserva.Descuento;
                            pReserva.MontoTotal = montoConDescuento + pReserva.Impuesto;
                        }
                        else
                        {
                            pReserva.MontoTotal = pReserva.Subtotal + pReserva.Impuesto;
                        }

                        pReserva.Adelanto = (pReserva.MontoTotal * porcAdelanto) / 100;

                        pReserva.MontoMensualidad = (pReserva.MontoTotal - pReserva.Adelanto) / meses;


                        _context.Reservaciones.Update(pReserva);
                        _context.SaveChanges();

                        mensaje = "Reservacion modificada correctamente";

                    }
                    else
                    {
                        mensaje = "Paquete no encontrado";
                    }
                } 
                else
                {
                    mensaje = "Cliente no encontrado";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message + ", informacion: " + ex.InnerException;
            }

            return mensaje;
        }

        //[Authorize]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int id)
        {
            string mensaje = "No se eliminó la reservación";

            try
            {
                var temp = await _context.Reservaciones.FirstOrDefaultAsync(f => f.Id == id);

                if (temp != null)
                {
                    _context.Reservaciones.Remove(temp);
                    _context.SaveChanges();

                    mensaje = "Reservación eliminada correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje += ex.Message;
            }
            return mensaje;
        }

        //***   MÉTODOS     EMAIL   ***
        private bool EnviarEmail(ReservaClienteEmail reservaClienteEmail)
        {
            string mensaje = "";

            try
            {
                bool enviado = false;

                EmailReservacion email = new EmailReservacion();

                email.EnviarPDF(reservaClienteEmail);

                enviado = true;

                return enviado;
            }
            catch (Exception e)
            {
                mensaje = "Error al enviar el correo " + e.Message;

                return false;
            }
        }
    }
}

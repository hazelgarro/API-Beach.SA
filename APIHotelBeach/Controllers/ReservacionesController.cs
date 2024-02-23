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
        //[Authorize]
        [HttpGet("ListaReservas")]
        public async Task<List<Reservacion>> ListaReservas()
        {
            var listReservas = await _context.Reservaciones.ToListAsync();

            return listReservas;
        }

        //lista de reservaciones de un cliente especifico
        //[Authorize]
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

        //Agregar reservacion
        //[Authorize]
        [HttpPost("AgregarReserva")]
        public async Task<string> AgregarReserva(Reservacion pReserva)
        {
            string mensaje = "";
            decimal montoConDescuento = 0;
            decimal porcAdelanto = 0;
            int meses = 0;
            bool clienteEncontrado = false;
            bool paqueteEncontrado = false;
            string emailGuardar = " ";
            string nombreCliente = " ";
            int ultimoId = 0;

            try
            {
                var reservas = await _context.Reservaciones.ToListAsync();

                foreach (var reservacion in reservas)
                {
                    ultimoId = reservacion.Id;
                }

                pReserva.Id = ultimoId + 1;

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

                    reservaClienteEmail.Id = pReserva.Id;
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

                        if (pReserva.TipoPago.Equals("Cheque"))
                        {
                            mensaje = "Reservacion agregada correctamente sin envío de email aún.";
                        }
                        else
                        {
                            if (EnviarEmail(reservaClienteEmail))
                            {
                                mensaje = "Reservacion agregada correctamente.";
                            }
                            else
                            {
                                mensaje = "Reservacion agregada correctamente, pero el email no pudo ser enviado.";
                            }
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

        //Agregar cheque
        //[Authorize]
        [HttpPost("AgregarCheque")]
        public async Task<string> AgregarCheque(Cheque pCheque)
        {
            string mensaje = "";
            ReservaPDFCheque reservaPDFCheque = new ReservaPDFCheque();
            bool clienteEncontrado = false;
            string emailGuardar = " ";
            string nombreCliente = " ";

            try
            {
                var reservas = await _context.Reservaciones.ToListAsync();
                foreach (var item in reservas)
                {
                    if (pCheque.IdReservacion == item.Id)
                    {
                        if (item.TipoPago.Equals("Cheque"))
                        {
                            var clientes = await _context.Clientes.ToListAsync();

                            foreach (var value in clientes)
                            {
                                if (!clienteEncontrado)
                                {
                                    if (item.CedulaCliente.Equals(value.Cedula))
                                    {
                                        clienteEncontrado = true;
                                        emailGuardar = value.Email;
                                        nombreCliente = value.NombreCompleto;
                                    }
                                }
                            }

                            reservaPDFCheque = new ReservaPDFCheque();

                            reservaPDFCheque.CedulaCliente = item.CedulaCliente;
                            reservaPDFCheque.Email = emailGuardar;
                            reservaPDFCheque.NombreCompleto = nombreCliente;
                            reservaPDFCheque.IdPaquete = item.IdPaquete;
                            reservaPDFCheque.TipoPago = item.TipoPago;
                            reservaPDFCheque.FechaReserva = item.FechaReserva;
                            reservaPDFCheque.Duracion = item.Duracion;
                            reservaPDFCheque.Subtotal = item.Subtotal;
                            reservaPDFCheque.Impuesto = item.Impuesto;
                            reservaPDFCheque.Descuento = item.Descuento;
                            reservaPDFCheque.MontoTotal = item.MontoTotal;
                            reservaPDFCheque.Adelanto = item.Adelanto;
                            reservaPDFCheque.MontoMensualidad = item.MontoMensualidad;
                            reservaPDFCheque.NumeroCheque = pCheque.NumeroCheque;
                            reservaPDFCheque.NombreBanco = pCheque.NombreBanco;

                        }
                        else
                        {
                            mensaje = "Los datos no coinciden";
                        }
                    }
                }

                _context.Cheques.Add(pCheque);
                _context.SaveChanges();

                if (EnviarEmailC(reservaPDFCheque))
                {
                    mensaje = "Cheque y reservacion agregada correctamente.";
                }
                else
                {
                    mensaje = "Cheque y reservacion agregada correctamente, pero el email no pudo ser enviado.";
                }
               
                
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message + " " + ex.InnerException.ToString;
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

        private bool EnviarEmailC(ReservaPDFCheque reservaPDFCheque)
        {
            string mensaje = "";

            try
            {
                bool enviado = false;

                EmailReservacion email = new EmailReservacion();

                email.EnviarPDFC(reservaPDFCheque);

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

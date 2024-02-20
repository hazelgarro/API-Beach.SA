using APIHotelBeach.Context;
using APIHotelBeach.Models;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIHotelBeach.Controllers
{
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

        //lista reservaciones
        [HttpGet("ListaReservas")]
        public async Task<List<Reservacion>> ListaReservas()
        {
            var listReservas = await _context.Reservaciones.ToListAsync();

            return listReservas;
        }

        [HttpPost("Agregar")]
        public async Task<string> Agregar(Reservacion pReserva)
        {
            string mensaje = "";
            decimal montoConDescuento = 0;
            decimal porcAdelanto = 0;
            int meses = 0;
            bool clienteEncontrado = false;
            bool paqueteEncontrado = false;

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
                            pReserva.Subtotal = item.Precio;
                            pReserva.Impuesto = (pReserva.Subtotal * 100) / item.PorcentajePrima;
                            porcAdelanto = item.PorcentajePrima;
                            meses = item.LimiteMeses;

                            if (pReserva.TipoPago.Equals("Efectivo"))
                            {
                                if (pReserva.Duracion >= 3 && pReserva.Duracion <= 6)
                                {
                                    pReserva.Descuento = (pReserva.Subtotal * 100) / 10;
                                }
                                else
                                {
                                    if (pReserva.Duracion >= 7 && pReserva.Duracion <= 9)
                                    {
                                        pReserva.Descuento = (pReserva.Subtotal * 100) / 15;
                                    }
                                    else
                                    {
                                        if (pReserva.Duracion >= 10 && pReserva.Duracion <= 12)
                                        {
                                            pReserva.Descuento = (pReserva.Subtotal * 100) / 20;
                                        }
                                        else
                                        {
                                            if (pReserva.Duracion >= 13)
                                            {
                                                pReserva.Descuento = (pReserva.Subtotal * 100) / 25;
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

                pReserva.Adelanto = (pReserva.MontoTotal * 100) / porcAdelanto;

                pReserva.MontoMensualidad = (pReserva.MontoTotal - pReserva.Adelanto) / meses;

                if (clienteEncontrado)
                {
                    if (paqueteEncontrado)
                    {
                        _context.Reservaciones.Add(pReserva);
                        _context.SaveChanges();

                        mensaje = "Reservacion agregada correctamente";
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



        //***   MÉTODOS     EMAIL   ***
        private bool EnviarEmail(Reservacion reservacion, Cliente usuario)
        {
            string mensaje = "";

            try
            {
                bool enviado = false;

                EmailReservacion email = new EmailReservacion();

                email.EnviarPDF(usuario, reservacion);

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

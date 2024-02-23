using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Layout.Borders;

namespace APIHotelBeach.Models
{
    public class EmailReservacion
    {
        public void EnviarPDF(ReservaClienteEmail reservaClienteEmail)
        {
            try
            {
                //Cuerpo del email
                string pathToTemplate = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailReservacion.html");
                string htmlBody = File.ReadAllText(pathToTemplate);

                // Generar el PDF
                byte[] pdfBytes = GenerarPDF(reservaClienteEmail);

                // Guardar el PDF como un archivo temporal
                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, pdfBytes);

                // Crear instancia del objeto email
                MailMessage email = new MailMessage();

                // Asunto
                email.Subject = "Datos de registro en formato PDF";

                // Destinatarios
                email.To.Add(new MailAddress(reservaClienteEmail.Email));

                // Copia al administrador
                email.To.Add(new MailAddress("jennifer.rodriguezestrada@outlook.com"));

                // Emisor
                email.From = new MailAddress("jennifer.rodriguezestrada@outlook.com");


                //html para el body del email
                htmlBody = htmlBody.Replace("{{Email}}", reservaClienteEmail.Email)
                               .Replace("{{NombreCompleto}}", reservaClienteEmail.NombreCompleto);

                //indicar que el contenido es en html
                email.IsBodyHtml = true;

                //prioridad
                email.Priority = MailPriority.Normal;

                //instanciar la vista del html para el cuerpo del email
                AlternateView view = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);

                email.AlternateViews.Add(view);


                // Leer el contenido del archivo temporal como un arreglo de bytes
                byte[] fileBytes = File.ReadAllBytes(tempFilePath);

                // Adjuntar el PDF al correo electrónico
                MemoryStream memoryStream = new MemoryStream(fileBytes);
                email.Attachments.Add(new Attachment(memoryStream, "FacturaReservacion.pdf", "application/pdf"));

                // Configurar el cliente SMTP
                SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("jennifer.rodriguezestrada@outlook.com", "4rr0zP1nt0"),
                    EnableSsl = true
                };

                // Enviar email
                smtp.Send(email);

                // Liberar los recursos
                email.Dispose();
                smtp.Dispose();

                // Eliminar el archivo temporal después de enviar el correo electrónico
                File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EnviarPDFC(ReservaPDFCheque reservaPDFCheque)
        {
            try
            {
                //Cuerpo del email
                string pathToTemplate = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailReservacion.html");
                string htmlBody = File.ReadAllText(pathToTemplate);

                // Generar el PDF
                byte[] pdfBytes = GenerarPDFC(reservaPDFCheque);

                // Guardar el PDF como un archivo temporal
                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, pdfBytes);

                // Crear instancia del objeto email
                MailMessage email = new MailMessage();

                // Asunto
                email.Subject = "Datos de registro en formato PDF";

                // Destinatarios
                email.To.Add(new MailAddress(reservaPDFCheque.Email));

                // Copia al administrador
                email.To.Add(new MailAddress("jennifer.rodriguezestrada@outlook.com"));

                // Emisor
                email.From = new MailAddress("jennifer.rodriguezestrada@outlook.com");


                //html para el body del email
                htmlBody = htmlBody.Replace("{{Email}}", reservaPDFCheque.Email)
                               .Replace("{{NombreCompleto}}", reservaPDFCheque.NombreCompleto);

                //indicar que el contenido es en html
                email.IsBodyHtml = true;

                //prioridad
                email.Priority = MailPriority.Normal;

                //instanciar la vista del html para el cuerpo del email
                AlternateView view = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);

                email.AlternateViews.Add(view);


                // Leer el contenido del archivo temporal como un arreglo de bytes
                byte[] fileBytes = File.ReadAllBytes(tempFilePath);

                // Adjuntar el PDF al correo electrónico
                MemoryStream memoryStream = new MemoryStream(fileBytes);
                email.Attachments.Add(new Attachment(memoryStream, "FacturaReservacion.pdf", "application/pdf"));

                // Configurar el cliente SMTP
                SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("jennifer.rodriguezestrada@outlook.com", "4rr0zP1nt0"),
                    EnableSsl = true
                };

                // Enviar email
                smtp.Send(email);

                // Liberar los recursos
                email.Dispose();
                smtp.Dispose();

                // Eliminar el archivo temporal después de enviar el correo electrónico
                File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GenerarPDF(ReservaClienteEmail reservaClienteEmail)
        {
            byte[] pdfBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                iText.Layout.Document document = new iText.Layout.Document(pdf);

                // Estilo para el título
                Style titleStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetBackgroundColor(new DeviceRgb(239, 118, 61))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(18);

                //Estilo para subtitulos
                Style subtitleStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(16)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetBackgroundColor(new DeviceRgb(238, 238, 238))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(10)
                    .SetMarginTop(20);

                // Estilo para el texto normal
                Style textStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.BLACK)
                    .SetMarginTop(10)
                    .SetMarginBottom(10);

                // Agregar título
                Paragraph title = new Paragraph("Hoteles Beach S.A | Factura")
                    .AddStyle(titleStyle);
                document.Add(title);

                //// Agregar texto con estilos
                document.Add(new Paragraph($"¡Hola, {reservaClienteEmail.NombreCompleto}! ¡Bienvenido a nuestro hotel, esperamos que disfrute su estadia!")
                    .AddStyle(textStyle));

                // Agregar título y detalles de la reservación
                document.Add(new Paragraph("Detalles de su reservación")
                    .AddStyle(subtitleStyle));

                // Crear tabla para los detalles de la reservación
                Table table = new Table(new float[] { 1, 4 })
                    .SetMarginTop(20)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Establecer los bordes de la tabla y las celdas como invisibles
                table.SetBorder(Border.NO_BORDER);
                table.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                // Tabla para detalles de la reservación

                //Nombre
                table.AddCell(new Cell().Add(new Paragraph("Nombre cliente:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.NombreCompleto)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Cédula
                table.AddCell(new Cell().Add(new Paragraph("Cédula cliente:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.CedulaCliente)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Fecha de reservacion
                table.AddCell(new Cell().Add(new Paragraph("Fecha de reservación:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.FechaReserva.ToString("dd/MM/yyyy"))).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Nombre Completo
                table.AddCell(new Cell().Add(new Paragraph("Duración:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Duracion.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table);

                // Agregar título y detalles de la factura
                document.Add(new Paragraph("Detalles factura")
                    .AddStyle(subtitleStyle));

                // Crear tabla para los detalles de la factura
                Table table2 = new Table(new float[] { 1, 4 })
                    .SetMarginTop(20)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Establecer los bordes de la tabla y las celdas como invisibles
                table2.SetBorder(Border.NO_BORDER);
                table2.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                // Tabla para detalles de la factura

                //Subtotal
                table2.AddCell(new Cell().Add(new Paragraph("Subtotal:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Subtotal.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Impuesto
                table2.AddCell(new Cell().Add(new Paragraph("Impuesto IVA:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Impuesto.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //descuento
                table2.AddCell(new Cell().Add(new Paragraph("Descuento:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Descuento.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //monto total
                table2.AddCell(new Cell().Add(new Paragraph("Monto total:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.MontoTotal.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //adelanto
                table2.AddCell(new Cell().Add(new Paragraph("Adelanto:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Adelanto.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table2);

                // Agregar título y detalles del pago
                document.Add(new Paragraph("Detalles pago")
                    .AddStyle(subtitleStyle));

                // Crear tabla para los detalles del pago
                Table table3 = new Table(new float[] { 1, 4 })
                    .SetMarginTop(20)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Establecer los bordes de la tabla y las celdas como invisibles
                table3.SetBorder(Border.NO_BORDER);
                table3.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                table3.AddCell(new Cell().Add(new Paragraph("Tipo pago:"))
                .SetFontColor(new DeviceRgb(239, 118, 61))
                .SetFontSize(14)
                .SetBorder(Border.NO_BORDER));
                table3.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.TipoPago.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table3.AddCell(new Cell().Add(new Paragraph("Mensualidad:"))
                   .SetFontColor(new DeviceRgb(239, 118, 61))
                   .SetFontSize(14)
                   .SetBorder(Border.NO_BORDER));
                table3.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.MontoMensualidad.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                // Derechos reservados
                document.Add(new Paragraph("@2024 | Hoteles Beach S.A.")
                    .SetFontColor(new DeviceRgb(136, 136, 136))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10)
                    .SetMarginTop(40));

                document.Close();

                pdfBytes = ms.ToArray();
            }

            return pdfBytes;
        }

        public byte[] GenerarPDFC(ReservaPDFCheque reservaPDFCheque)
        {
            byte[] pdfBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                iText.Layout.Document document = new iText.Layout.Document(pdf);

                // Estilo para el título
                Style titleStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetBackgroundColor(new DeviceRgb(239, 118, 61))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(18);

                //Estilo para subtitulos
                Style subtitleStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(16)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetBackgroundColor(new DeviceRgb(238, 238, 238))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(10)
                    .SetMarginTop(20);

                // Estilo para el texto normal
                Style textStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.BLACK)
                    .SetMarginTop(10)
                    .SetMarginBottom(10);

                // Agregar título
                Paragraph title = new Paragraph("Hoteles Beach S.A | Factura")
                    .AddStyle(titleStyle);
                document.Add(title);

                //// Agregar texto con estilos
                document.Add(new Paragraph($"¡Hola, {reservaPDFCheque.NombreCompleto}! ¡Bienvenido a nuestro hotel, esperamos que disfrute su estadia!")
                    .AddStyle(textStyle));

                // Agregar título y detalles de la reservación
                document.Add(new Paragraph("Detalles de su reservación")
                    .AddStyle(subtitleStyle));

                // Crear tabla para los detalles de la reservación
                Table table = new Table(new float[] { 1, 4 })
                    .SetMarginTop(20)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Establecer los bordes de la tabla y las celdas como invisibles
                table.SetBorder(Border.NO_BORDER);
                table.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                // Tabla para detalles de la reservación

                //Nombre
                table.AddCell(new Cell().Add(new Paragraph("Nombre cliente:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.NombreCompleto)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Cédula
                table.AddCell(new Cell().Add(new Paragraph("Cédula cliente:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.CedulaCliente)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Fecha de reservacion
                table.AddCell(new Cell().Add(new Paragraph("Fecha de reservación:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.FechaReserva.ToString("dd/MM/yyyy"))).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Nombre Completo
                table.AddCell(new Cell().Add(new Paragraph("Duración:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Duracion.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table);

                // Agregar título y detalles de la factura
                document.Add(new Paragraph("Detalles factura")
                    .AddStyle(subtitleStyle));

                // Crear tabla para los detalles de la factura
                Table table2 = new Table(new float[] { 1, 4 })
                    .SetMarginTop(20)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Establecer los bordes de la tabla y las celdas como invisibles
                table2.SetBorder(Border.NO_BORDER);
                table2.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                // Tabla para detalles de la factura

                //Subtotal
                table2.AddCell(new Cell().Add(new Paragraph("Subtotal:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Subtotal.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Impuesto
                table2.AddCell(new Cell().Add(new Paragraph("Impuesto IVA:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Impuesto.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //descuento
                table2.AddCell(new Cell().Add(new Paragraph("Descuento:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Descuento.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //monto total
                table2.AddCell(new Cell().Add(new Paragraph("Monto total:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.MontoTotal.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //adelanto
                table2.AddCell(new Cell().Add(new Paragraph("Adelanto:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Adelanto.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table2);

                // Agregar título y detalles del pago
                document.Add(new Paragraph("Detalles pago")
                    .AddStyle(subtitleStyle));

                // Crear tabla para los detalles del pago
                Table table3 = new Table(new float[] { 1, 4 })
                    .SetMarginTop(20)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Establecer los bordes de la tabla y las celdas como invisibles
                table3.SetBorder(Border.NO_BORDER);
                table3.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                table3.AddCell(new Cell().Add(new Paragraph("Tipo pago:"))
                .SetFontColor(new DeviceRgb(239, 118, 61))
                .SetFontSize(14)
                .SetBorder(Border.NO_BORDER));
                table3.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.TipoPago.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table3.AddCell(new Cell().Add(new Paragraph("Mensualidad:"))
                   .SetFontColor(new DeviceRgb(239, 118, 61))
                   .SetFontSize(14)
                   .SetBorder(Border.NO_BORDER));
                table3.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.MontoMensualidad.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(new Paragraph("Detalles del cheque")
                    .AddStyle(subtitleStyle));

                document.Add(new Paragraph($"Número de cheque: {reservaPDFCheque.NumeroCheque}").AddStyle(textStyle));
                document.Add(new Paragraph($"Nombre del banco: {reservaPDFCheque.NombreBanco}").AddStyle(textStyle));

                // Derechos reservados
                document.Add(new Paragraph("@2024 | Hoteles Beach S.A.")
                    .SetFontColor(new DeviceRgb(136, 136, 136))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10)
                    .SetMarginTop(40));

                document.Close();

                pdfBytes = ms.ToArray();
            }

            return pdfBytes;
        }
    }
}

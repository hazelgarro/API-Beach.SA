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

                byte[] pdfBytes = GenerarPDF(reservaClienteEmail);

                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, pdfBytes);

                MailMessage email = new MailMessage();

                email.Subject = "Datos reservación";

                email.To.Add(new MailAddress(reservaClienteEmail.Email));

                email.To.Add(new MailAddress("hotelbeachsa@outlook.com"));

                email.From = new MailAddress("hotelbeachsa@outlook.com");


                //html 
                htmlBody = htmlBody.Replace("{{Email}}", reservaClienteEmail.Email)
                               .Replace("{{NombreCompleto}}", reservaClienteEmail.NombreCompleto);

                email.IsBodyHtml = true;

                email.Priority = MailPriority.Normal;

                AlternateView view = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);

                email.AlternateViews.Add(view);

                byte[] fileBytes = File.ReadAllBytes(tempFilePath);

                // Adjuntar el PDF al correo electrónico
                MemoryStream memoryStream = new MemoryStream(fileBytes);
                email.Attachments.Add(new Attachment(memoryStream, "FacturaReservacion.pdf", "application/pdf"));

                //cliente SMTP
                SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("hotelbeachsa@outlook.com", "beachhotel24"),
                    EnableSsl = true
                };

                smtp.Send(email);

                email.Dispose();
                smtp.Dispose();

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
                string pathToTemplate = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailReservacion.html");
                string htmlBody = File.ReadAllText(pathToTemplate);

                byte[] pdfBytes = GenerarPDFC(reservaPDFCheque);

                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, pdfBytes);

                MailMessage email = new MailMessage();

                email.Subject = "Datos de registro en formato PDF";

                email.To.Add(new MailAddress(reservaPDFCheque.Email));

                email.To.Add(new MailAddress("hotelbeachsa@outlook.com"));


                email.From = new MailAddress("hotelbeachsa@outlook.com");

                htmlBody = htmlBody.Replace("{{Email}}", reservaPDFCheque.Email)
                               .Replace("{{NombreCompleto}}", reservaPDFCheque.NombreCompleto);

                email.IsBodyHtml = true;

                email.Priority = MailPriority.Normal;

                AlternateView view = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);

                email.AlternateViews.Add(view);

                byte[] fileBytes = File.ReadAllBytes(tempFilePath);

                MemoryStream memoryStream = new MemoryStream(fileBytes);
                email.Attachments.Add(new Attachment(memoryStream, "FacturaReservacion.pdf", "application/pdf"));

                SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("hotelbeachsa@outlook.com", "beachhotel24"),
                    EnableSsl = true
                };

                smtp.Send(email);

                email.Dispose();
                smtp.Dispose();

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
                Style titleStyle = new Style().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(20).SetFontColor(ColorConstants.WHITE)
                    .SetBackgroundColor(new DeviceRgb(239, 118, 61)).SetTextAlignment(TextAlignment.CENTER).SetPadding(18);

                //Estilo para subtitulos
                Style subtitleStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(16).SetFontColor(ColorConstants.GRAY)
                    .SetBackgroundColor(new DeviceRgb(238, 238, 238)).SetTextAlignment(TextAlignment.CENTER).SetPadding(10).SetMarginTop(20);

                // Estilo para el texto normal
                Style textStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12).SetFontColor(ColorConstants.BLACK)
                    .SetMarginTop(10).SetMarginBottom(10);

                //Título
                Paragraph title = new Paragraph("Hoteles Beach S.A | Factura").AddStyle(titleStyle);
                document.Add(title);

                //Saludo
               // document.Add(new Paragraph($"¡Hola, {reservaClienteEmail.NombreCompleto}! ¡Bienvenido a nuestro hotel, esperamos que disfrute su estadia!").AddStyle(textStyle));

                //Detalles reservación
                document.Add(new Paragraph("Detalles de su reservación").AddStyle(subtitleStyle));

                Table table = new Table(new float[] { 1, 4 }).SetMarginTop(20).SetWidth(UnitValue.CreatePercentValue(100));
                table.SetBorder(Border.NO_BORDER);
                table.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                table.AddCell(new Cell().Add(new Paragraph("Nombre cliente:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.NombreCompleto)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table.AddCell(new Cell().Add(new Paragraph("Cédula cliente:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.CedulaCliente)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table.AddCell(new Cell().Add(new Paragraph("Fecha de reservación:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.FechaReserva.ToString("dd/MM/yyyy"))).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table.AddCell(new Cell().Add(new Paragraph("Duración:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Duracion.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table);

                //Detalles factura
                document.Add(new Paragraph("Detalles factura").AddStyle(subtitleStyle));

                Table table2 = new Table(new float[] { 1, 4 }).SetMarginTop(20).SetWidth(UnitValue.CreatePercentValue(100));
                table2.SetBorder(Border.NO_BORDER);
                table2.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                table2.AddCell(new Cell().Add(new Paragraph("Subtotal:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Subtotal.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table2.AddCell(new Cell().Add(new Paragraph("Impuesto IVA:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Impuesto.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table2.AddCell(new Cell().Add(new Paragraph("Descuento:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Descuento.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table2.AddCell(new Cell().Add(new Paragraph("Monto total:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.MontoTotal.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table2.AddCell(new Cell().Add(new Paragraph("Adelanto:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.Adelanto.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table2);

                //Detalles pago
                document.Add(new Paragraph("Detalles pago").AddStyle(subtitleStyle));

                Table table3 = new Table(new float[] { 1, 4 }).SetMarginTop(20).SetWidth(UnitValue.CreatePercentValue(100));
                table3.SetBorder(Border.NO_BORDER);
                table3.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                table3.AddCell(new Cell().Add(new Paragraph("Tipo pago:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table3.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.TipoPago.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table3.AddCell(new Cell().Add(new Paragraph("Mensualidad:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table3.AddCell(new Cell().Add(new Paragraph(reservaClienteEmail.MontoMensualidad.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table3);

                //Derechos reservados
                document.Add(new Paragraph("@2024 | Hoteles Beach S.A.").SetFontColor(new DeviceRgb(136, 136, 136))
                    .SetTextAlignment(TextAlignment.CENTER).SetFontSize(10).SetMarginTop(40));

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
                Style titleStyle = new Style().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(20).SetFontColor(ColorConstants.WHITE)
                    .SetBackgroundColor(new DeviceRgb(239, 118, 61)).SetTextAlignment(TextAlignment.CENTER).SetPadding(18);

                //Estilo para subtitulos
                Style subtitleStyle = new Style().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(16).SetFontColor(ColorConstants.GRAY)
                    .SetBackgroundColor(new DeviceRgb(238, 238, 238)).SetTextAlignment(TextAlignment.CENTER).SetPadding(10).SetMarginTop(20);

                // Estilo para el texto normal
                Style textStyle = new Style().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12).SetFontColor(ColorConstants.BLACK)
                    .SetMarginTop(10).SetMarginBottom(10);

                //Título
                Paragraph title = new Paragraph("Hoteles Beach S.A | Factura").AddStyle(titleStyle);
                document.Add(title);

                //Saludo
                //document.Add(new Paragraph($"¡Hola, {reservaPDFCheque.NombreCompleto}! ¡Bienvenido a nuestro hotel, esperamos que disfrute su estadia!").AddStyle(textStyle));

                //Detalles de la reservación
                document.Add(new Paragraph("Detalles de su reservación").AddStyle(subtitleStyle));

                Table table = new Table(new float[] { 1, 4 }).SetMarginTop(20).SetWidth(UnitValue.CreatePercentValue(100));
                table.SetBorder(Border.NO_BORDER);
                table.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                table.AddCell(new Cell().Add(new Paragraph("Nombre cliente:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.NombreCompleto)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table.AddCell(new Cell().Add(new Paragraph("Cédula cliente:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.CedulaCliente)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table.AddCell(new Cell().Add(new Paragraph("Fecha de reservación:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.FechaReserva.ToString("dd/MM/yyyy"))).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table.AddCell(new Cell().Add(new Paragraph("Duración:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Duracion.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table);

                //Detalles factura
                document.Add(new Paragraph("Detalles factura").AddStyle(subtitleStyle));

                Table table2 = new Table(new float[] { 1, 4 }).SetMarginTop(20).SetWidth(UnitValue.CreatePercentValue(100));
                table2.SetBorder(Border.NO_BORDER);
                table2.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                table2.AddCell(new Cell().Add(new Paragraph("Subtotal:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Subtotal.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table2.AddCell(new Cell().Add(new Paragraph("Impuesto IVA:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Impuesto.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table2.AddCell(new Cell().Add(new Paragraph("Descuento:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Descuento.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table2.AddCell(new Cell().Add(new Paragraph("Monto total:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.MontoTotal.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table2.AddCell(new Cell().Add(new Paragraph("Adelanto:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table2.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.Adelanto.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table2);

                //Detalles pago
                document.Add(new Paragraph("Detalles pago").AddStyle(subtitleStyle));

                Table table3 = new Table(new float[] { 1, 4 }).SetMarginTop(20).SetWidth(UnitValue.CreatePercentValue(100));
                table3.SetBorder(Border.NO_BORDER);
                table3.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                table3.AddCell(new Cell().Add(new Paragraph("Tipo pago:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table3.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.TipoPago.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table3.AddCell(new Cell().Add(new Paragraph("Mensualidad:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table3.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.MontoMensualidad.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table3);

                //Detalles cheque
                document.Add(new Paragraph("Detalles cheque").AddStyle(subtitleStyle));

                Table table4 = new Table(new float[] { 1, 2 }).SetMarginTop(20).SetWidth(UnitValue.CreatePercentValue(100));
                table4.SetBorder(Border.NO_BORDER);
                table4.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);

                table4.AddCell(new Cell().Add(new Paragraph("Número cheque:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table4.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.NumeroCheque.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                table4.AddCell(new Cell().Add(new Paragraph("Nombre del banco:")).SetFontColor(new DeviceRgb(239, 118, 61)).SetFontSize(14).SetBorder(Border.NO_BORDER));
                table4.AddCell(new Cell().Add(new Paragraph(reservaPDFCheque.NombreBanco.ToString())).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table4);

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

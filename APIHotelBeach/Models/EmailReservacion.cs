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
        public void EnviarPDF(Cliente usuario)
        {
            try
            {
                //Cuerpo del email
                string pathToTemplate = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailReservacion.html");
                string htmlBody = File.ReadAllText(pathToTemplate);

                // Generar el PDF
                byte[] pdfBytes = GenerarPDF(usuario);

                // Guardar el PDF como un archivo temporal
                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, pdfBytes);

                // Crear instancia del objeto email
                MailMessage email = new MailMessage();

                // Asunto
                email.Subject = "Datos de registro en formato PDF";

                // Destinatarios
                email.To.Add(new MailAddress(usuario.Email));

                // Copia al administrador
                email.To.Add(new MailAddress("hotelbeachsa@outlook.com"));

                // Emisor
                email.From = new MailAddress("hotelbeachsa@outlook.com");


                //html para el body del email
                htmlBody = htmlBody.Replace("{{Email}}", usuario.Email)
                               .Replace("{{NombreCompleto}}", usuario.NombreCompleto)
                               .Replace("{{Password}}", usuario.Password);

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
                email.Attachments.Add(new Attachment(memoryStream, "RegistroUsuario.pdf", "application/pdf"));

                // Configurar el cliente SMTP
                SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("hotelbeachsa@outlook.com", "beachhotel24"),
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

        public byte[] GenerarPDF(Cliente cliente)
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
                    .SetPadding(10);

                // Estilo para el texto normal
                Style textStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.BLACK)
                    .SetMarginTop(10)
                    .SetMarginBottom(10);

                // Agregar título
                Paragraph title = new Paragraph("Hoteles Beach S.A Plataforma Web")
                    .AddStyle(titleStyle);
                document.Add(title);

                // Agregar texto con estilos
                document.Add(new Paragraph($"¡Hola, {cliente.NombreCompleto}! ¡Bienvenido a nuestro hotel, esperamos que disfrute su estadia!")
                    .AddStyle(textStyle));

                // Agregar título de detalles 
                document.Add(new Paragraph("Detalles de su reservación")
                    .AddStyle(subtitleStyle));

                // Crear tabla para los detalles 
                Table table = new Table(new float[] { 1, 3 })
                    .SetMarginTop(20)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Establecer los bordes de la tabla y las celdas como invisibles
                table.SetBorder(Border.NO_BORDER);
                table.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);


                // Agregar filas a la tabla con estilos
                //Email
                table.AddCell(new Cell().Add(new Paragraph("Email:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(cliente.Email)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Nombre Completo
                table.AddCell(new Cell().Add(new Paragraph("Nombre completo:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(cliente.NombreCompleto)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                //Contraseña
                table.AddCell(new Cell().Add(new Paragraph("Contraseña:"))
                    .SetFontColor(new DeviceRgb(239, 118, 61))
                    .SetFontSize(14)
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(cliente.Password)).AddStyle(textStyle).SetBorder(Border.NO_BORDER));

                document.Add(table);

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

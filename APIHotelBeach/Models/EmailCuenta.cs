using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using APIHotelBeach.Models;

namespace ApiHotelBeach.Models
{
    public class EmailCuenta
    {
        //enviar el email al usuario
        public void Enviar(Cliente usuario)
        {
            try
            {
                string pathToTemplate = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailUsuario.html");
                string htmlBody = File.ReadAllText(pathToTemplate);


                //crear instancia del obj email
                MailMessage email = new MailMessage();

                //asunto
                email.Subject = "Datos de registro en plataforma web Hoteles Beach S.A";

                //destinatarios
                email.To.Add(new MailAddress(usuario.Email));

                //copia al admin
                email.To.Add(new MailAddress("hotelbeachsa@outlook.com"));

                //emisor
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

                //agregar view al email
                SmtpClient smtp = new SmtpClient();

                //servidor smtp 
                smtp.Host = "smtp-mail.outlook.com";

                // # puerto 
                smtp.Port = 587;

                //seguridad tipo SSL
                smtp.EnableSsl = true;

                //credencialess por default para el buzón de correo
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("hotelbeachsa@outlook.com", "beachhotel24");

                //enviar email
                smtp.Send(email);

                //se liberan los recursos
                email.Dispose();
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

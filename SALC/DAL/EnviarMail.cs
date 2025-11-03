using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace SALC.EmailService
{
    public class EmailServicio
    {
        public bool EnviarInformePorCorreo(string destinatario, string rutaArchivoPdf)
        {
            try
            {
                // Configuración del servidor SMTP
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("pablogastonmaidana@gmail.com", "pablo05092003"),
                    EnableSsl = true
                };

                // Crear el mensaje
                var mensaje = new MailMessage();
                mensaje.From = new MailAddress("pablogastonmaidana@gmail.com", "Laboratorio SALC");
                mensaje.To.Add(destinatario);
                mensaje.Subject = "Informe de análisis clínico";
                mensaje.Body = "Estimado/a, se adjunta su informe en formato PDF.\n\nSaludos,\nLaboratorio SALC";

                // Adjuntar el PDF
                if (File.Exists(rutaArchivoPdf))
                    mensaje.Attachments.Add(new Attachment(rutaArchivoPdf));

                // Enviar
                smtp.Send(mensaje);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar correo: {ex.Message}");
                return false;
            }
        }
    }
}
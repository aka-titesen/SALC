using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SALC.BLL
{
    /// <summary>
    /// Implementación del servicio de envío de correos electrónicos siguiendo el patrón MVP.
    /// RF-08: Generar y Enviar Informe (parte 2: envío)
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsuario;
        private readonly string _smtpPassword;
        private readonly string _nombreRemitente;
        private readonly bool _habilitarSsl;

        public EmailService()
        {
            // Leer configuración desde App.config
            _smtpHost = ConfigurationManager.AppSettings["SMTP_Host"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"] ?? "587");
            _smtpUsuario = ConfigurationManager.AppSettings["SMTP_Usuario"] ?? "";
            _smtpPassword = ConfigurationManager.AppSettings["SMTP_Password"] ?? "";
            _nombreRemitente = ConfigurationManager.AppSettings["SMTP_NombreRemitente"] ?? "Laboratorio SALC";
            _habilitarSsl = bool.Parse(ConfigurationManager.AppSettings["SMTP_EnableSSL"] ?? "true");

            // Validar configuración
            if (string.IsNullOrWhiteSpace(_smtpUsuario) || string.IsNullOrWhiteSpace(_smtpPassword))
            {
                throw new InvalidOperationException(
                    "Configuración de correo incompleta. Verifique las claves SMTP_Usuario y SMTP_Password en App.config"
                );
            }
        }

        public bool EnviarInformePorCorreo(string destinatario, string nombrePaciente, string rutaArchivoPdf, string tipoAnalisis)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(destinatario))
                throw new ArgumentException("El email del destinatario es requerido", nameof(destinatario));

            if (string.IsNullOrWhiteSpace(nombrePaciente))
                throw new ArgumentException("El nombre del paciente es requerido", nameof(nombrePaciente));

            if (string.IsNullOrWhiteSpace(rutaArchivoPdf) || !File.Exists(rutaArchivoPdf))
                throw new ArgumentException("La ruta del archivo PDF es inválida o el archivo no existe", nameof(rutaArchivoPdf));

            if (string.IsNullOrWhiteSpace(tipoAnalisis))
                tipoAnalisis = "análisis clínico";

            try
            {
                // Configurar cliente SMTP
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsuario, _smtpPassword);
                    smtpClient.EnableSsl = _habilitarSsl;
                    smtpClient.Timeout = 30000; // 30 segundos

                    // Crear mensaje
                    using (var mensaje = new MailMessage())
                    {
                        mensaje.From = new MailAddress(_smtpUsuario, _nombreRemitente);
                        mensaje.To.Add(destinatario);
                        mensaje.Subject = $"Informe de Análisis Clínico - {tipoAnalisis}";
                        
                        // Cuerpo del mensaje con formato profesional
                        mensaje.Body = GenerarCuerpoMensaje(nombrePaciente, tipoAnalisis);
                        mensaje.IsBodyHtml = false; // Texto plano
                        mensaje.Priority = MailPriority.Normal;

                        // Adjuntar PDF
                        var archivo = new Attachment(rutaArchivoPdf);
                        mensaje.Attachments.Add(archivo);

                        // Enviar
                        smtpClient.Send(mensaje);

                        return true;
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                // Errores específicos del servidor SMTP
                throw new InvalidOperationException(
                    $"Error al enviar el correo electrónico: {smtpEx.Message}\n\n" +
                    "Verifique la configuración del servidor SMTP y las credenciales.", 
                    smtpEx
                );
            }
            catch (Exception ex)
            {
                // Otros errores
                throw new InvalidOperationException(
                    $"Error inesperado al enviar el correo: {ex.Message}", 
                    ex
                );
            }
        }

        private string GenerarCuerpoMensaje(string nombrePaciente, string tipoAnalisis)
        {
            return $@"Estimado/a {nombrePaciente},

Le informamos que los resultados de su {tipoAnalisis} ya se encuentran disponibles.

En el archivo adjunto encontrará el informe completo con los resultados validados por nuestro equipo médico.

Si tiene alguna consulta sobre los resultados, no dude en comunicarse con nosotros.

---
Laboratorio Clínico SALC
Sistema de Administración de Laboratorio Clínico

NOTA: Este es un correo automático. Por favor, no responda a este mensaje.
Si necesita realizar alguna consulta, comuníquese directamente con el laboratorio.
";
        }
    }
}

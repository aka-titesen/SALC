using System;

namespace SALC.BLL
{
    /// <summary>
    /// Servicio para el envío de correos electrónicos con informes PDF.
    /// RF-08: Generar y Enviar Informe (parte 2: envío)
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un informe PDF por correo electrónico al paciente.
        /// </summary>
        /// <param name="destinatario">Email del paciente destinatario</param>
        /// <param name="nombrePaciente">Nombre completo del paciente para personalizar el mensaje</param>
        /// <param name="rutaArchivoPdf">Ruta completa del archivo PDF a adjuntar</param>
        /// <param name="tipoAnalisis">Descripción del tipo de análisis realizado</param>
        /// <returns>True si el envío fue exitoso, False en caso contrario</returns>
        /// <exception cref="ArgumentException">Si los parámetros son inválidos</exception>
        /// <exception cref="InvalidOperationException">Si hay problemas con la configuración del servidor SMTP</exception>
        bool EnviarInformePorCorreo(string destinatario, string nombrePaciente, string rutaArchivoPdf, string tipoAnalisis);
    }
}

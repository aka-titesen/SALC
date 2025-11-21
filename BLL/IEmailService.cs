using System;

namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para el servicio de envío de correos electrónicos.
    /// Define las operaciones para enviar informes de análisis por email.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un informe de análisis por correo electrónico a un paciente
        /// </summary>
        /// <param name="destinatario">Email del destinatario</param>
        /// <param name="nombrePaciente">Nombre completo del paciente</param>
        /// <param name="rutaArchivoPdf">Ruta completa del archivo PDF del informe</param>
        /// <param name="tipoAnalisis">Descripción del tipo de análisis</param>
        /// <returns>True si el envío fue exitoso, false en caso contrario</returns>
        bool EnviarInformePorCorreo(string destinatario, string nombrePaciente, string rutaArchivoPdf, string tipoAnalisis);
    }
}

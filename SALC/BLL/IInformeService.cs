using System;

namespace SALC.BLL
{
    /// <summary>
    /// Servicio para la generación de informes PDF de análisis clínicos.
    /// RF-08: Generar y Enviar Informe
    /// </summary>
    public interface IInformeService
    {
        /// <summary>
        /// Genera un informe PDF para un análisis verificado y permite al usuario 
        /// seleccionar dónde guardarlo mediante un diálogo de Windows.
        /// </summary>
        /// <param name="idAnalisis">ID del análisis a informar</param>
        /// <returns>Ruta completa del archivo PDF generado, o null si el usuario canceló</returns>
        /// <exception cref="InvalidOperationException">Si el análisis no está verificado o no existe</exception>
        string GenerarPdfDeAnalisis(int idAnalisis);
    }
}

using System;

namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para el servicio de generación de informes PDF de análisis clínicos.
    /// Define las operaciones para crear documentos PDF con los resultados de análisis verificados.
    /// </summary>
    public interface IInformeService
    {
        /// <summary>
        /// Genera un informe PDF mostrando un diálogo para seleccionar la ubicación del archivo
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <returns>Ruta del archivo PDF generado o null si el usuario canceló</returns>
        string GenerarPdfDeAnalisis(int idAnalisis);

        /// <summary>
        /// Genera un informe PDF en una ruta específica sin mostrar diálogo
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <param name="rutaDestino">Ruta completa donde guardar el PDF</param>
        /// <returns>Ruta del archivo PDF generado</returns>
        string GenerarPdfDeAnalisis(int idAnalisis, string rutaDestino);
    }
}

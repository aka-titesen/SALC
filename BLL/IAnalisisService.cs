using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para el servicio de gestión de análisis clínicos.
    /// Define las operaciones para crear, modificar y consultar análisis y sus resultados.
    /// </summary>
    public interface IAnalisisService
    {
        /// <summary>
        /// Crea un nuevo análisis clínico
        /// </summary>
        /// <param name="dniPaciente">DNI del paciente</param>
        /// <param name="idTipoAnalisis">Tipo de análisis a realizar</param>
        /// <param name="dniMedicoCarga">DNI del médico que crea el análisis</param>
        /// <param name="observaciones">Observaciones generales del análisis</param>
        /// <returns>Análisis creado con su identificador asignado</returns>
        Analisis CrearAnalisis(int dniPaciente, int idTipoAnalisis, int dniMedicoCarga, string observaciones);

        /// <summary>
        /// Carga el resultado de una métrica en un análisis
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <param name="idMetrica">Identificador de la métrica</param>
        /// <param name="resultado">Valor del resultado obtenido</param>
        /// <param name="observaciones">Observaciones específicas de la métrica</param>
        void CargarResultado(int idAnalisis, int idMetrica, decimal resultado, string observaciones = null);

        /// <summary>
        /// Valida y firma un análisis, cambiando su estado a Verificado
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <param name="dniMedicoFirma">DNI del médico que firma y valida</param>
        void ValidarAnalisis(int idAnalisis, int dniMedicoFirma);

        /// <summary>
        /// Anula un análisis mediante baja lógica
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <param name="dniMedico">DNI del médico que anula</param>
        void AnularAnalisis(int idAnalisis, int dniMedico);

        /// <summary>
        /// Elimina un análisis mediante baja lógica (alias de AnularAnalisis)
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <param name="dniMedico">DNI del médico que elimina</param>
        void EliminarAnalisis(int idAnalisis, int dniMedico);

        /// <summary>
        /// Obtiene todos los análisis creados por un médico específico
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <returns>Colección de análisis del médico</returns>
        IEnumerable<Analisis> ObtenerAnalisisPorMedicoCarga(int dniMedico);

        /// <summary>
        /// Obtiene los análisis activos (no anulados) creados por un médico específico
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <returns>Colección de análisis activos del médico</returns>
        IEnumerable<Analisis> ObtenerAnalisisActivosPorMedicoCarga(int dniMedico);

        /// <summary>
        /// Obtiene todos los análisis de un paciente específico
        /// </summary>
        /// <param name="dniPaciente">DNI del paciente</param>
        /// <returns>Colección de análisis del paciente</returns>
        IEnumerable<Analisis> ObtenerAnalisisPorPaciente(int dniPaciente);

        /// <summary>
        /// Obtiene los análisis activos (no anulados) de un paciente específico
        /// </summary>
        /// <param name="dniPaciente">DNI del paciente</param>
        /// <returns>Colección de análisis activos del paciente</returns>
        IEnumerable<Analisis> ObtenerAnalisisActivosPorPaciente(int dniPaciente);

        /// <summary>
        /// Obtiene todos los análisis activos (no anulados) del sistema
        /// </summary>
        /// <returns>Colección de análisis activos</returns>
        IEnumerable<Analisis> ObtenerAnalisisActivos();

        /// <summary>
        /// Obtiene los resultados de métricas de un análisis específico
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <returns>Colección de resultados del análisis</returns>
        IEnumerable<AnalisisMetrica> ObtenerResultados(int idAnalisis);

        /// <summary>
        /// Obtiene un análisis por su identificador
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <returns>Análisis encontrado o null si no existe</returns>
        Analisis ObtenerAnalisisPorId(int idAnalisis);

        /// <summary>
        /// Verifica si un análisis está anulado
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <returns>True si el análisis está anulado, false en caso contrario</returns>
        bool EstaAnulado(int idAnalisis);

        /// <summary>
        /// Verifica si un análisis puede ser modificado (está en estado Pendiente)
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <returns>True si el análisis puede ser modificado, false en caso contrario</returns>
        bool PuedeSerModificado(int idAnalisis);
    }
}

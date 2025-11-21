using System;
using System.Collections.Generic;

namespace SALC.BLL
{
    /// <summary>
    /// Clase para transportar datos de productividad de médicos
    /// </summary>
    public class ReporteProductividad
    {
        /// <summary>
        /// DNI del médico
        /// </summary>
        public int DniMedico { get; set; }

        /// <summary>
        /// Nombre completo del médico
        /// </summary>
        public string NombreMedico { get; set; }

        /// <summary>
        /// Cantidad de análisis creados por el médico
        /// </summary>
        public int CantidadCreados { get; set; }

        /// <summary>
        /// Cantidad de análisis verificados por el médico
        /// </summary>
        public int CantidadVerificados { get; set; }
    }

    /// <summary>
    /// Clase para transportar datos de facturación por obra social
    /// </summary>
    public class ReporteFacturacion
    {
        /// <summary>
        /// Nombre de la obra social
        /// </summary>
        public string NombreObraSocial { get; set; }

        /// <summary>
        /// Cantidad de análisis realizados
        /// </summary>
        public int CantidadAnalisis { get; set; }

        /// <summary>
        /// Porcentaje respecto al total de análisis
        /// </summary>
        public decimal Porcentaje { get; set; }
    }

    /// <summary>
    /// Clase para transportar datos de demanda de análisis
    /// </summary>
    public class ReporteDemanda
    {
        /// <summary>
        /// Descripción del tipo de análisis
        /// </summary>
        public string TipoAnalisis { get; set; }

        /// <summary>
        /// Cantidad de veces solicitado
        /// </summary>
        public int CantidadSolicitados { get; set; }
    }

    /// <summary>
    /// Clase para transportar datos de alertas de valores críticos
    /// </summary>
    public class ReporteAlerta
    {
        /// <summary>
        /// Identificador del análisis
        /// </summary>
        public int IdAnalisis { get; set; }

        /// <summary>
        /// Nombre completo del paciente
        /// </summary>
        public string NombrePaciente { get; set; }

        /// <summary>
        /// Nombre de la métrica con valor crítico
        /// </summary>
        public string NombreMetrica { get; set; }

        /// <summary>
        /// Valor del resultado obtenido
        /// </summary>
        public decimal Resultado { get; set; }

        /// <summary>
        /// Valor mínimo de referencia
        /// </summary>
        public decimal? ValorMinimo { get; set; }

        /// <summary>
        /// Valor máximo de referencia
        /// </summary>
        public decimal? ValorMaximo { get; set; }

        /// <summary>
        /// Fecha del análisis
        /// </summary>
        public DateTime FechaAnalisis { get; set; }
    }

    /// <summary>
    /// Clase para transportar datos de carga de trabajo de un médico
    /// </summary>
    public class ReporteCargaTrabajo
    {
        /// <summary>
        /// Cantidad de análisis pendientes de verificar
        /// </summary>
        public int AnalisisPendientes { get; set; }

        /// <summary>
        /// Cantidad de análisis verificados en el mes actual
        /// </summary>
        public int AnalisisVerificadosMes { get; set; }
    }

    /// <summary>
    /// Interfaz para el servicio de generación de reportes y estadísticas.
    /// Define las operaciones para obtener reportes gerenciales y médicos del sistema.
    /// </summary>
    public interface IReportesService
    {
        /// <summary>
        /// Obtiene un reporte de productividad de médicos en un rango de fechas
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Lista con la productividad de cada médico</returns>
        List<ReporteProductividad> ObtenerProductividadMedicos(DateTime desde, DateTime hasta);

        /// <summary>
        /// Obtiene un reporte de facturación agrupado por obra social
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Lista con la facturación por obra social</returns>
        List<ReporteFacturacion> ObtenerFacturacionPorObraSocial(DateTime desde, DateTime hasta);

        /// <summary>
        /// Obtiene los tipos de análisis más solicitados
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <param name="top">Cantidad de tipos a retornar</param>
        /// <returns>Lista con los tipos de análisis más demandados</returns>
        List<ReporteDemanda> ObtenerTopAnalisis(DateTime desde, DateTime hasta, int top = 10);
        
        /// <summary>
        /// Obtiene un reporte de valores críticos (fuera de rango) para un médico
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Lista con los valores críticos encontrados</returns>
        List<ReporteAlerta> ObtenerValoresCriticos(int dniMedico, DateTime desde, DateTime hasta);

        /// <summary>
        /// Obtiene un reporte de carga de trabajo de un médico específico
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <returns>Información sobre la carga de trabajo del médico</returns>
        ReporteCargaTrabajo ObtenerCargaTrabajo(int dniMedico);
    }
}

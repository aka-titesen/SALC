using System;
using System.Collections.Generic;
using SALC.DAL;

namespace SALC.BLL
{
    /// <summary>
    /// Servicio de lógica de negocio para la generación de reportes y estadísticas del sistema.
    /// Proporciona información sobre productividad, facturación, demanda y alertas médicas.
    /// </summary>
    public class ReportesService : IReportesService
    {
        private readonly ReportesRepositorio _repo = new ReportesRepositorio();

        /// <summary>
        /// Obtiene un reporte de productividad de médicos en un rango de fechas
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Lista con la productividad de cada médico</returns>
        public List<ReporteProductividad> ObtenerProductividadMedicos(DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'Desde' no puede ser mayor a 'Hasta'");
                
            return _repo.ObtenerProductividadMedicos(desde, hasta);
        }

        /// <summary>
        /// Obtiene un reporte de facturación agrupado por obra social en un rango de fechas
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Lista con la facturación por obra social</returns>
        public List<ReporteFacturacion> ObtenerFacturacionPorObraSocial(DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'Desde' no puede ser mayor a 'Hasta'");
                
            return _repo.ObtenerFacturacionPorObraSocial(desde, hasta);
        }

        /// <summary>
        /// Obtiene los tipos de análisis más solicitados en un rango de fechas
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <param name="top">Cantidad de tipos de análisis a retornar</param>
        /// <returns>Lista con los tipos de análisis más demandados</returns>
        public List<ReporteDemanda> ObtenerTopAnalisis(DateTime desde, DateTime hasta, int top = 10)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'Desde' no puede ser mayor a 'Hasta'");
            
            if (top <= 0)
                throw new ArgumentException("El parámetro 'top' debe ser mayor a cero");
                
            return _repo.ObtenerTopAnalisis(desde, hasta, top);
        }

        /// <summary>
        /// Obtiene un reporte de valores críticos (fuera de rango) para un médico específico
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Lista con los valores críticos encontrados</returns>
        public List<ReporteAlerta> ObtenerValoresCriticos(int dniMedico, DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'Desde' no puede ser mayor a 'Hasta'");
                
            return _repo.ObtenerValoresCriticos(dniMedico, desde, hasta);
        }

        /// <summary>
        /// Obtiene un reporte de carga de trabajo de un médico específico
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <returns>Información sobre la carga de trabajo del médico</returns>
        public ReporteCargaTrabajo ObtenerCargaTrabajo(int dniMedico)
        {
            return _repo.ObtenerCargaTrabajo(dniMedico);
        }
    }
}

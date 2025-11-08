using System;
using System.Collections.Generic;

namespace SALC.BLL
{
    // Clases simples para transportar datos agregados de reportes
    public class ReporteProductividad
    {
        public int DniMedico { get; set; }
        public string NombreMedico { get; set; }
        public int CantidadCreados { get; set; }
        public int CantidadVerificados { get; set; }
    }

    public class ReporteFacturacion
    {
        public string NombreObraSocial { get; set; }
        public int CantidadAnalisis { get; set; }
        public decimal Porcentaje { get; set; }
    }

    public class ReporteDemanda
    {
        public string TipoAnalisis { get; set; }
        public int CantidadSolicitados { get; set; }
    }

    public class ReporteAlerta
    {
        public int IdAnalisis { get; set; }
        public string NombrePaciente { get; set; }
        public string NombreMetrica { get; set; }
        public decimal Resultado { get; set; }
        public decimal? ValorMinimo { get; set; }
        public decimal? ValorMaximo { get; set; }
        public DateTime FechaAnalisis { get; set; }
    }

    public class ReporteCargaTrabajo
    {
        public int AnalisisPendientes { get; set; }
        public int AnalisisVerificadosMes { get; set; }
    }

    public interface IReportesService
    {
        // Reportes Admin
        List<ReporteProductividad> ObtenerProductividadMedicos(DateTime desde, DateTime hasta);
        List<ReporteFacturacion> ObtenerFacturacionPorObraSocial(DateTime desde, DateTime hasta);
        List<ReporteDemanda> ObtenerTopAnalisis(DateTime desde, DateTime hasta, int top = 10);
        
        // Reportes Médico (para futuras implementaciones)
        List<ReporteAlerta> ObtenerValoresCriticos(int dniMedico, DateTime desde, DateTime hasta);
        ReporteCargaTrabajo ObtenerCargaTrabajo(int dniMedico);
    }
}

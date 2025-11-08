using System;
using System.Collections.Generic;
using SALC.DAL;

namespace SALC.BLL
{
    public class ReportesService : IReportesService
    {
        private readonly ReportesRepositorio _repo = new ReportesRepositorio();

        public List<ReporteProductividad> ObtenerProductividadMedicos(DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'Desde' no puede ser mayor a 'Hasta'");
                
            return _repo.ObtenerProductividadMedicos(desde, hasta);
        }

        public List<ReporteFacturacion> ObtenerFacturacionPorObraSocial(DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'Desde' no puede ser mayor a 'Hasta'");
                
            return _repo.ObtenerFacturacionPorObraSocial(desde, hasta);
        }

        public List<ReporteDemanda> ObtenerTopAnalisis(DateTime desde, DateTime hasta, int top = 10)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'Desde' no puede ser mayor a 'Hasta'");
            
            if (top <= 0)
                throw new ArgumentException("El parámetro 'top' debe ser mayor a cero");
                
            return _repo.ObtenerTopAnalisis(desde, hasta, top);
        }

        public List<ReporteAlerta> ObtenerValoresCriticos(int dniMedico, DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'Desde' no puede ser mayor a 'Hasta'");
                
            return _repo.ObtenerValoresCriticos(dniMedico, desde, hasta);
        }

        public ReporteCargaTrabajo ObtenerCargaTrabajo(int dniMedico)
        {
            return _repo.ObtenerCargaTrabajo(dniMedico);
        }
    }
}

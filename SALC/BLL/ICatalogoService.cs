using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface ICatalogoService
    {
        // Métodos que devuelven todos los registros (incluyendo inactivos)
        IEnumerable<ObraSocial> ObtenerObrasSociales();
        IEnumerable<TipoAnalisis> ObtenerTiposAnalisis();
        IEnumerable<Metrica> ObtenerMetricas();
        
        // Métodos que devuelven solo registros activos
        IEnumerable<ObraSocial> ObtenerObrasSocialesActivas();
        IEnumerable<TipoAnalisis> ObtenerTiposAnalisisActivos();
        IEnumerable<Metrica> ObtenerMetricasActivas();
        
        // ABM - Los métodos de eliminación realizan baja lógica
        void CrearObraSocial(ObraSocial os);
        void ActualizarObraSocial(ObraSocial os);
        void EliminarObraSocial(int id); // Baja lógica
        void CrearTipoAnalisis(TipoAnalisis ta);
        void ActualizarTipoAnalisis(TipoAnalisis ta);
        void EliminarTipoAnalisis(int id); // Baja lógica
        void CrearMetrica(Metrica m);
        void ActualizarMetrica(Metrica m);
        void EliminarMetrica(int id); // Baja lógica
    }
}

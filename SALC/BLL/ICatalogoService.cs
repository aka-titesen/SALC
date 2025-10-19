using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface ICatalogoService
    {
        // M�todos que devuelven todos los registros (incluyendo inactivos)
        IEnumerable<ObraSocial> ObtenerObrasSociales();
        IEnumerable<TipoAnalisis> ObtenerTiposAnalisis();
        IEnumerable<Metrica> ObtenerMetricas();
        
        // M�todos que devuelven solo registros activos
        IEnumerable<ObraSocial> ObtenerObrasSocialesActivas();
        IEnumerable<TipoAnalisis> ObtenerTiposAnalisisActivos();
        IEnumerable<Metrica> ObtenerMetricasActivas();
        
        // ABM - Los m�todos de eliminaci�n realizan baja l�gica
        void CrearObraSocial(ObraSocial os);
        void ActualizarObraSocial(ObraSocial os);
        void EliminarObraSocial(int id); // Baja l�gica
        void CrearTipoAnalisis(TipoAnalisis ta);
        void ActualizarTipoAnalisis(TipoAnalisis ta);
        void EliminarTipoAnalisis(int id); // Baja l�gica
        void CrearMetrica(Metrica m);
        void ActualizarMetrica(Metrica m);
        void EliminarMetrica(int id); // Baja l�gica

        // Gesti�n de relaciones Tipo An�lisis - M�tricas
        IEnumerable<TipoAnalisisMetrica> ObtenerRelacionesTipoAnalisisMetricas();
        IEnumerable<Metrica> ObtenerMetricasPorTipoAnalisis(int idTipoAnalisis);
        void CrearRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica);
        void EliminarRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica);
        void ActualizarRelacionesTipoAnalisis(int idTipoAnalisis, IEnumerable<int> idsMetricas);
        bool ExisteRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica);
    }
}

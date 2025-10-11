using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface ICatalogoService
    {
        IEnumerable<ObraSocial> ObtenerObrasSociales();
        IEnumerable<TipoAnalisis> ObtenerTiposAnalisis();
        IEnumerable<Metrica> ObtenerMetricas();
        // ABM
        void CrearObraSocial(ObraSocial os);
        void ActualizarObraSocial(ObraSocial os);
        void EliminarObraSocial(int id);
        void CrearTipoAnalisis(TipoAnalisis ta);
        void ActualizarTipoAnalisis(TipoAnalisis ta);
        void EliminarTipoAnalisis(int id);
        void CrearMetrica(Metrica m);
        void ActualizarMetrica(Metrica m);
        void EliminarMetrica(int id);
    }
}

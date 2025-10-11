using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface ICatalogoService
    {
        IEnumerable<ObraSocial> ObtenerObrasSociales();
        IEnumerable<TipoAnalisis> ObtenerTiposAnalisis();
        IEnumerable<Metrica> ObtenerMetricas();
    }
}

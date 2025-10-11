using System.Collections.Generic;
using SALC.Domain;
using SALC.DAL;

namespace SALC.BLL
{
    public class CatalogoService : ICatalogoService
    {
        private readonly CatalogoRepositorio _repo = new CatalogoRepositorio();
        public IEnumerable<Metrica> ObtenerMetricas()
        {
            return _repo.ObtenerMetricas();
        }

        public IEnumerable<ObraSocial> ObtenerObrasSociales()
        {
            return _repo.ObtenerObrasSociales();
        }

        public IEnumerable<TipoAnalisis> ObtenerTiposAnalisis()
        {
            return _repo.ObtenerTiposAnalisis();
        }

        // ABM
        public void CrearObraSocial(ObraSocial os) => _repo.CrearObraSocial(os);
        public void ActualizarObraSocial(ObraSocial os) => _repo.ActualizarObraSocial(os);
        public void EliminarObraSocial(int id) => _repo.EliminarObraSocial(id);
        public void CrearTipoAnalisis(TipoAnalisis ta) => _repo.CrearTipoAnalisis(ta);
        public void ActualizarTipoAnalisis(TipoAnalisis ta) => _repo.ActualizarTipoAnalisis(ta);
        public void EliminarTipoAnalisis(int id) => _repo.EliminarTipoAnalisis(id);
        public void CrearMetrica(Metrica m) => _repo.CrearMetrica(m);
        public void ActualizarMetrica(Metrica m) => _repo.ActualizarMetrica(m);
        public void EliminarMetrica(int id) => _repo.EliminarMetrica(id);
    }
}

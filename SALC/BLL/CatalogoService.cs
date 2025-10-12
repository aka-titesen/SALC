using System.Collections.Generic;
using SALC.Domain;
using SALC.DAL;

namespace SALC.BLL
{
    public class CatalogoService : ICatalogoService
    {
        private readonly CatalogoRepositorio _repo = new CatalogoRepositorio();
        
        // Métodos que devuelven todos los registros (incluyendo inactivos)
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

        // Métodos que devuelven solo registros activos (para combos y selecciones)
        public IEnumerable<Metrica> ObtenerMetricasActivas()
        {
            return _repo.ObtenerMetricasActivas();
        }

        public IEnumerable<ObraSocial> ObtenerObrasSocialesActivas()
        {
            return _repo.ObtenerObrasSocialesActivas();
        }

        public IEnumerable<TipoAnalisis> ObtenerTiposAnalisisActivos()
        {
            return _repo.ObtenerTiposAnalisisActivos();
        }

        // ABM - Los métodos de eliminación ahora realizan baja lógica
        public void CrearObraSocial(ObraSocial os) => _repo.CrearObraSocial(os);
        public void ActualizarObraSocial(ObraSocial os) => _repo.ActualizarObraSocial(os);
        public void EliminarObraSocial(int id) => _repo.EliminarObraSocial(id); // Baja lógica
        
        public void CrearTipoAnalisis(TipoAnalisis ta) => _repo.CrearTipoAnalisis(ta);
        public void ActualizarTipoAnalisis(TipoAnalisis ta) => _repo.ActualizarTipoAnalisis(ta);
        public void EliminarTipoAnalisis(int id) => _repo.EliminarTipoAnalisis(id); // Baja lógica
        
        public void CrearMetrica(Metrica m) => _repo.CrearMetrica(m);
        public void ActualizarMetrica(Metrica m) => _repo.ActualizarMetrica(m);
        public void EliminarMetrica(int id) => _repo.EliminarMetrica(id); // Baja lógica
    }
}

using System.Collections.Generic;
using SALC.Domain;
using SALC.DAL;
using System.Linq;

namespace SALC.BLL
{
    public class CatalogoService : ICatalogoService
    {
        private readonly CatalogoRepositorio _repo = new CatalogoRepositorio();
        private readonly TipoAnalisisMetricaRepositorio _tipoAnalisisMetricaRepo = new TipoAnalisisMetricaRepositorio();
        
        // M�todos que devuelven todos los registros (incluyendo inactivos)
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

        // M�todos que devuelven solo registros activos (para combos y selecciones)
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

        // ABM - Los m�todos de eliminaci�n ahora realizan baja l�gica
        public void CrearObraSocial(ObraSocial os) => _repo.CrearObraSocial(os);
        public void ActualizarObraSocial(ObraSocial os) => _repo.ActualizarObraSocial(os);
        public void EliminarObraSocial(int id) => _repo.EliminarObraSocial(id); // Baja l�gica
        
        public void CrearTipoAnalisis(TipoAnalisis ta) => _repo.CrearTipoAnalisis(ta);
        public void ActualizarTipoAnalisis(TipoAnalisis ta) => _repo.ActualizarTipoAnalisis(ta);
        public void EliminarTipoAnalisis(int id) => _repo.EliminarTipoAnalisis(id); // Baja l�gica
        
        public void CrearMetrica(Metrica m) => _repo.CrearMetrica(m);
        public void ActualizarMetrica(Metrica m) => _repo.ActualizarMetrica(m);
        public void EliminarMetrica(int id) => _repo.EliminarMetrica(id); // Baja l�gica

        // Gesti�n de relaciones Tipo An�lisis - M�tricas
        public IEnumerable<TipoAnalisisMetrica> ObtenerRelacionesTipoAnalisisMetricas()
        {
            return _tipoAnalisisMetricaRepo.ObtenerRelaciones();
        }

        public IEnumerable<Metrica> ObtenerMetricasPorTipoAnalisis(int idTipoAnalisis)
        {
            return _tipoAnalisisMetricaRepo.ObtenerMetricasPorTipoAnalisis(idTipoAnalisis);
        }

        public void CrearRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica)
        {
            if (!ExisteRelacionTipoAnalisisMetrica(idTipoAnalisis, idMetrica))
            {
                _tipoAnalisisMetricaRepo.CrearRelacion(idTipoAnalisis, idMetrica);
            }
        }

        public void EliminarRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica)
        {
            _tipoAnalisisMetricaRepo.EliminarRelacion(idTipoAnalisis, idMetrica);
        }

        public void ActualizarRelacionesTipoAnalisis(int idTipoAnalisis, IEnumerable<int> idsMetricas)
        {
            // Eliminar todas las relaciones existentes del tipo de an�lisis
            _tipoAnalisisMetricaRepo.EliminarTodasLasRelacionesDeTipoAnalisis(idTipoAnalisis);
            
            // Crear las nuevas relaciones
            foreach (var idMetrica in idsMetricas)
            {
                _tipoAnalisisMetricaRepo.CrearRelacion(idTipoAnalisis, idMetrica);
            }
        }

        public bool ExisteRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica)
        {
            return _tipoAnalisisMetricaRepo.ExisteRelacion(idTipoAnalisis, idMetrica);
        }
    }
}

using System;
using System.Collections.Generic;
using SALC.Models;
using SALC.DataAccess.Repositories;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para administración de catálogos siguiendo la convención 'Service'.
    /// </summary>
    public class AdministracionCatalogosService
    {
        private readonly ObraSocialRepository _repoObraSocial = new ObraSocialRepository();
        private readonly TipoAnalisisRepository _repoTipoAnalisis = new TipoAnalisisRepository();
        private readonly RolRepository _repoRol = new RolRepository();

        public List<ObraSocial> ObtenerObrasSociales()
        {
            try { return _repoObraSocial.ObtenerTodas(); }
            catch (Exception ex) { throw new InvalidOperationException($"Error al obtener obras sociales: {ex.Message}", ex); }
        }

        public bool CrearObraSocial(ObraSocial obra)
        {
            try
            {
                if (obra == null || !obra.EsValida()) throw new ArgumentException("Datos de obra social inválidos");
                if (_repoObraSocial.ObtenerPorCuit(obra.Cuit) != null) throw new InvalidOperationException($"Ya existe una obra social con CUIT: {obra.Cuit}");
                return _repoObraSocial.Crear(obra);
            }
            catch (Exception ex) { throw new InvalidOperationException($"Error al crear obra social: {ex.Message}", ex); }
        }

        public bool ActualizarObraSocial(ObraSocial obra)
        {
            try
            {
                if (obra == null || !obra.EsValida() || obra.IdObraSocial <= 0) throw new ArgumentException("Datos de obra social inválidos");
                if (!_repoObraSocial.ExistePorId(obra.IdObraSocial)) throw new ArgumentException("La obra social no existe");
                if (_repoObraSocial.ExisteCuitEnOtraObraSocial(obra.Cuit, obra.IdObraSocial)) throw new InvalidOperationException($"Ya existe una obra social con CUIT: {obra.Cuit}");
                return _repoObraSocial.Actualizar(obra);
            }
            catch (Exception ex) { throw new InvalidOperationException($"Error al actualizar obra social: {ex.Message}", ex); }
        }

        public bool EliminarObraSocial(int idObraSocial)
        {
            try { return _repoObraSocial.Eliminar(idObraSocial); }
            catch (Exception ex) { throw new InvalidOperationException($"Error al eliminar obra social: {ex.Message}", ex); }
        }

        public List<TipoAnalisis> ObtenerTiposAnalisis()
        {
            try { return _repoTipoAnalisis.ObtenerTodos(); }
            catch (Exception ex) { throw new InvalidOperationException($"Error al obtener tipos de análisis: {ex.Message}", ex); }
        }

        public bool CrearTipoAnalisis(TipoAnalisis tipo)
        {
            try
            {
                if (tipo == null || !tipo.EsValido()) throw new ArgumentException("Datos de tipo de análisis inválidos");
                if (_repoTipoAnalisis.ExistePorDescripcion(tipo.Descripcion)) throw new InvalidOperationException($"Ya existe un tipo de análisis con descripción '{tipo.Descripcion}'");
                return _repoTipoAnalisis.Crear(tipo);
            }
            catch (Exception ex) { throw new InvalidOperationException($"Error al crear tipo de análisis: {ex.Message}", ex); }
        }

        public bool ActualizarTipoAnalisis(TipoAnalisis tipo)
        {
            try
            {
                if (tipo == null || !tipo.EsValido()) throw new ArgumentException("Datos de tipo de análisis inválidos");
                if (!_repoTipoAnalisis.ExistePorId(tipo.IdTipoAnalisis)) throw new ArgumentException("El tipo de análisis no existe");
                return _repoTipoAnalisis.Actualizar(tipo);
            }
            catch (Exception ex) { throw new InvalidOperationException($"Error al actualizar tipo de análisis: {ex.Message}", ex); }
        }

        public bool EliminarTipoAnalisis(int idTipoAnalisis)
        {
            try { return _repoTipoAnalisis.Eliminar(idTipoAnalisis); }
            catch (Exception ex) { throw new InvalidOperationException($"Error al eliminar tipo de análisis: {ex.Message}", ex); }
        }

        public List<Rol> ObtenerRoles()
        {
            try { return _repoRol.ObtenerTodos(); }
            catch (Exception ex) { throw new InvalidOperationException($"Error al obtener roles: {ex.Message}", ex); }
        }
    }
}

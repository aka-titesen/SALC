using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SALC.Domain;
using SALC.DAL;
using SALC.Infraestructura;
using SALC.Infraestructura.Exceptions;

namespace SALC.BLL
{
    public class CatalogoService : ICatalogoService
    {
        private readonly CatalogoRepositorio _repo = new CatalogoRepositorio();
        private readonly TipoAnalisisMetricaRepositorio _tipoAnalisisMetricaRepo = new TipoAnalisisMetricaRepositorio();
        
        #region Métodos de Consulta

        public IEnumerable<Metrica> ObtenerMetricas()
        {
            try
            {
                return _repo.ObtenerMetricas();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener métricas", "ObtenerMetricas", sqlEx);
            }
        }

        public IEnumerable<ObraSocial> ObtenerObrasSociales()
        {
            try
            {
                return _repo.ObtenerObrasSociales();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener obras sociales", "ObtenerObrasSociales", sqlEx);
            }
        }

        public IEnumerable<TipoAnalisis> ObtenerTiposAnalisis()
        {
            try
            {
                return _repo.ObtenerTiposAnalisis();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener tipos de análisis", "ObtenerTiposAnalisis", sqlEx);
            }
        }

        public IEnumerable<Metrica> ObtenerMetricasActivas()
        {
            try
            {
                return _repo.ObtenerMetricasActivas();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener métricas activas", "ObtenerMetricasActivas", sqlEx);
            }
        }

        public IEnumerable<ObraSocial> ObtenerObrasSocialesActivas()
        {
            try
            {
                return _repo.ObtenerObrasSocialesActivas();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener obras sociales activas", "ObtenerObrasSocialesActivas", sqlEx);
            }
        }

        public IEnumerable<TipoAnalisis> ObtenerTiposAnalisisActivos()
        {
            try
            {
                return _repo.ObtenerTiposAnalisisActivos();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener tipos de análisis activos", "ObtenerTiposAnalisisActivos", sqlEx);
            }
        }

        #endregion

        #region Obras Sociales

        public void CrearObraSocial(ObraSocial os)
        {
            try
            {
                ValidarObraSocial(os);

                ExceptionHandler.LogInfo($"Creando obra social - CUIT: {os.Cuit}, Nombre: {os.Nombre}", "CrearObraSocial");

                _repo.CrearObraSocial(os);

                ExceptionHandler.LogInfo($"Obra social creada exitosamente - CUIT: {os.Cuit}", "CrearObraSocial");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al crear la obra social", "CrearObraSocial", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void ActualizarObraSocial(ObraSocial os)
        {
            try
            {
                ValidarObraSocial(os);

                if (os.IdObraSocial <= 0)
                    throw new SalcValidacionException("El ID de la obra social no es válido.", "idObraSocial");

                ExceptionHandler.LogInfo($"Actualizando obra social - ID: {os.IdObraSocial}, CUIT: {os.Cuit}", "ActualizarObraSocial");

                _repo.ActualizarObraSocial(os);

                ExceptionHandler.LogInfo($"Obra social actualizada exitosamente - ID: {os.IdObraSocial}", "ActualizarObraSocial");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al actualizar la obra social", "ActualizarObraSocial", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void EliminarObraSocial(int id)
        {
            try
            {
                if (id <= 0)
                    throw new SalcValidacionException("El ID de la obra social no es válido.", "id");

                ExceptionHandler.LogInfo($"Eliminando obra social (baja lógica) - ID: {id}", "EliminarObraSocial");

                _repo.EliminarObraSocial(id);

                ExceptionHandler.LogInfo($"Obra social eliminada exitosamente - ID: {id}", "EliminarObraSocial");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al eliminar la obra social", "EliminarObraSocial", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        #endregion

        #region Tipos de Análisis

        public void CrearTipoAnalisis(TipoAnalisis ta)
        {
            try
            {
                ValidarTipoAnalisis(ta);

                ExceptionHandler.LogInfo($"Creando tipo de análisis - Descripción: {ta.Descripcion}", "CrearTipoAnalisis");

                _repo.CrearTipoAnalisis(ta);

                ExceptionHandler.LogInfo($"Tipo de análisis creado exitosamente - Descripción: {ta.Descripcion}", "CrearTipoAnalisis");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al crear el tipo de análisis", "CrearTipoAnalisis", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void ActualizarTipoAnalisis(TipoAnalisis ta)
        {
            try
            {
                ValidarTipoAnalisis(ta);

                if (ta.IdTipoAnalisis <= 0)
                    throw new SalcValidacionException("El ID del tipo de análisis no es válido.", "idTipoAnalisis");

                ExceptionHandler.LogInfo($"Actualizando tipo de análisis - ID: {ta.IdTipoAnalisis}, Descripción: {ta.Descripcion}", "ActualizarTipoAnalisis");

                _repo.ActualizarTipoAnalisis(ta);

                ExceptionHandler.LogInfo($"Tipo de análisis actualizado exitosamente - ID: {ta.IdTipoAnalisis}", "ActualizarTipoAnalisis");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al actualizar el tipo de análisis", "ActualizarTipoAnalisis", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void EliminarTipoAnalisis(int id)
        {
            try
            {
                if (id <= 0)
                    throw new SalcValidacionException("El ID del tipo de análisis no es válido.", "id");

                ExceptionHandler.LogInfo($"Eliminando tipo de análisis (baja lógica) - ID: {id}", "EliminarTipoAnalisis");

                _repo.EliminarTipoAnalisis(id);

                ExceptionHandler.LogInfo($"Tipo de análisis eliminado exitosamente - ID: {id}", "EliminarTipoAnalisis");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al eliminar el tipo de análisis", "EliminarTipoAnalisis", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        #endregion

        #region Métricas

        public void CrearMetrica(Metrica m)
        {
            try
            {
                ValidarMetrica(m);

                ExceptionHandler.LogInfo($"Creando métrica - Nombre: {m.Nombre}, Unidad: {m.UnidadMedida}", "CrearMetrica");

                _repo.CrearMetrica(m);

                ExceptionHandler.LogInfo($"Métrica creada exitosamente - Nombre: {m.Nombre}", "CrearMetrica");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al crear la métrica", "CrearMetrica", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void ActualizarMetrica(Metrica m)
        {
            try
            {
                ValidarMetrica(m);

                if (m.IdMetrica <= 0)
                    throw new SalcValidacionException("El ID de la métrica no es válido.", "idMetrica");

                ExceptionHandler.LogInfo($"Actualizando métrica - ID: {m.IdMetrica}, Nombre: {m.Nombre}", "ActualizarMetrica");

                _repo.ActualizarMetrica(m);

                ExceptionHandler.LogInfo($"Métrica actualizada exitosamente - ID: {m.IdMetrica}", "ActualizarMetrica");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al actualizar la métrica", "ActualizarMetrica", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void EliminarMetrica(int id)
        {
            try
            {
                if (id <= 0)
                    throw new SalcValidacionException("El ID de la métrica no es válido.", "id");

                ExceptionHandler.LogInfo($"Eliminando métrica (baja lógica) - ID: {id}", "EliminarMetrica");

                _repo.EliminarMetrica(id);

                ExceptionHandler.LogInfo($"Métrica eliminada exitosamente - ID: {id}", "EliminarMetrica");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al eliminar la métrica", "EliminarMetrica", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        #endregion

        #region Relaciones Tipo Análisis - Métricas

        public IEnumerable<TipoAnalisisMetrica> ObtenerRelacionesTipoAnalisisMetricas()
        {
            try
            {
                return _tipoAnalisisMetricaRepo.ObtenerRelaciones();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener relaciones tipo análisis-métricas", "ObtenerRelacionesTipoAnalisisMetricas", sqlEx);
            }
        }

        public IEnumerable<Metrica> ObtenerMetricasPorTipoAnalisis(int idTipoAnalisis)
        {
            try
            {
                if (idTipoAnalisis <= 0)
                    throw new SalcValidacionException("El ID del tipo de análisis no es válido.", "idTipoAnalisis");

                return _tipoAnalisisMetricaRepo.ObtenerMetricasPorTipoAnalisis(idTipoAnalisis);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener métricas del tipo de análisis", "ObtenerMetricasPorTipoAnalisis", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void CrearRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica)
        {
            try
            {
                if (idTipoAnalisis <= 0)
                    throw new SalcValidacionException("El ID del tipo de análisis no es válido.", "idTipoAnalisis");

                if (idMetrica <= 0)
                    throw new SalcValidacionException("El ID de la métrica no es válido.", "idMetrica");

                if (!ExisteRelacionTipoAnalisisMetrica(idTipoAnalisis, idMetrica))
                {
                    ExceptionHandler.LogInfo($"Creando relación tipo análisis-métrica - TipoAnalisis: {idTipoAnalisis}, Métrica: {idMetrica}", "CrearRelacionTipoAnalisisMetrica");

                    _tipoAnalisisMetricaRepo.CrearRelacion(idTipoAnalisis, idMetrica);

                    ExceptionHandler.LogInfo($"Relación creada exitosamente", "CrearRelacionTipoAnalisisMetrica");
                }
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al crear la relación tipo análisis-métrica", "CrearRelacionTipoAnalisisMetrica", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void EliminarRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica)
        {
            try
            {
                if (idTipoAnalisis <= 0)
                    throw new SalcValidacionException("El ID del tipo de análisis no es válido.", "idTipoAnalisis");

                if (idMetrica <= 0)
                    throw new SalcValidacionException("El ID de la métrica no es válido.", "idMetrica");

                ExceptionHandler.LogInfo($"Eliminando relación tipo análisis-métrica - TipoAnalisis: {idTipoAnalisis}, Métrica: {idMetrica}", "EliminarRelacionTipoAnalisisMetrica");

                _tipoAnalisisMetricaRepo.EliminarRelacion(idTipoAnalisis, idMetrica);

                ExceptionHandler.LogInfo($"Relación eliminada exitosamente", "EliminarRelacionTipoAnalisisMetrica");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al eliminar la relación tipo análisis-métrica", "EliminarRelacionTipoAnalisisMetrica", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void ActualizarRelacionesTipoAnalisis(int idTipoAnalisis, IEnumerable<int> idsMetricas)
        {
            try
            {
                if (idTipoAnalisis <= 0)
                    throw new SalcValidacionException("El ID del tipo de análisis no es válido.", "idTipoAnalisis");

                if (idsMetricas == null)
                    throw new SalcValidacionException("La lista de métricas no puede ser nula.", "idsMetricas");

                ExceptionHandler.LogInfo($"Actualizando relaciones de tipo de análisis - ID: {idTipoAnalisis}, Cantidad de métricas: {idsMetricas.Count()}", "ActualizarRelacionesTipoAnalisis");

                // Eliminar todas las relaciones existentes del tipo de análisis
                _tipoAnalisisMetricaRepo.EliminarTodasLasRelacionesDeTipoAnalisis(idTipoAnalisis);

                // Crear las nuevas relaciones
                foreach (var idMetrica in idsMetricas)
                {
                    if (idMetrica > 0)
                    {
                        _tipoAnalisisMetricaRepo.CrearRelacion(idTipoAnalisis, idMetrica);
                    }
                }

                ExceptionHandler.LogInfo($"Relaciones actualizadas exitosamente - ID: {idTipoAnalisis}", "ActualizarRelacionesTipoAnalisis");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al actualizar las relaciones del tipo de análisis", "ActualizarRelacionesTipoAnalisis", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public bool ExisteRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica)
        {
            try
            {
                return _tipoAnalisisMetricaRepo.ExisteRelacion(idTipoAnalisis, idMetrica);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al verificar la relación tipo análisis-métrica", "ExisteRelacionTipoAnalisisMetrica", sqlEx);
            }
        }

        #endregion

        #region Validaciones

        private void ValidarObraSocial(ObraSocial os)
        {
            if (os == null)
                throw new SalcValidacionException("Los datos de la obra social son obligatorios.", "obraSocial");

            if (string.IsNullOrWhiteSpace(os.Cuit))
                throw new SalcValidacionException("El CUIT de la obra social es obligatorio.", "cuit");

            if (os.Cuit.Length < 10)
                throw new SalcValidacionException("El CUIT debe tener al menos 10 caracteres.", "cuit");

            if (string.IsNullOrWhiteSpace(os.Nombre))
                throw new SalcValidacionException("El nombre de la obra social es obligatorio.", "nombre");
        }

        private void ValidarTipoAnalisis(TipoAnalisis ta)
        {
            if (ta == null)
                throw new SalcValidacionException("Los datos del tipo de análisis son obligatorios.", "tipoAnalisis");

            if (string.IsNullOrWhiteSpace(ta.Descripcion))
                throw new SalcValidacionException("La descripción del tipo de análisis es obligatoria.", "descripcion");
        }

        private void ValidarMetrica(Metrica m)
        {
            if (m == null)
                throw new SalcValidacionException("Los datos de la métrica son obligatorios.", "metrica");

            if (string.IsNullOrWhiteSpace(m.Nombre))
                throw new SalcValidacionException("El nombre de la métrica es obligatorio.", "nombre");

            if (string.IsNullOrWhiteSpace(m.UnidadMedida))
                throw new SalcValidacionException("La unidad de medida de la métrica es obligatoria.", "unidadMedida");

            if (m.ValorMinimo.HasValue && m.ValorMaximo.HasValue && m.ValorMinimo > m.ValorMaximo)
                throw new SalcValidacionException("El valor mínimo no puede ser mayor que el valor máximo.", "valorMinimo");
        }

        #endregion
    }
}

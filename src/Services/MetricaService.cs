using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Models;
using SALC.DataAccess.Repositories;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gestión de métricas según ERS v2.7
    /// Implementa la lógica de negocio para RF-04: ABM de Catálogos (métricas)
    /// </summary>
    public class MetricaService
    {
        private readonly MetricaRepository _metricaRepository;

        public MetricaService()
        {
            _metricaRepository = new MetricaRepository();
        }

        #region Operaciones de Consulta

        /// <summary>
        /// Obtiene todas las métricas
        /// </summary>
        public List<Metrica> ObtenerMetricas()
        {
            try
            {
                return _metricaRepository.ObtenerTodas();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener métricas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene una métrica por ID
        /// </summary>
        public Metrica ObtenerMetrica(int idMetrica)
        {
            try
            {
                return _metricaRepository.ObtenerPorId(idMetrica);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener métrica con ID {idMetrica}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Busca métricas por nombre
        /// </summary>
        public List<Metrica> BuscarMetricas(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return ObtenerMetricas();

                return _metricaRepository.BuscarPorNombre(nombre);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al buscar métricas: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Creación

        /// <summary>
        /// Crea una nueva métrica
        /// </summary>
        public bool CrearMetrica(Metrica metrica)
        {
            try
            {
                // Validaciones de negocio
                ValidarMetricaParaCreacion(metrica);

                // Crear en base de datos
                return _metricaRepository.Crear(metrica);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al crear métrica: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Actualización

        /// <summary>
        /// Actualiza una métrica existente
        /// </summary>
        public bool ActualizarMetrica(Metrica metrica)
        {
            try
            {
                // Validaciones de negocio
                ValidarMetricaParaActualizacion(metrica);

                // Actualizar en base de datos
                return _metricaRepository.Actualizar(metrica);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar métrica: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Eliminación

        /// <summary>
        /// Elimina una métrica (solo si no tiene análisis asociados)
        /// </summary>
        public bool EliminarMetrica(int idMetrica)
        {
            try
            {
                // Verificar que la métrica existe
                var metrica = _metricaRepository.ObtenerPorId(idMetrica);
                if (metrica == null)
                    throw new ArgumentException("Métrica no encontrada");

                // Verificar que no tenga análisis asociados
                if (_metricaRepository.TieneAnalisisAsociados(idMetrica))
                    throw new InvalidOperationException("No se puede eliminar una métrica que tiene análisis asociados");

                return _metricaRepository.Eliminar(idMetrica);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar métrica: {ex.Message}", ex);
            }
        }

        #endregion

        #region Métodos de Validación Privados

        private void ValidarMetricaParaCreacion(Metrica metrica)
        {
            if (metrica == null)
                throw new ArgumentNullException(nameof(metrica));

            if (!metrica.EsValida())
                throw new ArgumentException("Los datos de la métrica son inválidos");

            // Verificar que no existe otra métrica con el mismo nombre
            if (_metricaRepository.ExistePorNombre(metrica.Nombre))
                throw new ArgumentException($"Ya existe una métrica con el nombre '{metrica.Nombre}'");

            // Validar rangos de valores
            if (metrica.ValorMinimo.HasValue && metrica.ValorMaximo.HasValue)
            {
                if (metrica.ValorMinimo.Value >= metrica.ValorMaximo.Value)
                    throw new ArgumentException("El valor mínimo debe ser menor al valor máximo");
            }

            // Validar que los valores sean positivos o cero
            if (metrica.ValorMinimo.HasValue && metrica.ValorMinimo.Value < 0)
                throw new ArgumentException("El valor mínimo no puede ser negativo");

            if (metrica.ValorMaximo.HasValue && metrica.ValorMaximo.Value < 0)
                throw new ArgumentException("El valor máximo no puede ser negativo");
        }

        private void ValidarMetricaParaActualizacion(Metrica metrica)
        {
            if (metrica == null)
                throw new ArgumentNullException(nameof(metrica));

            if (!metrica.EsValida())
                throw new ArgumentException("Los datos de la métrica son inválidos");

            // Verificar que la métrica existe
            if (!_metricaRepository.ExistePorId(metrica.IdMetrica))
                throw new ArgumentException($"No existe una métrica con ID {metrica.IdMetrica}");

            // Verificar que no existe otra métrica con el mismo nombre
            if (_metricaRepository.ExisteNombreEnOtraMetrica(metrica.Nombre, metrica.IdMetrica))
                throw new ArgumentException($"Ya existe otra métrica con el nombre '{metrica.Nombre}'");

            // Validar rangos de valores (mismas validaciones que creación)
            if (metrica.ValorMinimo.HasValue && metrica.ValorMaximo.HasValue)
            {
                if (metrica.ValorMinimo.Value >= metrica.ValorMaximo.Value)
                    throw new ArgumentException("El valor mínimo debe ser menor al valor máximo");
            }

            if (metrica.ValorMinimo.HasValue && metrica.ValorMinimo.Value < 0)
                throw new ArgumentException("El valor mínimo no puede ser negativo");

            if (metrica.ValorMaximo.HasValue && metrica.ValorMaximo.Value < 0)
                throw new ArgumentException("El valor máximo no puede ser negativo");
        }

        #endregion

        #region Métodos de Análisis y Estadísticas

        /// <summary>
        /// Obtiene métricas por unidad de medida
        /// </summary>
        public List<Metrica> ObtenerMetricasPorUnidad(string unidadMedida)
        {
            try
            {
                var todasMetricas = ObtenerMetricas();
                return todasMetricas.Where(m => string.Equals(m.UnidadMedida, unidadMedida, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener métricas por unidad: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene métricas que tienen rangos de referencia definidos
        /// </summary>
        public List<Metrica> ObtenerMetricasConRangos()
        {
            try
            {
                var todasMetricas = ObtenerMetricas();
                return todasMetricas.Where(m => m.TieneRangoReferencia).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener métricas con rangos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si una métrica puede ser eliminada
        /// </summary>
        public bool PuedeEliminarMetrica(int idMetrica)
        {
            try
            {
                return !_metricaRepository.TieneAnalisisAsociados(idMetrica);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al verificar si la métrica puede eliminarse: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
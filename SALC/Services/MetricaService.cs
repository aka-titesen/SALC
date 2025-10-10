using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Models;
using SALC.DataAccess.Repositories;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gesti�n de m�tricas seg�n ERS v2.7
    /// Implementa la l�gica de negocio para RF-04: ABM de Cat�logos (m�tricas)
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
        /// Obtiene todas las m�tricas
        /// </summary>
        public List<Metrica> ObtenerMetricas()
        {
            try
            {
                return _metricaRepository.ObtenerTodas();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener m�tricas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene una m�trica por ID
        /// </summary>
        public Metrica ObtenerMetrica(int idMetrica)
        {
            try
            {
                return _metricaRepository.ObtenerPorId(idMetrica);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener m�trica con ID {idMetrica}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Busca m�tricas por nombre
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
                throw new InvalidOperationException($"Error al buscar m�tricas: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Creaci�n

        /// <summary>
        /// Crea una nueva m�trica
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
                throw new InvalidOperationException($"Error al crear m�trica: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Actualizaci�n

        /// <summary>
        /// Actualiza una m�trica existente
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
                throw new InvalidOperationException($"Error al actualizar m�trica: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Eliminaci�n

        /// <summary>
        /// Elimina una m�trica (solo si no tiene an�lisis asociados)
        /// </summary>
        public bool EliminarMetrica(int idMetrica)
        {
            try
            {
                // Verificar que la m�trica existe
                var metrica = _metricaRepository.ObtenerPorId(idMetrica);
                if (metrica == null)
                    throw new ArgumentException("M�trica no encontrada");

                // Verificar que no tenga an�lisis asociados
                if (_metricaRepository.TieneAnalisisAsociados(idMetrica))
                    throw new InvalidOperationException("No se puede eliminar una m�trica que tiene an�lisis asociados");

                return _metricaRepository.Eliminar(idMetrica);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar m�trica: {ex.Message}", ex);
            }
        }

        #endregion

        #region M�todos de Validaci�n Privados

        private void ValidarMetricaParaCreacion(Metrica metrica)
        {
            if (metrica == null)
                throw new ArgumentNullException(nameof(metrica));

            if (!metrica.EsValida())
                throw new ArgumentException("Los datos de la m�trica son inv�lidos");

            // Verificar que no existe otra m�trica con el mismo nombre
            if (_metricaRepository.ExistePorNombre(metrica.Nombre))
                throw new ArgumentException($"Ya existe una m�trica con el nombre '{metrica.Nombre}'");

            // Validar rangos de valores
            if (metrica.ValorMinimo.HasValue && metrica.ValorMaximo.HasValue)
            {
                if (metrica.ValorMinimo.Value >= metrica.ValorMaximo.Value)
                    throw new ArgumentException("El valor m�nimo debe ser menor al valor m�ximo");
            }

            // Validar que los valores sean positivos o cero
            if (metrica.ValorMinimo.HasValue && metrica.ValorMinimo.Value < 0)
                throw new ArgumentException("El valor m�nimo no puede ser negativo");

            if (metrica.ValorMaximo.HasValue && metrica.ValorMaximo.Value < 0)
                throw new ArgumentException("El valor m�ximo no puede ser negativo");
        }

        private void ValidarMetricaParaActualizacion(Metrica metrica)
        {
            if (metrica == null)
                throw new ArgumentNullException(nameof(metrica));

            if (!metrica.EsValida())
                throw new ArgumentException("Los datos de la m�trica son inv�lidos");

            // Verificar que la m�trica existe
            if (!_metricaRepository.ExistePorId(metrica.IdMetrica))
                throw new ArgumentException($"No existe una m�trica con ID {metrica.IdMetrica}");

            // Verificar que no existe otra m�trica con el mismo nombre
            if (_metricaRepository.ExisteNombreEnOtraMetrica(metrica.Nombre, metrica.IdMetrica))
                throw new ArgumentException($"Ya existe otra m�trica con el nombre '{metrica.Nombre}'");

            // Validar rangos de valores (mismas validaciones que creaci�n)
            if (metrica.ValorMinimo.HasValue && metrica.ValorMaximo.HasValue)
            {
                if (metrica.ValorMinimo.Value >= metrica.ValorMaximo.Value)
                    throw new ArgumentException("El valor m�nimo debe ser menor al valor m�ximo");
            }

            if (metrica.ValorMinimo.HasValue && metrica.ValorMinimo.Value < 0)
                throw new ArgumentException("El valor m�nimo no puede ser negativo");

            if (metrica.ValorMaximo.HasValue && metrica.ValorMaximo.Value < 0)
                throw new ArgumentException("El valor m�ximo no puede ser negativo");
        }

        #endregion

        #region M�todos de An�lisis y Estad�sticas

        /// <summary>
        /// Obtiene m�tricas por unidad de medida
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
                throw new InvalidOperationException($"Error al obtener m�tricas por unidad: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene m�tricas que tienen rangos de referencia definidos
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
                throw new InvalidOperationException($"Error al obtener m�tricas con rangos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si una m�trica puede ser eliminada
        /// </summary>
        public bool PuedeEliminarMetrica(int idMetrica)
        {
            try
            {
                return !_metricaRepository.TieneAnalisisAsociados(idMetrica);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al verificar si la m�trica puede eliminarse: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Models;
using SALC.DataAccess.Repositories;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gestión de análisis según ERS v2.7
    /// Implementa la lógica de negocio para RF-05, RF-06, RF-07: Crear, Cargar y Validar Análisis
    /// </summary>
    public class AnalisisService
    {
        private readonly AnalisisRepository _analisisRepository;
        private readonly TipoAnalisisRepository _tipoAnalisisRepository;
        private readonly PacienteRepository _pacienteRepository;
        private readonly MetricaRepository _metricaRepository;

        public AnalisisService()
        {
            _analisisRepository = new AnalisisRepository();
            _tipoAnalisisRepository = new TipoAnalisisRepository();
            _pacienteRepository = new PacienteRepository();
            _metricaRepository = new MetricaRepository();
        }

        #region Operaciones de Consulta

        /// <summary>
        /// Obtiene todos los análisis con filtros opcionales
        /// </summary>
        public List<Analisis> ObtenerAnalisis(int? dniPaciente = null, int? dniMedico = null, int? idEstado = null)
        {
            try
            {
                return _analisisRepository.ObtenerTodos(dniPaciente, dniMedico, idEstado);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener análisis: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un análisis específico por ID
        /// </summary>
        public Analisis ObtenerAnalisis(int idAnalisis)
        {
            try
            {
                return _analisisRepository.ObtenerPorId(idAnalisis);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener análisis con ID {idAnalisis}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de análisis disponibles
        /// </summary>
        public List<TipoAnalisis> ObtenerTiposAnalisis()
        {
            try
            {
                return _tipoAnalisisRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener tipos de análisis: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene análisis por médico (RF-09)
        /// </summary>
        public List<Analisis> ObtenerAnalisisPorMedico(int dniMedico)
        {
            try
            {
                return _analisisRepository.ObtenerPorMedicoCarga(dniMedico);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener análisis del médico {dniMedico}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene análisis pendientes de verificación
        /// </summary>
        public List<Analisis> ObtenerAnalisisPendientes()
        {
            try
            {
                return _analisisRepository.ObtenerPendientesVerificacion();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener análisis pendientes: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Creación (RF-05)

        /// <summary>
        /// Crea un nuevo análisis (RF-05: Crear Análisis)
        /// </summary>
        public bool CrearAnalisis(Analisis analisis)
        {
            try
            {
                // Validaciones de negocio
                ValidarAnalisisParaCreacion(analisis);

                // Establecer valores por defecto
                analisis.IdEstado = 1; // Sin verificar
                analisis.FechaCreacion = DateTime.Now;
                analisis.DniFirma = null;
                analisis.FechaFirma = null;

                // Crear en base de datos
                return _analisisRepository.Crear(analisis);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al crear análisis: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Carga de Resultados (RF-06)

        /// <summary>
        /// Carga resultados de métricas en un análisis (RF-06: Cargar Resultados)
        /// </summary>
        public bool CargarResultados(int idAnalisis, List<AnalisisMetrica> resultados)
        {
            try
            {
                // Validar que el análisis existe y está en estado "Sin verificar"
                var analisis = _analisisRepository.ObtenerPorId(idAnalisis);
                if (analisis == null)
                    throw new ArgumentException("Análisis no encontrado");

                if (analisis.IdEstado != 1) // No está en estado "Sin verificar"
                    throw new InvalidOperationException("Solo se pueden cargar resultados en análisis sin verificar");

                // Validar resultados
                ValidarResultados(resultados);

                // Cargar resultados en base de datos
                return _analisisRepository.CargarResultados(idAnalisis, resultados);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al cargar resultados: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un resultado específico de una métrica
        /// </summary>
        public bool ActualizarResultado(int idAnalisis, int idMetrica, decimal nuevoValor, string observaciones = null)
        {
            try
            {
                // Validar que el análisis está en estado "Sin verificar"
                var analisis = _analisisRepository.ObtenerPorId(idAnalisis);
                if (analisis?.IdEstado != 1)
                    throw new InvalidOperationException("Solo se pueden actualizar resultados en análisis sin verificar");

                return _analisisRepository.ActualizarResultado(idAnalisis, idMetrica, nuevoValor, observaciones);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar resultado: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Validación (RF-07)

        /// <summary>
        /// Valida un análisis (RF-07: Validar Análisis)
        /// </summary>
        public bool ValidarAnalisis(int idAnalisis, int dniMedicoFirma)
        {
            try
            {
                // Validar que el análisis existe
                var analisis = _analisisRepository.ObtenerPorId(idAnalisis);
                if (analisis == null)
                    throw new ArgumentException("Análisis no encontrado");

                // Validar que está en estado "Sin verificar"
                if (analisis.IdEstado != 1)
                    throw new InvalidOperationException("El análisis ya fue verificado");

                // Validar que tiene resultados cargados
                var resultados = _analisisRepository.ObtenerResultados(idAnalisis);
                if (resultados == null || !resultados.Any())
                    throw new InvalidOperationException("No se puede validar un análisis sin resultados");

                // Validar que el médico existe y está activo
                // (Esta validación debería hacerse en una capa superior)

                // Actualizar estado, firma y fecha
                return _analisisRepository.ValidarAnalisis(idAnalisis, dniMedicoFirma, DateTime.Now);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al validar análisis: {ex.Message}", ex);
            }
        }

        #endregion

        #region Métodos de Validación Privados

        private void ValidarAnalisisParaCreacion(Analisis analisis)
        {
            if (analisis == null)
                throw new ArgumentNullException(nameof(analisis));

            if (!analisis.EsValido())
                throw new ArgumentException("Los datos del análisis son inválidos");

            // Verificar que el paciente existe
            if (!_pacienteRepository.ExistePorDni(analisis.DniPaciente))
                throw new ArgumentException("El paciente especificado no existe");

            // Verificar que el tipo de análisis existe
            var tipoAnalisis = _tipoAnalisisRepository.ObtenerPorId(analisis.IdTipoAnalisis);
            if (tipoAnalisis == null)
                throw new ArgumentException("El tipo de análisis especificado no existe");

            // Verificar que el médico de carga existe y es médico
            // (Esta validación se haría con UsuarioRepository en una implementación completa)

            // Verificar que no exista otro análisis igual pendiente para el mismo paciente
            var analisisPendiente = _analisisRepository.ObtenerAnalisisPendientePaciente(
                analisis.DniPaciente, analisis.IdTipoAnalisis);
            if (analisisPendiente != null)
                throw new InvalidOperationException(
                    "Ya existe un análisis pendiente del mismo tipo para este paciente");
        }

        private void ValidarResultados(List<AnalisisMetrica> resultados)
        {
            if (resultados == null || !resultados.Any())
                throw new ArgumentException("Debe proporcionar al menos un resultado");

            foreach (var resultado in resultados)
            {
                if (!resultado.EsValido())
                    throw new ArgumentException($"Resultado inválido para métrica ID {resultado.IdMetrica}");

                // Verificar que la métrica existe
                var metrica = _metricaRepository.ObtenerPorId(resultado.IdMetrica);
                if (metrica == null)
                    throw new ArgumentException($"La métrica con ID {resultado.IdMetrica} no existe");

                // Validar que el resultado esté en un rango razonable (opcional)
                if (resultado.Resultado < 0)
                    throw new ArgumentException($"El resultado para {metrica.Nombre} no puede ser negativo");
            }

            // Verificar que no haya métricas duplicadas
            var metricasDuplicadas = resultados.GroupBy(r => r.IdMetrica)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (metricasDuplicadas.Any())
                throw new ArgumentException("No se pueden cargar resultados duplicados para la misma métrica");
        }

        #endregion

        #region Métodos de Consulta Específicos

        /// <summary>
        /// Obtiene el historial de análisis de un paciente
        /// </summary>
        public List<Analisis> ObtenerHistorialPaciente(int dniPaciente)
        {
            try
            {
                return _analisisRepository.ObtenerPorPaciente(dniPaciente);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener historial del paciente {dniPaciente}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene análisis verificados (para informes)
        /// </summary>
        public List<Analisis> ObtenerAnalisisVerificados()
        {
            try
            {
                return _analisisRepository.ObtenerVerificados();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener análisis verificados: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si un análisis puede ser editado por un médico específico
        /// </summary>
        public bool PuedeMedicoEditarAnalisis(int idAnalisis, int dniMedico)
        {
            try
            {
                var analisis = _analisisRepository.ObtenerPorId(idAnalisis);
                return analisis != null && 
                       analisis.DniCarga == dniMedico && 
                       analisis.IdEstado == 1; // Sin verificar
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al verificar permisos de edición: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
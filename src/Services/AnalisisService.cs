using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Models;
using SALC.DataAccess.Repositories;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gesti�n de an�lisis seg�n ERS v2.7
    /// Implementa la l�gica de negocio para RF-05, RF-06, RF-07: Crear, Cargar y Validar An�lisis
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
        /// Obtiene todos los an�lisis con filtros opcionales
        /// </summary>
        public List<Analisis> ObtenerAnalisis(int? dniPaciente = null, int? dniMedico = null, int? idEstado = null)
        {
            try
            {
                return _analisisRepository.ObtenerTodos(dniPaciente, dniMedico, idEstado);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener an�lisis: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un an�lisis espec�fico por ID
        /// </summary>
        public Analisis ObtenerAnalisis(int idAnalisis)
        {
            try
            {
                return _analisisRepository.ObtenerPorId(idAnalisis);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener an�lisis con ID {idAnalisis}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de an�lisis disponibles
        /// </summary>
        public List<TipoAnalisis> ObtenerTiposAnalisis()
        {
            try
            {
                return _tipoAnalisisRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener tipos de an�lisis: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene an�lisis por m�dico (RF-09)
        /// </summary>
        public List<Analisis> ObtenerAnalisisPorMedico(int dniMedico)
        {
            try
            {
                return _analisisRepository.ObtenerPorMedicoCarga(dniMedico);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener an�lisis del m�dico {dniMedico}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene an�lisis pendientes de verificaci�n
        /// </summary>
        public List<Analisis> ObtenerAnalisisPendientes()
        {
            try
            {
                return _analisisRepository.ObtenerPendientesVerificacion();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener an�lisis pendientes: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Creaci�n (RF-05)

        /// <summary>
        /// Crea un nuevo an�lisis (RF-05: Crear An�lisis)
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
                throw new InvalidOperationException($"Error al crear an�lisis: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Carga de Resultados (RF-06)

        /// <summary>
        /// Carga resultados de m�tricas en un an�lisis (RF-06: Cargar Resultados)
        /// </summary>
        public bool CargarResultados(int idAnalisis, List<AnalisisMetrica> resultados)
        {
            try
            {
                // Validar que el an�lisis existe y est� en estado "Sin verificar"
                var analisis = _analisisRepository.ObtenerPorId(idAnalisis);
                if (analisis == null)
                    throw new ArgumentException("An�lisis no encontrado");

                if (analisis.IdEstado != 1) // No est� en estado "Sin verificar"
                    throw new InvalidOperationException("Solo se pueden cargar resultados en an�lisis sin verificar");

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
        /// Actualiza un resultado espec�fico de una m�trica
        /// </summary>
        public bool ActualizarResultado(int idAnalisis, int idMetrica, decimal nuevoValor, string observaciones = null)
        {
            try
            {
                // Validar que el an�lisis est� en estado "Sin verificar"
                var analisis = _analisisRepository.ObtenerPorId(idAnalisis);
                if (analisis?.IdEstado != 1)
                    throw new InvalidOperationException("Solo se pueden actualizar resultados en an�lisis sin verificar");

                return _analisisRepository.ActualizarResultado(idAnalisis, idMetrica, nuevoValor, observaciones);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar resultado: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Validaci�n (RF-07)

        /// <summary>
        /// Valida un an�lisis (RF-07: Validar An�lisis)
        /// </summary>
        public bool ValidarAnalisis(int idAnalisis, int dniMedicoFirma)
        {
            try
            {
                // Validar que el an�lisis existe
                var analisis = _analisisRepository.ObtenerPorId(idAnalisis);
                if (analisis == null)
                    throw new ArgumentException("An�lisis no encontrado");

                // Validar que est� en estado "Sin verificar"
                if (analisis.IdEstado != 1)
                    throw new InvalidOperationException("El an�lisis ya fue verificado");

                // Validar que tiene resultados cargados
                var resultados = _analisisRepository.ObtenerResultados(idAnalisis);
                if (resultados == null || !resultados.Any())
                    throw new InvalidOperationException("No se puede validar un an�lisis sin resultados");

                // Validar que el m�dico existe y est� activo
                // (Esta validaci�n deber�a hacerse en una capa superior)

                // Actualizar estado, firma y fecha
                return _analisisRepository.ValidarAnalisis(idAnalisis, dniMedicoFirma, DateTime.Now);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al validar an�lisis: {ex.Message}", ex);
            }
        }

        #endregion

        #region M�todos de Validaci�n Privados

        private void ValidarAnalisisParaCreacion(Analisis analisis)
        {
            if (analisis == null)
                throw new ArgumentNullException(nameof(analisis));

            if (!analisis.EsValido())
                throw new ArgumentException("Los datos del an�lisis son inv�lidos");

            // Verificar que el paciente existe
            if (!_pacienteRepository.ExistePorDni(analisis.DniPaciente))
                throw new ArgumentException("El paciente especificado no existe");

            // Verificar que el tipo de an�lisis existe
            var tipoAnalisis = _tipoAnalisisRepository.ObtenerPorId(analisis.IdTipoAnalisis);
            if (tipoAnalisis == null)
                throw new ArgumentException("El tipo de an�lisis especificado no existe");

            // Verificar que el m�dico de carga existe y es m�dico
            // (Esta validaci�n se har�a con UsuarioRepository en una implementaci�n completa)

            // Verificar que no exista otro an�lisis igual pendiente para el mismo paciente
            var analisisPendiente = _analisisRepository.ObtenerAnalisisPendientePaciente(
                analisis.DniPaciente, analisis.IdTipoAnalisis);
            if (analisisPendiente != null)
                throw new InvalidOperationException(
                    "Ya existe un an�lisis pendiente del mismo tipo para este paciente");
        }

        private void ValidarResultados(List<AnalisisMetrica> resultados)
        {
            if (resultados == null || !resultados.Any())
                throw new ArgumentException("Debe proporcionar al menos un resultado");

            foreach (var resultado in resultados)
            {
                if (!resultado.EsValido())
                    throw new ArgumentException($"Resultado inv�lido para m�trica ID {resultado.IdMetrica}");

                // Verificar que la m�trica existe
                var metrica = _metricaRepository.ObtenerPorId(resultado.IdMetrica);
                if (metrica == null)
                    throw new ArgumentException($"La m�trica con ID {resultado.IdMetrica} no existe");

                // Validar que el resultado est� en un rango razonable (opcional)
                if (resultado.Resultado < 0)
                    throw new ArgumentException($"El resultado para {metrica.Nombre} no puede ser negativo");
            }

            // Verificar que no haya m�tricas duplicadas
            var metricasDuplicadas = resultados.GroupBy(r => r.IdMetrica)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (metricasDuplicadas.Any())
                throw new ArgumentException("No se pueden cargar resultados duplicados para la misma m�trica");
        }

        #endregion

        #region M�todos de Consulta Espec�ficos

        /// <summary>
        /// Obtiene el historial de an�lisis de un paciente
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
        /// Obtiene an�lisis verificados (para informes)
        /// </summary>
        public List<Analisis> ObtenerAnalisisVerificados()
        {
            try
            {
                return _analisisRepository.ObtenerVerificados();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener an�lisis verificados: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si un an�lisis puede ser editado por un m�dico espec�fico
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
                throw new InvalidOperationException($"Error al verificar permisos de edici�n: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
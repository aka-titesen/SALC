using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.DAL;
using SALC.Infraestructura;
using SALC.Infraestructura.Exceptions;

namespace SALC.BLL
{
    /// <summary>
    /// Servicio de lógica de negocio para la gestión de análisis clínicos.
    /// Implementa las reglas de negocio y coordina las operaciones con el repositorio.
    /// </summary>
    public class AnalisisService : IAnalisisService
    {
        private readonly AnalisisRepositorio _analisisRepo = new AnalisisRepositorio();
        private readonly AnalisisMetricaRepositorio _amRepo = new AnalisisMetricaRepositorio();
        private readonly CatalogoRepositorio _catRepo = new CatalogoRepositorio();
        
        private const int EstadoSinVerificar = 1;
        private const int EstadoVerificado = 2;
        private const int EstadoAnulado = 3;

        /// <summary>
        /// Crea un nuevo análisis clínico
        /// </summary>
        /// <param name="dniPaciente">DNI del paciente al que se realiza el análisis</param>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <param name="dniMedicoCarga">DNI del médico que crea el análisis</param>
        /// <param name="observaciones">Observaciones adicionales sobre el análisis</param>
        /// <returns>El análisis creado con su identificador asignado</returns>
        public Analisis CrearAnalisis(int dniPaciente, int idTipoAnalisis, int dniMedicoCarga, string observaciones)
        {
            try
            {
                // Validaciones de negocio
                if (dniPaciente <= 0)
                    throw new SalcValidacionException("El DNI del paciente debe ser válido.", "dniPaciente");
                
                if (idTipoAnalisis <= 0)
                    throw new SalcValidacionException("Debe seleccionar un tipo de análisis válido.", "idTipoAnalisis");
                
                if (dniMedicoCarga <= 0)
                    throw new SalcValidacionException("El DNI del médico no es válido.", "dniMedicoCarga");

                ExceptionHandler.LogInfo($"Creando análisis - Paciente: {dniPaciente}, Tipo: {idTipoAnalisis}, Médico: {dniMedicoCarga}", "CrearAnalisis");

                var a = new Analisis
                {
                    DniPaciente = dniPaciente,
                    IdTipoAnalisis = idTipoAnalisis,
                    DniCarga = dniMedicoCarga,
                    IdEstado = EstadoSinVerificar,
                    DniFirma = null,
                    FechaCreacion = DateTime.Now,
                    FechaFirma = null,
                    Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
                };

                var analisisCreado = _analisisRepo.CrearYDevolver(a);
                
                ExceptionHandler.LogInfo($"Análisis creado exitosamente - ID: {analisisCreado.IdAnalisis}", "CrearAnalisis");
                
                return analisisCreado;
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al crear el análisis en la base de datos", "CrearAnalisis", sqlEx);
            }
            catch (SalcException)
            {
                throw; // Re-lanzar excepciones SALC
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogWarning($"Error inesperado al crear análisis: {ex.Message}", "CrearAnalisis");
                throw new SalcException(
                    "Error al crear el análisis",
                    "No se pudo crear el análisis. Por favor, intente nuevamente.",
                    "CREATE_ANALISIS_ERROR"
                );
            }
        }

        /// <summary>
        /// Carga o actualiza el resultado de una métrica en un análisis
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <param name="idMetrica">Identificador de la métrica</param>
        /// <param name="resultado">Valor del resultado obtenido</param>
        /// <param name="observaciones">Observaciones sobre este resultado específico</param>
        public void CargarResultado(int idAnalisis, int idMetrica, decimal resultado, string observaciones = null)
        {
            try
            {
                // Validaciones
                if (idAnalisis <= 0)
                    throw new SalcValidacionException("El ID del análisis no es válido.", "idAnalisis");
                
                if (idMetrica <= 0)
                    throw new SalcValidacionException("El ID de la métrica no es válido.", "idMetrica");

                var a = _analisisRepo.ObtenerPorId(idAnalisis);
                if (a == null) 
                    throw new SalcBusinessException($"No se encontró el análisis con ID {idAnalisis}.");
                
                if (a.IdEstado != EstadoSinVerificar) 
                    throw new SalcBusinessException("No se pueden modificar resultados en un análisis verificado o anulado.");

                ExceptionHandler.LogInfo($"Cargando resultado - Análisis: {idAnalisis}, Métrica: {idMetrica}, Valor: {resultado}", "CargarResultado");

                _amRepo.UpsertResultado(new AnalisisMetrica
                {
                    IdAnalisis = idAnalisis,
                    IdMetrica = idMetrica,
                    Resultado = resultado,
                    Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
                });

                ExceptionHandler.LogInfo($"Resultado cargado exitosamente - Análisis: {idAnalisis}, Métrica: {idMetrica}", "CargarResultado");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al cargar el resultado en la base de datos", "CargarResultado", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogWarning($"Error inesperado al cargar resultado: {ex.Message}", "CargarResultado");
                throw new SalcException(
                    "Error al cargar el resultado",
                    "No se pudo guardar el resultado. Por favor, intente nuevamente.",
                    "CARGAR_RESULTADO_ERROR"
                );
            }
        }

        /// <summary>
        /// Obtiene todos los análisis creados por un médico específico
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <returns>Colección de análisis del médico</returns>
        public IEnumerable<Analisis> ObtenerAnalisisPorMedicoCarga(int dniMedico)
        {
            try
            {
                if (dniMedico <= 0)
                    throw new SalcValidacionException("El DNI del médico no es válido.", "dniMedico");

                return _analisisRepo.ObtenerPorMedicoCarga(dniMedico);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener análisis del médico", "ObtenerAnalisisPorMedicoCarga", sqlEx);
            }
        }

        /// <summary>
        /// Obtiene los análisis activos (no anulados) creados por un médico específico
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <returns>Colección de análisis activos del médico</returns>
        public IEnumerable<Analisis> ObtenerAnalisisActivosPorMedicoCarga(int dniMedico)
        {
            try
            {
                if (dniMedico <= 0)
                    throw new SalcValidacionException("El DNI del médico no es válido.", "dniMedico");

                return _analisisRepo.ObtenerActivosPorMedicoCarga(dniMedico);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener análisis activos del médico", "ObtenerAnalisisActivosPorMedicoCarga", sqlEx);
            }
        }

        /// <summary>
        /// Obtiene todos los análisis de un paciente específico
        /// </summary>
        /// <param name="dniPaciente">DNI del paciente</param>
        /// <returns>Colección de análisis del paciente</returns>
        public IEnumerable<Analisis> ObtenerAnalisisPorPaciente(int dniPaciente)
        {
            try
            {
                if (dniPaciente <= 0)
                    throw new SalcValidacionException("El DNI del paciente no es válido.", "dniPaciente");

                return _analisisRepo.ObtenerPorPaciente(dniPaciente);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener análisis del paciente", "ObtenerAnalisisPorPaciente", sqlEx);
            }
        }

        /// <summary>
        /// Obtiene los análisis activos (no anulados) de un paciente específico
        /// </summary>
        /// <param name="dniPaciente">DNI del paciente</param>
        /// <returns>Colección de análisis activos del paciente</returns>
        public IEnumerable<Analisis> ObtenerAnalisisActivosPorPaciente(int dniPaciente)
        {
            try
            {
                if (dniPaciente <= 0)
                    throw new SalcValidacionException("El DNI del paciente no es válido.", "dniPaciente");

                return _analisisRepo.ObtenerActivosPorPaciente(dniPaciente);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener análisis activos del paciente", "ObtenerAnalisisActivosPorPaciente", sqlEx);
            }
        }

        /// <summary>
        /// Obtiene todos los resultados de métricas de un análisis específico
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <returns>Colección de resultados del análisis</returns>
        public IEnumerable<AnalisisMetrica> ObtenerResultados(int idAnalisis)
        {
            try
            {
                if (idAnalisis <= 0)
                    throw new SalcValidacionException("El ID del análisis no es válido.", "idAnalisis");

                return _amRepo.ObtenerResultados(idAnalisis);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener resultados del análisis", "ObtenerResultados", sqlEx);
            }
        }

        /// <summary>
        /// Valida y firma un análisis, cambiando su estado a Verificado
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis a validar</param>
        /// <param name="dniMedicoFirma">DNI del médico que valida el análisis</param>
        public void ValidarAnalisis(int idAnalisis, int dniMedicoFirma)
        {
            try
            {
                if (idAnalisis <= 0)
                    throw new SalcValidacionException("El ID del análisis no es válido.", "idAnalisis");
                
                if (dniMedicoFirma <= 0)
                    throw new SalcValidacionException("El DNI del médico no es válido.", "dniMedicoFirma");

                var a = _analisisRepo.ObtenerPorId(idAnalisis);
                if (a == null) 
                    throw new SalcBusinessException($"No se encontró el análisis con ID {idAnalisis}.");
                
                if (a.IdEstado != EstadoSinVerificar) 
                    throw new SalcBusinessException("El análisis ya fue verificado o está anulado");
                
                // Regla de negocio: solo el médico que creó (dni_carga) puede firmar
                if (a.DniCarga != dniMedicoFirma)
                    throw new SalcAuthorizationException("ValidarAnalisis", "Médico que creó el análisis");

                ExceptionHandler.LogInfo($"Validando análisis - ID: {idAnalisis}, Médico: {dniMedicoFirma}", "ValidarAnalisis");

                a.IdEstado = EstadoVerificado;
                a.DniFirma = dniMedicoFirma;
                a.FechaFirma = DateTime.Now;
                _analisisRepo.Actualizar(a);

                ExceptionHandler.LogInfo($"Análisis validado exitosamente - ID: {idAnalisis}", "ValidarAnalisis");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al validar el análisis", "ValidarAnalisis", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogWarning($"Error inesperado al validar análisis: {ex.Message}", "ValidarAnalisis");
                throw new SalcException(
                    "Error al validar el análisis",
                    "No se pudo validar el análisis. Por favor, intente nuevamente.",
                    "VALIDAR_ANALISIS_ERROR"
                );
            }
        }

        /// <summary>
        /// Anula un análisis, cambiando su estado a Anulado (baja lógica)
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis a anular</param>
        /// <param name="dniMedico">DNI del médico que anula el análisis</param>
        public void AnularAnalisis(int idAnalisis, int dniMedico)
        {
            try
            {
                if (idAnalisis <= 0)
                    throw new SalcValidacionException("El ID del análisis no es válido.", "idAnalisis");
                
                if (dniMedico <= 0)
                    throw new SalcValidacionException("El DNI del médico no es válido.", "dniMedico");

                var a = _analisisRepo.ObtenerPorId(idAnalisis);
                if (a == null) 
                    throw new SalcBusinessException($"No se encontró el análisis con ID {idAnalisis}.");
                
                if (a.IdEstado == EstadoAnulado) 
                    throw new SalcBusinessException("El análisis ya está anulado");
                
                // Solo el médico que creó el análisis puede anularlo
                if (a.DniCarga != dniMedico)
                    throw new SalcAuthorizationException("AnularAnalisis", "Médico que creó el análisis");

                ExceptionHandler.LogInfo($"Anulando análisis - ID: {idAnalisis}, Médico: {dniMedico}", "AnularAnalisis");

                a.IdEstado = EstadoAnulado;
                _analisisRepo.Actualizar(a);

                ExceptionHandler.LogInfo($"Análisis anulado exitosamente - ID: {idAnalisis}", "AnularAnalisis");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al anular el análisis", "AnularAnalisis", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogWarning($"Error inesperado al anular análisis: {ex.Message}", "AnularAnalisis");
                throw new SalcException(
                    "Error al anular el análisis",
                    "No se pudo anular el análisis. Por favor, intente nuevamente.",
                    "ANULAR_ANALISIS_ERROR"
                );
            }
        }

        /// <summary>
        /// Elimina un análisis (alias de AnularAnalisis para compatibilidad)
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis a eliminar</param>
        /// <param name="dniMedico">DNI del médico que elimina el análisis</param>
        public void EliminarAnalisis(int idAnalisis, int dniMedico)
        {
            AnularAnalisis(idAnalisis, dniMedico);
        }

        /// <summary>
        /// Obtiene un análisis específico por su identificador
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <returns>El análisis solicitado o null si no existe</returns>
        public Analisis ObtenerAnalisisPorId(int idAnalisis)
        {
            try
            {
                if (idAnalisis <= 0)
                    throw new SalcValidacionException("El ID del análisis no es válido.", "idAnalisis");

                return _analisisRepo.ObtenerPorId(idAnalisis);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener el análisis", "ObtenerAnalisisPorId", sqlEx);
            }
        }

        /// <summary>
        /// Obtiene todos los análisis activos (no anulados) del sistema
        /// </summary>
        /// <returns>Colección de análisis activos</returns>
        public IEnumerable<Analisis> ObtenerAnalisisActivos()
        {
            try
            {
                return _analisisRepo.ObtenerActivos();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener análisis activos", "ObtenerAnalisisActivos", sqlEx);
            }
        }

        /// <summary>
        /// Verifica si un análisis está anulado
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <returns>True si el análisis está anulado, false en caso contrario</returns>
        public bool EstaAnulado(int idAnalisis)
        {
            try
            {
                var analisis = _analisisRepo.ObtenerPorId(idAnalisis);
                return analisis?.IdEstado == EstadoAnulado;
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al verificar estado del análisis", "EstaAnulado", sqlEx);
            }
        }

        /// <summary>
        /// Verifica si un análisis puede ser modificado (está en estado Sin Verificar)
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis</param>
        /// <returns>True si el análisis puede ser modificado, false en caso contrario</returns>
        public bool PuedeSerModificado(int idAnalisis)
        {
            try
            {
                var analisis = _analisisRepo.ObtenerPorId(idAnalisis);
                return analisis?.IdEstado == EstadoSinVerificar;
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al verificar si el análisis puede modificarse", "PuedeSerModificado", sqlEx);
            }
        }
    }
}

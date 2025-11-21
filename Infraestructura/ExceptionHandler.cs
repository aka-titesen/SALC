using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using SALC.Infraestructura.Exceptions;
using SALC.Infraestructura.Logging;

namespace SALC.Infraestructura
{
    /// <summary>
    /// Helper centralizado para el manejo de excepciones en toda la aplicación
    /// </summary>
    public static class ExceptionHandler
    {
        private static readonly Logger _logger = Logger.Instance;

        /// <summary>
        /// Maneja una excepción de manera centralizada, mostrando mensaje al usuario y registrando en log
        /// </summary>
        /// <param name="ex">Excepción a manejar</param>
        /// <param name="contexto">Contexto donde ocurrió el error (ej: "Login", "Crear Análisis")</param>
        /// <param name="mostrarAlUsuario">Si debe mostrar mensaje al usuario</param>
        /// <returns>Mensaje amigable para el usuario</returns>
        public static string ManejarExcepcion(Exception ex, string contexto = "", bool mostrarAlUsuario = true)
        {
            string mensajeUsuario;
            string mensajeLog;

            // Determinar tipo de excepción y preparar mensajes
            if (ex is SalcException salcEx)
            {
                mensajeUsuario = salcEx.UserFriendlyMessage ?? salcEx.Message;
                mensajeLog = $"[{contexto}] {salcEx.ErrorCode}: {salcEx.Message}";
                
                if (salcEx.ShouldLog)
                {
                    _logger.Error(mensajeLog, ex);
                }
            }
            else if (ex is SqlException sqlEx)
            {
                // Convertir SqlException a SalcDatabaseException para mejor manejo
                var dbException = new SalcDatabaseException($"Error de base de datos en {contexto}", sqlEx);
                mensajeUsuario = dbException.UserFriendlyMessage;
                mensajeLog = $"[{contexto}] SQL Error {sqlEx.Number}: {sqlEx.Message}";
                _logger.Error(mensajeLog, ex);
            }
            else if (ex is UnauthorizedAccessException)
            {
                mensajeUsuario = "No tiene permisos para realizar esta operación.";
                mensajeLog = $"[{contexto}] Unauthorized Access: {ex.Message}";
                _logger.Warning(mensajeLog);
            }
            else if (ex is ArgumentException || ex is ArgumentNullException)
            {
                mensajeUsuario = "Los datos proporcionados no son válidos. Por favor, verifique la información ingresada.";
                mensajeLog = $"[{contexto}] Argument Error: {ex.Message}";
                _logger.Warning(mensajeLog);
            }
            else if (ex is InvalidOperationException)
            {
                mensajeUsuario = ex.Message; // Suelen ser mensajes de negocio claros
                mensajeLog = $"[{contexto}] Invalid Operation: {ex.Message}";
                _logger.Warning(mensajeLog);
            }
            else
            {
                // Excepción no manejada
                mensajeUsuario = "Ha ocurrido un error inesperado. Por favor, contacte al administrador del sistema.";
                mensajeLog = $"[{contexto}] Unhandled Exception: {ex.GetType().Name} - {ex.Message}";
                _logger.Error(mensajeLog, ex);
            }

            // Mostrar al usuario si se solicita
            if (mostrarAlUsuario)
            {
                MostrarMensajeError(mensajeUsuario, contexto);
            }

            return mensajeUsuario;
        }

        /// <summary>
        /// Maneja una excepción de validación
        /// </summary>
        public static void ManejarValidacion(string mensaje, string campo = null, bool mostrarAlUsuario = true)
        {
            var ex = new SalcValidacionException(mensaje, campo);
            ManejarExcepcion(ex, "Validación", mostrarAlUsuario);
        }

        /// <summary>
        /// Maneja una excepción de base de datos
        /// </summary>
        public static void ManejarErrorBaseDatos(SqlException sqlEx, string operacion = "", bool mostrarAlUsuario = true)
        {
            var ex = new SalcDatabaseException($"Error en operación: {operacion}", operacion, sqlEx);
            ManejarExcepcion(ex, operacion, mostrarAlUsuario);
        }

        /// <summary>
        /// Maneja una excepción de regla de negocio
        /// </summary>
        public static void ManejarReglaNegocio(string mensaje, bool mostrarAlUsuario = true)
        {
            var ex = new SalcBusinessException(mensaje);
            ManejarExcepcion(ex, "Regla de Negocio", mostrarAlUsuario);
        }

        /// <summary>
        /// Maneja una excepción de autorización
        /// </summary>
        public static void ManejarNoAutorizado(string operacion, string rolRequerido = null, bool mostrarAlUsuario = true)
        {
            var ex = new SalcAuthorizationException(operacion, rolRequerido);
            ManejarExcepcion(ex, "Autorización", mostrarAlUsuario);
        }

        /// <summary>
        /// Muestra un mensaje de error al usuario
        /// </summary>
        private static void MostrarMensajeError(string mensaje, string titulo = "Error")
        {
            MessageBox.Show(
                mensaje,
                $"SALC - {titulo}",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        /// <summary>
        /// Registra un mensaje informativo en el log
        /// </summary>
        public static void LogInfo(string mensaje, string contexto = "")
        {
            var mensajeLog = string.IsNullOrEmpty(contexto) ? mensaje : $"[{contexto}] {mensaje}";
            _logger.Info(mensajeLog);
        }

        /// <summary>
        /// Registra una advertencia en el log
        /// </summary>
        public static void LogWarning(string mensaje, string contexto = "")
        {
            var mensajeLog = string.IsNullOrEmpty(contexto) ? mensaje : $"[{contexto}] {mensaje}";
            _logger.Warning(mensajeLog);
        }

        /// <summary>
        /// Registra un mensaje de depuración en el log
        /// </summary>
        public static void LogDebug(string mensaje, string contexto = "")
        {
            var mensajeLog = string.IsNullOrEmpty(contexto) ? mensaje : $"[{contexto}] {mensaje}";
            _logger.Debug(mensajeLog);
        }

        /// <summary>
        /// Ejecuta una acción con manejo automático de excepciones
        /// </summary>
        public static void EjecutarConManejo(Action accion, string contexto = "")
        {
            try
            {
                accion();
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, contexto, mostrarAlUsuario: true);
            }
        }

        /// <summary>
        /// Ejecuta una función con manejo automático de excepciones
        /// </summary>
        public static T EjecutarConManejo<T>(Func<T> funcion, string contexto = "", T valorPorDefecto = default(T))
        {
            try
            {
                return funcion();
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, contexto, mostrarAlUsuario: true);
                return valorPorDefecto;
            }
        }
    }
}

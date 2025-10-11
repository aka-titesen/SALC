using System;

namespace SALC.Services
{
    /// <summary>
    /// Clase base para todos los servicios del sistema SALC.
    /// Implementa funcionalidad común de lógica de negocio.
    /// </summary>
    public abstract class BaseService : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Valida que los parámetros de entrada no sean nulos
        /// </summary>
        /// <param name="parameter">Parámetro a validar</param>
        /// <param name="parameterName">Nombre del parámetro</param>
        /// <exception cref="ArgumentNullException">Se lanza si el parámetro es nulo</exception>
        protected void ValidateNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName, $"El parámetro {parameterName} no puede ser nulo.");
            }
        }

        /// <summary>
        /// Valida que una cadena no sea nula o vacía
        /// </summary>
        /// <param name="value">Valor a validar</param>
        /// <param name="parameterName">Nombre del parámetro</param>
        /// <exception cref="ArgumentException">Se lanza si la cadena es nula o vacía</exception>
        protected void ValidateNotNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"El parámetro {parameterName} no puede ser nulo o vacío.", parameterName);
            }
        }

        /// <summary>
        /// Valida que un número entero sea mayor que cero
        /// </summary>
        /// <param name="value">Valor a validar</param>
        /// <param name="parameterName">Nombre del parámetro</param>
        /// <exception cref="ArgumentException">Se lanza si el valor no es mayor que cero</exception>
        protected void ValidatePositiveInteger(int value, string parameterName)
        {
            if (value <= 0)
            {
                throw new ArgumentException($"El parámetro {parameterName} debe ser mayor que cero.", parameterName);
            }
        }

        /// <summary>
        /// Registra información para auditoría
        /// </summary>
        /// <param name="action">Acción realizada</param>
        /// <param name="details">Detalles adicionales</param>
        protected virtual void LogAuditInfo(string action, string details = null)
        {
            // TODO: Implementar logging de auditoría
            // Por ahora solo para estructura, se puede implementar con NLog, log4net, etc.
        }

        /// <summary>
        /// Registra un error
        /// </summary>
        /// <param name="exception">Excepción ocurrida</param>
        /// <param name="context">Contexto donde ocurrió el error</param>
        protected virtual void LogError(Exception exception, string context = null)
        {
            // TODO: Implementar logging de errores
            // Por ahora solo para estructura
        }

        /// <summary>
        /// Libera los recursos utilizados
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Patrón Dispose para liberar recursos
        /// </summary>
        /// <param name="disposing">Indica si se está liberando desde Dispose</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Liberar recursos específicos del servicio
                _disposed = true;
            }
        }
    }
}

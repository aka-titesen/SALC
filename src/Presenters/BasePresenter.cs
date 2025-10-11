using System;
using System.Windows.Forms;

namespace SALC.Presenters
{
    /// <summary>
    /// Clase base para todos los presentadores del sistema SALC.
    /// Implementa el patrón MVP (Model-View-Presenter) con vista pasiva.
    /// </summary>
    /// <typeparam name="TView">Tipo de la interfaz de vista</typeparam>
    public abstract class BasePresenter<TView> : IDisposable where TView : class
    {
        protected TView _view;
        private bool _disposed = false;

        /// <summary>
        /// Constructor que inicializa el presentador con la vista
        /// </summary>
        /// <param name="view">Instancia de la vista</param>
        public BasePresenter(TView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            Initialize();
        }

        /// <summary>
        /// Vista asociada al presentador
        /// </summary>
        public TView View => _view;

        /// <summary>
        /// Método de inicialización llamado en el constructor
        /// </summary>
        protected virtual void Initialize()
        {
            // Configuración inicial común a todos los presentadores
        }

        /// <summary>
        /// Maneja errores de manera consistente en toda la aplicación
        /// </summary>
        /// <param name="exception">Excepción ocurrida</param>
        /// <param name="userMessage">Mensaje personalizado para el usuario</param>
        protected virtual void HandleError(Exception exception, string userMessage = null)
        {
            // Log del error (cuando se implemente logging)
            LogError(exception);

            // Mostrar mensaje al usuario
            string message = userMessage ?? "Ha ocurrido un error inesperado. Por favor, contacte al administrador.";
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Muestra un mensaje de información al usuario
        /// </summary>
        /// <param name="message">Mensaje a mostrar</param>
        /// <param name="title">Título del mensaje</param>
        protected virtual void ShowInfo(string message, string title = "Información")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Muestra un mensaje de confirmación al usuario
        /// </summary>
        /// <param name="message">Mensaje de confirmación</param>
        /// <param name="title">Título del mensaje</param>
        /// <returns>True si el usuario confirma, False en caso contrario</returns>
        protected virtual bool ShowConfirmation(string message, string title = "Confirmación")
        {
            var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        /// <summary>
        /// Valida los datos de entrada antes de procesarlos
        /// </summary>
        /// <returns>True si los datos son válidos</returns>
        protected virtual bool ValidateInput()
        {
            return true;
        }

        /// <summary>
        /// Registra errores para auditoría
        /// </summary>
        /// <param name="exception">Excepción a registrar</param>
        protected virtual void LogError(Exception exception)
        {
            // TODO: Implementar logging
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
                // Limpiar suscripciones a eventos si es necesario
                _view = null;
                _disposed = true;
            }
        }
    }
}

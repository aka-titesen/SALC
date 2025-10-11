using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz base para todas las vistas del sistema SALC.
    /// Define funcionalidad com�n que deben implementar todas las vistas.
    /// </summary>
    public interface IBaseView
    {
        /// <summary>
        /// Muestra un mensaje de error al usuario
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        void ShowError(string message);

        /// <summary>
        /// Muestra un mensaje de informaci�n al usuario
        /// </summary>
        /// <param name="message">Mensaje informativo</param>
        void ShowInfo(string message);

        /// <summary>
        /// Muestra un mensaje de confirmaci�n al usuario
        /// </summary>
        /// <param name="message">Mensaje de confirmaci�n</param>
        /// <returns>True si el usuario confirma, False en caso contrario</returns>
        bool ShowConfirmation(string message);

        /// <summary>
        /// Habilita o deshabilita los controles de la vista
        /// </summary>
        /// <param name="enabled">True para habilitar, False para deshabilitar</param>
        void EnableControls(bool enabled);

        /// <summary>
        /// Limpia todos los controles de entrada de la vista
        /// </summary>
        void ClearControls();

        /// <summary>
        /// Establece el foco en un control espec�fico
        /// </summary>
        /// <param name="controlName">Nombre del control</param>
        void SetFocus(string controlName = null);

        /// <summary>
        /// Valida los datos de entrada de la vista
        /// </summary>
        /// <returns>True si los datos son v�lidos</returns>
        bool ValidateInput();

        /// <summary>
        /// Cierra la vista
        /// </summary>
        void Close();
    }
}
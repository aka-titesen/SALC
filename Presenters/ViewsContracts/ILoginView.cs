using System;

namespace SALC.Presenters.ViewsContracts
{
    /// <summary>
    /// Interfaz de contrato para la vista de inicio de sesión.
    /// Define las propiedades y eventos que debe implementar la vista de login.
    /// </summary>
    public interface ILoginView
    {
        /// <summary>
        /// Obtiene el texto del DNI ingresado por el usuario
        /// </summary>
        string DniTexto { get; }

        /// <summary>
        /// Obtiene la contraseña ingresada por el usuario
        /// </summary>
        string Contrasenia { get; }

        /// <summary>
        /// Evento que se dispara cuando el usuario presiona el botón de acceder
        /// </summary>
        event EventHandler AccederClick;

        /// <summary>
        /// Muestra un mensaje de error al usuario
        /// </summary>
        /// <param name="mensaje">Mensaje de error a mostrar</param>
        void MostrarError(string mensaje);

        /// <summary>
        /// Cierra la vista de login
        /// </summary>
        void Cerrar();

        /// <summary>
        /// Limpia los campos de DNI y contraseña
        /// </summary>
        void LimpiarCampos();
    }
}

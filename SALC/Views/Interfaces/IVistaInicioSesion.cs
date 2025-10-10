using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de inicio de sesión según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para RF-01: Autenticar Usuario
    /// </summary>
    public interface IVistaInicioSesion
    {
        #region Propiedades (en español)
        string NombreUsuario { get; }
        string Contrasena { get; }
        #endregion

        #region Eventos (en español)
        event EventHandler InicioSesionSolicitado;
        #endregion

        #region Métodos de la Vista (en español)
        void MostrarMensajeValidacion(string mensaje);
        void MostrarErrorInicioSesion(string mensaje);
        void NavegarAlDashboard();
        void LimpiarFormulario();
        void HabilitarControles(bool habilitado);
        void MostrarProgreso(bool mostrar);
        #endregion
    }
}
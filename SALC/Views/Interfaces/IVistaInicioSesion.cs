using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de inicio de sesi�n seg�n ERS v2.7
    /// Implementa el patr�n MVP (Model-View-Presenter) para RF-01: Autenticar Usuario
    /// </summary>
    public interface IVistaInicioSesion
    {
        #region Propiedades (en espa�ol)
        string NombreUsuario { get; }
        string Contrasena { get; }
        #endregion

        #region Eventos (en espa�ol)
        event EventHandler InicioSesionSolicitado;
        #endregion

        #region M�todos de la Vista (en espa�ol)
        void MostrarMensajeValidacion(string mensaje);
        void MostrarErrorInicioSesion(string mensaje);
        void NavegarAlDashboard();
        void LimpiarFormulario();
        void HabilitarControles(bool habilitado);
        void MostrarProgreso(bool mostrar);
        #endregion
    }
}
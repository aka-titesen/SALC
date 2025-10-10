using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de historial de an�lisis seg�n ERS v2.7
    /// Implementa el patr�n MVP (Model-View-Presenter) para RF-09: Visualizar Historial
    /// </summary>
    public interface IVistaHistorial
    {
        #region Eventos de la Vista (en espa�ol)
        event EventHandler ActualizacionSolicitada;
        event EventHandler CierreSolicitado;
        event EventHandler<int> VerDetalleAnalisis;
        event EventHandler<int> FiltrarPorPaciente;
        #endregion

        #region M�todos de la Vista (en espa�ol)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void CargarHistorialAnalisis(List<Models.Analisis> analisis);
        void FiltrarPorUsuario(int dniUsuario, string rolUsuario);
        void LimpiarFiltros();
        #endregion
    }
}
using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gesti�n de m�tricas seg�n ERS v2.7
    /// Implementa el patr�n MVP (Model-View-Presenter) para RF-04: ABM de Cat�logos
    /// </summary>
    public interface IVistaMetricas
    {
        #region Eventos de la Vista (en espa�ol)
        event EventHandler CreacionSolicitada;
        event EventHandler EdicionSolicitada;
        event EventHandler EliminacionSolicitada;
        event EventHandler CierreSolicitado;
        #endregion

        #region M�todos de la Vista (en espa�ol)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void LimpiarFormulario();
        void HabilitarFormulario(bool habilitado);
        void CargarDatosMetrica(Models.Metrica metrica);
        void ActualizarListaMetricas();
        #endregion
    }
}
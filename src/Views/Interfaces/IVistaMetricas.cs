using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gestión de métricas según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para RF-04: ABM de Catálogos
    /// </summary>
    public interface IVistaMetricas
    {
        #region Eventos de la Vista (en español)
        event EventHandler CreacionSolicitada;
        event EventHandler EdicionSolicitada;
        event EventHandler EliminacionSolicitada;
        event EventHandler CierreSolicitado;
        #endregion

        #region Métodos de la Vista (en español)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void LimpiarFormulario();
        void HabilitarFormulario(bool habilitado);
        void CargarDatosMetrica(Models.Metrica metrica);
        void ActualizarListaMetricas();
        #endregion
    }
}
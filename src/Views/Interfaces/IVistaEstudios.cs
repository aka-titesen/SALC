using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gestión de estudios/análisis según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para RF-05, RF-06, RF-07
    /// </summary>
    public interface IVistaEstudios
    {
        #region Eventos de la Vista (en español)
        event EventHandler CreacionSolicitada;
        event EventHandler EdicionSolicitada;
        event EventHandler EliminacionSolicitada;
        event EventHandler BusquedaSolicitada;
        event EventHandler ValidacionSolicitada;
        event EventHandler CierreSolicitado;
        #endregion

        #region Propiedades de la Vista (en español)
        string TextoBusqueda { get; }
        #endregion

        #region Métodos de la Vista (en español)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void LimpiarFormulario();
        void HabilitarFormulario(bool habilitado);
        void CargarDatosEstudio(Models.Analisis analisis);
        void ActualizarListaEstudios();
        void MostrarResultados(System.Collections.Generic.List<Models.AnalisisMetrica> resultados);
        #endregion
    }
}
using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gesti�n de estudios/an�lisis seg�n ERS v2.7
    /// Implementa el patr�n MVP (Model-View-Presenter) para RF-05, RF-06, RF-07
    /// </summary>
    public interface IVistaEstudios
    {
        #region Eventos de la Vista (en espa�ol)
        event EventHandler CreacionSolicitada;
        event EventHandler EdicionSolicitada;
        event EventHandler EliminacionSolicitada;
        event EventHandler BusquedaSolicitada;
        event EventHandler ValidacionSolicitada;
        event EventHandler CierreSolicitado;
        #endregion

        #region Propiedades de la Vista (en espa�ol)
        string TextoBusqueda { get; }
        #endregion

        #region M�todos de la Vista (en espa�ol)
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
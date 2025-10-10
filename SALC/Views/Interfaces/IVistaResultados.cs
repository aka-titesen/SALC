using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de resultados de análisis según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para RF-06: Cargar Resultados
    /// </summary>
    public interface IVistaResultados
    {
        #region Eventos de la Vista (en español)
        event EventHandler CargaSolicitada;
        event EventHandler ValidacionSolicitada;
        event EventHandler GuardadoSolicitado;
        event EventHandler CierreSolicitado;
        #endregion

        #region Métodos de la Vista (en español)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void CargarResultados(List<Models.AnalisisMetrica> resultados);
        void HabilitarEdicion(bool habilitado);
        void LimpiarFormulario();
        #endregion
    }
}
using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gestión de pacientes según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para RF-03: ABM de Pacientes
    /// </summary>
    public interface IVistaPacientes
    {
        #region Eventos de la Vista (en español)
        event EventHandler CreacionSolicitada;
        event EventHandler EdicionSolicitada;
        event EventHandler EliminacionSolicitada;
        event EventHandler BusquedaSolicitada;
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
        void CargarDatosPaciente(Models.Paciente paciente);
        void ActualizarListaPacientes();
        #endregion
    }
}
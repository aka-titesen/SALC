using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gestión de obras sociales según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para RF-04: ABM de Catálogos
    /// </summary>
    public interface IVistaObrasSociales
    {
        #region Eventos de la Vista (en español)
        event EventHandler CargarObrasSociales;
        event EventHandler<int> EliminarObraSocial;
        event EventHandler<Models.ObraSocial> GuardarObraSocial;
        event EventHandler<int> EditarObraSocial;
        event EventHandler<string> BuscarObraSocial;
        event EventHandler CreacionSolicitada;
        event EventHandler EdicionSolicitada;
        event EventHandler EliminacionSolicitada;
        event EventHandler CierreSolicitado;
        #endregion

        #region Propiedades de la Vista (en español)
        List<Models.ObraSocial> ObrasSociales { get; set; }
        Models.ObraSocial ObraSocialSeleccionada { get; set; }
        bool EstaEditando { get; set; }
        #endregion

        #region Métodos de la Vista (en español)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void LimpiarFormulario();
        void CargarDatosObraSocial(Models.ObraSocial obraSocial);
        void ActualizarListaObrasSociales();
        void HabilitarFormulario(bool habilitado);
        #endregion
    }
}
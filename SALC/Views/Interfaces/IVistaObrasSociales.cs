using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gesti�n de obras sociales seg�n ERS v2.7
    /// Implementa el patr�n MVP (Model-View-Presenter) para RF-04: ABM de Cat�logos
    /// </summary>
    public interface IVistaObrasSociales
    {
        #region Eventos de la Vista (en espa�ol)
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

        #region Propiedades de la Vista (en espa�ol)
        List<Models.ObraSocial> ObrasSociales { get; set; }
        Models.ObraSocial ObraSocialSeleccionada { get; set; }
        bool EstaEditando { get; set; }
        #endregion

        #region M�todos de la Vista (en espa�ol)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void LimpiarFormulario();
        void CargarDatosObraSocial(Models.ObraSocial obraSocial);
        void ActualizarListaObrasSociales();
        void HabilitarFormulario(bool habilitado);
        #endregion
    }
}
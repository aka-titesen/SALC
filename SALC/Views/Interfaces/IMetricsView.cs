using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gesti�n de m�tricas seg�n ERS v2.7
    /// </summary>
    public interface IVistaMetricas
    {
        event EventHandler CreacionSolicitada;
        event EventHandler EdicionSolicitada;
        event EventHandler EliminacionSolicitada;
        event EventHandler CierreSolicitado;
        void MostrarMensaje(string titulo, string mensaje);
    }
}

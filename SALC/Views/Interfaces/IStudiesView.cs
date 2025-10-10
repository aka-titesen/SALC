using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gesti�n de estudios/an�lisis seg�n ERS v2.7
    /// </summary>
    public interface IVistaEstudios
    {
        event EventHandler CreacionSolicitada;
        event EventHandler EdicionSolicitada;
        event EventHandler EliminacionSolicitada;
        event EventHandler BusquedaSolicitada;
        event EventHandler CierreSolicitado;
        string TextoBusqueda { get; }
        void MostrarMensaje(string titulo, string mensaje);
    }
}

using System;
using System.Collections.Generic;
using SALC.Domain;

namespace SALC.Presenters.ViewsContracts
{
    public interface IPanelAsistenteView
    {
        // Historial pacientes
        event EventHandler BuscarHistorialClick;
        string HistorialDniPacienteTexto { get; }
        void CargarHistorialAnalisis(IEnumerable<Analisis> analisis);

        // Generar informe verificado
        event EventHandler GenerarInformeClick;
        string AnalisisIdParaInformeTexto { get; }

        // Mensajes
        void MostrarMensaje(string texto, bool esError = false);
    }
}

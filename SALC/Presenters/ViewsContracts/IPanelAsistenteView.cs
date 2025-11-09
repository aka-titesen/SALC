using System;
using System.Collections.Generic;
using SALC.Domain;

namespace SALC.Presenters.ViewsContracts
{
    public interface IPanelAsistenteView
    {
        // Lista de pacientes con búsqueda
        event EventHandler BuscarPacientesClick;
        string BusquedaPacienteTexto { get; }
        void CargarListaPacientes(IEnumerable<Paciente> pacientes);
        Paciente PacienteSeleccionado { get; }

        // Historial de análisis - Solo apertura de ventana modal
        event EventHandler VerHistorialClick;

        // Eventos heredados (no usados en nueva implementación)
        event EventHandler GenerarPdfClick;
        event EventHandler EnviarInformeClick;

        // Mensajes
        void MostrarMensaje(string texto, bool esError = false);
    }
}

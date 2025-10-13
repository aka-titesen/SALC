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

        // Historial de análisis
        event EventHandler VerHistorialClick;
        void CargarHistorialAnalisis(IEnumerable<object> analisisConEstados);
        object AnalisisSeleccionado { get; }

        // Acciones para análisis verificados
        event EventHandler GenerarPdfClick;
        event EventHandler EnviarInformeClick;
        void HabilitarAccionesAnalisis(bool habilitar);

        // Mensajes
        void MostrarMensaje(string texto, bool esError = false);
    }
}

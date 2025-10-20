using System;
using System.Collections.Generic;
using SALC.Domain;

namespace SALC.Presenters.ViewsContracts
{
    public interface IGestionPacientesAsistenteView
    {
        // Lista de pacientes
        event EventHandler BuscarPacientesClick;
        event EventHandler NuevoPacienteClick;
        event EventHandler EditarPacienteClick;
        event EventHandler RefrescarClick;

        // Propiedades de búsqueda
        string TextoBusqueda { get; }
        
        // Datos de la vista
        void CargarListaPacientes(IEnumerable<Paciente> pacientes);
        Paciente PacienteSeleccionado { get; }
        
        // Control de botones
        void HabilitarAcciones(bool habilitar);
        void HabilitarEdicion(bool habilitar);
        
        // Mensajes y estado
        void MostrarMensaje(string mensaje, bool esError = false);
        void MostrarCargando(bool cargando);
        
        // Actualización de datos
        void ActualizarContadores(int totalPacientes, int pacientesActivos);
        
        // Eventos de cierre o finalización
        event EventHandler VistaInicializada;
    }
}
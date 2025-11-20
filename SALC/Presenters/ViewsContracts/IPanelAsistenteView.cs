using System;
using System.Collections.Generic;
using SALC.Domain;

namespace SALC.Presenters.ViewsContracts
{
    /// <summary>
    /// Interfaz de contrato para la vista del panel del asistente.
    /// Define las propiedades y eventos para las funcionalidades del asistente:
    /// búsqueda de pacientes, visualización de historial y generación de informes.
    /// </summary>
    public interface IPanelAsistenteView
    {
        /// <summary>
        /// Evento que se dispara cuando se solicita buscar pacientes
        /// </summary>
        event EventHandler BuscarPacientesClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita ver el historial de un paciente
        /// </summary>
        event EventHandler VerHistorialClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita generar un PDF de informe
        /// </summary>
        event EventHandler GenerarPdfClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita enviar un informe por email
        /// </summary>
        event EventHandler EnviarInformeClick;

        /// <summary>
        /// Obtiene el texto de búsqueda de pacientes
        /// </summary>
        string BusquedaPacienteTexto { get; }

        /// <summary>
        /// Obtiene el paciente actualmente seleccionado
        /// </summary>
        Paciente PacienteSeleccionado { get; }

        /// <summary>
        /// Carga la lista de pacientes en la vista
        /// </summary>
        /// <param name="pacientes">Colección de pacientes a mostrar</param>
        void CargarListaPacientes(IEnumerable<Paciente> pacientes);

        /// <summary>
        /// Muestra un mensaje al usuario
        /// </summary>
        /// <param name="texto">Mensaje a mostrar</param>
        /// <param name="esError">Indica si es un mensaje de error</param>
        void MostrarMensaje(string texto, bool esError = false);
    }
}

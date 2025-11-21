using System;
using System.Collections.Generic;
using SALC.Domain;

namespace SALC.Presenters.ViewsContracts
{
    /// <summary>
    /// Interfaz de contrato para la vista de gestión de pacientes del asistente.
    /// Define las propiedades y eventos para administrar pacientes desde el rol de asistente.
    /// </summary>
    public interface IGestionPacientesAsistenteView
    {
        /// <summary>
        /// Evento que se dispara cuando se solicita buscar pacientes
        /// </summary>
        event EventHandler BuscarPacientesClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita crear un nuevo paciente
        /// </summary>
        event EventHandler NuevoPacienteClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita editar un paciente
        /// </summary>
        event EventHandler EditarPacienteClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita refrescar la lista
        /// </summary>
        event EventHandler RefrescarClick;

        /// <summary>
        /// Evento que se dispara cuando la vista ha sido inicializada
        /// </summary>
        event EventHandler VistaInicializada;

        /// <summary>
        /// Obtiene el texto de búsqueda ingresado por el usuario
        /// </summary>
        string TextoBusqueda { get; }

        /// <summary>
        /// Obtiene el paciente actualmente seleccionado en la lista
        /// </summary>
        Paciente PacienteSeleccionado { get; }

        /// <summary>
        /// Carga la lista de pacientes en la vista
        /// </summary>
        /// <param name="pacientes">Colección de pacientes a mostrar</param>
        void CargarListaPacientes(IEnumerable<Paciente> pacientes);

        /// <summary>
        /// Habilita o deshabilita las acciones disponibles
        /// </summary>
        /// <param name="habilitar">True para habilitar, false para deshabilitar</param>
        void HabilitarAcciones(bool habilitar);

        /// <summary>
        /// Habilita o deshabilita la edición de datos
        /// </summary>
        /// <param name="habilitar">True para habilitar, false para deshabilitar</param>
        void HabilitarEdicion(bool habilitar);

        /// <summary>
        /// Muestra un mensaje al usuario
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        /// <param name="esError">Indica si es un mensaje de error</param>
        void MostrarMensaje(string mensaje, bool esError = false);

        /// <summary>
        /// Muestra u oculta un indicador de carga
        /// </summary>
        /// <param name="cargando">True para mostrar, false para ocultar</param>
        void MostrarCargando(bool cargando);

        /// <summary>
        /// Actualiza los contadores de pacientes en la vista
        /// </summary>
        /// <param name="totalPacientes">Total de pacientes</param>
        /// <param name="pacientesActivos">Cantidad de pacientes activos</param>
        void ActualizarContadores(int totalPacientes, int pacientesActivos);
    }
}
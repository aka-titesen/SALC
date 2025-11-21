using System;
using System.Collections.Generic;
using SALC.Domain;
using SALC.Presenters;

namespace SALC.Presenters.ViewsContracts
{
    /// <summary>
    /// Interfaz de contrato para la vista del panel del médico.
    /// Define todas las propiedades y eventos para las funcionalidades del médico:
    /// gestión de pacientes, creación de análisis, carga de resultados y firma digital.
    /// </summary>
    public interface IPanelMedicoView
    {
        #region Crear Análisis

        /// <summary>
        /// Evento que se dispara cuando se solicita crear un análisis
        /// </summary>
        event EventHandler CrearAnalisisClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita buscar un paciente para crear análisis
        /// </summary>
        event EventHandler BuscarPacienteCrearClick;

        /// <summary>
        /// Obtiene el DNI del paciente ingresado para crear análisis
        /// </summary>
        string CrearAnalisisDniPacienteTexto { get; }

        /// <summary>
        /// Obtiene el ID del tipo de análisis seleccionado
        /// </summary>
        int? TipoAnalisisSeleccionadoId { get; }

        /// <summary>
        /// Obtiene las observaciones del análisis a crear
        /// </summary>
        string CrearAnalisisObservaciones { get; }

        /// <summary>
        /// Carga los tipos de análisis disponibles
        /// </summary>
        void CargarTiposAnalisis(IEnumerable<TipoAnalisis> tipos);

        /// <summary>
        /// Muestra el paciente seleccionado para crear análisis
        /// </summary>
        void MostrarPacienteSeleccionado(Paciente paciente);

        /// <summary>
        /// Limpia la selección de paciente
        /// </summary>
        void LimpiarPacienteSeleccionado();

        #endregion

        #region Cargar Resultados

        /// <summary>
        /// Evento que se dispara cuando se solicita guardar resultados
        /// </summary>
        event EventHandler CargarResultadosGuardarClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita buscar un análisis para cargar resultados
        /// </summary>
        event EventHandler BuscarAnalisisResultadosClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita cargar las métricas de un análisis
        /// </summary>
        event EventHandler CargarMetricasAnalisisClick;

        /// <summary>
        /// Obtiene el ID del análisis para cargar resultados
        /// </summary>
        string AnalisisIdParaResultadosTexto { get; }

        /// <summary>
        /// Carga las métricas para edición de resultados
        /// </summary>
        void CargarResultadosParaEdicion(IList<MetricaConResultado> filas);

        /// <summary>
        /// Lee los resultados editados por el usuario
        /// </summary>
        IList<MetricaConResultado> LeerResultadosEditados();

        /// <summary>
        /// Muestra el análisis seleccionado para cargar resultados
        /// </summary>
        void MostrarAnalisisParaResultados(Analisis analisis, Paciente paciente, TipoAnalisis tipo);

        /// <summary>
        /// Limpia el análisis seleccionado para resultados
        /// </summary>
        void LimpiarAnalisisParaResultados();

        #endregion

        #region Validar y Firmar

        /// <summary>
        /// Evento que se dispara cuando se solicita firmar un análisis
        /// </summary>
        event EventHandler FirmarAnalisisClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita buscar un análisis para firmar
        /// </summary>
        event EventHandler BuscarAnalisisFirmarClick;

        /// <summary>
        /// Obtiene el ID del análisis para firmar
        /// </summary>
        string AnalisisIdParaFirmaTexto { get; }

        /// <summary>
        /// Muestra el análisis seleccionado para firmar
        /// </summary>
        void MostrarAnalisisParaFirmar(Analisis analisis, Paciente paciente, TipoAnalisis tipo);

        /// <summary>
        /// Limpia el análisis seleccionado para firma
        /// </summary>
        void LimpiarAnalisisParaFirmar();

        /// <summary>
        /// Muestra los resultados para validación antes de firmar
        /// </summary>
        void MostrarResultadosParaValidacion(IList<AnalisisMetrica> resultados);

        #endregion

        #region Gestión de Pacientes

        /// <summary>
        /// Evento que se dispara cuando se solicita editar un paciente
        /// </summary>
        event EventHandler PacientesEditarClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita eliminar un paciente (baja lógica)
        /// </summary>
        event EventHandler PacientesEliminarClick;

        /// <summary>
        /// Evento que se dispara cuando cambia el texto de búsqueda de pacientes
        /// </summary>
        event EventHandler<string> PacientesBuscarTextoChanged;

        /// <summary>
        /// Evento que se dispara cuando cambia el filtro de estado de pacientes
        /// </summary>
        event EventHandler<string> PacientesFiltroEstadoChanged;

        /// <summary>
        /// Carga la lista de pacientes en la vista
        /// </summary>
        void CargarPacientes(System.Collections.IEnumerable pacientes);

        /// <summary>
        /// Obtiene el DNI del paciente seleccionado
        /// </summary>
        int? ObtenerPacienteSeleccionadoDni();

        #endregion

        #region Mensajes y Navegación

        /// <summary>
        /// Muestra un mensaje al usuario
        /// </summary>
        /// <param name="texto">Mensaje a mostrar</param>
        /// <param name="esError">Indica si es un mensaje de error</param>
        void MostrarMensaje(string texto, bool esError = false);

        /// <summary>
        /// Activa la pestaña de cargar resultados
        /// </summary>
        void ActivarTabResultados();

        /// <summary>
        /// Activa la pestaña de validación
        /// </summary>
        void ActivarTabValidacion();

        #endregion
    }
}

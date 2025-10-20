using System;
using System.Collections.Generic;
using SALC.Domain;
using SALC.Presenters;

namespace SALC.Presenters.ViewsContracts
{
    public interface IPanelMedicoView
    {
        // Crear análisis (RF-05)
        event EventHandler CrearAnalisisClick;
        event EventHandler BuscarPacienteCrearClick;
        string CrearAnalisisDniPacienteTexto { get; }
        int? TipoAnalisisSeleccionadoId { get; }
        string CrearAnalisisObservaciones { get; }
        void CargarTiposAnalisis(IEnumerable<TipoAnalisis> tipos);
        void MostrarPacienteSeleccionado(Paciente paciente);
        void LimpiarPacienteSeleccionado();

        // Cargar resultados (RF-06)
        event EventHandler CargarResultadosGuardarClick;
        event EventHandler BuscarAnalisisResultadosClick;
        event EventHandler CargarMetricasAnalisisClick;
        string AnalisisIdParaResultadosTexto { get; }
        void CargarResultadosParaEdicion(IList<ResultadoEdicionDto> filas);
        IList<ResultadoEdicionDto> LeerResultadosEditados();
        void MostrarAnalisisParaResultados(Analisis analisis, Paciente paciente, TipoAnalisis tipo);
        void LimpiarAnalisisParaResultados();

        // Validar/Firmar (RF-07)
        event EventHandler FirmarAnalisisClick;
        event EventHandler BuscarAnalisisFirmarClick;
        string AnalisisIdParaFirmaTexto { get; }
        void MostrarAnalisisParaFirmar(Analisis analisis, Paciente paciente, TipoAnalisis tipo);
        void LimpiarAnalisisParaFirmar();
        void MostrarResultadosParaValidacion(IList<AnalisisMetrica> resultados);

        // Generar informe (RF-08) - Solo análisis verificados
        event EventHandler GenerarInformeClick;
        event EventHandler BuscarAnalisisInformeClick;
        string AnalisisIdParaInformeTexto { get; }
        void MostrarAnalisisParaInforme(Analisis analisis, Paciente paciente, TipoAnalisis tipo);
        void LimpiarAnalisisParaInforme();

        // Gestión de Pacientes (RF-03) - Médico puede modificar y dar de baja
        event EventHandler PacientesEditarClick;
        event EventHandler PacientesEliminarClick; // Baja lógica
        event EventHandler<string> PacientesBuscarTextoChanged;
        event EventHandler<string> PacientesFiltroEstadoChanged;
        
        // Datos/selección Pacientes
        void CargarPacientes(System.Collections.IEnumerable pacientes);
        int? ObtenerPacienteSeleccionadoDni();

        // Mensajes y navegación
        void MostrarMensaje(string texto, bool esError = false);
        void ActivarTabResultados();
        void ActivarTabValidacion();
    }
}

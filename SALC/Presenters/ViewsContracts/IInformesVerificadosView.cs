using System;
using System.Collections.Generic;

namespace SALC.Presenters.ViewsContracts
{
    public interface IInformesVerificadosView
    {
        // Eventos
        event EventHandler BuscarAnalisisClick;
        event EventHandler LimpiarFiltrosClick;
        event EventHandler GenerarPdfClick;
        event EventHandler EnviarEmailClick;
        event EventHandler EnviarWhatsAppClick;
        event EventHandler AnalisisSeleccionCambiada;

        // Propiedades de filtros
        DateTime FechaDesde { get; }
        DateTime FechaHasta { get; }
        int? MedicoSeleccionadoId { get; }
        string TextoBusquedaPaciente { get; }
        string ObservacionesEnvio { get; }

        // Métodos de carga de datos
        void CargarMedicos(IEnumerable<object> medicos);
        void CargarAnalisisVerificados(IEnumerable<object> analisis);

        // Estado y selección
        object AnalisisSeleccionado { get; }
        void HabilitarAcciones(bool habilitar);
        void LimpiarFiltros();

        // Mensajes
        void MostrarMensaje(string texto, bool esError = false);
    }
}
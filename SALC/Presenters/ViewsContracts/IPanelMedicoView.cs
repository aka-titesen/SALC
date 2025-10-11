using System;
using System.Collections.Generic;
using SALC.Domain;
using SALC.Presenters;

namespace SALC.Presenters.ViewsContracts
{
    public interface IPanelMedicoView
    {
        // Crear análisis
        event EventHandler CrearAnalisisClick;
        // Datos crear análisis
        string CrearAnalisisDniPacienteTexto { get; }
        int? TipoAnalisisSeleccionadoId { get; }
        string CrearAnalisisObservaciones { get; }
        void CargarTiposAnalisis(IEnumerable<TipoAnalisis> tipos);

        // Cargar resultados
        event EventHandler CargarResultadosGuardarClick;
        string AnalisisIdParaResultadosTexto { get; }
        void CargarResultadosParaEdicion(IList<ResultadoEdicionDto> filas);
        IList<ResultadoEdicionDto> LeerResultadosEditados();

        // Validar/Firmar
        event EventHandler FirmarAnalisisClick;
        string AnalisisIdParaFirmaTexto { get; }

        // Generar informe (stub)
        event EventHandler GenerarInformeClick;

        // Mensajes
        void MostrarMensaje(string texto, bool esError = false);
    }
}

using System;
using System.Collections.Generic;
using SALC.Views.Interfaces;
using SALC.Services;

namespace SALC.Presenters
{
    /// <summary>
    /// Presentador para la vista de reportes de análisis (RF-08, RF-09) siguiendo ERS v2.7
    /// </summary>
    public class PresentadorReportes
    {
        private readonly IVistaReportes _vista;
        private readonly ServicioReportesAnalisis _servicio;

        public PresentadorReportes(IVistaReportes vista)
        {
            _vista = vista ?? throw new ArgumentNullException(nameof(vista));
            _servicio = new ServicioReportesAnalisis();

            SuscribirEventos();
            Inicializar();
        }

        private void SuscribirEventos()
        {
            _vista.CargarReportes += (s, e) => Cargar();
            _vista.BuscarReportes += (s, texto) => Buscar(texto);
            _vista.FiltrarReportes += (s, filtro) => Filtrar(filtro);
            _vista.VerReporteCompleto += (s, index) => VerDetalle(index);
            _vista.ExportarReportePdf += (s, index) => ExportarPdf(index);
            _vista.ExportarReporteCsv += (s, index) => ExportarCsv(index);
        }

        private void Inicializar()
        {
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                var reportes = _servicio.ObtenerTodosLosReportes();
                _vista.CargarDatosReportes(reportes);
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al cargar reportes: {ex.Message}");
            }
        }

        private void Buscar(string texto)
        {
            try
            {
                var reportes = _servicio.BuscarReportes(texto);
                _vista.CargarDatosReportes(reportes);
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al buscar reportes: {ex.Message}");
            }
        }

        private void Filtrar(FiltroReporte filtro)
        {
            try
            {
                var reportes = _servicio.FiltrarReportes(filtro);
                _vista.CargarDatosReportes(reportes);
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al filtrar reportes: {ex.Message}");
            }
        }

        private void VerDetalle(int index)
        {
            try
            {
                if (index < 0 || index >= _vista.Reportes?.Count)
                    return;

                var seleccionado = _vista.Reportes[index];
                var detalle = _servicio.ObtenerDetallesAnalisis(seleccionado.IdReporte);
                if (detalle == null)
                {
                    _vista.MostrarError("No se pudieron cargar los detalles del análisis.");
                    return;
                }

                // TODO: Abrir formulario de detalle en español si existe. Por ahora solo mostramos mensaje.
                _vista.MostrarMensaje("Detalles", $"Paciente: {detalle.NombrePaciente}\nAnálisis: {detalle.TipoAnalisis} - {detalle.FechaAnalisis:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al ver reporte: {ex.Message}");
            }
        }

        private void ExportarPdf(int index)
        {
            try
            {
                if (index < 0 || index >= _vista.Reportes?.Count)
                    return;

                var reporte = _vista.Reportes[index];
                _servicio.ExportarAPdf(reporte);
                _vista.MostrarMensaje("Exportación Exitosa", $"Reporte de {reporte.NombrePaciente} exportado a PDF correctamente");
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al exportar a PDF: {ex.Message}");
            }
        }

        private void ExportarCsv(int index)
        {
            try
            {
                if (index < 0 || index >= _vista.Reportes?.Count)
                    return;

                var reporte = _vista.Reportes[index];
                _servicio.ExportarACsv(reporte);
                _vista.MostrarMensaje("Exportación Exitosa", $"Reporte de {reporte.NombrePaciente} exportado a CSV correctamente");
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al exportar a CSV: {ex.Message}");
            }
        }
    }
}

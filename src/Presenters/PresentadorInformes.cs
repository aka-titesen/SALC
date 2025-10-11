using System;
using System.Collections.Generic;
using SALC.Views.Interfaces;
using SALC.Services;

namespace SALC.Presenters
{
    /// <summary>
    /// Presentador para la vista de informes de análisis (RF-08, RF-09) siguiendo ERS v2.7
    /// </summary>
    public class PresentadorInformes
    {
        private readonly IVistaInformes _vista;
    private readonly InformesAnalisisService _servicio;

        public PresentadorInformes(IVistaInformes vista)
        {
            _vista = vista ?? throw new ArgumentNullException(nameof(vista));
            _servicio = new InformesAnalisisService();

            SuscribirEventos();
            Inicializar();
        }

        private void SuscribirEventos()
        {
            _vista.CargarInformes += (s, e) => Cargar();
            _vista.BuscarInformes += (s, texto) => Buscar(texto);
            _vista.FiltrarInformes += (s, filtro) => Filtrar(filtro);
            _vista.VerInformeCompleto += (s, index) => VerDetalle(index);
            _vista.ExportarInformePdf += (s, index) => ExportarPdf(index);
            _vista.ExportarInformeCsv += (s, index) => ExportarCsv(index);
        }

        private void Inicializar()
        {
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                var informes = _servicio.ObtenerTodosLosInformes();
                _vista.CargarDatosInformes(informes);
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al cargar informes: {ex.Message}");
            }
        }

        private void Buscar(string texto)
        {
            try
            {
                var informes = _servicio.BuscarInformes(texto);
                _vista.CargarDatosInformes(informes);
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al buscar informes: {ex.Message}");
            }
        }

        private void Filtrar(FiltroInforme filtro)
        {
            try
            {
                var informes = _servicio.FiltrarInformes(filtro);
                _vista.CargarDatosInformes(informes);
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al filtrar informes: {ex.Message}");
            }
        }

        private void VerDetalle(int index)
        {
            try
            {
                if (index < 0 || index >= _vista.Informes?.Count)
                    return;

                var seleccionado = _vista.Informes[index];
                var detalle = _servicio.ObtenerDetallesAnalisis(seleccionado.IdReporte);
                if (detalle == null)
                {
                    _vista.MostrarError("No se pudieron cargar los detalles del análisis.");
                    return;
                }

                _vista.MostrarMensaje("Detalles", $"Paciente: {detalle.NombrePaciente}\nAnálisis: {detalle.TipoAnalisis} - {detalle.FechaAnalisis:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al ver informe: {ex.Message}");
            }
        }

        private void ExportarPdf(int index)
        {
            try
            {
                if (index < 0 || index >= _vista.Informes?.Count)
                    return;

                var informe = _vista.Informes[index];
                _servicio.ExportarAPdf(informe);
                _vista.MostrarMensaje("Exportación Exitosa", $"Informe de {informe.NombrePaciente} exportado a PDF correctamente");
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
                if (index < 0 || index >= _vista.Informes?.Count)
                    return;

                var informe = _vista.Informes[index];
                _servicio.ExportarACsv(informe);
                _vista.MostrarMensaje("Exportación Exitosa", $"Informe de {informe.NombrePaciente} exportado a CSV correctamente");
            }
            catch (Exception ex)
            {
                _vista.MostrarError($"Error al exportar a CSV: {ex.Message}");
            }
        }
    }
}

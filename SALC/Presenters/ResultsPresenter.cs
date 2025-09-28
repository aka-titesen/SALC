// Presenters/ResultsPresenter.cs
using System;
using System.Collections.Generic;
using SALC.Views.Interfaces;
using SALC.Models;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la carga de resultados
    /// Maneja la lógica de presentación entre la vista y los servicios de negocio
    /// </summary>
    public class ResultsPresenter
    {
        private readonly IResultsView _view;

        public ResultsPresenter(IResultsView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            _view.LoadPendingStudies += OnLoadPendingStudies;
            _view.LoadStudyResults += OnLoadStudyResults;
            _view.SaveResult += OnSaveResult;
            _view.CompleteStudy += OnCompleteStudy;
            _view.SearchStudy += OnSearchStudy;
        }

        private void OnLoadPendingStudies(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implementar carga desde el servicio de datos
                // var pendingStudies = _studyService.GetPendingStudies();
                // _view.PendingStudies = pendingStudies;
                // _view.RefreshPendingStudiesList();

                // Por ahora, datos de ejemplo
                _view.PendingStudies = GetSamplePendingStudies();
                _view.RefreshPendingStudiesList();
                _view.ShowMessage("Estudios pendientes cargados correctamente.");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al cargar estudios pendientes: {ex.Message}");
            }
        }

        private void OnLoadStudyResults(object sender, int studyId)
        {
            try
            {
                // TODO: Implementar carga de resultados existentes
                // var results = _resultService.GetResultsByStudyId(studyId);
                // _view.StudyResults = results;
                // _view.RefreshResultsList();
                
                // También cargar métricas disponibles para este tipo de estudio
                // var metrics = _metricService.GetMetricsByStudyType(studyType);
                // _view.AvailableMetrics = metrics;
                // _view.LoadMetricsForStudy();

                _view.ShowMessage($"Cargando resultados para estudio ID: {studyId}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al cargar resultados del estudio: {ex.Message}");
            }
        }

        private void OnSaveResult(object sender, ResultadoAnalisis resultado)
        {
            try
            {
                // TODO: Implementar guardado a través del servicio
                // _resultService.SaveResult(resultado);
                
                _view.ShowMessage("Resultado guardado correctamente.");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al guardar resultado: {ex.Message}");
            }
        }

        private void OnCompleteStudy(object sender, int studyId)
        {
            try
            {
                // TODO: Implementar completar estudio a través del servicio
                // _studyService.CompleteStudy(studyId);
                
                _view.ShowMessage($"Estudio ID {studyId} marcado como completado.");
                OnLoadPendingStudies(sender, EventArgs.Empty); // Recargar lista
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al completar estudio: {ex.Message}");
            }
        }

        private void OnSearchStudy(object sender, string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    OnLoadPendingStudies(sender, EventArgs.Empty);
                    return;
                }

                // TODO: Implementar búsqueda a través del servicio
                // var studies = _studyService.SearchPendingStudies(searchTerm);
                // _view.PendingStudies = studies;
                // _view.RefreshPendingStudiesList();
                
                _view.ShowMessage($"Búsqueda realizada para: {searchTerm}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error en la búsqueda: {ex.Message}");
            }
        }

        /// <summary>
        /// Datos de ejemplo para testing de la interfaz
        /// </summary>
        private List<Analisis> GetSamplePendingStudies()
        {
            return new List<Analisis>
            {
                new Analisis
                {
                    Id = 1,
                    IdTipoAnalisis = 1,
                    IdEstado = 2, // En proceso
                    DocPaciente = 12345678,
                    Prioridad = "Alta",
                    FechaCreacion = DateTime.Today.AddDays(-1),
                    Observaciones = "Paciente en ayunas",
                    Paciente = new Paciente { NroDoc = 12345678, Nombre = "Ana", Apellido = "García" },
                    TipoAnalisis = new TipoAnalisis { IdTipo = 1, Descripcion = "Hemograma Completo" },
                    Estado = new Estado { Id = 2, TipoEstado = "En Proceso" }
                },
                new Analisis
                {
                    Id = 2,
                    IdTipoAnalisis = 2,
                    IdEstado = 1, // Pendiente
                    DocPaciente = 87654321,
                    Prioridad = "Media",
                    FechaCreacion = DateTime.Today,
                    Observaciones = "",
                    Paciente = new Paciente { NroDoc = 87654321, Nombre = "Carlos", Apellido = "Ruiz" },
                    TipoAnalisis = new TipoAnalisis { IdTipo = 2, Descripcion = "Perfil Lipídico" },
                    Estado = new Estado { Id = 1, TipoEstado = "Pendiente" }
                },
                new Analisis
                {
                    Id = 3,
                    IdTipoAnalisis = 3,
                    IdEstado = 2, // En proceso
                    DocPaciente = 23456789,
                    Prioridad = "Baja",
                    FechaCreacion = DateTime.Today.AddDays(-2),
                    Observaciones = "Control de rutina",
                    Paciente = new Paciente { NroDoc = 23456789, Nombre = "María", Apellido = "López" },
                    TipoAnalisis = new TipoAnalisis { IdTipo = 3, Descripcion = "Glucemia" },
                    Estado = new Estado { Id = 2, TipoEstado = "En Proceso" }
                }
            };
        }
    }
}
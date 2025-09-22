// Presenters/MetricsPresenter.cs
using System;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la gestión de métricas y parámetros
    /// Maneja la lógica de presentación entre la vista y los servicios de negocio
    /// </summary>
    public class MetricsPresenter
    {
        private readonly IMetricsView _view;

        public MetricsPresenter(IMetricsView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            // TODO: Suscribirse a eventos de la vista cuando se implemente IMetricsView
        }

        // TODO: Implementar métodos del presenter para métricas
    }
}
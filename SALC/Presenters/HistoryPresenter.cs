// Presenters/HistoryPresenter.cs
using System;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para el historial de órdenes
    /// Maneja la lógica de presentación entre la vista y los servicios de negocio
    /// </summary>
    public class HistoryPresenter
    {
        private readonly IHistoryView _view;

        public HistoryPresenter(IHistoryView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            // TODO: Suscribirse a eventos de la vista cuando se implemente IHistoryView
        }

        // TODO: Implementar métodos del presenter para historial
    }
}
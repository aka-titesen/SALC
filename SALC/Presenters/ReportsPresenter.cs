// Presenters/ReportsPresenter.cs
using System;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la generaci�n de informes
    /// Maneja la l�gica de presentaci�n entre la vista y los servicios de negocio
    /// </summary>
    public class ReportsPresenter
    {
        private readonly IReportsView _view;

        public ReportsPresenter(IReportsView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            // TODO: Suscribirse a eventos de la vista cuando se implemente IReportsView
        }

        // TODO: Implementar m�todos del presenter para reportes
    }
}
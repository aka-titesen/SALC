// Presenters/BackupsPresenter.cs
using System;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la gestión de copias de seguridad
    /// Maneja la lógica de presentación entre la vista y los servicios de negocio
    /// </summary>
    public class BackupsPresenter
    {
        private readonly IBackupsView _view;

        public BackupsPresenter(IBackupsView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            // TODO: Suscribirse a eventos de la vista cuando se implemente IBackupsView
        }

        // TODO: Implementar métodos del presenter para backups
    }
}
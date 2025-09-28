// Presenters/UserManagementPresenter.cs
using System;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la gestión de usuarios (versión mejorada)
    /// Maneja la lógica de presentación entre la vista y los servicios de negocio
    /// </summary>
    public class UserManagementPresenter
    {
        private readonly IUserManagementView _view;

        public UserManagementPresenter(IUserManagementView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            // TODO: Suscribirse a eventos de la vista cuando se implemente completamente
        }

        // TODO: Implementar métodos del presenter para gestión de usuarios
    }
}
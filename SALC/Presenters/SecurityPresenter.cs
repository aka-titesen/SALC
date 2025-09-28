// Presenters/SecurityPresenter.cs
using System;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la supervisión de seguridad
    /// Maneja la lógica de presentación entre la vista y los servicios de negocio
    /// </summary>
    public class SecurityPresenter
    {
        private readonly ISecurityView _view;

        public SecurityPresenter(ISecurityView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            // TODO: Suscribirse a eventos de la vista cuando se implemente ISecurityView
        }

        // TODO: Implementar métodos del presenter para seguridad
    }
}
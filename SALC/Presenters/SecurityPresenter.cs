// Presenters/SecurityPresenter.cs
using System;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la supervisi�n de seguridad
    /// Maneja la l�gica de presentaci�n entre la vista y los servicios de negocio
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

        // TODO: Implementar m�todos del presenter para seguridad
    }
}
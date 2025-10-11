using System;
using System.Collections.Generic;
using SALC.Presenters.Auth;
using SALC.Presenters.Dashboard;
using SALC.Presenters.Patients;
using SALC.Presenters.Analysis;
// using SALC.Presenters.Reports; // Obsoleto: usar PresentadorInformes
using SALC.Presenters;
using SALC.Presenters.Admin;
using SALC.Views.Interfaces;

namespace SALC.Factory
{
    /// <summary>
    /// Factory para crear instancias de presentadores.
    /// Implementa el patr�n Factory para desacoplar la creaci�n de objetos.
    /// </summary>
    public static class PresenterFactory
    {
        private static readonly Dictionary<Type, Func<object, object>> _presenterFactories =
            new Dictionary<Type, Func<object, object>>();

        /// <summary>
        /// Inicializa el factory con las configuraciones de presentadores
        /// </summary>
        static PresenterFactory()
        {
            RegisterPresenterFactories();
        }

        /// <summary>
        /// Registra las factories para cada tipo de presentador
        /// </summary>
        private static void RegisterPresenterFactories()
        {
            // Autenticación (vista española ya implementada por formulario InicioSesionForm)

            // Panel principal (español)
            _presenterFactories[typeof(IVistaPanelPrincipal)] = view => new PanelPrincipalPresenter(view as IVistaPanelPrincipal);

            // Patient Presenters
            _presenterFactories[typeof(IPacienteView)] = view => new PacientePresenter(view as IPacienteView);

            // Analysis Presenters
            _presenterFactories[typeof(IAnalisisView)] = view => new AnalisisPresenter(view as IAnalisisView);
            _presenterFactories[typeof(IResultadosView)] = view => new ResultadosPresenter(view as IResultadosView);

            // Informes (renombrado desde "Reportes")
            _presenterFactories[typeof(IVistaInformes)] = view => new PresentadorInformes(view as IVistaInformes);

            // Administración (migración a español pendiente si se requiere fábrica centralizada)
        }

        /// <summary>
        /// Crea un presentador para la vista especificada
        /// </summary>
        /// <typeparam name="TView">Tipo de la interfaz de vista</typeparam>
        /// <typeparam name="TPresenter">Tipo del presentador</typeparam>
        /// <param name="view">Instancia de la vista</param>
        /// <returns>Instancia del presentador</returns>
        public static TPresenter CreatePresenter<TView, TPresenter>(TView view)
            where TView : class
            where TPresenter : class
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            var viewType = typeof(TView);

            if (_presenterFactories.TryGetValue(viewType, out var factory))
            {
                return factory(view) as TPresenter;
            }

            throw new NotSupportedException($"No hay factory registrada para el tipo de vista {viewType.Name}");
        }

        /// <summary>
        /// Crea un presentador basado en el tipo de vista
        /// </summary>
        /// <param name="viewType">Tipo de la vista</param>
        /// <param name="view">Instancia de la vista</param>
        /// <returns>Instancia del presentador</returns>
        public static object CreatePresenter(Type viewType, object view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (_presenterFactories.TryGetValue(viewType, out var factory))
            {
                return factory(view);
            }

            throw new NotSupportedException($"No hay factory registrada para el tipo de vista {viewType.Name}");
        }

        /// <summary>
        /// Registra una factory personalizada para un tipo de vista
        /// </summary>
        /// <typeparam name="TView">Tipo de la interfaz de vista</typeparam>
        /// <param name="factory">Factory function</param>
        public static void RegisterFactory<TView>(Func<TView, object> factory) where TView : class
        {
            _presenterFactories[typeof(TView)] = view => factory(view as TView);
        }

        /// <summary>
        /// Verifica si existe una factory registrada para el tipo de vista
        /// </summary>
        /// <param name="viewType">Tipo de la vista</param>
        /// <returns>True si existe la factory</returns>
        public static bool HasFactory(Type viewType)
        {
            return _presenterFactories.ContainsKey(viewType);
        }
    }
}

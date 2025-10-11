using System;
using System.Collections.Generic;
using SALC.Services.Auth;
using SALC.Services.Core;
using SALC.Services.Admin;
using SALC.Services.Reports;
using SALC.DataAccess.Repositories;

namespace SALC.Factory
{
    /// <summary>
    /// Factory para crear instancias de servicios.
    /// Implementa el patrón Factory y básicamente Dependency Injection manual.
    /// </summary>
    public static class ServiceFactory
    {
        private static readonly Dictionary<Type, Func<object>> _serviceFactories = 
            new Dictionary<Type, Func<object>>();

        private static readonly Dictionary<Type, object> _singletonInstances = 
            new Dictionary<Type, object>();

        /// <summary>
        /// Inicializa el factory con las configuraciones de servicios
        /// </summary>
        static ServiceFactory()
        {
            RegisterServiceFactories();
        }

        /// <summary>
        /// Registra las factories para cada tipo de servicio
        /// </summary>
        private static void RegisterServiceFactories()
        {
            // Authentication Services (Singleton)
            RegisterSingleton<UserAuthentication>(() => new UserAuthentication());
            RegisterSingleton<AuthorizationService>(() => new AuthorizationService());
            RegisterSingleton<SessionManager>(() => new SessionManager());

            // Core Business Services (Transient)
            RegisterTransient<UsuarioService>(() => new UsuarioService());
            RegisterTransient<PacienteService>(() => new PacienteService());
            RegisterTransient<AnalisisService>(() => new AnalisisService());
            RegisterTransient<MetricaService>(() => new MetricaService());

            // Administrative Services (Transient)
            RegisterTransient<AdminCatalogService>(() => new AdminCatalogService());
            RegisterTransient<BackupService>(() => new BackupService());
            RegisterTransient<SystemConfigService>(() => new SystemConfigService());

            // Report Services (Transient)
            RegisterTransient<ReportService>(() => new ReportService());
            RegisterTransient<PDFGeneratorService>(() => new PDFGeneratorService());
            RegisterTransient<EmailService>(() => new EmailService());

            // Repository Services (Transient)
            RegisterTransient<UsuarioRepository>(() => new UsuarioRepository());
            RegisterTransient<PacienteRepository>(() => new PacienteRepository());
            RegisterTransient<AnalisisRepository>(() => new AnalisisRepository());
            RegisterTransient<MetricaRepository>(() => new MetricaRepository());
            RegisterTransient<RolRepository>(() => new RolRepository());
            RegisterTransient<ObraSocialRepository>(() => new ObraSocialRepository());
            RegisterTransient<TipoAnalisisRepository>(() => new TipoAnalisisRepository());
        }

        /// <summary>
        /// Registra un servicio como Singleton
        /// </summary>
        /// <typeparam name="T">Tipo del servicio</typeparam>
        /// <param name="factory">Factory function</param>
        private static void RegisterSingleton<T>(Func<T> factory) where T : class
        {
            _serviceFactories[typeof(T)] = () => 
            {
                if (!_singletonInstances.TryGetValue(typeof(T), out var instance))
                {
                    instance = factory();
                    _singletonInstances[typeof(T)] = instance;
                }
                return instance;
            };
        }

        /// <summary>
        /// Registra un servicio como Transient (nueva instancia cada vez)
        /// </summary>
        /// <typeparam name="T">Tipo del servicio</typeparam>
        /// <param name="factory">Factory function</param>
        private static void RegisterTransient<T>(Func<T> factory) where T : class
        {
            _serviceFactories[typeof(T)] = () => factory();
        }

        /// <summary>
        /// Obtiene una instancia del servicio especificado
        /// </summary>
        /// <typeparam name="T">Tipo del servicio</typeparam>
        /// <returns>Instancia del servicio</returns>
        public static T GetService<T>() where T : class
        {
            var serviceType = typeof(T);
            
            if (_serviceFactories.TryGetValue(serviceType, out var factory))
            {
                return factory() as T;
            }

            throw new NotSupportedException($"No hay factory registrada para el tipo de servicio {serviceType.Name}");
        }

        /// <summary>
        /// Obtiene una instancia del servicio basado en el tipo
        /// </summary>
        /// <param name="serviceType">Tipo del servicio</param>
        /// <returns>Instancia del servicio</returns>
        public static object GetService(Type serviceType)
        {
            if (_serviceFactories.TryGetValue(serviceType, out var factory))
            {
                return factory();
            }

            throw new NotSupportedException($"No hay factory registrada para el tipo de servicio {serviceType.Name}");
        }

        /// <summary>
        /// Verifica si existe una factory registrada para el tipo de servicio
        /// </summary>
        /// <param name="serviceType">Tipo del servicio</param>
        /// <returns>True si existe la factory</returns>
        public static bool HasService(Type serviceType)
        {
            return _serviceFactories.ContainsKey(serviceType);
        }

        /// <summary>
        /// Registra una factory personalizada para un tipo de servicio
        /// </summary>
        /// <typeparam name="T">Tipo del servicio</typeparam>
        /// <param name="factory">Factory function</param>
        /// <param name="singleton">Si debe ser tratado como singleton</param>
        public static void RegisterService<T>(Func<T> factory, bool singleton = false) where T : class
        {
            if (singleton)
            {
                RegisterSingleton(factory);
            }
            else
            {
                RegisterTransient(factory);
            }
        }

        /// <summary>
        /// Limpia todas las instancias singleton (útil para testing)
        /// </summary>
        public static void ClearSingletons()
        {
            foreach (var instance in _singletonInstances.Values)
            {
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _singletonInstances.Clear();
        }
    }
}
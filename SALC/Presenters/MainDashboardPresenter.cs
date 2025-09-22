using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    public static class AuthorizationService
    {
        private static readonly Dictionary<string, HashSet<AppFeature>> _permissions = new Dictionary<string, HashSet<AppFeature>>(StringComparer.OrdinalIgnoreCase)
        {
            { "admin", new HashSet<AppFeature>
                {
                    AppFeature.Dashboard,
                    AppFeature.GestionUsuarios,
                    AppFeature.ConfigSistema,
                    AppFeature.CopiasSeguridad,
                    AppFeature.Seguridad
                }
            },
            { "clinico", new HashSet<AppFeature>
                {
                    AppFeature.GestionPacientes,
                    AppFeature.GestionEstudios,
                    AppFeature.CargaResultados,
                    AppFeature.GenerarInformes,
                    AppFeature.Notificaciones,
                    AppFeature.HistorialOrdenes,
                    AppFeature.Turnos
                }
            },
            { "asistente", new HashSet<AppFeature>
                {
                    AppFeature.GestionPacientes,
                    // Sin GestionEstudios para asistente (ERS RF2.1)
                    AppFeature.GenerarInformes,
                    AppFeature.Notificaciones,
                    AppFeature.HistorialOrdenes,
                    AppFeature.Turnos
                }
            }
        };

        public static IReadOnlyCollection<AppFeature> GetFeaturesFor(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return Array.Empty<AppFeature>();

            if (_permissions.TryGetValue(role.Trim().ToLowerInvariant(), out var set))
            {
                return set.ToList();
            }

            return Array.Empty<AppFeature>();
        }

        public static bool IsAllowed(string role, AppFeature feature)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;
            if (!_permissions.TryGetValue(role.Trim().ToLowerInvariant(), out var set)) return false;
            return set.Contains(feature);
        }
    }

    public class MainDashboardPresenter
    {
        private readonly IMainDashboardView _view;

        public MainDashboardPresenter(IMainDashboardView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));

            var user = UserAuthentication.CurrentUser;
            var name = user != null ? $"{user.Nombre} {user.Apellido}".Trim() : "Usuario";
            var role = user?.Rol ?? string.Empty;

            _view.SetUserInfo(name, role);
            _view.SetHeaderTitle(GetTitleByRole(role));

            var features = AuthorizationService.GetFeaturesFor(role);
            _view.SetAvailableFeatures(features);
        }

        private string GetTitleByRole(string role)
        {
            string r = (role ?? string.Empty).Trim().ToLowerInvariant();
            switch (r)
            {
                case "admin": return "Panel de Administración";
                case "clinico": return "Panel Clínico";
                case "asistente": return "Panel de Asistente";
                default: return "Panel Principal";
            }
        }
    }
}

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
                    
                    // GESTIÓN DE USUARIOS Y ENTIDADES
                    AppFeature.GestionUsuarios,          // ABM usuarios internos (usuario, doctor, asistente) 
                    AppFeature.GestionPacientesAdmin,    // ABM pacientes (vista administrativa completa)
                    AppFeature.GestionDoctoresExternos,  // ABM doctores externos
                    
                    // GESTIÓN DE CATÁLOGOS
                    AppFeature.GestionTiposAnalisis,     // ABM tipo_analisis
                    AppFeature.GestionMetricas,          // ABM metrica  
                    AppFeature.GestionObrasSociales,     // ABM obra_social
                    AppFeature.GestionEstados,           // ABM estado y estado_usuario
                    AppFeature.GestionRoles,             // ABM rol
                    
                    // CONFIGURACIÓN Y SISTEMA
                    AppFeature.ConfigSistema,            // Configuración general
                    AppFeature.CopiasSeguridad,          // Backup y restore
                    AppFeature.Seguridad,                // Auditoría y logs
                    AppFeature.AuditoriaAccesos          // Logs de accesos de usuarios
                }
            },
            { "clinico", new HashSet<AppFeature>
                {
                    AppFeature.GestionPacientes,
                    AppFeature.GestionEstudios,      // Clínicos pueden CREAR análisis (RF-06)
                    AppFeature.CargaResultados,       // Clínicos pueden CARGAR y VALIDAR resultados (RF-07, RF-18)
                    AppFeature.GenerarInformes,       // Clínicos pueden imprimir informes validados
                    AppFeature.Notificaciones,        // Clínicos pueden notificar pacientes (RF-16)
                    AppFeature.HistorialOrdenes
                    // NOTA: Turnos NO existe en el sistema SALC
                }
            },
            { "asistente", new HashSet<AppFeature>
                {
                    AppFeature.GestionPacientes,      // Asistentes pueden gestionar pacientes (RF-03, RF-04)
                    // IMPORTANTE: Asistentes NO tienen GestionEstudios (no pueden crear análisis)
                    // IMPORTANTE: Asistentes NO tienen CargaResultados (no pueden validar)
                    AppFeature.RecepcionMuestras,     // Nueva feature: solo para asistentes (RF-17)
                    AppFeature.GenerarInformes,       // Asistentes pueden IMPRIMIR informes ya validados
                    AppFeature.HistorialOrdenes       // Asistentes pueden ver historial
                    // NOTA: Turnos NO existe en el sistema SALC
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

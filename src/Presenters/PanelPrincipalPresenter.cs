using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    public static class ServicioAutorizacion
    {
        private static readonly Dictionary<string, HashSet<AppFeature>> _permisos = new Dictionary<string, HashSet<AppFeature>>(StringComparer.OrdinalIgnoreCase)
        {
            { "admin", new HashSet<AppFeature>
                {
                    AppFeature.GestionUsuarios,
                    AppFeature.GestionPacientesAdmin,
                    AppFeature.GestionDoctoresExternos,
                    AppFeature.GestionTiposAnalisis,
                    AppFeature.GestionMetricas,
                    AppFeature.GestionObrasSociales,
                    AppFeature.GestionEstados,
                    AppFeature.GestionRoles,
                    AppFeature.CopiasSeguridad,
                }
            },
            { "clinico", new HashSet<AppFeature>
                {
                    AppFeature.GestionPacientes,
                    AppFeature.GestionEstudios,
                    AppFeature.CargaResultados,
                    AppFeature.GenerarInformes,
                    AppFeature.Notificaciones,
                    AppFeature.HistorialOrdenes
                }
            },
            { "asistente", new HashSet<AppFeature>
                {
                    AppFeature.GestionPacientes,
                    AppFeature.RecepcionMuestras,
                    AppFeature.GenerarInformes,
                    AppFeature.HistorialOrdenes
                }
            }
        };

        public static IReadOnlyCollection<AppFeature> FuncionalidadesPara(string rol)
        {
            if (string.IsNullOrWhiteSpace(rol)) return Array.Empty<AppFeature>();
            return _permisos.TryGetValue(rol.Trim().ToLowerInvariant(), out var set) ? set.ToList() : Array.Empty<AppFeature>();
        }

        public static bool Permite(string rol, AppFeature feature)
        {
            if (string.IsNullOrWhiteSpace(rol)) return false;
            return _permisos.TryGetValue(rol.Trim().ToLowerInvariant(), out var set) && set.Contains(feature);
        }
    }

    public class PanelPrincipalPresenter
    {
        private readonly IVistaPanelPrincipal _vista;

        public PanelPrincipalPresenter(IVistaPanelPrincipal vista)
        {
            _vista = vista ?? throw new ArgumentNullException(nameof(vista));

            var user = UserAuthentication.CurrentUser;
            var nombre = user != null ? $"{user.Nombre} {user.Apellido}".Trim() : "Usuario";
            var rol = user?.Rol ?? string.Empty;

            _vista.EstablecerInformacionUsuario(nombre, rol);
            _vista.EstablecerTituloEncabezado(TituloPorRol(rol));

            var features = ServicioAutorizacion.FuncionalidadesPara(rol);
            _vista.EstablecerFuncionalidadesDisponibles(features);
        }

        private string TituloPorRol(string rol)
        {
            string r = (rol ?? string.Empty).Trim().ToLowerInvariant();
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

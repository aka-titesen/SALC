using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista del panel principal (dashboard) según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para la navegación principal
    /// </summary>
    public interface IVistaPanelPrincipal
    {
        #region Métodos de Configuración (en español)
        void EstablecerTituloEncabezado(string titulo);
        void EstablecerInformacionUsuario(string nombreCompleto, string rol);
        void EstablecerFuncionalidadesDisponibles(IReadOnlyCollection<AppFeature> funcionalidades);
        #endregion

        #region Eventos Comunes (en español)
        event EventHandler CierreSesionSolicitado;
        event EventHandler PacientesSolicitado;
        event EventHandler EstudiosSolicitado;
        event EventHandler ResultadosSolicitado;
        event EventHandler ReportesSolicitado;
        event EventHandler NotificacionesSolicitado;
        event EventHandler HistorialSolicitado;
        #endregion

        #region Eventos de Administración de Usuarios (Solo Administrador - en español)
        event EventHandler GestionUsuariosSolicitada;
        event EventHandler PacientesAdminSolicitado;
        #endregion

        #region Eventos de Administración de Catálogos (Solo Administrador - en español)
        event EventHandler TiposAnalisisSolicitado;
        event EventHandler MetricasSolicitado;
        event EventHandler ObrasSocialesSolicitado;
        event EventHandler EstadosSolicitado;
        event EventHandler RolesSolicitado;
        #endregion

        #region Eventos de Configuración y Sistema (Solo Administrador - en español)
        event EventHandler CopiasSeguridad


;
        event EventHandler ConfiguracionSistemaSolicitada;
        #endregion

        #region Métodos de la Vista (en español)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void ActualizarEstadoUsuario(string estado);
        void HabilitarOpcion(AppFeature funcionalidad, bool habilitada);
        #endregion
    }
}
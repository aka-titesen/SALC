using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista del panel principal (dashboard) seg�n ERS v2.7
    /// Implementa el patr�n MVP (Model-View-Presenter) para la navegaci�n principal
    /// </summary>
    public interface IVistaPanelPrincipal
    {
        #region M�todos de Configuraci�n (en espa�ol)
        void EstablecerTituloEncabezado(string titulo);
        void EstablecerInformacionUsuario(string nombreCompleto, string rol);
        void EstablecerFuncionalidadesDisponibles(IReadOnlyCollection<AppFeature> funcionalidades);
        #endregion

        #region Eventos Comunes (en espa�ol)
    event EventHandler CierreSesionSolicitado;
    event EventHandler PacientesSolicitado;
    event EventHandler EstudiosSolicitado;
    event EventHandler ResultadosSolicitado;
    event EventHandler RecepcionMuestrasSolicitado;
    event EventHandler InformesSolicitado;
    event EventHandler NotificacionesSolicitado;
    event EventHandler HistorialSolicitado;
        #endregion

        #region Eventos de Administraci�n de Usuarios (Solo Administrador - en espa�ol)
    event EventHandler GestionUsuariosSolicitada;
    event EventHandler PacientesAdminSolicitado;
    event EventHandler DoctoresExternosSolicitado;
        #endregion

        #region Eventos de Administraci�n de Cat�logos (Solo Administrador - en espa�ol)
    event EventHandler TiposAnalisisSolicitado;
    event EventHandler MetricasSolicitado;
    event EventHandler ObrasSocialesSolicitado;
    event EventHandler EstadosSolicitado;
    event EventHandler RolesSolicitado;
        #endregion

        #region Eventos de Configuraci�n y Sistema (Solo Administrador - en espa�ol)
    event EventHandler CopiasSeguridadSolicitado;
    event EventHandler ConfiguracionSistemaSolicitada;
        #endregion

        #region M�todos de la Vista (en espa�ol)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void ActualizarEstadoUsuario(string estado);
        void HabilitarOpcion(AppFeature funcionalidad, bool habilitada);
        #endregion
    }
}

using System;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de configuración del sistema según ERS v2.7
    /// Solo disponible para usuarios Administrador
    /// Implementa el patrón MVP (Model-View-Presenter)
    /// </summary>
    public interface IVistaConfiguracionSistema
    {
        #region Eventos (en español)
        event EventHandler CargarConfiguracion;
        event EventHandler<ConfiguracionSistema> GuardarConfiguracion;
        event EventHandler ProbarRutaCopias;
        event EventHandler VerEstadisticasSistema;
        event EventHandler CierreSolicitado;
        #endregion

        #region Propiedades (en español)
        ConfiguracionSistema ConfiguracionActual { get; set; }
        EstadisticasSistema EstadisticasSistema { get; set; }
        #endregion

        #region Métodos de la Vista (en español)
        void CargarDatosConfiguracion(ConfiguracionSistema configuracion);
        void CargarDatosEstadisticas(EstadisticasSistema estadisticas);
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void MostrarErrorValidacion(string campo, string error);
        void LimpiarErroresValidacion();
        #endregion
    }

    #region Clases de Apoyo (en español)
    /// <summary>
    /// Configuración del sistema SALC
    /// </summary>
    public class ConfiguracionSistema
    {
        public string RutaCopiasSeguridad { get; set; }
        public int DiasRetencionCopias { get; set; }
        public TimeSpan HoraCopiaDiaria { get; set; }
        public bool CopiaAutomaticaHabilitada { get; set; }
        public string ServidorEmail { get; set; }
        public int PuertoEmail { get; set; }
        public string UsuarioEmail { get; set; }
        public bool EmailHabilitado { get; set; }
        public int TiempoExpiracionSesion { get; set; }
        public bool LogearAccesos { get; set; }
    }

    /// <summary>
    /// Estadísticas del sistema SALC
    /// </summary>
    public class EstadisticasSistema
    {
        public int TotalUsuarios { get; set; }
        public int TotalPacientes { get; set; }
        public int TotalAnalisis { get; set; }
        public int AnalisisPendientes { get; set; }
        public int AnalisisVerificados { get; set; }
        public DateTime UltimaCopiaSeguridad { get; set; }
        public long TamanoDatabaseMB { get; set; }
        public string VersionSistema { get; set; }
    }
    #endregion
}
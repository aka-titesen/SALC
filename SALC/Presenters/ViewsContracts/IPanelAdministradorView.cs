using System;

namespace SALC.Presenters.ViewsContracts
{
    public interface IPanelAdministradorView
    {
        // Usuarios
        event EventHandler UsuariosNuevoClick;
        event EventHandler UsuariosEditarClick;
        event EventHandler UsuariosEliminarClick;
        event EventHandler<string> UsuariosBuscarTextoChanged;
        event EventHandler UsuariosDetalleClick;
        event EventHandler<string> UsuariosFiltroEstadoChanged;

        // Datos/selección Usuarios
        void CargarUsuarios(System.Collections.IEnumerable usuarios);
        int? ObtenerUsuarioSeleccionadoDni();

        // Catálogos - Obras Sociales
        event EventHandler ObrasSocialesNuevoClick;
        event EventHandler ObrasSocialesEditarClick;
        event EventHandler ObrasSocialesEliminarClick;
        event EventHandler<string> ObrasSocialesBuscarTextoChanged;
        event EventHandler<string> ObrasSocialesFiltroEstadoChanged;

        // Datos/selección Obras Sociales
        void CargarObrasSociales(System.Collections.IEnumerable obrasSociales);
        int? ObtenerObraSocialSeleccionadaId();

        // Catálogos - Tipos de Análisis 
        event EventHandler TiposAnalisisNuevoClick;
        event EventHandler TiposAnalisisEditarClick;
        event EventHandler TiposAnalisisEliminarClick;
        event EventHandler<string> TiposAnalisisBuscarTextoChanged;
        event EventHandler<string> TiposAnalisisFiltroEstadoChanged;

        // Datos/selección Tipos de Análisis
        void CargarTiposAnalisis(System.Collections.IEnumerable tiposAnalisis);
        int? ObtenerTipoAnalisisSeleccionadoId();

        // Catálogos - Métricas
        event EventHandler MetricasNuevoClick;
        event EventHandler MetricasEditarClick;
        event EventHandler MetricasEliminarClick;
        event EventHandler<string> MetricasBuscarTextoChanged;
        event EventHandler<string> MetricasFiltroEstadoChanged;

        // Datos/selección Métricas
        void CargarMetricas(System.Collections.IEnumerable metricas);
        int? ObtenerMetricaSeleccionadaId();

        // Relaciones Tipo Análisis - Métricas
        event EventHandler RelacionesTipoAnalisisMetricaGestionarClick;
        
        // Backups
        event EventHandler EjecutarBackupClick;
        event EventHandler ProgramarBackupClick;

        // Salud de BD
        event EventHandler ProbarConexionClick;

        // Mensajes
        void MostrarMensaje(string texto, string titulo = "SALC", bool esError = false);
    }
}

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

        // Datos/selección Usuarios
        void CargarUsuarios(System.Collections.IEnumerable usuarios);
        int? ObtenerUsuarioSeleccionadoDni();

        // Pacientes
        event EventHandler PacientesNuevoClick;
        event EventHandler PacientesEditarClick;
        event EventHandler PacientesEliminarClick;
        event EventHandler<string> PacientesBuscarTextoChanged;

        // Datos/selección Pacientes
        void CargarPacientes(System.Collections.IEnumerable pacientes);
        int? ObtenerPacienteSeleccionadoDni();

        // Catálogos
        event EventHandler ObrasSocialesNuevoClick;
        event EventHandler ObrasSocialesEditarClick;
        event EventHandler ObrasSocialesEliminarClick;
        event EventHandler<string> ObrasSocialesBuscarTextoChanged;

        // Datos/selección Obras Sociales
        void CargarObrasSociales(System.Collections.IEnumerable obrasSociales);
        int? ObtenerObraSocialSeleccionadaId();

        event EventHandler TiposAnalisisNuevoClick;
        event EventHandler TiposAnalisisEditarClick;
        event EventHandler TiposAnalisisEliminarClick;
        event EventHandler<string> TiposAnalisisBuscarTextoChanged;

        // Datos/selección Tipos de Análisis
        void CargarTiposAnalisis(System.Collections.IEnumerable tiposAnalisis);
        int? ObtenerTipoAnalisisSeleccionadoId();

        event EventHandler MetricasNuevoClick;
        event EventHandler MetricasEditarClick;
        event EventHandler MetricasEliminarClick;
        event EventHandler<string> MetricasBuscarTextoChanged;

        // Datos/selección Métricas
        void CargarMetricas(System.Collections.IEnumerable metricas);
        int? ObtenerMetricaSeleccionadaId();

        // Backups
        event EventHandler EjecutarBackupClick;
        event EventHandler ProgramarBackupClick;

        // Salud de BD
        event EventHandler ProbarConexionClick;

        // Mensajes
        void MostrarMensaje(string texto, string titulo = "SALC", bool esError = false);
    }
}

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

        // Pacientes
        event EventHandler PacientesNuevoClick;
        event EventHandler PacientesEditarClick;
        event EventHandler PacientesEliminarClick;
        event EventHandler<string> PacientesBuscarTextoChanged;

        // Catálogos
        event EventHandler ObrasSocialesNuevoClick;
        event EventHandler ObrasSocialesEditarClick;
        event EventHandler ObrasSocialesEliminarClick;
        event EventHandler<string> ObrasSocialesBuscarTextoChanged;

        event EventHandler TiposAnalisisNuevoClick;
        event EventHandler TiposAnalisisEditarClick;
        event EventHandler TiposAnalisisEliminarClick;
        event EventHandler<string> TiposAnalisisBuscarTextoChanged;

        event EventHandler MetricasNuevoClick;
        event EventHandler MetricasEditarClick;
        event EventHandler MetricasEliminarClick;
        event EventHandler<string> MetricasBuscarTextoChanged;

        // Backups
        event EventHandler EjecutarBackupClick;
        event EventHandler ProgramarBackupClick;
    }
}

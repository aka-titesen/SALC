using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface IBackupService
    {
        // Ejecuci�n de backups
        void EjecutarBackup(string rutaArchivoBak, int? dniUsuario = null, string tipoBackup = "Manual");
        void EjecutarBackupAutomatico();
        
        // Gesti�n del historial
        List<HistorialBackup> ObtenerHistorialBackups(int limite = 50);
        HistorialBackup ObtenerUltimoBackup();
        void LimpiarBackupsAntiguos(int diasRetencion);
        
        // Configuraci�n
        ConfiguracionBackup ObtenerConfiguracion();
        void ActualizarConfiguracion(ConfiguracionBackup config);
        
        // Programaci�n autom�tica
        void ProgramarBackup(string expresionHorario, string rutaArchivoBak);
        void ProgramarBackupAutomatico(ConfiguracionBackup config);
        void EliminarTareaProgramada();
        bool ExisteTareaProgramada();
        string ObtenerEstadoTarea();
        
        // Utilidades
        string FormatearTamanoArchivo(long bytes);
    }
}

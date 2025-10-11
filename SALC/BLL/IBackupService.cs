namespace SALC.BLL
{
    public interface IBackupService
    {
        void EjecutarBackup(string rutaArchivoBak);
        void ProgramarBackup(string expresionHorario, string rutaArchivoBak);
    }
}

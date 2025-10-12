using System;
using System.Data.SqlClient;
using System.IO;
using SALC.Infraestructura;

namespace SALC.BLL
{
    public class BackupService : IBackupService
    {
        public void EjecutarBackup(string rutaArchivoBak)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivoBak))
                throw new ArgumentException("Ruta de backup inválida.", nameof(rutaArchivoBak));
            var dir = Path.GetDirectoryName(rutaArchivoBak);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            // Detectar base de datos del connection string
            var cs = System.Configuration.ConfigurationManager.ConnectionStrings["SALC"].ConnectionString;
            var builder = new SqlConnectionStringBuilder(cs);
            var database = builder.InitialCatalog;

            var tsql = $"BACKUP DATABASE [{database}] TO DISK = @ruta WITH INIT, STATS = 5";
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(tsql, cn))
            {
                cmd.Parameters.AddWithValue("@ruta", rutaArchivoBak);
                cn.Open();
                cmd.CommandTimeout = 0; // permitir backups largos
                cmd.ExecuteNonQuery();
            }
        }

        public void ProgramarBackup(string expresionHorario, string rutaArchivoBak)
        {
            // TODO: Definir integración con Programador de tareas de Windows.
            // Por ahora, no implementado.
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SALC.Views;
using SALC.BLL;
using SALC.DAL;

namespace SALC
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Verificar si se está ejecutando un backup automático
            if (args != null && args.Length > 0)
            {
                if (args.Contains("/backup") && args.Contains("/auto"))
                {
                    EjecutarBackupAutomatico();
                    return; // Salir sin mostrar interfaz gráfica
                }
            }
            
            // Iniciar directamente con el login
            var login = new FrmLogin();
            
            // Inyección manual mínima
            var usuariosRepo = new UsuarioRepositorio();
            var hasher = new DefaultPasswordHasher();
            var auth = new AutenticacionService(usuariosRepo, hasher);
            var presenter = new SALC.Presenters.LoginPresenter(login, auth);
            
            // Ejecutar la aplicación con el login como formulario principal
            Application.Run(login);
        }

        private static void EjecutarBackupAutomatico()
        {
            try
            {
                var backupService = new BackupService();
                backupService.EjecutarBackupAutomatico();
                
                // Log del resultado (opcional - podría escribir a Event Log de Windows)
                System.Diagnostics.EventLog.WriteEntry("SALC", 
                    "Backup automático ejecutado correctamente", 
                    System.Diagnostics.EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                // Log del error
                System.Diagnostics.EventLog.WriteEntry("SALC", 
                    $"Error en backup automático: {ex.Message}", 
                    System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}

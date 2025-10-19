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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
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
    }
}

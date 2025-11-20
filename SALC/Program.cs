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
    /// <summary>
    /// Clase principal que contiene el punto de entrada de la aplicación.
    /// Inicializa la configuración de Windows Forms y la pantalla de login.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal de la aplicación.
        /// Configura Windows Forms, inicializa los servicios necesarios y muestra la pantalla de login.
        /// </summary>
        /// <param name="args">Argumentos de línea de comandos</param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Crear la vista de login
            var login = new FrmLogin();
            
            // Configurar servicios mediante inyección de dependencias manual
            var usuariosRepo = new UsuarioRepositorio();
            var hasher = new DefaultPasswordHasher();
            var auth = new AutenticacionService(usuariosRepo, hasher);
            var presenter = new SALC.Presenters.LoginPresenter(login, auth);
            
            // Ejecutar la aplicación con el login como formulario principal
            Application.Run(login);
        }
    }
}

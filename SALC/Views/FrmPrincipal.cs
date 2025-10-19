using System;
using System.Windows.Forms;
using SALC.BLL;
using SALC.DAL;

namespace SALC.Views
{
    public class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            Text = "SALC - Iniciando Sistema";
            Size = new System.Drawing.Size(400, 300);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            
            Load += FrmPrincipal_Load;
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            try
            {
                // Crear y mostrar el login
                var login = new FrmLogin();
                
                // Inyección manual mínima
                var usuariosRepo = new UsuarioRepositorio();
                var hasher = new DefaultPasswordHasher();
                var auth = new AutenticacionService(usuariosRepo, hasher);
                var presenter = new SALC.Presenters.LoginPresenter(login, auth);
                
                // Mostrar el login como diálogo
                var result = login.ShowDialog();
                
                // Después del login, cerrar este formulario
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el sistema: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}

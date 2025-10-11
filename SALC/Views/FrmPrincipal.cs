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
            IsMdiContainer = true;
            Text = "SALC - Sistema de Administración de Laboratorio Clínico";
            WindowState = FormWindowState.Maximized;
            Load += FrmPrincipal_Load;
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            var login = new FrmLogin { MdiParent = this };
            // Inyección manual mínima
            var usuariosRepo = new UsuarioRepositorio();
            var hasher = new DefaultPasswordHasher();
            var auth = new AutenticacionService(usuariosRepo, hasher);
            var presenter = new SALC.Presenters.LoginPresenter(login, auth);
            login.Show();
        }
    }
}

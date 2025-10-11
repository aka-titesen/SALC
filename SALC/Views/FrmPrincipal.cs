using System;
using System.Windows.Forms;

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
            login.Show();
        }
    }
}

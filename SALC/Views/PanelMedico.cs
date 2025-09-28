using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class PanelMedico : Form
    {
        public PanelMedico()
        {
            InitializeComponent();
        }

        private void btnHistoricos_Click(object sender, EventArgs e)
        {
            this.Hide();

            var f = new HistorialEstudiosForm();
            f.ShowDialog(this);

            this.Show();
        }
    }
}

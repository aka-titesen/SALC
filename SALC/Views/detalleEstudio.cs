using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SALC.Views
{
    public partial class detalleEstudio : Form
    {
        public detalleEstudio()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarMetrica metricaAgregar = new AgregarMetrica();
            metricaAgregar.ShowDialog();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            editarMetrica metricaEditar = new editarMetrica();
            metricaEditar.ShowDialog();
        }
    }
}

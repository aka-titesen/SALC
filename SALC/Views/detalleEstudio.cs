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
            // TODO: Implementar formulario de edición de métricas
            // Referencia a editarMetrica eliminada - archivo obsoleto
            MessageBox.Show("Funcionalidad de edición de métrica en desarrollo.", "En desarrollo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

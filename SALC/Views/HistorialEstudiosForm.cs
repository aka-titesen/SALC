using SALC.Views;
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
    public partial class HistorialEstudiosForm : Form
    {
        public HistorialEstudiosForm()
        {
            InitializeComponent();
        }

        private void HistorialEstudiosForm_Load(object sender, EventArgs e)
        {

        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {

        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDetalles_Click(object sender, EventArgs e)
        {
            detalleEstudio detalle = new detalleEstudio();
            detalle.ShowDialog();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            agregar_estudio estudio = new agregar_estudio();
            estudio.ShowDialog();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            editarEstudio editar_estudio = new editarEstudio();
            editar_estudio.ShowDialog();
        }
    }
}

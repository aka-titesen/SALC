using System;
using System.Windows.Forms;
using SALC.Domain;

namespace SALC.Views.PanelAdministrador
{
    public class FrmObraSocialEdit : Form
    {
        private TextBox txtCuit, txtNombre;
        private ComboBox cboEstado;
        private Button btnOk, btnCancel;
        private int? idOriginal;

        public FrmObraSocialEdit(ObraSocial existente = null)
        {
            Text = existente == null ? "Nueva Obra Social" : "Editar Obra Social";
            Width = 420; Height = 250; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            var lblCuit = new Label { Text = "CUIT:", Left = 20, Top = 20, Width = 120 };
            txtCuit = new TextBox { Left = 150, Top = 18, Width = 200, MaxLength = 13 };
            
            var lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 55, Width = 120 };
            txtNombre = new TextBox { Left = 150, Top = 53, Width = 200, MaxLength = 50 };
            
            // Agregar combo de estado
            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 90, Width = 120 };
            cboEstado = new ComboBox { Left = 150, Top = 88, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            btnOk = new Button { Text = "Aceptar", Left = 170, Top = 140, Width = 80, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancelar", Left = 260, Top = 140, Width = 90, DialogResult = DialogResult.Cancel };
            AcceptButton = btnOk; CancelButton = btnCancel;
            btnOk.Click += (s, e) => { if (!Validar()) this.DialogResult = DialogResult.None; };

            Controls.AddRange(new Control[] { 
                lblCuit, txtCuit, lblNombre, txtNombre, lblEstado, cboEstado, btnOk, btnCancel 
            });

            if (existente != null)
            {
                idOriginal = existente.IdObraSocial;
                txtCuit.Text = existente.Cuit;
                txtCuit.Enabled = false; // No permitir modificar el CUIT
                txtNombre.Text = existente.Nombre;
                cboEstado.SelectedItem = existente.Estado ?? "Activo";
            }
            else
            {
                // Valores por defecto para nueva obra social
                cboEstado.SelectedItem = "Activo";
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtCuit.Text)) 
            { 
                MessageBox.Show("CUIT requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtCuit.Focus(); 
                return false; 
            }

            var cuit = txtCuit.Text.Trim();
            if (cuit.Length < 10 || cuit.Length > 13)
            {
                MessageBox.Show("CUIT debe tener entre 10 y 13 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCuit.Focus();
                return false;
            }

            // Validar que solo contenga números y guiones
            foreach (char c in cuit)
            {
                if (!char.IsDigit(c) && c != '-')
                {
                    MessageBox.Show("CUIT solo puede contener números y guiones.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCuit.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text)) 
            { 
                MessageBox.Show("Nombre requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtNombre.Focus(); 
                return false; 
            }
            
            if (cboEstado.SelectedItem == null) 
            { 
                MessageBox.Show("Seleccione un estado.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                cboEstado.Focus(); 
                return false; 
            }
            
            return true;
        }

        public ObraSocial ObtenerObraSocial()
        {
            return new ObraSocial
            {
                IdObraSocial = idOriginal ?? 0, // Para nuevas será 0, se asigna en la BD
                Cuit = txtCuit.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };
        }
    }
}
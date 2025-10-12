using System;
using System.Windows.Forms;
using SALC.Domain;

namespace SALC.Views.PanelAdministrador
{
    public class FrmTipoAnalisisEdit : Form
    {
        private TextBox txtDescripcion;
        private ComboBox cboEstado;
        private Button btnOk, btnCancel;
        private int? idOriginal;

        public FrmTipoAnalisisEdit(TipoAnalisis existente = null)
        {
            Text = existente == null ? "Nuevo Tipo de Análisis" : "Editar Tipo de Análisis";
            Width = 420; Height = 220; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            var lblDescripcion = new Label { Text = "Descripción:", Left = 20, Top = 20, Width = 120 };
            txtDescripcion = new TextBox { Left = 150, Top = 18, Width = 200, MaxLength = 100 };
            
            // Agregar combo de estado
            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 55, Width = 120 };
            cboEstado = new ComboBox { Left = 150, Top = 53, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            btnOk = new Button { Text = "Aceptar", Left = 170, Top = 110, Width = 80, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancelar", Left = 260, Top = 110, Width = 90, DialogResult = DialogResult.Cancel };
            AcceptButton = btnOk; CancelButton = btnCancel;
            btnOk.Click += (s, e) => { if (!Validar()) this.DialogResult = DialogResult.None; };

            Controls.AddRange(new Control[] { 
                lblDescripcion, txtDescripcion, lblEstado, cboEstado, btnOk, btnCancel 
            });

            if (existente != null)
            {
                idOriginal = existente.IdTipoAnalisis;
                txtDescripcion.Text = existente.Descripcion;
                cboEstado.SelectedItem = existente.Estado ?? "Activo";
            }
            else
            {
                // Valores por defecto para nuevo tipo de análisis
                cboEstado.SelectedItem = "Activo";
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text)) 
            { 
                MessageBox.Show("Descripción requerida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtDescripcion.Focus(); 
                return false; 
            }

            if (txtDescripcion.Text.Trim().Length < 3)
            {
                MessageBox.Show("La descripción debe tener al menos 3 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
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

        public TipoAnalisis ObtenerTipoAnalisis()
        {
            return new TipoAnalisis
            {
                IdTipoAnalisis = idOriginal ?? 0, // Para nuevos será 0, se asigna en la BD
                Descripcion = txtDescripcion.Text.Trim(),
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };
        }
    }
}
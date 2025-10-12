using System;
using System.Windows.Forms;
using SALC.Domain;

namespace SALC.Views.PanelAdministrador
{
    public class FrmMetricaEdit : Form
    {
        private TextBox txtNombre, txtUnidadMedida, txtValorMinimo, txtValorMaximo;
        private ComboBox cboEstado;
        private Button btnOk, btnCancel;
        private int? idOriginal;

        public FrmMetricaEdit(Metrica existente = null)
        {
            Text = existente == null ? "Nueva Métrica" : "Editar Métrica";
            Width = 420; Height = 320; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            var lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 20, Width = 120 };
            txtNombre = new TextBox { Left = 150, Top = 18, Width = 200, MaxLength = 100 };
            
            var lblUnidadMedida = new Label { Text = "Unidad de Medida:", Left = 20, Top = 55, Width = 120 };
            txtUnidadMedida = new TextBox { Left = 150, Top = 53, Width = 200, MaxLength = 20 };
            
            var lblValorMinimo = new Label { Text = "Valor Mínimo:", Left = 20, Top = 90, Width = 120 };
            txtValorMinimo = new TextBox { Left = 150, Top = 88, Width = 200 };
            
            var lblValorMaximo = new Label { Text = "Valor Máximo:", Left = 20, Top = 125, Width = 120 };
            txtValorMaximo = new TextBox { Left = 150, Top = 123, Width = 200 };
            
            // Agregar combo de estado
            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 160, Width = 120 };
            cboEstado = new ComboBox { Left = 150, Top = 158, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            // Agregar etiquetas de ayuda
            var lblAyudaMin = new Label { 
                Text = "(Opcional - Solo números decimales)", 
                Left = 150, Top = 110, Width = 200, 
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F)
            };
            var lblAyudaMax = new Label { 
                Text = "(Opcional - Solo números decimales)", 
                Left = 150, Top = 145, Width = 200, 
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F)
            };

            btnOk = new Button { Text = "Aceptar", Left = 170, Top = 210, Width = 80, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancelar", Left = 260, Top = 210, Width = 90, DialogResult = DialogResult.Cancel };
            AcceptButton = btnOk; CancelButton = btnCancel;
            btnOk.Click += (s, e) => { if (!Validar()) this.DialogResult = DialogResult.None; };

            Controls.AddRange(new Control[] { 
                lblNombre, txtNombre, lblUnidadMedida, txtUnidadMedida, 
                lblValorMinimo, txtValorMinimo, lblAyudaMin,
                lblValorMaximo, txtValorMaximo, lblAyudaMax,
                lblEstado, cboEstado, btnOk, btnCancel 
            });

            if (existente != null)
            {
                idOriginal = existente.IdMetrica;
                txtNombre.Text = existente.Nombre;
                txtUnidadMedida.Text = existente.UnidadMedida;
                txtValorMinimo.Text = existente.ValorMinimo?.ToString() ?? "";
                txtValorMaximo.Text = existente.ValorMaximo?.ToString() ?? "";
                cboEstado.SelectedItem = existente.Estado ?? "Activo";
            }
            else
            {
                // Valores por defecto para nueva métrica
                cboEstado.SelectedItem = "Activo";
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) 
            { 
                MessageBox.Show("Nombre requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtNombre.Focus(); 
                return false; 
            }

            if (txtNombre.Text.Trim().Length < 2)
            {
                MessageBox.Show("El nombre debe tener al menos 2 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUnidadMedida.Text)) 
            { 
                MessageBox.Show("Unidad de medida requerida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtUnidadMedida.Focus(); 
                return false; 
            }

            // Validar valores mínimo y máximo si se proporcionan
            decimal? valorMinimo = null, valorMaximo = null;

            if (!string.IsNullOrWhiteSpace(txtValorMinimo.Text))
            {
                if (!decimal.TryParse(txtValorMinimo.Text.Trim(), out decimal min))
                {
                    MessageBox.Show("El valor mínimo debe ser un número decimal válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtValorMinimo.Focus();
                    return false;
                }
                valorMinimo = min;
            }

            if (!string.IsNullOrWhiteSpace(txtValorMaximo.Text))
            {
                if (!decimal.TryParse(txtValorMaximo.Text.Trim(), out decimal max))
                {
                    MessageBox.Show("El valor máximo debe ser un número decimal válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtValorMaximo.Focus();
                    return false;
                }
                valorMaximo = max;
            }

            // Validar que mínimo <= máximo si ambos están presentes
            if (valorMinimo.HasValue && valorMaximo.HasValue && valorMinimo.Value > valorMaximo.Value)
            {
                MessageBox.Show("El valor mínimo no puede ser mayor que el valor máximo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValorMinimo.Focus();
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

        public Metrica ObtenerMetrica()
        {
            decimal? valorMinimo = null, valorMaximo = null;

            if (!string.IsNullOrWhiteSpace(txtValorMinimo.Text) && decimal.TryParse(txtValorMinimo.Text.Trim(), out decimal min))
                valorMinimo = min;

            if (!string.IsNullOrWhiteSpace(txtValorMaximo.Text) && decimal.TryParse(txtValorMaximo.Text.Trim(), out decimal max))
                valorMaximo = max;

            return new Metrica
            {
                IdMetrica = idOriginal ?? 0, // Para nuevas será 0, se asigna en la BD
                Nombre = txtNombre.Text.Trim(),
                UnidadMedida = txtUnidadMedida.Text.Trim(),
                ValorMinimo = valorMinimo,
                ValorMaximo = valorMaximo,
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };
        }
    }
}
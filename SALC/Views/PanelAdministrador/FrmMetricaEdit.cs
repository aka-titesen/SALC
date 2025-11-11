using System;
using System.Windows.Forms;
using System.Drawing;
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
            Width = 450;
            Height = 450;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            // Título del formulario
            var lblTitulo = new Label
            {
                Text = existente == null ? "Crear Nueva Métrica de Laboratorio" : "Modificar Métrica",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, 15),
                Size = new Size(400, 30),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = existente == null
                    ? "Defina los parámetros de la nueva métrica de análisis"
                    : "Actualice la información de la métrica",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(400, 20),
                BackColor = Color.Transparent
            };

            // Campos
            var lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 80, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtNombre = new TextBox { Left = 150, Top = 78, Width = 250, MaxLength = 100, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblUnidadMedida = new Label { Text = "Unidad Medida:", Left = 20, Top = 115, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtUnidadMedida = new TextBox { Left = 150, Top = 113, Width = 250, MaxLength = 20, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblValorMinimo = new Label { Text = "Valor Mínimo:", Left = 20, Top = 150, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtValorMinimo = new TextBox { Left = 150, Top = 148, Width = 250, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblAyudaMin = new Label
            {
                Text = "Valor mínimo de referencia (opcional, solo números)",
                Left = 150,
                Top = 175,
                Width = 280,
                Height = 15,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                BackColor = Color.Transparent
            };

            var lblValorMaximo = new Label { Text = "Valor Máximo:", Left = 20, Top = 200, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtValorMaximo = new TextBox { Left = 150, Top = 198, Width = 250, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblAyudaMax = new Label
            {
                Text = "Valor máximo de referencia (opcional, solo números)",
                Left = 150,
                Top = 225,
                Width = 280,
                Height = 15,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                BackColor = Color.Transparent
            };

            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 250, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            cboEstado = new ComboBox { Left = 150, Top = 248, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            // Nota informativa
            var lblNota = new Label
            {
                Text = "Ejemplos: Glucosa (mg/dL), Hemoglobina (g/dL), Colesterol (mg/dL)",
                Left = 20,
                Top = 285,
                Width = 400,
                Height = 30,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                BackColor = Color.Transparent
            };

            // Botones
            btnOk = new Button
            {
                Text = existente == null ? "Crear Métrica" : "Guardar Cambios",
                Left = 180,
                Top = 365,
                Width = 120,
                Height = 35,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;

            btnCancel = new Button
            {
                Text = "Cancelar",
                Left = 310,
                Top = 365,
                Width = 90,
                Height = 35,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            AcceptButton = btnOk;
            CancelButton = btnCancel;
            btnOk.Click += (s, e) => { if (!Validar()) this.DialogResult = DialogResult.None; };

            Controls.AddRange(new Control[]
            {
                lblTitulo, lblSubtitulo,
                lblNombre, txtNombre,
                lblUnidadMedida, txtUnidadMedida,
                lblValorMinimo, txtValorMinimo, lblAyudaMin,
                lblValorMaximo, txtValorMaximo, lblAyudaMax,
                lblEstado, cboEstado,
                lblNota,
                btnOk, btnCancel
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
                cboEstado.SelectedItem = "Activo";
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("La unidad de medida es obligatoria.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnidadMedida.Focus();
                return false;
            }

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
                IdMetrica = idOriginal ?? 0,
                Nombre = txtNombre.Text.Trim(),
                UnidadMedida = txtUnidadMedida.Text.Trim(),
                ValorMinimo = valorMinimo,
                ValorMaximo = valorMaximo,
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };
        }
    }
}
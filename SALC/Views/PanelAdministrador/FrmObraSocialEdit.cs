using System;
using System.Windows.Forms;
using System.Drawing;
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
            Width = 450;
            Height = 350;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            // Título del formulario
            var lblTitulo = new Label
            {
                Text = existente == null ? "Crear Nueva Obra Social" : "Modificar Obra Social",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, 15),
                Size = new Size(400, 30),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = existente == null
                    ? "Complete los datos de la nueva obra social"
                    : "Actualice la información de la obra social",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(400, 20),
                BackColor = Color.Transparent
            };

            // Campos
            var lblCuit = new Label { Text = "CUIT:", Left = 20, Top = 80, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtCuit = new TextBox { Left = 150, Top = 78, Width = 250, MaxLength = 13, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 115, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtNombre = new TextBox { Left = 150, Top = 113, Width = 250, MaxLength = 50, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 150, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            cboEstado = new ComboBox { Left = 150, Top = 148, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            // Nota explicativa
            var lblNota = new Label
            {
                Text = "El CUIT debe tener formato XX-XXXXXXXX-X (con guiones opcionales)",
                Left = 20,
                Top = 185,
                Width = 400,
                Height = 30,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                BackColor = Color.Transparent
            };

            // Botones
            btnOk = new Button
            {
                Text = existente == null ? "Crear Obra Social" : "Guardar Cambios",
                Left = 160,
                Top = 265,
                Width = 140,
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
                Top = 265,
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
                lblCuit, txtCuit,
                lblNombre, txtNombre,
                lblEstado, cboEstado,
                lblNota,
                btnOk, btnCancel
            });

            if (existente != null)
            {
                idOriginal = existente.IdObraSocial;
                txtCuit.Text = existente.Cuit;
                txtCuit.Enabled = false;
                txtCuit.BackColor = Color.FromArgb(240, 240, 240);
                txtNombre.Text = existente.Nombre;
                cboEstado.SelectedItem = existente.Estado ?? "Activo";

                // Nota sobre CUIT bloqueado
                var lblNotaCuit = new Label
                {
                    Text = "El CUIT no puede modificarse una vez creada la obra social",
                    Left = 20,
                    Top = 215,
                    Width = 400,
                    Height = 15,
                    Font = new Font("Segoe UI", 8, FontStyle.Italic),
                    ForeColor = Color.FromArgb(192, 57, 43),
                    BackColor = Color.Transparent
                };
                Controls.Add(lblNotaCuit);
            }
            else
            {
                cboEstado.SelectedItem = "Activo";
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtCuit.Text))
            {
                MessageBox.Show("El CUIT es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCuit.Focus();
                return false;
            }

            var cuit = txtCuit.Text.Trim();
            if (cuit.Length < 10 || cuit.Length > 13)
            {
                MessageBox.Show("El CUIT debe tener entre 10 y 13 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCuit.Focus();
                return false;
            }

            foreach (char c in cuit)
            {
                if (!char.IsDigit(c) && c != '-')
                {
                    MessageBox.Show("El CUIT solo puede contener números y guiones.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCuit.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                IdObraSocial = idOriginal ?? 0,
                Cuit = txtCuit.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };
        }
    }
}
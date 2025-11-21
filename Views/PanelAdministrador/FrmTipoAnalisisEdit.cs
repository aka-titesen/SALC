using System;
using System.Windows.Forms;
using System.Drawing;
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
            Width = 450;
            Height = 300;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            // Título del formulario
            var lblTitulo = new Label
            {
                Text = existente == null ? "Crear Nuevo Tipo de Análisis" : "Modificar Tipo de Análisis",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, 15),
                Size = new Size(400, 30),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = existente == null
                    ? "Defina la descripción del nuevo tipo de análisis clínico"
                    : "Actualice la información del tipo de análisis",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(400, 20),
                BackColor = Color.Transparent
            };

            // Campos
            var lblDescripcion = new Label { Text = "Descripción:", Left = 20, Top = 80, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtDescripcion = new TextBox { Left = 150, Top = 78, Width = 250, MaxLength = 100, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 115, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            cboEstado = new ComboBox { Left = 150, Top = 113, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            // Nota informativa
            var lblNota = new Label
            {
                Text = "Ejemplos: Hemograma Completo, Glucemia, Perfil Lipídico, etc.",
                Left = 20,
                Top = 150,
                Width = 400,
                Height = 30,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                BackColor = Color.Transparent
            };

            // Botones
            btnOk = new Button
            {
                Text = existente == null ? "Crear Tipo" : "Guardar Cambios",
                Left = 180,
                Top = 215,
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
                Top = 215,
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
                lblDescripcion, txtDescripcion,
                lblEstado, cboEstado,
                lblNota,
                btnOk, btnCancel
            });

            if (existente != null)
            {
                idOriginal = existente.IdTipoAnalisis;
                txtDescripcion.Text = existente.Descripcion;
                cboEstado.SelectedItem = existente.Estado ?? "Activo";
            }
            else
            {
                cboEstado.SelectedItem = "Activo";
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("La descripción es obligatoria.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                IdTipoAnalisis = idOriginal ?? 0,
                Descripcion = txtDescripcion.Text.Trim(),
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };
        }
    }
}
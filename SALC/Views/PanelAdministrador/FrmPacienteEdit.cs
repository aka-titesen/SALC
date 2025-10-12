using System;
using System.Windows.Forms;
using SALC.Domain;

namespace SALC.Views.PanelAdministrador
{
    public class FrmPacienteEdit : Form
    {
        private TextBox txtDni, txtNombre, txtApellido, txtEmail, txtTelefono, txtSexo;
        private DateTimePicker dtpFechaNac;
        private ComboBox cboEstado;
        private Button btnOk, btnCancel;
        private int? dniOriginal;

        public FrmPacienteEdit(Paciente existente = null)
        {
            Text = existente == null ? "Nuevo Paciente" : "Editar Paciente";
            Width = 420; Height = 400; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            var lblDni = new Label { Text = "DNI:", Left = 20, Top = 20, Width = 120 };
            txtDni = new TextBox { Left = 150, Top = 18, Width = 200 };
            var lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 55, Width = 120 };
            txtNombre = new TextBox { Left = 150, Top = 53, Width = 200 };
            var lblApellido = new Label { Text = "Apellido:", Left = 20, Top = 90, Width = 120 };
            txtApellido = new TextBox { Left = 150, Top = 88, Width = 200 };
            var lblFecha = new Label { Text = "Fecha Nac:", Left = 20, Top = 125, Width = 120 };
            dtpFechaNac = new DateTimePicker { Left = 150, Top = 123, Width = 200, Format = DateTimePickerFormat.Short };
            var lblSexo = new Label { Text = "Sexo (M/F/X):", Left = 20, Top = 160, Width = 120 };
            txtSexo = new TextBox { Left = 150, Top = 158, Width = 60, MaxLength = 1 };
            var lblEmail = new Label { Text = "Email:", Left = 20, Top = 195, Width = 120 };
            txtEmail = new TextBox { Left = 150, Top = 193, Width = 200 };
            var lblTel = new Label { Text = "Teléfono:", Left = 20, Top = 230, Width = 120 };
            txtTelefono = new TextBox { Left = 150, Top = 228, Width = 200 };
            
            // Agregar combo de estado
            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 265, Width = 120 };
            cboEstado = new ComboBox { Left = 150, Top = 263, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            btnOk = new Button { Text = "Aceptar", Left = 170, Top = 310, Width = 80, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancelar", Left = 260, Top = 310, Width = 90, DialogResult = DialogResult.Cancel };
            AcceptButton = btnOk; CancelButton = btnCancel;
            btnOk.Click += (s, e) => { if (!Validar()) this.DialogResult = DialogResult.None; };

            Controls.AddRange(new Control[] { lblDni, txtDni, lblNombre, txtNombre, lblApellido, txtApellido, lblFecha, dtpFechaNac, lblSexo, txtSexo, lblEmail, txtEmail, lblTel, txtTelefono, lblEstado, cboEstado, btnOk, btnCancel });

            if (existente != null)
            {
                dniOriginal = existente.Dni;
                txtDni.Text = existente.Dni.ToString();
                txtDni.Enabled = false;
                txtNombre.Text = existente.Nombre;
                txtApellido.Text = existente.Apellido;
                dtpFechaNac.Value = existente.FechaNac;
                txtSexo.Text = existente.Sexo.ToString();
                txtEmail.Text = existente.Email;
                txtTelefono.Text = existente.Telefono;
                cboEstado.SelectedItem = existente.Estado ?? "Activo";
            }
            else
            {
                // Valores por defecto para nuevo paciente
                cboEstado.SelectedItem = "Activo";
            }
        }

        private bool Validar()
        {
            if (!dniOriginal.HasValue)
            {
                if (!int.TryParse(txtDni.Text.Trim(), out var dni) || dni <= 0)
                {
                    MessageBox.Show("DNI inválido.");
                    txtDni.Focus();
                    return false;
                }
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Nombre requerido."); txtNombre.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtApellido.Text)) { MessageBox.Show("Apellido requerido."); txtApellido.Focus(); return false; }
            var sx = (txtSexo.Text ?? string.Empty).Trim().ToUpperInvariant();
            if (sx != "M" && sx != "F" && sx != "X") { MessageBox.Show("Sexo debe ser M, F o X."); txtSexo.Focus(); return false; }
            if (dtpFechaNac.Value.Date > DateTime.Today) { MessageBox.Show("Fecha de nacimiento no puede ser futura."); dtpFechaNac.Focus(); return false; }
            if (cboEstado.SelectedItem == null) { MessageBox.Show("Seleccione un estado."); cboEstado.Focus(); return false; }
            return true;
        }

        public Paciente ObtenerPaciente()
        {
            return new Paciente
            {
                Dni = dniOriginal ?? int.Parse(txtDni.Text.Trim()),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                FechaNac = dtpFechaNac.Value.Date,
                Sexo = (txtSexo.Text ?? "").Trim().ToUpperInvariant()[0],
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                IdObraSocial = null, // editable en próxima iteración desde catálogo/combobox
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };
        }
    }
}

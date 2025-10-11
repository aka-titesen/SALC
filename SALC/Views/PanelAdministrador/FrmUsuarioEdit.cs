using System;
using System.Windows.Forms;
using SALC.Domain;

namespace SALC.Views.PanelAdministrador
{
    public class FrmUsuarioEdit : Form
    {
        private TextBox txtDni, txtNombre, txtApellido, txtEmail, txtPassword;
        private ComboBox cboRol, cboEstado;
        // Médico
        private TextBox txtMatricula, txtEspecialidad;
        // Asistente
        private TextBox txtDniSupervisor;
        private DateTimePicker dtpIngreso;

        private Button btnOk, btnCancel;
        private Usuario baseUsuario;

        public FrmUsuarioEdit(Usuario existente = null)
        {
            Text = existente == null ? "Nuevo Usuario" : "Editar Usuario";
            Width = 520; Height = 440; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            baseUsuario = existente;

            int L = 20, T = 20, Wl = 140, Wt = 300, Hstep = 30;
            Controls.Add(new Label { Text = "DNI:", Left = L, Top = T, Width = Wl });
            txtDni = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            T += Hstep;
            Controls.Add(new Label { Text = "Nombre:", Left = L, Top = T, Width = Wl });
            txtNombre = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            T += Hstep;
            Controls.Add(new Label { Text = "Apellido:", Left = L, Top = T, Width = Wl });
            txtApellido = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            T += Hstep;
            Controls.Add(new Label { Text = "Email:", Left = L, Top = T, Width = Wl });
            txtEmail = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            T += Hstep;
            Controls.Add(new Label { Text = "Contraseña:", Left = L, Top = T, Width = Wl });
            txtPassword = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt, PasswordChar = '•' };
            T += Hstep;
            Controls.Add(new Label { Text = "Rol:", Left = L, Top = T, Width = Wl });
            cboRol = new ComboBox { Left = L + Wl, Top = T - 2, Width = Wt, DropDownStyle = ComboBoxStyle.DropDownList };
            cboRol.Items.AddRange(new object[] { "Administrador", "Médico", "Asistente" });
            T += Hstep;
            Controls.Add(new Label { Text = "Estado:", Left = L, Top = T, Width = Wl });
            cboEstado = new ComboBox { Left = L + Wl, Top = T - 2, Width = Wt, DropDownStyle = ComboBoxStyle.DropDownList };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            // Línea separador
            T += Hstep + 10;
            Controls.Add(new Label { Text = "Datos de Médico:", Left = L, Top = T, Width = Wl + 80 });
            T += Hstep;
            Controls.Add(new Label { Text = "Matrícula:", Left = L, Top = T, Width = Wl });
            txtMatricula = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            T += Hstep;
            Controls.Add(new Label { Text = "Especialidad:", Left = L, Top = T, Width = Wl });
            txtEspecialidad = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };

            // Asistente
            T += Hstep + 10;
            Controls.Add(new Label { Text = "Datos de Asistente:", Left = L, Top = T, Width = Wl + 80 });
            T += Hstep;
            Controls.Add(new Label { Text = "DNI Supervisor:", Left = L, Top = T, Width = Wl });
            txtDniSupervisor = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            T += Hstep;
            Controls.Add(new Label { Text = "Fecha Ingreso:", Left = L, Top = T, Width = Wl });
            dtpIngreso = new DateTimePicker { Left = L + Wl, Top = T - 2, Width = Wt, Format = DateTimePickerFormat.Short };

            btnOk = new Button { Text = "Aceptar", Left = Width - 240, Top = Height - 100, Width = 90, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancelar", Left = Width - 140, Top = Height - 100, Width = 90, DialogResult = DialogResult.Cancel };
            AcceptButton = btnOk; CancelButton = btnCancel;
            btnOk.Click += (s, e) => { if (!Validar()) this.DialogResult = DialogResult.None; };
            Controls.Add(btnOk);
            Controls.Add(btnCancel);

            cboRol.SelectedIndexChanged += (s, e) => ToggleRoleSections();

            if (existente != null)
            {
                txtDni.Text = existente.Dni.ToString(); txtDni.Enabled = false;
                txtNombre.Text = existente.Nombre; txtApellido.Text = existente.Apellido; txtEmail.Text = existente.Email;
                cboRol.SelectedIndex = existente.IdRol - 1;
                cboEstado.SelectedItem = existente.Estado;
            }
            else
            {
                cboRol.SelectedIndex = 0; cboEstado.SelectedIndex = 0;
            }
            ToggleRoleSections();
        }

        private void ToggleRoleSections()
        {
            var rolIdx = cboRol.SelectedIndex; // 0=Admin,1=Medico,2=Asistente
            var isMed = rolIdx == 1; var isAsis = rolIdx == 2;
            txtMatricula.Enabled = isMed; txtEspecialidad.Enabled = isMed;
            txtDniSupervisor.Enabled = isAsis; dtpIngreso.Enabled = isAsis;
        }

        private bool Validar()
        {
            if (!int.TryParse(txtDni.Text.Trim(), out var dni) || dni <= 0) { MessageBox.Show("DNI inválido"); return false; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Nombre requerido"); return false; }
            if (string.IsNullOrWhiteSpace(txtApellido.Text)) { MessageBox.Show("Apellido requerido"); return false; }
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@")) { MessageBox.Show("Email inválido"); return false; }
            // En creación, exigir contraseña; en edición permitir dejar vacía para conservar
            if (baseUsuario == null && string.IsNullOrWhiteSpace(txtPassword.Text)) { MessageBox.Show("Contraseña requerida"); return false; }
            if (cboRol.SelectedIndex == 1) // Médico
            {
                if (!int.TryParse(txtMatricula.Text.Trim(), out var _)) { MessageBox.Show("Matrícula inválida"); return false; }
                if (string.IsNullOrWhiteSpace(txtEspecialidad.Text)) { MessageBox.Show("Especialidad requerida"); return false; }
            }
            if (cboRol.SelectedIndex == 2) // Asistente
            {
                if (!int.TryParse(txtDniSupervisor.Text.Trim(), out var _)) { MessageBox.Show("DNI Supervisor inválido"); return false; }
            }
            return true;
        }

        public (Usuario Usuario, Medico Medico, Asistente Asistente) ObtenerDatos()
        {
            var rolId = cboRol.SelectedIndex + 1;
            var usuario = new Usuario
            {
                Dni = int.Parse(txtDni.Text.Trim()),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                PasswordHash = string.IsNullOrWhiteSpace(txtPassword.Text) && baseUsuario != null ? baseUsuario.PasswordHash : txtPassword.Text,
                IdRol = rolId,
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };

            Medico medico = null; Asistente asistente = null;
            if (rolId == 2)
            {
                medico = new Medico
                {
                    Dni = usuario.Dni,
                    NroMatricula = int.Parse(txtMatricula.Text.Trim()),
                    Especialidad = txtEspecialidad.Text.Trim()
                };
            }
            else if (rolId == 3)
            {
                asistente = new Asistente
                {
                    Dni = usuario.Dni,
                    DniSupervisor = int.Parse(txtDniSupervisor.Text.Trim()),
                    FechaIngreso = dtpIngreso.Value.Date
                };
            }

            return (usuario, medico, asistente);
        }
    }
}

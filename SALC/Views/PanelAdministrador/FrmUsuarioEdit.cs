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
            Width = 520; Height = 500; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            baseUsuario = existente;

            InicializarControles();
            if (existente != null)
            {
                CargarDatosExistentes(existente);
            }
            else
            {
                cboRol.SelectedIndex = 0; 
                cboEstado.SelectedIndex = 0;
                dtpIngreso.Value = DateTime.Today;
            }
            ToggleRoleSections();
        }

        private void InicializarControles()
        {
            int L = 20, T = 20, Wl = 140, Wt = 300, Hstep = 30;

            // DNI
            var lblDni = new Label { Text = "DNI:", Left = L, Top = T, Width = Wl };
            txtDni = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            Controls.Add(lblDni);
            Controls.Add(txtDni);
            T += Hstep;

            // Nombre
            var lblNombre = new Label { Text = "Nombre:", Left = L, Top = T, Width = Wl };
            txtNombre = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            Controls.Add(lblNombre);
            Controls.Add(txtNombre);
            T += Hstep;

            // Apellido
            var lblApellido = new Label { Text = "Apellido:", Left = L, Top = T, Width = Wl };
            txtApellido = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            Controls.Add(lblApellido);
            Controls.Add(txtApellido);
            T += Hstep;

            // Email
            var lblEmail = new Label { Text = "Email:", Left = L, Top = T, Width = Wl };
            txtEmail = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            T += Hstep;

            // Contraseña
            var lblPassword = new Label { Text = "Contraseña:", Left = L, Top = T, Width = Wl };
            txtPassword = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt, PasswordChar = '•' };
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            T += Hstep;

            // Rol
            var lblRol = new Label { Text = "Rol:", Left = L, Top = T, Width = Wl };
            cboRol = new ComboBox { Left = L + Wl, Top = T - 2, Width = Wt, DropDownStyle = ComboBoxStyle.DropDownList };
            cboRol.Items.AddRange(new object[] { "Administrador", "Médico", "Asistente" });
            cboRol.SelectedIndexChanged += (s, e) => ToggleRoleSections();
            Controls.Add(lblRol);
            Controls.Add(cboRol);
            T += Hstep;

            // Estado
            var lblEstado = new Label { Text = "Estado:", Left = L, Top = T, Width = Wl };
            cboEstado = new ComboBox { Left = L + Wl, Top = T - 2, Width = Wt, DropDownStyle = ComboBoxStyle.DropDownList };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });
            Controls.Add(lblEstado);
            Controls.Add(cboEstado);

            // Línea separador para datos de Médico
            T += Hstep + 15;
            var lblMedicoSec = new Label { Text = "Datos de Médico:", Left = L, Top = T, Width = Wl + 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            Controls.Add(lblMedicoSec);
            T += 25;

            // Matrícula
            var lblMatricula = new Label { Text = "Matrícula:", Left = L, Top = T, Width = Wl };
            txtMatricula = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            Controls.Add(lblMatricula);
            Controls.Add(txtMatricula);
            T += Hstep;

            // Especialidad
            var lblEspecialidad = new Label { Text = "Especialidad:", Left = L, Top = T, Width = Wl };
            txtEspecialidad = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            Controls.Add(lblEspecialidad);
            Controls.Add(txtEspecialidad);

            // Línea separador para datos de Asistente
            T += Hstep + 15;
            var lblAsistenteSec = new Label { Text = "Datos de Asistente:", Left = L, Top = T, Width = Wl + 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            Controls.Add(lblAsistenteSec);
            T += 25;

            // DNI Supervisor
            var lblDniSupervisor = new Label { Text = "DNI Supervisor:", Left = L, Top = T, Width = Wl };
            txtDniSupervisor = new TextBox { Left = L + Wl, Top = T - 2, Width = Wt };
            Controls.Add(lblDniSupervisor);
            Controls.Add(txtDniSupervisor);
            T += Hstep;

            // Fecha Ingreso
            var lblFechaIngreso = new Label { Text = "Fecha Ingreso:", Left = L, Top = T, Width = Wl };
            dtpIngreso = new DateTimePicker { Left = L + Wl, Top = T - 2, Width = Wt, Format = DateTimePickerFormat.Short };
            Controls.Add(lblFechaIngreso);
            Controls.Add(dtpIngreso);

            // Botones
            btnOk = new Button { Text = "Aceptar", Left = Width - 240, Top = Height - 70, Width = 90, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancelar", Left = Width - 140, Top = Height - 70, Width = 90, DialogResult = DialogResult.Cancel };
            AcceptButton = btnOk; 
            CancelButton = btnCancel;
            btnOk.Click += (s, e) => { if (!Validar()) this.DialogResult = DialogResult.None; };
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
        }

        private void CargarDatosExistentes(Usuario existente)
        {
            txtDni.Text = existente.Dni.ToString(); 
            txtDni.ReadOnly = true; // Solo lectura en edición
            txtNombre.Text = existente.Nombre; 
            txtApellido.Text = existente.Apellido; 
            txtEmail.Text = existente.Email;
            cboRol.SelectedIndex = existente.IdRol - 1;
            cboRol.Enabled = false; // Deshabilitar cambio de rol en edición
            cboEstado.SelectedItem = existente.Estado;
            
            // Agregar información visual sobre por qué el rol está deshabilitado
            var lblRolInfo = new Label 
            { 
                Text = "(No se puede modificar el rol de un usuario existente)", 
                Left = cboRol.Left + cboRol.Width + 10, 
                Top = cboRol.Top + 2, 
                Width = 250, 
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Italic)
            };
            Controls.Add(lblRolInfo);
            
            // Para .NET Framework 4.7.2, usamos un enfoque alternativo al PlaceholderText
            if (baseUsuario != null)
            {
                txtPassword.ForeColor = System.Drawing.Color.Gray;
                txtPassword.Text = "Dejar vacío para mantener contraseña actual";
                txtPassword.Enter += (s, e) => {
                    if (txtPassword.Text == "Dejar vacío para mantener contraseña actual")
                    {
                        txtPassword.Text = "";
                        txtPassword.ForeColor = System.Drawing.Color.Black;
                        txtPassword.PasswordChar = '•';
                    }
                };
                txtPassword.Leave += (s, e) => {
                    if (string.IsNullOrEmpty(txtPassword.Text))
                    {
                        txtPassword.PasswordChar = '\0';
                        txtPassword.Text = "Dejar vacío para mantener contraseña actual";
                        txtPassword.ForeColor = System.Drawing.Color.Gray;
                    }
                };
            }
        }

        private void ToggleRoleSections()
        {
            var rolIdx = cboRol.SelectedIndex; // 0=Admin,1=Medico,2=Asistente
            var isMed = rolIdx == 1; 
            var isAsis = rolIdx == 2;
            
            // Habilitar/deshabilitar campos según el rol
            txtMatricula.Enabled = isMed; 
            txtEspecialidad.Enabled = isMed;
            txtDniSupervisor.Enabled = isAsis; 
            dtpIngreso.Enabled = isAsis;

            // Limpiar campos si no corresponden al rol actual
            if (!isMed)
            {
                txtMatricula.Clear();
                txtEspecialidad.Clear();
            }
            if (!isAsis)
            {
                txtDniSupervisor.Clear();
            }
        }

        private bool Validar()
        {
            if (!int.TryParse(txtDni.Text.Trim(), out var dni) || dni <= 0) 
            { 
                MessageBox.Show("DNI inválido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtDni.Focus();
                return false; 
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) 
            { 
                MessageBox.Show("Nombre requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtNombre.Focus();
                return false; 
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text)) 
            { 
                MessageBox.Show("Apellido requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtApellido.Focus();
                return false; 
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@")) 
            { 
                MessageBox.Show("Email inválido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtEmail.Focus();
                return false; 
            }
            
            // En creación, exigir contraseña; en edición permitir dejar vacía para conservar
            if (baseUsuario == null && string.IsNullOrWhiteSpace(txtPassword.Text)) 
            { 
                MessageBox.Show("Contraseña requerida", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                txtPassword.Focus();
                return false; 
            }
            
            // Validaciones específicas por rol
            if (cboRol.SelectedIndex == 1) // Médico
            {
                if (!int.TryParse(txtMatricula.Text.Trim(), out var _)) 
                { 
                    MessageBox.Show("Matrícula inválida para médico", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                    txtMatricula.Focus();
                    return false; 
                }
                if (string.IsNullOrWhiteSpace(txtEspecialidad.Text)) 
                { 
                    MessageBox.Show("Especialidad requerida para médico", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                    txtEspecialidad.Focus();
                    return false; 
                }
            }
            
            if (cboRol.SelectedIndex == 2) // Asistente
            {
                if (!int.TryParse(txtDniSupervisor.Text.Trim(), out var _)) 
                { 
                    MessageBox.Show("DNI Supervisor inválido para asistente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                    txtDniSupervisor.Focus();
                    return false; 
                }
            }
            return true;
        }

        public (Usuario Usuario, Medico Medico, Asistente Asistente) ObtenerDatos()
        {
            var rolId = cboRol.SelectedIndex + 1;
            
            // Si es edición y el campo de contraseña contiene el texto placeholder, conservar la contraseña actual
            string passwordValue = txtPassword.Text;
            if (baseUsuario != null && txtPassword.Text == "Dejar vacío para mantener contraseña actual")
            {
                passwordValue = baseUsuario.PasswordHash;
            }
            
            var usuario = new Usuario
            {
                Dni = int.Parse(txtDni.Text.Trim()),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                PasswordHash = string.IsNullOrWhiteSpace(passwordValue) && baseUsuario != null ? baseUsuario.PasswordHash : passwordValue,
                IdRol = rolId,
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };

            Medico medico = null; 
            Asistente asistente = null;
            
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

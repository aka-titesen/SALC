using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Domain;

namespace SALC.Views.PanelAdministrador
{
    public class FrmUsuarioEdit : Form
    {
        private TextBox txtDni, txtNombre, txtApellido, txtEmail, txtPassword;
        private ComboBox cboRol, cboEstado;
        private TextBox txtMatricula, txtEspecialidad;
        private TextBox txtDniSupervisor;
        private DateTimePicker dtpIngreso;
        private Button btnOk, btnCancel;
        private Usuario baseUsuario;
        private Label lblMedicoSec, lblAsistenteSec;

        public FrmUsuarioEdit(Usuario existente = null)
        {
            Text = existente == null ? "Nuevo Usuario" : "Editar Usuario";
            Width = 500;
            Height = 650;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
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
            // Título del formulario
            var lblTitulo = new Label
            {
                Text = baseUsuario == null ? "Crear Nuevo Usuario" : "Modificar Información del Usuario",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, 15),
                Size = new Size(450, 30),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = baseUsuario == null
                    ? "Complete los datos del nuevo usuario del sistema"
                    : "Actualice los datos necesarios del usuario",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(450, 20),
                BackColor = Color.Transparent
            };

            // Campos comunes
            var lblDni = new Label { Text = "DNI:", Left = 20, Top = 80, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtDni = new TextBox { Left = 170, Top = 78, Width = 280, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 115, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtNombre = new TextBox { Left = 170, Top = 113, Width = 280, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblApellido = new Label { Text = "Apellido:", Left = 20, Top = 150, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtApellido = new TextBox { Left = 170, Top = 148, Width = 280, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblEmail = new Label { Text = "Email:", Left = 20, Top = 185, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtEmail = new TextBox { Left = 170, Top = 183, Width = 280, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblPassword = new Label { Text = "Contraseña:", Left = 20, Top = 220, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtPassword = new TextBox { Left = 170, Top = 218, Width = 280, Font = new Font("Segoe UI", 10), PasswordChar = '•', BorderStyle = BorderStyle.FixedSingle };

            var lblRol = new Label { Text = "Rol:", Left = 20, Top = 255, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            cboRol = new ComboBox { Left = 170, Top = 253, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cboRol.Items.AddRange(new object[] { "Administrador", "Médico", "Asistente" });
            cboRol.SelectedIndexChanged += (s, e) => ToggleRoleSections();

            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 290, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            cboEstado = new ComboBox { Left = 170, Top = 288, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            // Sección Médico
            lblMedicoSec = new Label
            {
                Text = "Datos Específicos de Médico",
                Left = 20,
                Top = 335,
                Width = 450,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblMatricula = new Label { Text = "Matrícula:", Left = 20, Top = 370, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtMatricula = new TextBox { Left = 170, Top = 368, Width = 280, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblEspecialidad = new Label { Text = "Especialidad:", Left = 20, Top = 405, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtEspecialidad = new TextBox { Left = 170, Top = 403, Width = 280, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            // Sección Asistente
            lblAsistenteSec = new Label
            {
                Text = "Datos Específicos de Asistente",
                Left = 20,
                Top = 450,
                Width = 450,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblDniSupervisor = new Label { Text = "DNI Supervisor:", Left = 20, Top = 485, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            txtDniSupervisor = new TextBox { Left = 170, Top = 483, Width = 280, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            var lblFechaIngreso = new Label { Text = "Fecha Ingreso:", Left = 20, Top = 520, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            dtpIngreso = new DateTimePicker { Left = 170, Top = 518, Width = 280, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10) };

            // Botones
            btnOk = new Button
            {
                Text = baseUsuario == null ? "Crear Usuario" : "Guardar Cambios",
                Left = 230,
                Top = 575,
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
                Left = 360,
                Top = 575,
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
                lblDni, txtDni,
                lblNombre, txtNombre,
                lblApellido, txtApellido,
                lblEmail, txtEmail,
                lblPassword, txtPassword,
                lblRol, cboRol,
                lblEstado, cboEstado,
                lblMedicoSec, lblMatricula, txtMatricula, lblEspecialidad, txtEspecialidad,
                lblAsistenteSec, lblDniSupervisor, txtDniSupervisor, lblFechaIngreso, dtpIngreso,
                btnOk, btnCancel
            });
        }

        private void CargarDatosExistentes(Usuario existente)
        {
            txtDni.Text = existente.Dni.ToString();
            txtDni.ReadOnly = true;
            txtDni.BackColor = Color.FromArgb(240, 240, 240);
            
            txtNombre.Text = existente.Nombre;
            txtApellido.Text = existente.Apellido;
            txtEmail.Text = existente.Email;
            cboRol.SelectedIndex = existente.IdRol - 1;
            cboRol.Enabled = false;
            cboRol.BackColor = Color.FromArgb(240, 240, 240);
            cboEstado.SelectedItem = existente.Estado;

            // Nota sobre rol bloqueado
            var lblNotaRol = new Label
            {
                Text = "El rol no puede modificarse una vez creado el usuario",
                Left = 20,
                Top = 315,
                Width = 450,
                Height = 15,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(192, 57, 43),
                BackColor = Color.Transparent
            };
            Controls.Add(lblNotaRol);

            // Placeholder para contraseña
            txtPassword.PasswordChar = '\0';
            txtPassword.Text = "Dejar vacío para mantener contraseña actual";
            txtPassword.ForeColor = Color.Gray;
            
            txtPassword.Enter += (s, e) =>
            {
                if (txtPassword.Text == "Dejar vacío para mantener contraseña actual")
                {
                    txtPassword.Text = "";
                    txtPassword.ForeColor = Color.Black;
                    txtPassword.PasswordChar = '•';
                }
            };
            
            txtPassword.Leave += (s, e) =>
            {
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    txtPassword.PasswordChar = '\0';
                    txtPassword.Text = "Dejar vacío para mantener contraseña actual";
                    txtPassword.ForeColor = Color.Gray;
                }
            };
        }

        private void ToggleRoleSections()
        {
            var rolIdx = cboRol.SelectedIndex;
            var isMed = rolIdx == 1;
            var isAsis = rolIdx == 2;

            // Sección Médico
            lblMedicoSec.Visible = true;
            txtMatricula.Enabled = isMed;
            txtEspecialidad.Enabled = isMed;
            txtMatricula.BackColor = isMed ? Color.White : Color.FromArgb(240, 240, 240);
            txtEspecialidad.BackColor = isMed ? Color.White : Color.FromArgb(240, 240, 240);

            // Sección Asistente
            lblAsistenteSec.Visible = true;
            txtDniSupervisor.Enabled = isAsis;
            dtpIngreso.Enabled = isAsis;
            txtDniSupervisor.BackColor = isAsis ? Color.White : Color.FromArgb(240, 240, 240);

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
                MessageBox.Show("El DNI debe ser un número válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El apellido es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Debe ingresar un email válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (baseUsuario == null && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("La contraseña es obligatoria para usuarios nuevos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (cboRol.SelectedIndex == 1)
            {
                if (!int.TryParse(txtMatricula.Text.Trim(), out var _))
                {
                    MessageBox.Show("La matrícula debe ser un número válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMatricula.Focus();
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                {
                    MessageBox.Show("La especialidad es obligatoria para médicos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEspecialidad.Focus();
                    return false;
                }
            }

            if (cboRol.SelectedIndex == 2)
            {
                if (!int.TryParse(txtDniSupervisor.Text.Trim(), out var _))
                {
                    MessageBox.Show("El DNI del supervisor debe ser un número válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDniSupervisor.Focus();
                    return false;
                }
            }
            
            return true;
        }

        public (Usuario Usuario, Medico Medico, Asistente Asistente) ObtenerDatos()
        {
            var rolId = cboRol.SelectedIndex + 1;

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

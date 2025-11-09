using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using SALC.Domain;
using SALC.BLL;

namespace SALC.Views.PanelAsistente
{
    /// <summary>
    /// Formulario de edición de pacientes para el rol ASISTENTE
    /// RESTRICCIÓN: No permite modificar el estado del paciente (baja lógica)
    /// </summary>
    public class FrmPacienteEditAsistente : Form
    {
        private TextBox txtDni, txtNombre, txtApellido, txtEmail, txtTelefono, txtSexo;
        private DateTimePicker dtpFechaNac;
        private ComboBox cboObraSocial;
        private Label lblEstadoActual; // Solo lectura
        private Button btnOk, btnCancel;
        private int? dniOriginal;
        private string estadoOriginal; // Guardamos el estado original
        private readonly ICatalogoService _catalogoService;

        public FrmPacienteEditAsistente(Paciente existente = null)
        {
            _catalogoService = new CatalogoService();
            
            Text = existente == null ? "Nuevo Paciente" : "Editar Paciente";
            Width = 450;
            Height = 500;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            // Título del formulario
            var lblTitulo = new Label
            {
                Text = existente == null ? "Crear Nuevo Paciente" : "Modificar Información del Paciente",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };

            var lblSubtitulo = new Label
            {
                Text = existente == null 
                    ? "Complete los datos del nuevo paciente" 
                    : "Actualice los datos necesarios (el estado no puede modificarse)",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(400, 20)
            };

            // Campos del formulario
            var lblDni = new Label { Text = "DNI:", Left = 20, Top = 80, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtDni = new TextBox { Left = 150, Top = 78, Width = 250, Font = new Font("Segoe UI", 10) };

            var lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 115, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtNombre = new TextBox { Left = 150, Top = 113, Width = 250, Font = new Font("Segoe UI", 10) };

            var lblApellido = new Label { Text = "Apellido:", Left = 20, Top = 150, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtApellido = new TextBox { Left = 150, Top = 148, Width = 250, Font = new Font("Segoe UI", 10) };

            var lblFecha = new Label { Text = "Fecha Nac:", Left = 20, Top = 185, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            dtpFechaNac = new DateTimePicker 
            { 
                Left = 150, 
                Top = 183, 
                Width = 250, 
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10)
            };

            var lblSexo = new Label { Text = "Sexo (M/F/X):", Left = 20, Top = 220, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtSexo = new TextBox { Left = 150, Top = 218, Width = 80, MaxLength = 1, Font = new Font("Segoe UI", 10) };

            var lblEmail = new Label { Text = "Email:", Left = 20, Top = 255, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtEmail = new TextBox { Left = 150, Top = 253, Width = 250, Font = new Font("Segoe UI", 10) };

            var lblTel = new Label { Text = "Teléfono:", Left = 20, Top = 290, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtTelefono = new TextBox { Left = 150, Top = 288, Width = 250, Font = new Font("Segoe UI", 10) };

            var lblObraSocial = new Label { Text = "Obra Social:", Left = 20, Top = 325, Width = 120, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cboObraSocial = new ComboBox 
            { 
                Left = 150, 
                Top = 323, 
                Width = 250, 
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            // ? CAMPO DE ESTADO - SOLO LECTURA PARA ASISTENTE
            var lblEstado = new Label 
            { 
                Text = "Estado:", 
                Left = 20, 
                Top = 360, 
                Width = 120, 
                Font = new Font("Segoe UI", 10, FontStyle.Bold) 
            };

            lblEstadoActual = new Label
            {
                Text = existente?.Estado ?? "Activo",
                Left = 150,
                Top = 360,
                Width = 250,
                Height = 30,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 240, 240), // Fondo gris para indicar solo lectura
                ForeColor = existente?.Estado == "Activo" 
                    ? Color.FromArgb(39, 174, 96) 
                    : Color.FromArgb(192, 57, 43),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Nota explicativa
            var lblNotaEstado = new Label
            {
                Text = "El estado del paciente no puede modificarse desde el rol Asistente",
                Left = 20,
                Top = 395,
                Width = 400,
                Height = 20,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(192, 57, 43),
                BackColor = Color.Transparent
            };

            // Botones
            btnOk = new Button 
            { 
                Text = existente == null ? "Crear Paciente" : "Guardar Cambios",
                Left = 180, 
                Top = 430, 
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
                Top = 430, 
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

            Controls.AddRange(new Control[] { 
                lblTitulo, lblSubtitulo,
                lblDni, txtDni, 
                lblNombre, txtNombre, 
                lblApellido, txtApellido, 
                lblFecha, dtpFechaNac, 
                lblSexo, txtSexo, 
                lblEmail, txtEmail, 
                lblTel, txtTelefono, 
                lblObraSocial, cboObraSocial, 
                lblEstado, lblEstadoActual, lblNotaEstado,
                btnOk, btnCancel 
            });

            CargarObrasSociales();

            if (existente != null)
            {
                dniOriginal = existente.Dni;
                estadoOriginal = existente.Estado; // Guardar estado original
                
                txtDni.Text = existente.Dni.ToString();
                txtDni.Enabled = false; // DNI no se puede modificar
                txtNombre.Text = existente.Nombre;
                txtApellido.Text = existente.Apellido;
                dtpFechaNac.Value = existente.FechaNac;
                txtSexo.Text = existente.Sexo.ToString();
                txtEmail.Text = existente.Email;
                txtTelefono.Text = existente.Telefono;
                lblEstadoActual.Text = existente.Estado;
                
                // Seleccionar obra social
                if (existente.IdObraSocial.HasValue)
                {
                    var item = cboObraSocial.Items.Cast<PanelAdministrador.ObraSocialComboItem>()
                        .FirstOrDefault(x => x.Id == existente.IdObraSocial.Value);
                    if (item != null)
                        cboObraSocial.SelectedItem = item;
                }
                else
                {
                    cboObraSocial.SelectedIndex = 0; // "Sin obra social"
                }
            }
            else
            {
                // Valores por defecto para nuevo paciente (siempre Activo)
                estadoOriginal = "Activo";
                lblEstadoActual.Text = "Activo (nuevo paciente)";
                lblEstadoActual.ForeColor = Color.FromArgb(39, 174, 96);
                cboObraSocial.SelectedIndex = 0; // "Sin obra social"
            }
        }

        private void CargarObrasSociales()
        {
            try
            {
                cboObraSocial.Items.Clear();
                
                // Agregar opción "Sin obra social"
                cboObraSocial.Items.Add(new PanelAdministrador.ObraSocialComboItem { Id = null, Nombre = "Sin obra social" });
                
                // Cargar obras sociales activas
                var obrasSociales = _catalogoService.ObtenerObrasSocialesActivas()
                    .OrderBy(os => os.Nombre)
                    .ToList();
                
                foreach (var os in obrasSociales)
                {
                    cboObraSocial.Items.Add(new PanelAdministrador.ObraSocialComboItem { Id = os.IdObraSocial, Nombre = os.Nombre });
                }
                
                if (cboObraSocial.Items.Count > 0)
                    cboObraSocial.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar obras sociales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboObraSocial.Items.Add(new PanelAdministrador.ObraSocialComboItem { Id = null, Nombre = "Sin obra social" });
                cboObraSocial.SelectedIndex = 0;
            }
        }

        private bool Validar()
        {
            if (!dniOriginal.HasValue)
            {
                if (!int.TryParse(txtDni.Text.Trim(), out var dni) || dni <= 0)
                {
                    MessageBox.Show("DNI inválido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDni.Focus();
                    return false;
                }
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
            
            var sx = (txtSexo.Text ?? string.Empty).Trim().ToUpperInvariant();
            if (sx != "M" && sx != "F" && sx != "X")
            {
                MessageBox.Show("El sexo debe ser M (Masculino), F (Femenino) o X (Otro).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSexo.Focus();
                return false;
            }
            
            if (dtpFechaNac.Value.Date > DateTime.Today)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser futura.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaNac.Focus();
                return false;
            }
            
            if (cboObraSocial.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una obra social.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboObraSocial.Focus();
                return false;
            }
            
            return true;
        }

        public Paciente ObtenerPaciente()
        {
            var obraSocialSeleccionada = cboObraSocial.SelectedItem as PanelAdministrador.ObraSocialComboItem;
            
            return new Paciente
            {
                Dni = dniOriginal ?? int.Parse(txtDni.Text.Trim()),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                FechaNac = dtpFechaNac.Value.Date,
                Sexo = (txtSexo.Text ?? "").Trim().ToUpperInvariant()[0],
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                IdObraSocial = obraSocialSeleccionada?.Id,
                // ? IMPORTANTE: El estado NO se puede modificar, se mantiene el original
                Estado = estadoOriginal
            };
        }
    }
}

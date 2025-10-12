using System;
using System.Linq;
using System.Windows.Forms;
using SALC.Domain;
using SALC.BLL;

namespace SALC.Views.PanelAdministrador
{
    public class FrmPacienteEdit : Form
    {
        private TextBox txtDni, txtNombre, txtApellido, txtEmail, txtTelefono, txtSexo;
        private DateTimePicker dtpFechaNac;
        private ComboBox cboEstado, cboObraSocial;
        private Button btnOk, btnCancel;
        private int? dniOriginal;
        private readonly ICatalogoService _catalogoService;

        public FrmPacienteEdit(Paciente existente = null)
        {
            _catalogoService = new CatalogoService();
            
            Text = existente == null ? "Nuevo Paciente" : "Editar Paciente";
            Width = 420; Height = 450; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
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
            
            // Agregar combo de obra social
            var lblObraSocial = new Label { Text = "Obra Social:", Left = 20, Top = 265, Width = 120 };
            cboObraSocial = new ComboBox { Left = 150, Top = 263, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            
            // Agregar combo de estado
            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 300, Width = 120 };
            cboEstado = new ComboBox { Left = 150, Top = 298, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });

            btnOk = new Button { Text = "Aceptar", Left = 170, Top = 350, Width = 80, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancelar", Left = 260, Top = 350, Width = 90, DialogResult = DialogResult.Cancel };
            AcceptButton = btnOk; CancelButton = btnCancel;
            btnOk.Click += (s, e) => { if (!Validar()) this.DialogResult = DialogResult.None; };

            Controls.AddRange(new Control[] { 
                lblDni, txtDni, lblNombre, txtNombre, lblApellido, txtApellido, lblFecha, dtpFechaNac, 
                lblSexo, txtSexo, lblEmail, txtEmail, lblTel, txtTelefono, 
                lblObraSocial, cboObraSocial, lblEstado, cboEstado, btnOk, btnCancel 
            });

            CargarObrasSociales();

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
                
                // Seleccionar obra social
                if (existente.IdObraSocial.HasValue)
                {
                    var item = cboObraSocial.Items.Cast<ObraSocialComboItem>()
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
                // Valores por defecto para nuevo paciente
                cboEstado.SelectedItem = "Activo";
                cboObraSocial.SelectedIndex = 0; // "Sin obra social"
            }
        }

        private void CargarObrasSociales()
        {
            try
            {
                cboObraSocial.Items.Clear();
                
                // Agregar opción "Sin obra social"
                cboObraSocial.Items.Add(new ObraSocialComboItem { Id = null, Nombre = "Sin obra social" });
                
                // Cargar obras sociales activas
                var obrasSociales = _catalogoService.ObtenerObrasSocialesActivas()
                    .OrderBy(os => os.Nombre)
                    .ToList();
                
                foreach (var os in obrasSociales)
                {
                    cboObraSocial.Items.Add(new ObraSocialComboItem { Id = os.IdObraSocial, Nombre = os.Nombre });
                }
                
                if (cboObraSocial.Items.Count > 0)
                    cboObraSocial.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar obras sociales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Agregar al menos la opción sin obra social
                cboObraSocial.Items.Add(new ObraSocialComboItem { Id = null, Nombre = "Sin obra social" });
                cboObraSocial.SelectedIndex = 0;
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
            if (cboObraSocial.SelectedItem == null) { MessageBox.Show("Seleccione una obra social."); cboObraSocial.Focus(); return false; }
            return true;
        }

        public Paciente ObtenerPaciente()
        {
            var obraSocialSeleccionada = cboObraSocial.SelectedItem as ObraSocialComboItem;
            
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
                Estado = cboEstado.SelectedItem?.ToString() ?? "Activo"
            };
        }
    }

    // Clase auxiliar para el ComboBox de obras sociales
    public class ObraSocialComboItem
    {
        public int? Id { get; set; }
        public string Nombre { get; set; }
        
        public override string ToString()
        {
            return Nombre;
        }
    }
}

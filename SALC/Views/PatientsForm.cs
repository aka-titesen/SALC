// Views/PatientsForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Views.Interfaces;

namespace SALC.Views
{
    public partial class PatientsForm : Form, IPatientsView
    {
        #region Eventos de la Interfaz IPatientsView
        public event EventHandler CreateRequested;
        public event EventHandler EditRequested;
        public event EventHandler DeleteRequested;
        public event EventHandler SearchRequested;
        public event EventHandler CloseRequested;
        #endregion

        #region Propiedades de la Interfaz IPatientsView
        public string SearchText => txtSearch.Text;
        #endregion

        #region Eventos personalizados para funcionalidad extendida
        public event EventHandler LoadPatients;
        public event EventHandler<int> DeletePatient;
        public event EventHandler<Paciente> SavePatient;
        public event EventHandler<int> EditPatient;
        public event EventHandler<string> SearchPatient;
        public event EventHandler<int> ViewPatientHistory;
        #endregion

        #region Propiedades de la Interfaz
        public List<Paciente> Patients { get; set; } = new List<Paciente>();
        public Paciente SelectedPatient { get; set; }
        public bool IsEditing { get; set; }
        public List<ObraSocial> SocialWorks { get; set; } = new List<ObraSocial>();
        #endregion

        #region Controles de la UI
        private Panel topPanel;
        private Panel contentPanel;
        private Panel formPanel;
        private Panel listPanel;
        private DataGridView dgvPatients;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnNew;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnHistory;
        
        // Formulario de paciente
        private TextBox txtNroDoc;
        private ComboBox cbTipoDoc;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private ComboBox cbSexo;
        private DateTimePicker dtpFechaNacimiento;
        private TextBox txtDireccion;
        private TextBox txtLocalidad;
        private TextBox txtProvincia;
        private TextBox txtTelefono;
        private TextBox txtMail;
        private ComboBox cbObraSocial;
        private Button btnSave;
        private Button btnCancel;
        #endregion

        public PatientsForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Gestión de Pacientes";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.WindowState = FormWindowState.Maximized;
        }

        private void InitializeCustomComponents()
        {
            CreateTopPanel();
            CreateContentPanel();
            SetupDataGridView();
            SetupFormPanel();
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            Label titleLabel = new Label
            {
                Text = "?? Gestión de Pacientes",
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(23, 162, 184),
                Size = new Size(300, 40),
                Location = new Point(20, 20)
            };

            Panel searchPanel = new Panel
            {
                Size = new Size(400, 40),
                Location = new Point(this.Width - 440, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            txtSearch = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(0, 5),
                Font = new Font("Microsoft Sans Serif", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnSearch = new Button
            {
                Text = "Buscar",
                Size = new Size(80, 30),
                Location = new Point(310, 5),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9)
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            searchPanel.Controls.AddRange(new Control[] { txtSearch, btnSearch });
            topPanel.Controls.AddRange(new Control[] { titleLabel, searchPanel });
            this.Controls.Add(topPanel);
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(20)
            };

            // Panel izquierdo para la lista
            listPanel = new Panel
            {
                Width = 700,
                Dock = DockStyle.Left,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // Panel derecho para el formulario
            formPanel = new Panel
            {
                Width = 450,
                Dock = DockStyle.Right,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            contentPanel.Controls.AddRange(new Control[] { listPanel, formPanel });
            this.Controls.Add(contentPanel);
        }

        private void SetupDataGridView()
        {
            // Botones de acción
            Panel buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            btnNew = CreateActionButton("Nuevo", Color.FromArgb(40, 167, 69), 0);
            btnEdit = CreateActionButton("Editar", Color.FromArgb(0, 123, 255), 90);
            btnDelete = CreateActionButton("Eliminar", Color.FromArgb(220, 53, 69), 180);
            btnHistory = CreateActionButton("Historial", Color.FromArgb(23,162,184), 270);

            btnNew.Click += BtnNew_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnHistory.Click += BtnHistory_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnNew, btnEdit, btnDelete, btnHistory });

            // DataGridView
            dgvPatients = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Microsoft Sans Serif", 9)
            };

            // Configurar columnas
            dgvPatients.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "NroDoc", HeaderText = "Documento", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "TipoDoc", HeaderText = "Tipo", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "NombreCompleto", HeaderText = "Nombre Completo", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "FechaNacimiento", HeaderText = "Fecha Nac.", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "ObraSocial", HeaderText = "Obra Social", Width = 150 }
            });

            dgvPatients.SelectionChanged += DgvPatients_SelectionChanged;

            listPanel.Controls.AddRange(new Control[] { buttonPanel, dgvPatients });
        }

        private Button CreateActionButton(string text, Color backColor, int x)
        {
            return new Button
            {
                Text = text,
                Size = new Size(80, 30),
                Location = new Point(x, 10),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9)
            };
        }

        private void SetupFormPanel()
        {
            Label formTitle = new Label
            {
                Text = "Datos del Paciente",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(400, 25),
                Location = new Point(20, 20)
            };

            int yPos = 60;
            int spacing = 35;

            // Número de documento
            CreateFormField("Número de Documento:", ref txtNroDoc, yPos);
            yPos += spacing;

            // Tipo de documento
            Label lblTipoDoc = CreateLabel("Tipo de Documento:", yPos);
            cbTipoDoc = new ComboBox
            {
                Size = new Size(200, 25),
                Location = new Point(200, yPos),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft Sans Serif", 9)
            };
            cbTipoDoc.Items.AddRange(new string[] { "DNI", "LE", "LC", "CI" });
            cbTipoDoc.SelectedIndex = 0;
            yPos += spacing;

            // Nombre
            CreateFormField("Nombre:", ref txtNombre, yPos);
            yPos += spacing;

            // Apellido
            CreateFormField("Apellido:", ref txtApellido, yPos);
            yPos += spacing;

            // Sexo
            Label lblSexo = CreateLabel("Sexo:", yPos);
            cbSexo = new ComboBox
            {
                Size = new Size(200, 25),
                Location = new Point(200, yPos),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft Sans Serif", 9)
            };
            cbSexo.Items.AddRange(new string[] { "M", "F" });
            yPos += spacing;

            // Fecha de nacimiento
            Label lblFechaNac = CreateLabel("Fecha de Nacimiento:", yPos);
            dtpFechaNacimiento = new DateTimePicker
            {
                Size = new Size(200, 25),
                Location = new Point(200, yPos),
                Font = new Font("Microsoft Sans Serif", 9),
                Format = DateTimePickerFormat.Short
            };
            yPos += spacing;

            // Dirección
            CreateFormField("Dirección:", ref txtDireccion, yPos);
            yPos += spacing;

            // Localidad
            CreateFormField("Localidad:", ref txtLocalidad, yPos);
            yPos += spacing;

            // Provincia
            CreateFormField("Provincia:", ref txtProvincia, yPos);
            yPos += spacing;

            // Teléfono
            CreateFormField("Teléfono:", ref txtTelefono, yPos);
            yPos += spacing;

            // Email
            CreateFormField("Email:", ref txtMail, yPos);
            yPos += spacing;

            // Obra Social
            Label lblObraSocial = CreateLabel("Obra Social:", yPos);
            cbObraSocial = new ComboBox
            {
                Size = new Size(200, 25),
                Location = new Point(200, yPos),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft Sans Serif", 9)
            };
            yPos += spacing + 20;

            // Botones
            btnSave = new Button
            {
                Text = "Guardar",
                Size = new Size(90, 35),
                Location = new Point(200, yPos),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancelar",
                Size = new Size(90, 35),
                Location = new Point(300, yPos),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 10)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            formPanel.Controls.AddRange(new Control[] { 
                formTitle, lblTipoDoc, cbTipoDoc, lblSexo, cbSexo, 
                lblFechaNac, dtpFechaNacimiento, lblObraSocial, cbObraSocial,
                btnSave, btnCancel 
            });

            ClearForm();
        }

        private void CreateFormField(string labelText, ref TextBox textBox, int yPos)
        {
            Label label = CreateLabel(labelText, yPos);
            textBox = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(200, yPos),
                Font = new Font("Microsoft Sans Serif", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            formPanel.Controls.AddRange(new Control[] { label, textBox });
        }

        private Label CreateLabel(string text, int yPos)
        {
            return new Label
            {
                Text = text,
                Size = new Size(180, 20),
                Location = new Point(20, yPos + 3),
                Font = new Font("Microsoft Sans Serif", 9),
                ForeColor = Color.FromArgb(52, 58, 64)
            };
        }

        #region Event Handlers
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchPatient?.Invoke(this, txtSearch.Text);
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            IsEditing = false;
            ClearForm();
            EnableForm(true);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (SelectedPatient != null)
            {
                IsEditing = true;
                LoadPatientData(SelectedPatient);
                EnableForm(true);
                EditPatient?.Invoke(this, SelectedPatient.NroDoc);
            }
            else
            {
                ShowMessage("Seleccione un paciente para editar.");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (SelectedPatient != null)
            {
                var result = MessageBox.Show(
                    $"¿Está seguro que desea eliminar al paciente {SelectedPatient.Nombre} {SelectedPatient.Apellido}?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DeletePatient?.Invoke(this, SelectedPatient.NroDoc);
                }
            }
            else
            {
                ShowMessage("Seleccione un paciente para eliminar.");
            }
        }

        private void BtnHistory_Click(object sender, EventArgs e)
        {
            if (SelectedPatient != null)
            {
                ViewPatientHistory?.Invoke(this, SelectedPatient.NroDoc);
            }
            else
            {
                ShowMessage("Seleccione un paciente para ver su historial.");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                var paciente = new Paciente
                {
                    NroDoc = int.Parse(txtNroDoc.Text),
                    TipoDoc = cbTipoDoc.SelectedItem.ToString(),
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    Sexo = cbSexo.SelectedItem?.ToString(),
                    FechaNacimiento = dtpFechaNacimiento.Value,
                    Direccion = txtDireccion.Text,
                    Localidad = txtLocalidad.Text,
                    Provincia = txtProvincia.Text,
                    Telefono = txtTelefono.Text,
                    Mail = txtMail.Text,
                    IdObraSocial = cbObraSocial.SelectedValue != null ? (int)cbObraSocial.SelectedValue : 0
                };

                SavePatient?.Invoke(this, paciente);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            EnableForm(false);
        }

        private void DgvPatients_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPatients.SelectedRows.Count > 0)
            {
                var row = dgvPatients.SelectedRows[0];
                int nroDoc = (int)row.Cells["NroDoc"].Value;
                SelectedPatient = Patients.Find(p => p.NroDoc == nroDoc);
            }
        }
        #endregion

        #region Interface Implementation
        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowMessage(string message)
        {
            ShowMessage("Información", message);
        }

        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ClearForm()
        {
            txtNroDoc.Clear();
            cbTipoDoc.SelectedIndex = 0;
            txtNombre.Clear();
            txtApellido.Clear();
            cbSexo.SelectedIndex = -1;
            dtpFechaNacimiento.Value = DateTime.Now;
            txtDireccion.Clear();
            txtLocalidad.Clear();
            txtProvincia.Clear();
            txtTelefono.Clear();
            txtMail.Clear();
            cbObraSocial.SelectedIndex = -1;
            EnableForm(false);
        }

        public void LoadPatientData(Paciente paciente)
        {
            txtNroDoc.Text = paciente.NroDoc.ToString();
            cbTipoDoc.SelectedItem = paciente.TipoDoc;
            txtNombre.Text = paciente.Nombre;
            txtApellido.Text = paciente.Apellido;
            cbSexo.SelectedItem = paciente.Sexo;
            dtpFechaNacimiento.Value = paciente.FechaNacimiento;
            txtDireccion.Text = paciente.Direccion;
            txtLocalidad.Text = paciente.Localidad;
            txtProvincia.Text = paciente.Provincia;
            txtTelefono.Text = paciente.Telefono;
            txtMail.Text = paciente.Mail;
            
            foreach (var item in cbObraSocial.Items)
            {
                if (item is ObraSocial os && os.IdObraSocial == paciente.IdObraSocial)
                {
                    cbObraSocial.SelectedItem = item;
                    break;
                }
            }
        }

        public void RefreshPatientList()
        {
            dgvPatients.Rows.Clear();
            foreach (var paciente in Patients)
            {
                dgvPatients.Rows.Add(
                    paciente.NroDoc,
                    paciente.TipoDoc,
                    $"{paciente.Nombre} {paciente.Apellido}",
                    paciente.FechaNacimiento.ToShortDateString(),
                    paciente.Telefono,
                    paciente.ObraSocial?.Nombre ?? "Sin obra social"
                );
            }
        }
        #endregion

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtNroDoc.Text) || !int.TryParse(txtNroDoc.Text, out _))
            {
                ShowError("El número de documento es requerido y debe ser numérico.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                ShowError("El nombre es requerido.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                ShowError("El apellido es requerido.");
                return false;
            }

            if (cbSexo.SelectedIndex == -1)
            {
                ShowError("Debe seleccionar el sexo.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtMail.Text) && !IsValidEmail(txtMail.Text))
            {
                ShowError("El formato del email no es válido.");
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void EnableForm(bool enabled)
        {
            txtNroDoc.Enabled = enabled && !IsEditing; // El documento no se puede editar
            cbTipoDoc.Enabled = enabled;
            txtNombre.Enabled = enabled;
            txtApellido.Enabled = enabled;
            cbSexo.Enabled = enabled;
            dtpFechaNacimiento.Enabled = enabled;
            txtDireccion.Enabled = enabled;
            txtLocalidad.Enabled = enabled;
            txtProvincia.Enabled = enabled;
            txtTelefono.Enabled = enabled;
            txtMail.Enabled = enabled;
            cbObraSocial.Enabled = enabled;
            btnSave.Enabled = enabled;
            btnCancel.Enabled = enabled;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadPatients?.Invoke(this, EventArgs.Empty);
        }
    }

    // Clases temporales para esta vista
    public class Paciente
    {
        public int NroDoc { get; set; }
        public string TipoDoc { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public int IdObraSocial { get; set; }
        public ObraSocial ObraSocial { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }

    public class ObraSocial
    {
        public int IdObraSocial { get; set; }
        public string Cuit { get; set; }
        public string Nombre { get; set; }
    }
}

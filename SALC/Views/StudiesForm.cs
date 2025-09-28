// Views/StudiesForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.UI;

namespace SALC.Views
{
    public class StudiesForm : Form
    {
        private Panel topPanel;
        private Panel contentPanel;
        private Panel formPanel;
        private Panel listPanel;
        private DataGridView dgvStudies;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnNew;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnDetails;

        private ComboBox cbPaciente;
        private ComboBox cbTipoAnalisis;
        private ComboBox cbDoctor;
        private ComboBox cbEstado;
        private ComboBox cbPrioridad;
        private TextBox txtObservaciones;
        private DateTimePicker dtpFechaCreacion;
        private Button btnSave;
        private Button btnCancel;

        public StudiesForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Gestión de Estudios";
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
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(20)
            };

            Label titleLabel = new Label
            {
                Text = "Gestión de Estudios",
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                ForeColor = SALCColors.Studies,
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
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9)
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => { /* Solo UI */ };

            searchPanel.Controls.AddRange(new Control[] { txtSearch, btnSearch });
            topPanel.Controls.AddRange(new Control[] { titleLabel, searchPanel });
            this.Controls.Add(topPanel);
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SALCColors.Background,
                Padding = new Padding(20)
            };

            listPanel = new Panel
            {
                Width = 700,
                Dock = DockStyle.Left,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(10)
            };

            formPanel = new Panel
            {
                Width = 450,
                Dock = DockStyle.Right,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(20)
            };

            contentPanel.Controls.AddRange(new Control[] { listPanel, formPanel });
            this.Controls.Add(contentPanel);
        }

        private void SetupDataGridView()
        {
            Panel buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(10)
            };

            btnNew = CreateActionButton("Nuevo", SALCColors.Success, 0);
            btnEdit = CreateActionButton("Editar", SALCColors.Primary, 90);
            btnDelete = CreateActionButton("Eliminar", SALCColors.Danger, 180);
            btnDetails = CreateActionButton("Detalles", SALCColors.Info, 270);

            buttonPanel.Controls.AddRange(new Control[] { btnNew, btnEdit, btnDelete, btnDetails });

            dgvStudies = new DataGridView
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

            dgvStudies.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Paciente", HeaderText = "Paciente", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo de Análisis", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Doctor", HeaderText = "Doctor", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "FechaCreacion", HeaderText = "Fecha Creación", Width = 120 }
            });

            listPanel.Controls.AddRange(new Control[] { buttonPanel, dgvStudies });
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
                Text = "Datos del Estudio",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                Size = new Size(400, 25),
                Location = new Point(20, 20)
            };

            int yPos = 60;
            int spacing = 40;

            Label lblPaciente = CreateLabel("Paciente:", yPos);
            cbPaciente = new ComboBox { Size = new Size(250, 25), Location = new Point(150, yPos), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Microsoft Sans Serif", 9) };
            yPos += spacing;

            Label lblTipoAnalisis = CreateLabel("Tipo de Análisis:", yPos);
            cbTipoAnalisis = new ComboBox { Size = new Size(250, 25), Location = new Point(150, yPos), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Microsoft Sans Serif", 9) };
            yPos += spacing;

            Label lblDoctor = CreateLabel("Doctor:", yPos);
            cbDoctor = new ComboBox { Size = new Size(250, 25), Location = new Point(150, yPos), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Microsoft Sans Serif", 9) };
            yPos += spacing;

            Label lblEstado = CreateLabel("Estado:", yPos);
            cbEstado = new ComboBox { Size = new Size(250, 25), Location = new Point(150, yPos), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Microsoft Sans Serif", 9) };
            yPos += spacing;

            Label lblPrioridad = CreateLabel("Prioridad:", yPos);
            cbPrioridad = new ComboBox { Size = new Size(250, 25), Location = new Point(150, yPos), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Microsoft Sans Serif", 9) };
            cbPrioridad.Items.AddRange(new string[] { "Baja", "Media", "Alta" });
            cbPrioridad.SelectedIndex = 1;
            yPos += spacing;

            Label lblFecha = CreateLabel("Fecha:", yPos);
            dtpFechaCreacion = new DateTimePicker { Size = new Size(250, 25), Location = new Point(150, yPos), Font = new Font("Microsoft Sans Serif", 9), Format = DateTimePickerFormat.Short };
            yPos += spacing;

            Label lblObservaciones = CreateLabel("Observaciones:", yPos);
            txtObservaciones = new TextBox { Size = new Size(250, 60), Location = new Point(150, yPos), Font = new Font("Microsoft Sans Serif", 9), BorderStyle = BorderStyle.FixedSingle, Multiline = true, ScrollBars = ScrollBars.Vertical };
            yPos += 80;

            btnSave = new Button { Text = "Guardar", Size = new Size(90, 35), Location = new Point(150, yPos), BackColor = SALCColors.Success, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold) };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => MessageBox.Show("Guardar estudio (solo UI)");

            btnCancel = new Button { Text = "Cancelar", Size = new Size(90, 35), Location = new Point(250, yPos), BackColor = SALCColors.Secondary, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Microsoft Sans Serif", 10) };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { /* Solo UI */ };

            formPanel.Controls.AddRange(new Control[] { 
                formTitle, lblPaciente, cbPaciente, lblTipoAnalisis, cbTipoAnalisis,
                lblDoctor, cbDoctor, lblEstado, cbEstado, lblPrioridad, cbPrioridad,
                lblFecha, dtpFechaCreacion, lblObservaciones, txtObservaciones,
                btnSave, btnCancel 
            });
        }

        private Label CreateLabel(string text, int yPos)
        {
            return new Label
            {
                Text = text,
                Size = new Size(140, 20),
                Location = new Point(20, yPos + 3),
                Font = new Font("Microsoft Sans Serif", 9),
                ForeColor = SALCColors.TextPrimary
            };
        }
    }
}
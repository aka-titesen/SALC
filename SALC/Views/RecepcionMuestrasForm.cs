// Views/RecepcionMuestrasForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.UI;
using SALC.Models;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para que los ASISTENTES recepcionen muestras de análisis (RF-17).
    /// Esta funcionalidad es EXCLUSIVA para el rol Asistente.
    /// Los clínicos NO acceden a esta pantalla.
    /// </summary>
    public class RecepcionMuestrasForm : Form
    {
        // Información del usuario actual
        private string _nombreUsuario;
        private string _nombreSupervisor;
        private int _dniAsistente;

        // Controles de la UI
        private Panel headerPanel;
        private Label lblTitle;
        private Label lblSupervisor;  // Etiqueta obligatoria para asistentes
        private Panel topPanel;
        private Panel contentPanel;
        private Panel formPanel;
        private Panel listPanel;
        private DataGridView dgvAnalisisPendientes;
        
        // Formulario de recepción
        private TextBox txtCodigoAnalisis;
        private TextBox txtPaciente;
        private TextBox txtTipoAnalisis;
        private DateTimePicker dtpFechaRecepcion;
        private TextBox txtObservaciones;
        private Button btnRecepcionar;
        private Button btnCancelar;
        private Button btnBuscar;
        
        // Estado
        private Analisis _analisisSeleccionado;

        public RecepcionMuestrasForm(int dniAsistente, string nombreUsuario, string nombreSupervisor)
        {
            _dniAsistente = dniAsistente;
            _nombreUsuario = nombreUsuario;
            _nombreSupervisor = nombreSupervisor;
            
            InitializeComponent();
            InitializeCustomComponents();
            LoadAnalisisPendientes();
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Recepción de Muestras";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SALCColors.Background;
            this.WindowState = FormWindowState.Maximized;
        }

        private void InitializeCustomComponents()
        {
            CreateHeaderPanel();
            CreateTopPanel();
            CreateContentPanel();
            SetupListPanel();
            SetupFormPanel();
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = SALCColors.Primary,
                Padding = new Padding(20)
            };

            lblTitle = new Label
            {
                Text = "RECEPCIÓN DE MUESTRAS",
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            // IMPORTANTE: Etiqueta de supervisor SIEMPRE visible para asistentes
            lblSupervisor = new Label
            {
                Text = $"Supervisor: {_nombreSupervisor}",
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 255, 200),
                AutoSize = true,
                Location = new Point(20, 45)
            };

            Label lblUsuario = new Label
            {
                Text = $"Asistente: {_nombreUsuario}",
                Font = new Font("Microsoft Sans Serif", 10),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(this.Width - 250, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSupervisor);
            headerPanel.Controls.Add(lblUsuario);
            this.Controls.Add(headerPanel);
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(20, 15, 20, 15)
            };

            Label lblInfo = new Label
            {
                Text = "?? Seleccione un análisis pendiente o ingrese el código para recepcionar la muestra",
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Italic),
                ForeColor = SALCColors.TextSecondary,
                AutoSize = true,
                Location = new Point(20, 18)
            };

            topPanel.Controls.Add(lblInfo);
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
                Dock = DockStyle.Fill,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(20, 0, 0, 0)
            };

            contentPanel.Controls.Add(listPanel);
            contentPanel.Controls.Add(formPanel);
            this.Controls.Add(contentPanel);
        }

        private void SetupListPanel()
        {
            Label listTitle = new Label
            {
                Text = "Análisis Pendientes de Recepción",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(10, 5, 0, 0)
            };

            dgvAnalisisPendientes = new DataGridView
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

            dgvAnalisisPendientes.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Paciente", HeaderText = "Paciente", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo Análisis", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "DoctorSolicitante", HeaderText = "Doctor Solicitante", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "FechaCreacion", HeaderText = "Fecha Creación", Width = 120 }
            });

            dgvAnalisisPendientes.SelectionChanged += (s, e) =>
            {
                if (dgvAnalisisPendientes.SelectedRows.Count > 0)
                {
                    var row = dgvAnalisisPendientes.SelectedRows[0];
                    LoadAnalisisToForm(row);
                }
            };

            listPanel.Controls.Add(dgvAnalisisPendientes);
            listPanel.Controls.Add(listTitle);
        }

        private void SetupFormPanel()
        {
            Label formTitle = new Label
            {
                Text = "Datos de Recepción",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                Size = new Size(400, 25),
                Location = new Point(20, 20)
            };

            int yPos = 60;
            int spacing = 50;

            // Código de análisis
            Label lblCodigo = CreateLabel("Código Análisis:", yPos);
            txtCodigoAnalisis = new TextBox
            {
                Size = new Size(250, 25),
                Location = new Point(180, yPos),
                Font = new Font("Microsoft Sans Serif", 10),
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true
            };

            btnBuscar = new Button
            {
                Text = "Buscar",
                Size = new Size(80, 25),
                Location = new Point(440, yPos),
                BackColor = SALCColors.Info,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            yPos += spacing;

            // Paciente
            Label lblPaciente = CreateLabel("Paciente:", yPos);
            txtPaciente = new TextBox
            {
                Size = new Size(340, 25),
                Location = new Point(180, yPos),
                Font = new Font("Microsoft Sans Serif", 10),
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            yPos += spacing;

            // Tipo de análisis
            Label lblTipoAnalisis = CreateLabel("Tipo de Análisis:", yPos);
            txtTipoAnalisis = new TextBox
            {
                Size = new Size(340, 25),
                Location = new Point(180, yPos),
                Font = new Font("Microsoft Sans Serif", 10),
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            yPos += spacing;

            // Fecha de recepción
            Label lblFechaRecepcion = CreateLabel("Fecha Recepción:", yPos);
            dtpFechaRecepcion = new DateTimePicker
            {
                Size = new Size(250, 25),
                Location = new Point(180, yPos),
                Font = new Font("Microsoft Sans Serif", 10),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm"
            };
            yPos += spacing;

            // Observaciones
            Label lblObservaciones = CreateLabel("Observaciones:", yPos);
            txtObservaciones = new TextBox
            {
                Size = new Size(340, 80),
                Location = new Point(180, yPos),
                Font = new Font("Microsoft Sans Serif", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            yPos += 100;

            // Botones de acción
            btnRecepcionar = new Button
            {
                Text = "? Recepcionar Muestra",
                Size = new Size(180, 40),
                Location = new Point(180, yPos),
                BackColor = SALCColors.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold),
                Enabled = false
            };
            btnRecepcionar.FlatAppearance.BorderSize = 0;
            btnRecepcionar.Click += BtnRecepcionar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Size = new Size(100, 40),
                Location = new Point(370, yPos),
                BackColor = SALCColors.Secondary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 10)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => LimpiarFormulario();

            formPanel.Controls.AddRange(new Control[]
            {
                formTitle, lblCodigo, txtCodigoAnalisis, btnBuscar,
                lblPaciente, txtPaciente, lblTipoAnalisis, txtTipoAnalisis,
                lblFechaRecepcion, dtpFechaRecepcion, lblObservaciones, txtObservaciones,
                btnRecepcionar, btnCancelar
            });
        }

        private Label CreateLabel(string text, int yPos)
        {
            return new Label
            {
                Text = text,
                Size = new Size(170, 20),
                Location = new Point(20, yPos + 3),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary
            };
        }

        private void LoadAnalisisPendientes()
        {
            // TODO: Cargar desde la BD los análisis que aún no tienen dni_recepcion
            // Placeholder con datos de ejemplo
            dgvAnalisisPendientes.Rows.Clear();
            
            dgvAnalisisPendientes.Rows.Add(
                "1001",
                "García, Ana María",
                "Hemograma Completo",
                "Dr. Benítez, Ricardo",
                DateTime.Now.AddHours(-2).ToString("dd/MM/yyyy HH:mm")
            );
            
            dgvAnalisisPendientes.Rows.Add(
                "1002",
                "Martínez, Luis Alberto",
                "Perfil Lipídico",
                "Dr. Suárez, Elena",
                DateTime.Now.AddHours(-5).ToString("dd/MM/yyyy HH:mm")
            );
        }

        private void LoadAnalisisToForm(DataGridViewRow row)
        {
            if (row == null) return;

            txtCodigoAnalisis.Text = row.Cells["Id"].Value?.ToString() ?? "";
            txtPaciente.Text = row.Cells["Paciente"].Value?.ToString() ?? "";
            txtTipoAnalisis.Text = row.Cells["TipoAnalisis"].Value?.ToString() ?? "";
            dtpFechaRecepcion.Value = DateTime.Now;
            txtObservaciones.Clear();
            
            btnRecepcionar.Enabled = true;
            
            // TODO: Cargar el objeto completo desde la BD
            _analisisSeleccionado = new Analisis
            {
                Id = int.Parse(txtCodigoAnalisis.Text)
            };
        }

        private void BtnRecepcionar_Click(object sender, EventArgs e)
        {
            if (_analisisSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un análisis para recepcionar.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"¿Confirma la recepción de la muestra para el análisis ID {_analisisSeleccionado.Id}?\n\n" +
                $"Paciente: {txtPaciente.Text}\n" +
                $"Tipo: {txtTipoAnalisis.Text}\n" +
                $"Fecha recepción: {dtpFechaRecepcion.Value:dd/MM/yyyy HH:mm}",
                "Confirmar Recepción",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                // TODO: Ejecutar RF-17
                // UPDATE analisis 
                // SET dni_recepcion = @DniAsistente, 
                //     fecha_recepcion = @FechaRecepcion
                // WHERE id_analisis = @IdAnalisis

                MessageBox.Show(
                    $"? Muestra recepcionada exitosamente\n\n" +
                    $"ID Análisis: {_analisisSeleccionado.Id}\n" +
                    $"Asistente: {_nombreUsuario}\n" +
                    $"Supervisor: {_nombreSupervisor}\n" +
                    $"Fecha: {dtpFechaRecepcion.Value:dd/MM/yyyy HH:mm}",
                    "Recepción Exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LimpiarFormulario();
                LoadAnalisisPendientes();
            }
        }

        private void LimpiarFormulario()
        {
            txtCodigoAnalisis.Clear();
            txtPaciente.Clear();
            txtTipoAnalisis.Clear();
            txtObservaciones.Clear();
            dtpFechaRecepcion.Value = DateTime.Now;
            btnRecepcionar.Enabled = false;
            _analisisSeleccionado = null;
        }
    }
}

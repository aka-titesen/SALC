// Views/AdminCatalogForm.cs - Version stub for compilation
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SALC.Views
{
    public partial class AdminCatalogFormStub : Form
    {
        public AdminCatalogFormStub()
        {
            InitializeComponent();
            this.Text = "SALC - Gestión de Catálogos";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            
            CreateControls();
        }

        private void CreateControls()
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(20)
            };
            
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Título
            var titleLabel = new Label
            {
                Text = "Gestión de Catálogos del Sistema",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };

            // Panel de opciones
            var optionsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(50)
            };

            var catalogOptions = new string[]
            {
                "?? Tipos de Análisis",
                "?? Métricas y Parámetros", 
                "?? Obras Sociales",
                "?? Estados del Sistema",
                "?? Roles de Usuario"
            };

            foreach (var option in catalogOptions)
            {
                var button = new Button
                {
                    Text = option,
                    Size = new Size(300, 50),
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                    Margin = new Padding(10)
                };
                button.FlatAppearance.BorderSize = 0;
                button.Click += (s, e) => 
                {
                    MessageBox.Show($"Funcionalidad '{option}' en desarrollo.\n\nSe implementará en la próxima versión del sistema.", 
                        "SALC - Desarrollo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };
                optionsPanel.Controls.Add(button);
            }

            // Botón cerrar
            var closeButton = new Button
            {
                Text = "Cerrar",
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Right
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            mainPanel.Controls.Add(titleLabel, 0, 0);
            mainPanel.Controls.Add(optionsPanel, 0, 1);
            mainPanel.Controls.Add(closeButton, 0, 2);

            this.Controls.Add(mainPanel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
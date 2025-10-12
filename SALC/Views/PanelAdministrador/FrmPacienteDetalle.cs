using System;
using System.Windows.Forms;
using SALC.Domain;
using SALC.DAL;

namespace SALC.Views.PanelAdministrador
{
    public class FrmPacienteDetalle : Form
    {
        private GroupBox gbDatosPersonales;
        private GroupBox gbDatosContacto;
        private GroupBox gbObraSocial;
        private Label lblDni, lblNombre, lblApellido, lblFechaNac, lblEdad, lblSexo, lblEstado;
        private Label lblDniVal, lblNombreVal, lblApellidoVal, lblFechaNacVal, lblEdadVal, lblSexoVal, lblEstadoVal;
        private Label lblEmail, lblTelefono;
        private Label lblEmailVal, lblTelefonoVal;
        private Label lblObraSocialNombre, lblObraSocialCuit;
        private Label lblObraSocialNombreVal, lblObraSocialCuitVal;
        private Button btnCerrar;

        private readonly int _dniPaciente;
        private readonly ObraSocialRepositorio _obraSocialRepo;

        public FrmPacienteDetalle(Paciente paciente)
        {
            if (paciente == null) throw new ArgumentNullException(nameof(paciente));
            
            _dniPaciente = paciente.Dni;
            _obraSocialRepo = new ObraSocialRepositorio();

            InitializeComponent();
            CargarDatosPaciente(paciente);
            CargarDatosObraSocial(paciente.IdObraSocial);
        }

        private void InitializeComponent()
        {
            Text = "Detalle de Paciente";
            Width = 520;
            Height = 480;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            // GroupBox Datos Personales
            gbDatosPersonales = new GroupBox
            {
                Text = "Datos Personales",
                Left = 20,
                Top = 20,
                Width = 460,
                Height = 160
            };
            Controls.Add(gbDatosPersonales);

            int labelTop = 30;
            int valueTop = 30;
            int leftLabel = 20;
            int leftValue = 120;
            int step = 22;

            // DNI
            lblDni = new Label { Text = "DNI:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblDniVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 150 };
            gbDatosPersonales.Controls.Add(lblDni);
            gbDatosPersonales.Controls.Add(lblDniVal);
            labelTop += step;
            valueTop += step;

            // Nombre
            lblNombre = new Label { Text = "Nombre:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblNombreVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 200 };
            gbDatosPersonales.Controls.Add(lblNombre);
            gbDatosPersonales.Controls.Add(lblNombreVal);
            labelTop += step;
            valueTop += step;

            // Apellido
            lblApellido = new Label { Text = "Apellido:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblApellidoVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 200 };
            gbDatosPersonales.Controls.Add(lblApellido);
            gbDatosPersonales.Controls.Add(lblApellidoVal);
            labelTop += step;
            valueTop += step;

            // Fecha de Nacimiento y Edad en la misma línea
            lblFechaNac = new Label { Text = "Fecha Nac:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblFechaNacVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 100 };
            lblEdad = new Label { Text = "Edad:", Left = leftValue + 110, Top = labelTop, Width = 40, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblEdadVal = new Label { Text = "", Left = leftValue + 150, Top = valueTop, Width = 80 };
            gbDatosPersonales.Controls.Add(lblFechaNac);
            gbDatosPersonales.Controls.Add(lblFechaNacVal);
            gbDatosPersonales.Controls.Add(lblEdad);
            gbDatosPersonales.Controls.Add(lblEdadVal);
            labelTop += step;
            valueTop += step;

            // Sexo y Estado en la misma línea
            lblSexo = new Label { Text = "Sexo:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblSexoVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 100 };
            lblEstado = new Label { Text = "Estado:", Left = leftValue + 110, Top = labelTop, Width = 50, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblEstadoVal = new Label { Text = "", Left = leftValue + 160, Top = valueTop, Width = 80 };
            gbDatosPersonales.Controls.Add(lblSexo);
            gbDatosPersonales.Controls.Add(lblSexoVal);
            gbDatosPersonales.Controls.Add(lblEstado);
            gbDatosPersonales.Controls.Add(lblEstadoVal);

            // GroupBox Datos de Contacto
            gbDatosContacto = new GroupBox
            {
                Text = "Datos de Contacto",
                Left = 20,
                Top = 200,
                Width = 460,
                Height = 80
            };
            Controls.Add(gbDatosContacto);

            // Email
            lblEmail = new Label { Text = "Email:", Left = 20, Top = 30, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblEmailVal = new Label { Text = "", Left = 100, Top = 30, Width = 340 };
            gbDatosContacto.Controls.Add(lblEmail);
            gbDatosContacto.Controls.Add(lblEmailVal);

            // Teléfono
            lblTelefono = new Label { Text = "Teléfono:", Left = 20, Top = 52, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblTelefonoVal = new Label { Text = "", Left = 100, Top = 52, Width = 200 };
            gbDatosContacto.Controls.Add(lblTelefono);
            gbDatosContacto.Controls.Add(lblTelefonoVal);

            // GroupBox Obra Social
            gbObraSocial = new GroupBox
            {
                Text = "Obra Social",
                Left = 20,
                Top = 300,
                Width = 460,
                Height = 80
            };
            Controls.Add(gbObraSocial);

            // Nombre Obra Social
            lblObraSocialNombre = new Label { Text = "Nombre:", Left = 20, Top = 30, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblObraSocialNombreVal = new Label { Text = "", Left = 100, Top = 30, Width = 340 };
            gbObraSocial.Controls.Add(lblObraSocialNombre);
            gbObraSocial.Controls.Add(lblObraSocialNombreVal);

            // CUIT Obra Social
            lblObraSocialCuit = new Label { Text = "CUIT:", Left = 20, Top = 52, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblObraSocialCuitVal = new Label { Text = "", Left = 100, Top = 52, Width = 200 };
            gbObraSocial.Controls.Add(lblObraSocialCuit);
            gbObraSocial.Controls.Add(lblObraSocialCuitVal);

            // Botón Cerrar
            btnCerrar = new Button
            {
                Text = "Cerrar",
                Left = 405,
                Top = 400,
                Width = 80,
                DialogResult = DialogResult.OK
            };
            AcceptButton = btnCerrar;
            Controls.Add(btnCerrar);
        }

        private void CargarDatosPaciente(Paciente paciente)
        {
            lblDniVal.Text = paciente.Dni.ToString();
            lblNombreVal.Text = paciente.Nombre;
            lblApellidoVal.Text = paciente.Apellido;
            lblFechaNacVal.Text = paciente.FechaNac.ToString("dd/MM/yyyy");
            
            // Calcular edad
            var hoy = DateTime.Today;
            var edad = hoy.Year - paciente.FechaNac.Year;
            if (paciente.FechaNac.Date > hoy.AddYears(-edad)) edad--;
            lblEdadVal.Text = $"{edad} años";

            lblSexoVal.Text = ObtenerDescripcionSexo(paciente.Sexo);
            lblEstadoVal.Text = paciente.Estado;

            // Colorear el estado
            lblEstadoVal.ForeColor = paciente.Estado == "Activo" 
                ? System.Drawing.Color.Green 
                : System.Drawing.Color.Red;

            // Datos de contacto
            lblEmailVal.Text = string.IsNullOrWhiteSpace(paciente.Email) ? "(No especificado)" : paciente.Email;
            lblTelefonoVal.Text = string.IsNullOrWhiteSpace(paciente.Telefono) ? "(No especificado)" : paciente.Telefono;
        }

        private void CargarDatosObraSocial(int? idObraSocial)
        {
            try
            {
                if (idObraSocial.HasValue)
                {
                    var obraSocial = _obraSocialRepo.ObtenerPorId(idObraSocial.Value);
                    if (obraSocial != null)
                    {
                        lblObraSocialNombreVal.Text = obraSocial.Nombre;
                        lblObraSocialCuitVal.Text = obraSocial.Cuit;
                        
                        // Si la obra social está inactiva, mostrarlo
                        if (obraSocial.Estado == "Inactivo")
                        {
                            lblObraSocialNombreVal.Text += " (INACTIVA)";
                            lblObraSocialNombreVal.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    else
                    {
                        lblObraSocialNombreVal.Text = "Obra social no encontrada";
                        lblObraSocialCuitVal.Text = "-";
                        lblObraSocialNombreVal.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    lblObraSocialNombreVal.Text = "Sin obra social";
                    lblObraSocialCuitVal.Text = "-";
                    lblObraSocialNombreVal.ForeColor = System.Drawing.Color.Gray;
                }
            }
            catch (Exception ex)
            {
                lblObraSocialNombreVal.Text = $"Error al cargar obra social: {ex.Message}";
                lblObraSocialCuitVal.Text = "-";
                lblObraSocialNombreVal.ForeColor = System.Drawing.Color.Red;
            }
        }

        private string ObtenerDescripcionSexo(char sexo)
        {
            switch (sexo)
            {
                case 'M': return "Masculino";
                case 'F': return "Femenino";
                case 'X': return "Otro";
                default: return "No especificado";
            }
        }
    }
}
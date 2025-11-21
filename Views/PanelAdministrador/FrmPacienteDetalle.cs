using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Domain;
using SALC.DAL;

namespace SALC.Views.PanelAdministrador
{
    public class FrmPacienteDetalle : Form
    {
        private readonly int _dniPaciente;
        private readonly ObraSocialRepositorio _obraSocialRepo;
        private Button btnCerrar;

        public FrmPacienteDetalle(Paciente paciente)
        {
            if (paciente == null) throw new ArgumentNullException(nameof(paciente));

            _dniPaciente = paciente.Dni;
            _obraSocialRepo = new ObraSocialRepositorio();

            InitializeComponent(paciente);
        }

        private void InitializeComponent(Paciente paciente)
        {
            Text = "Información del Paciente";
            Width = 520;
            Height = 600;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            // Título
            var lblTitulo = new Label
            {
                Text = "Información Completa del Paciente",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, 15),
                Size = new Size(470, 30),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = "Vista detallada de todos los datos del paciente (solo lectura)",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(470, 20),
                BackColor = Color.Transparent
            };

            // Sección: Datos Personales
            var lblSeccionPersonal = new Label
            {
                Text = "Datos Personales",
                Left = 20,
                Top = 80,
                Width = 470,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblDni = new Label { Text = "DNI:", Left = 20, Top = 115, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblDniVal = CreateReadOnlyLabel(paciente.Dni.ToString(), 170, 115);

            var lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 150, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblNombreVal = CreateReadOnlyLabel(paciente.Nombre, 170, 150);

            var lblApellido = new Label { Text = "Apellido:", Left = 20, Top = 185, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblApellidoVal = CreateReadOnlyLabel(paciente.Apellido, 170, 185);

            var lblFechaNac = new Label { Text = "Fecha Nac:", Left = 20, Top = 220, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var edad = CalcularEdad(paciente.FechaNac);
            var lblFechaNacVal = CreateReadOnlyLabel(string.Format("{0} ({1} años)", paciente.FechaNac.ToString("dd/MM/yyyy"), edad), 170, 220);

            var lblSexo = new Label { Text = "Sexo:", Left = 20, Top = 255, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblSexoVal = CreateReadOnlyLabel(ObtenerDescripcionSexo(paciente.Sexo), 170, 255);

            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 290, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblEstadoVal = new Label
            {
                Text = paciente.Estado,
                Left = 170,
                Top = 290,
                Width = 300,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255),
                ForeColor = paciente.Estado == "Activo" ? Color.FromArgb(39, 174, 96) : Color.FromArgb(192, 57, 43),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Sección: Datos de Contacto
            var lblSeccionContacto = new Label
            {
                Text = "Datos de Contacto",
                Left = 20,
                Top = 330,
                Width = 470,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblEmail = new Label { Text = "Email:", Left = 20, Top = 365, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblEmailVal = CreateReadOnlyLabel(
                string.IsNullOrWhiteSpace(paciente.Email) ? "(No especificado)" : paciente.Email,
                170, 365);

            var lblTelefono = new Label { Text = "Teléfono:", Left = 20, Top = 400, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblTelefonoVal = CreateReadOnlyLabel(
                string.IsNullOrWhiteSpace(paciente.Telefono) ? "(No especificado)" : paciente.Telefono,
                170, 400);

            // Sección: Obra Social
            var lblSeccionObraSocial = new Label
            {
                Text = "Obra Social",
                Left = 20,
                Top = 440,
                Width = 470,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var datosObraSocial = ObtenerDatosObraSocial(paciente.IdObraSocial);

            var lblObraSocial = new Label { Text = "Nombre:", Left = 20, Top = 475, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblObraSocialVal = CreateReadOnlyLabel(datosObraSocial.Item1, 170, 475);

            var lblCuitOS = new Label { Text = "CUIT:", Left = 20, Top = 510, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblCuitOSVal = CreateReadOnlyLabel(datosObraSocial.Item2, 170, 510);

            // Botón Cerrar
            btnCerrar = new Button
            {
                Text = "Cerrar",
                Left = 380,
                Top = 555,
                Width = 90,
                Height = 35,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;

            AcceptButton = btnCerrar;

            Controls.AddRange(new Control[]
            {
                lblTitulo, lblSubtitulo,
                lblSeccionPersonal,
                lblDni, lblDniVal,
                lblNombre, lblNombreVal,
                lblApellido, lblApellidoVal,
                lblFechaNac, lblFechaNacVal,
                lblSexo, lblSexoVal,
                lblEstado, lblEstadoVal,
                lblSeccionContacto,
                lblEmail, lblEmailVal,
                lblTelefono, lblTelefonoVal,
                lblSeccionObraSocial,
                lblObraSocial, lblObraSocialVal,
                lblCuitOS, lblCuitOSVal,
                btnCerrar
            });
        }

        private Label CreateReadOnlyLabel(string text, int left, int top)
        {
            return new Label
            {
                Text = text,
                Left = left,
                Top = top,
                Width = 300,
                Height = 25,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255),
                ForeColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        private Tuple<string, string> ObtenerDatosObraSocial(int? idObraSocial)
        {
            try
            {
                if (idObraSocial.HasValue)
                {
                    var obraSocial = _obraSocialRepo.ObtenerPorId(idObraSocial.Value);
                    if (obraSocial != null)
                    {
                        var nombre = obraSocial.Nombre;
                        if (obraSocial.Estado == "Inactivo")
                            nombre += " (INACTIVA)";
                        return Tuple.Create(nombre, obraSocial.Cuit);
                    }
                    return Tuple.Create("Obra social no encontrada", "-");
                }
                return Tuple.Create("Sin obra social", "-");
            }
            catch (Exception)
            {
                return Tuple.Create("Error al cargar obra social", "-");
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
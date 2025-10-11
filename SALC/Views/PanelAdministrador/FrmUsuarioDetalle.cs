using System;
using System.Windows.Forms;
using SALC.Domain;
using SALC.DAL;

namespace SALC.Views.PanelAdministrador
{
    public class FrmUsuarioDetalle : Form
    {
        private GroupBox gbDatosGenerales;
        private GroupBox gbDatosEspecificos;
        private Label lblDni, lblNombre, lblApellido, lblEmail, lblRol, lblEstado;
        private Label lblDniVal, lblNombreVal, lblApellidoVal, lblEmailVal, lblRolVal, lblEstadoVal;
        private Label lblEspecificos;
        private Button btnCerrar;

        private readonly int _dniUsuario;
        private readonly MedicoRepositorio _medicoRepo;
        private readonly AsistenteRepositorio _asistenteRepo;

        public FrmUsuarioDetalle(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            
            _dniUsuario = usuario.Dni;
            _medicoRepo = new MedicoRepositorio();
            _asistenteRepo = new AsistenteRepositorio();

            InitializeComponent();
            CargarDatosUsuario(usuario);
            CargarDatosEspecificos(usuario.IdRol);
        }

        private void InitializeComponent()
        {
            Text = "Detalle de Usuario";
            Width = 500;
            Height = 400;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            // GroupBox Datos Generales
            gbDatosGenerales = new GroupBox
            {
                Text = "Datos Generales",
                Left = 20,
                Top = 20,
                Width = 440,
                Height = 180
            };
            Controls.Add(gbDatosGenerales);

            int labelTop = 30;
            int valueTop = 30;
            int leftLabel = 20;
            int leftValue = 120;
            int step = 25;

            // DNI
            lblDni = new Label { Text = "DNI:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblDniVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 200 };
            gbDatosGenerales.Controls.Add(lblDni);
            gbDatosGenerales.Controls.Add(lblDniVal);
            labelTop += step;
            valueTop += step;

            // Nombre
            lblNombre = new Label { Text = "Nombre:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblNombreVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 200 };
            gbDatosGenerales.Controls.Add(lblNombre);
            gbDatosGenerales.Controls.Add(lblNombreVal);
            labelTop += step;
            valueTop += step;

            // Apellido
            lblApellido = new Label { Text = "Apellido:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblApellidoVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 200 };
            gbDatosGenerales.Controls.Add(lblApellido);
            gbDatosGenerales.Controls.Add(lblApellidoVal);
            labelTop += step;
            valueTop += step;

            // Email
            lblEmail = new Label { Text = "Email:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblEmailVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 280 };
            gbDatosGenerales.Controls.Add(lblEmail);
            gbDatosGenerales.Controls.Add(lblEmailVal);
            labelTop += step;
            valueTop += step;

            // Rol
            lblRol = new Label { Text = "Rol:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblRolVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 200 };
            gbDatosGenerales.Controls.Add(lblRol);
            gbDatosGenerales.Controls.Add(lblRolVal);
            labelTop += step;
            valueTop += step;

            // Estado
            lblEstado = new Label { Text = "Estado:", Left = leftLabel, Top = labelTop, Width = 80, Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold) };
            lblEstadoVal = new Label { Text = "", Left = leftValue, Top = valueTop, Width = 200 };
            gbDatosGenerales.Controls.Add(lblEstado);
            gbDatosGenerales.Controls.Add(lblEstadoVal);

            // GroupBox Datos Específicos
            gbDatosEspecificos = new GroupBox
            {
                Text = "Datos Específicos del Rol",
                Left = 20,
                Top = 220,
                Width = 440,
                Height = 100
            };
            Controls.Add(gbDatosEspecificos);

            lblEspecificos = new Label
            {
                Text = "",
                Left = 20,
                Top = 30,
                Width = 400,
                Height = 60,
                AutoSize = false
            };
            gbDatosEspecificos.Controls.Add(lblEspecificos);

            // Botón Cerrar
            btnCerrar = new Button
            {
                Text = "Cerrar",
                Left = 385,
                Top = 340,
                Width = 80,
                DialogResult = DialogResult.OK
            };
            AcceptButton = btnCerrar;
            Controls.Add(btnCerrar);
        }

        private void CargarDatosUsuario(Usuario usuario)
        {
            lblDniVal.Text = usuario.Dni.ToString();
            lblNombreVal.Text = usuario.Nombre;
            lblApellidoVal.Text = usuario.Apellido;
            lblEmailVal.Text = usuario.Email;
            lblRolVal.Text = ObtenerNombreRol(usuario.IdRol);
            lblEstadoVal.Text = usuario.Estado;

            // Colorear el estado
            lblEstadoVal.ForeColor = usuario.Estado == "Activo" 
                ? System.Drawing.Color.Green 
                : System.Drawing.Color.Red;
        }

        private void CargarDatosEspecificos(int idRol)
        {
            try
            {
                switch (idRol)
                {
                    case 1: // Administrador
                        lblEspecificos.Text = "Usuario administrador con acceso completo al sistema.\n" +
                                            "Puede gestionar usuarios, pacientes, análisis y configuraciones.";
                        break;
                    case 2: // Médico
                        var medico = _medicoRepo.ObtenerPorId(_dniUsuario);
                        if (medico != null)
                        {
                            lblEspecificos.Text = $"Número de Matrícula: {medico.NroMatricula}\n" +
                                                $"Especialidad: {medico.Especialidad}\n" +
                                                "Puede crear, cargar y validar análisis clínicos.";
                        }
                        else
                        {
                            lblEspecificos.Text = "Error: No se encontraron datos específicos de médico.";
                            lblEspecificos.ForeColor = System.Drawing.Color.Red;
                        }
                        break;
                    case 3: // Asistente
                        var asistente = _asistenteRepo.ObtenerPorId(_dniUsuario);
                        if (asistente != null)
                        {
                            lblEspecificos.Text = $"DNI Supervisor: {asistente.DniSupervisor}\n" +
                                                $"Fecha de Ingreso: {asistente.FechaIngreso:dd/MM/yyyy}\n" +
                                                "Puede visualizar análisis y generar informes de análisis verificados.";
                        }
                        else
                        {
                            lblEspecificos.Text = "Error: No se encontraron datos específicos de asistente.";
                            lblEspecificos.ForeColor = System.Drawing.Color.Red;
                        }
                        break;
                    default:
                        lblEspecificos.Text = "Rol no reconocido.";
                        break;
                }
            }
            catch (Exception ex)
            {
                lblEspecificos.Text = $"Error al cargar datos específicos: {ex.Message}";
                lblEspecificos.ForeColor = System.Drawing.Color.Red;
            }
        }

        private string ObtenerNombreRol(int idRol)
        {
            switch (idRol)
            {
                case 1: return "Administrador";
                case 2: return "Médico";
                case 3: return "Asistente";
                default: return "Desconocido";
            }
        }
    }
}
using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Domain;
using SALC.DAL;

namespace SALC.Views.PanelAdministrador
{
    public class FrmUsuarioDetalle : Form
    {
        private readonly int _dniUsuario;
        private readonly MedicoRepositorio _medicoRepo;
        private readonly AsistenteRepositorio _asistenteRepo;
        private Button btnCerrar;

        public FrmUsuarioDetalle(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));

            _dniUsuario = usuario.Dni;
            _medicoRepo = new MedicoRepositorio();
            _asistenteRepo = new AsistenteRepositorio();

            InitializeComponent(usuario);
        }

        private void InitializeComponent(Usuario usuario)
        {
            Text = "Información del Usuario";
            Width = 500;
            Height = 520;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            // Título del formulario
            var lblTitulo = new Label
            {
                Text = "Información Completa del Usuario",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, 15),
                Size = new Size(450, 30),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = "Vista detallada de todos los datos del usuario (solo lectura)",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(450, 20),
                BackColor = Color.Transparent
            };

            // Sección: Datos Generales
            var lblSeccionGeneral = new Label
            {
                Text = "Datos Generales",
                Left = 20,
                Top = 80,
                Width = 450,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Campos de solo lectura
            var lblDni = new Label { Text = "DNI:", Left = 20, Top = 115, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblDniVal = new Label 
            { 
                Text = usuario.Dni.ToString(), 
                Left = 170, 
                Top = 115, 
                Width = 280, 
                Height = 25,
                Font = new Font("Segoe UI", 10), 
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 150, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblNombreVal = new Label 
            { 
                Text = usuario.Nombre, 
                Left = 170, 
                Top = 150, 
                Width = 280, 
                Height = 25,
                Font = new Font("Segoe UI", 10), 
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblApellido = new Label { Text = "Apellido:", Left = 20, Top = 185, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblApellidoVal = new Label 
            { 
                Text = usuario.Apellido, 
                Left = 170, 
                Top = 185, 
                Width = 280, 
                Height = 25,
                Font = new Font("Segoe UI", 10), 
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblEmail = new Label { Text = "Email:", Left = 20, Top = 220, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblEmailVal = new Label 
            { 
                Text = usuario.Email, 
                Left = 170, 
                Top = 220, 
                Width = 280, 
                Height = 25,
                Font = new Font("Segoe UI", 10), 
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblRol = new Label { Text = "Rol:", Left = 20, Top = 255, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblRolVal = new Label 
            { 
                Text = ObtenerNombreRol(usuario.IdRol), 
                Left = 170, 
                Top = 255, 
                Width = 280, 
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold), 
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 250, 255),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblEstado = new Label { Text = "Estado:", Left = 20, Top = 290, Width = 140, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var lblEstadoVal = new Label 
            { 
                Text = usuario.Estado, 
                Left = 170, 
                Top = 290, 
                Width = 140, 
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold), 
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255),
                ForeColor = usuario.Estado == "Activo" ? Color.FromArgb(39, 174, 96) : Color.FromArgb(192, 57, 43),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Sección: Datos Específicos
            var lblSeccionEspecifico = new Label
            {
                Text = "Datos Específicos del Rol",
                Left = 20,
                Top = 330,
                Width = 450,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblEspecificos = new Label
            {
                Text = ObtenerDatosEspecificos(usuario.IdRol),
                Left = 20,
                Top = 365,
                Width = 450,
                Height = 80,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255),
                ForeColor = Color.FromArgb(44, 62, 80),
                Padding = new Padding(15),
                AutoSize = false
            };

            // Botón Cerrar
            btnCerrar = new Button
            {
                Text = "Cerrar",
                Left = 360,
                Top = 465,
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
                lblSeccionGeneral,
                lblDni, lblDniVal,
                lblNombre, lblNombreVal,
                lblApellido, lblApellidoVal,
                lblEmail, lblEmailVal,
                lblRol, lblRolVal,
                lblEstado, lblEstadoVal,
                lblSeccionEspecifico,
                lblEspecificos,
                btnCerrar
            });
        }

        private string ObtenerDatosEspecificos(int idRol)
        {
            try
            {
                switch (idRol)
                {
                    case 1: // Administrador
                        return "Rol: Administrador del Sistema\n\n" +
                               "Permisos: Acceso completo al sistema.\n" +
                               "Puede gestionar usuarios, pacientes, análisis, catálogos y configuraciones.";

                    case 2: // Médico
                        var medico = _medicoRepo.ObtenerPorId(_dniUsuario);
                        if (medico != null)
                        {
                            return $"Rol: Médico Clínico\n\n" +
                                   $"Matrícula Profesional: {medico.NroMatricula}\n" +
                                   $"Especialidad: {medico.Especialidad}\n\n" +
                                   "Permisos: Crear análisis, cargar resultados y validar/firmar estudios.";
                        }
                        return "Error: No se encontraron datos específicos de médico.";

                    case 3: // Asistente
                        var asistente = _asistenteRepo.ObtenerPorId(_dniUsuario);
                        if (asistente != null)
                        {
                            return $"Rol: Asistente de Laboratorio\n\n" +
                                   $"DNI Supervisor: {asistente.DniSupervisor}\n" +
                                   $"Fecha de Ingreso: {asistente.FechaIngreso:dd/MM/yyyy}\n\n" +
                                   "Permisos: Gestionar pacientes, visualizar análisis y generar informes.";
                        }
                        return "Error: No se encontraron datos específicos de asistente.";

                    default:
                        return "Rol no reconocido en el sistema.";
                }
            }
            catch (Exception ex)
            {
                return $"Error al cargar datos específicos:\n{ex.Message}";
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
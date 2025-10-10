using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SALC.Models;
using SALC.Services;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para agregar nuevos análisis al sistema
    /// Implementa RF-05: Crear Análisis según ERS v2.7
    /// </summary>
    public partial class AgregarEstudio : Form
    {
        private readonly AnalisisService _analisisService;
        private readonly PacienteService _pacienteService;
        private List<Paciente> _pacientes;
        private List<TipoAnalisis> _tiposAnalisis;
        private int _dniMedicoActual;

        public AgregarEstudio(int dniMedico)
        {
            InitializeComponent();
            _analisisService = new AnalisisService();
            _pacienteService = new PacienteService();
            _dniMedicoActual = dniMedico;
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                CargarPacientes();
                CargarTiposAnalisis();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPacientes()
        {
            _pacientes = _pacienteService.ObtenerPacientes();
            
            cmbPaciente.Items.Clear();
            cmbPaciente.Items.Add(new ElementoComboBox("Seleccionar paciente...", 0));
            
            foreach (var paciente in _pacientes)
            {
                string texto = $"{paciente.NombreCompleto} - DNI: {paciente.Dni}";
                cmbPaciente.Items.Add(new ElementoComboBox(texto, paciente.Dni));
            }
            
            cmbPaciente.SelectedIndex = 0;
        }

        private void CargarTiposAnalisis()
        {
            _tiposAnalisis = _analisisService.ObtenerTiposAnalisis();
            
            cmbTipoAnalisis.Items.Clear();
            cmbTipoAnalisis.Items.Add(new ElementoComboBox("Seleccionar tipo de análisis...", 0));
            
            foreach (var tipo in _tiposAnalisis)
            {
                cmbTipoAnalisis.Items.Add(new ElementoComboBox(tipo.Descripcion, tipo.IdTipoAnalisis));
            }
            
            cmbTipoAnalisis.SelectedIndex = 0;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarDatos())
                {
                    var pacienteSeleccionado = (ElementoComboBox)cmbPaciente.SelectedItem;
                    var tipoAnalisisSeleccionado = (ElementoComboBox)cmbTipoAnalisis.SelectedItem;

                    var analisis = new Analisis
                    {
                        IdTipoAnalisis = (int)tipoAnalisisSeleccionado.Valor,
                        IdEstado = 1, // Sin verificar por defecto
                        DniPaciente = (int)pacienteSeleccionado.Valor,
                        DniCarga = _dniMedicoActual,
                        FechaCreacion = DateTime.Now,
                        Observaciones = string.IsNullOrWhiteSpace(txtObservaciones.Text) ? 
                                       null : txtObservaciones.Text.Trim()
                    };

                    bool exito = _analisisService.CrearAnalisis(analisis);

                    if (exito)
                    {
                        MessageBox.Show("Análisis creado exitosamente.", "Éxito", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al crear el análisis.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidarDatos()
        {
            // Validar paciente seleccionado
            if (cmbPaciente.SelectedIndex <= 0)
            {
                MessageBox.Show("Debe seleccionar un paciente.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbPaciente.Focus();
                return false;
            }

            // Validar tipo de análisis seleccionado
            if (cmbTipoAnalisis.SelectedIndex <= 0)
            {
                MessageBox.Show("Debe seleccionar un tipo de análisis.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTipoAnalisis.Focus();
                return false;
            }

            return true;
        }

        private class ElementoComboBox
        {
            public string Texto { get; set; }
            public object Valor { get; set; }

            public ElementoComboBox(string texto, object valor)
            {
                Texto = texto;
                Valor = valor;
            }

            public override string ToString()
            {
                return Texto;
            }
        }
    }
}
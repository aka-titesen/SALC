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
    /// Formulario para agregar nuevos pacientes al sistema
    /// Implementa RF-03: ABM de Pacientes según ERS v2.7
    /// </summary>
    public partial class AgregarPaciente : Form
    {
        private readonly PacienteService _pacienteService;
        private List<ObraSocial> _obrasSociales;

        public AgregarPaciente()
        {
            InitializeComponent();
            _pacienteService = new PacienteService();
            CargarObrasSociales();
        }

        private void CargarObrasSociales()
        {
            try
            {
                _obrasSociales = _pacienteService.ObtenerObrasSociales();
                
                cmbObraSocial.Items.Clear();
                cmbObraSocial.Items.Add(new ComboBoxItem("Seleccionar obra social...", 0));
                
                foreach (var obraSocial in _obrasSociales)
                {
                    cmbObraSocial.Items.Add(new ComboBoxItem($"{obraSocial.Nombre} - {obraSocial.Cuit}", obraSocial.IdObraSocial));
                }
                
                cmbObraSocial.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar obras sociales: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarDatos())
                {
                    var paciente = new Paciente
                    {
                        Dni = int.Parse(txtDni.Text.Trim()),
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        FechaNacimiento = dtpFechaNacimiento.Value,
                        Sexo = rbMasculino.Checked ? "M" : rbFemenino.Checked ? "F" : "X",
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                        Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                        IdObraSocial = cmbObraSocial.SelectedIndex > 0 ? 
                                      ((ComboBoxItem)cmbObraSocial.SelectedItem).Value : 
                                      (int?)null
                    };

                    bool exito = _pacienteService.CrearPaciente(paciente);

                    if (exito)
                    {
                        MessageBox.Show("Paciente agregado exitosamente.", "Éxito", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al agregar el paciente.", "Error", 
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
            // Validar DNI
            if (string.IsNullOrWhiteSpace(txtDni.Text))
            {
                MessageBox.Show("El DNI es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return false;
            }

            if (!int.TryParse(txtDni.Text, out int dni) || dni <= 0)
            {
                MessageBox.Show("El DNI debe ser un número válido y positivo.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return false;
            }

            // Validar nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            // Validar apellido
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El apellido es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            // Validar fecha de nacimiento
            if (dtpFechaNacimiento.Value > DateTime.Now)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser futura.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaNacimiento.Focus();
                return false;
            }

            // Validar edad mínima (0 años) y máxima (150 años)
            int edad = DateTime.Now.Year - dtpFechaNacimiento.Value.Year;
            if (edad > 150)
            {
                MessageBox.Show("La fecha de nacimiento no es válida (edad superior a 150 años).", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaNacimiento.Focus();
                return false;
            }

            // Validar sexo
            if (!rbMasculino.Checked && !rbFemenino.Checked && !rbOtro.Checked)
            {
                MessageBox.Show("Debe seleccionar el sexo del paciente.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rbMasculino.Focus();
                return false;
            }

            // Validar email si está presente
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Por favor ingrese un email válido.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números y teclas de control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir números, espacios, guiones y paréntesis para teléfonos
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && 
                e.KeyChar != ' ' && e.KeyChar != '-' && e.KeyChar != '(' && e.KeyChar != ')')
            {
                e.Handled = true;
            }
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public ComboBoxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
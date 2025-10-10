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
    /// Formulario para agregar nuevas métricas al sistema
    /// Implementa RF-04: ABM de Catálogos según ERS v2.7
    /// </summary>
    public partial class AgregarMetrica : Form
    {
        private readonly MetricaService _metricaService;

        public AgregarMetrica()
        {
            InitializeComponent();
            _metricaService = new MetricaService();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarDatos())
                {
                    var metrica = new Metrica
                    {
                        Nombre = txtNombre.Text.Trim(),
                        UnidadMedida = txtUnidadMedida.Text.Trim(),
                        ValorMinimo = string.IsNullOrWhiteSpace(txtValorMinimo.Text) ? 
                                     (decimal?)null : 
                                     decimal.Parse(txtValorMinimo.Text),
                        ValorMaximo = string.IsNullOrWhiteSpace(txtValorMaximo.Text) ? 
                                     (decimal?)null : 
                                     decimal.Parse(txtValorMaximo.Text)
                    };

                    bool exito = _metricaService.CrearMetrica(metrica);

                    if (exito)
                    {
                        MessageBox.Show("Métrica agregada exitosamente.", "Éxito", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al agregar la métrica.", "Error", 
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
            // Validar nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre de la métrica es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            // Validar unidad de medida
            if (string.IsNullOrWhiteSpace(txtUnidadMedida.Text))
            {
                MessageBox.Show("La unidad de medida es obligatoria.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnidadMedida.Focus();
                return false;
            }

            // Validar valores numéricos si están presentes
            if (!string.IsNullOrWhiteSpace(txtValorMinimo.Text))
            {
                if (!decimal.TryParse(txtValorMinimo.Text, out decimal valorMin))
                {
                    MessageBox.Show("El valor mínimo debe ser un número válido.", "Validación", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtValorMinimo.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtValorMaximo.Text))
            {
                if (!decimal.TryParse(txtValorMaximo.Text, out decimal valorMax))
                {
                    MessageBox.Show("El valor máximo debe ser un número válido.", "Validación", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtValorMaximo.Focus();
                    return false;
                }
            }

            // Validar que el valor mínimo sea menor al máximo
            if (!string.IsNullOrWhiteSpace(txtValorMinimo.Text) && 
                !string.IsNullOrWhiteSpace(txtValorMaximo.Text))
            {
                decimal valorMin = decimal.Parse(txtValorMinimo.Text);
                decimal valorMax = decimal.Parse(txtValorMaximo.Text);

                if (valorMin >= valorMax)
                {
                    MessageBox.Show("El valor mínimo debe ser menor al valor máximo.", "Validación", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtValorMinimo.Focus();
                    return false;
                }
            }

            return true;
        }

        private void txtValorMinimo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números, punto decimal y teclas de control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            // Permitir solo un punto decimal
            if ((e.KeyChar == '.' || e.KeyChar == ',') && ((TextBox)sender).Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void txtValorMaximo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Misma validación que valor mínimo
            txtValorMinimo_KeyPress(sender, e);
        }
    }
}
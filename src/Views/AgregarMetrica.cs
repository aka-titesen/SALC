using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SALC.Models;
using SALC.Services;
using SALC.Common;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para agregar nuevas métricas al sistema
    /// Implementa RF-04: ABM de Catálogos según ERS v2.7
    /// Sigue el patrón MVP (Model-View-Presenter)
    /// </summary>
    public partial class AgregarMetrica : Form
    {
        #region Campos privados
        private readonly MetricaService _metricaService;
        private ErrorProvider _proveedorErrores;
        #endregion

        #region Constructor
        public AgregarMetrica()
        {
            InitializeComponent();
            _metricaService = new MetricaService();
            InicializarProveedorErrores();
            ConfigurarValidacionesTiempoReal();
        }
        #endregion

        #region Inicialización
        private void InicializarProveedorErrores()
        {
            _proveedorErrores = new ErrorProvider();
            _proveedorErrores.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        private void ConfigurarValidacionesTiempoReal()
        {
            // Validación en tiempo real para campos numéricos
            txtValorMinimo.Validating += TxtValorMinimo_Validating;
            txtValorMaximo.Validating += TxtValorMaximo_Validating;
            txtNombre.Validating += TxtNombre_Validating;
            txtUnidadMedida.Validating += TxtUnidadMedida_Validating;
        }
        #endregion

        #region Eventos de Validación en Tiempo Real (Patrón MVP - Vista)
        private void TxtNombre_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                _proveedorErrores.SetError(textBox, "El nombre de la métrica es obligatorio.");
                e.Cancel = true;
            }
            else
            {
                _proveedorErrores.SetError(textBox, "");
            }
        }

        private void TxtUnidadMedida_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                _proveedorErrores.SetError(textBox, "La unidad de medida es obligatoria.");
                e.Cancel = true;
            }
            else
            {
                _proveedorErrores.SetError(textBox, "");
            }
        }

        private void TxtValorMinimo_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (!decimal.TryParse(textBox.Text, out decimal valor))
                {
                    _proveedorErrores.SetError(textBox, "El valor mínimo debe ser un número válido.");
                    e.Cancel = true;
                }
                else
                {
                    _proveedorErrores.SetError(textBox, "");
                    ValidarRangoValores(); // Validar el rango cuando cambie el mínimo
                }
            }
            else
            {
                _proveedorErrores.SetError(textBox, "");
            }
        }

        private void TxtValorMaximo_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (!decimal.TryParse(textBox.Text, out decimal valor))
                {
                    _proveedorErrores.SetError(textBox, "El valor máximo debe ser un número válido.");
                    e.Cancel = true;
                }
                else
                {
                    _proveedorErrores.SetError(textBox, "");
                    ValidarRangoValores(); // Validar el rango cuando cambie el máximo
                }
            }
            else
            {
                _proveedorErrores.SetError(textBox, "");
            }
        }

        private void ValidarRangoValores()
        {
            if (!string.IsNullOrWhiteSpace(txtValorMinimo.Text) && 
                !string.IsNullOrWhiteSpace(txtValorMaximo.Text) &&
                decimal.TryParse(txtValorMinimo.Text, out decimal valorMin) &&
                decimal.TryParse(txtValorMaximo.Text, out decimal valorMax))
            {
                if (valorMin >= valorMax)
                {
                    _proveedorErrores.SetError(txtValorMinimo, "El valor mínimo debe ser menor al valor máximo.");
                    _proveedorErrores.SetError(txtValorMaximo, "El valor máximo debe ser mayor al valor mínimo.");
                }
                else
                {
                    _proveedorErrores.SetError(txtValorMinimo, "");
                    _proveedorErrores.SetError(txtValorMaximo, "");
                }
            }
        }
        #endregion

        #region Eventos de Botones
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Forzar validación de todos los campos antes de guardar
                if (!ValidarFormularioCompleto())
                {
                    MostrarMensajeError("Por favor, corrija los errores antes de continuar.");
                    return;
                }

                // Crear el modelo usando datos validados (Patrón MVP - Model)
                var metrica = CrearModeloMetrica();

                // Usar el servicio para validar y guardar (Patrón MVP - delegando al Service)
                bool exito = _metricaService.CrearMetrica(metrica);

                if (exito)
                {
                    MostrarMensajeExito("Métrica agregada exitosamente.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MostrarMensajeError("Error al agregar la métrica. Verifique que el nombre no esté duplicado.");
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError($"Error inesperado: {ex.Message}");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region Métodos de Validación (Patrón MVP - Vista)
        private bool ValidarFormularioCompleto()
        {
            bool esValido = true;

            // Validar usando el ErrorProvider existente
            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox)
                {
                    if (!string.IsNullOrEmpty(_proveedorErrores.GetError(textBox)))
                    {
                        esValido = false;
                    }
                }
            }

            // Validaciones adicionales específicas del formulario
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                _proveedorErrores.SetError(txtNombre, "El nombre de la métrica es obligatorio.");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(txtUnidadMedida.Text))
            {
                _proveedorErrores.SetError(txtUnidadMedida, "La unidad de medida es obligatoria.");
                esValido = false;
            }

            return esValido;
        }

        private Metrica CrearModeloMetrica()
        {
            return new Metrica
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
        }
        #endregion

        #region Métodos de Interfaz de Usuario (Patrón MVP - Vista)
        private void MostrarMensajeExito(string mensaje)
        {
            MessageBox.Show(mensaje, "Éxito", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MostrarMensajeError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region Validación de Entrada de Datos (KeyPress)
        private void txtValorMinimo_KeyPress(object sender, KeyPressEventArgs e)
        {
            ValidarEntradaNumerica(sender, e);
        }

        private void txtValorMaximo_KeyPress(object sender, KeyPressEventArgs e)
        {
            ValidarEntradaNumerica(sender, e);
        }

        private void ValidarEntradaNumerica(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números, punto decimal, coma decimal y teclas de control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && 
                e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            // Permitir solo un separador decimal
            var textBox = sender as TextBox;
            if ((e.KeyChar == '.' || e.KeyChar == ',') && 
                (textBox.Text.Contains('.') || textBox.Text.Contains(',')))
            {
                e.Handled = true;
            }
        }
        #endregion

        #region Limpieza de Recursos
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _proveedorErrores?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
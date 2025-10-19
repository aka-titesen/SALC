using System;
using SALC.BLL;
using SALC.Presenters.ViewsContracts;
using SALC.Views;

namespace SALC.Presenters
{
    public class LoginPresenter
    {
        private readonly ILoginView _view;
        private readonly IAutenticacionService _authService;

        public LoginPresenter(ILoginView view, IAutenticacionService authService)
        {
            _view = view;
            _authService = authService;
            _view.AccederClick += (s, e) => OnAcceder();
        }

        private void OnAcceder()
        {
            // Validación de entrada
            if (!int.TryParse(_view.DniTexto, out var dni))
            {
                _view.MostrarError("DNI debe ser numérico.");
                return;
            }
            
            var pass = _view.Contrasenia;
            if (string.IsNullOrWhiteSpace(pass))
            {
                _view.MostrarError("Ingrese la contraseña.");
                return;
            }

            try
            {
                // Intentar autenticación
                var usuario = _authService.ValidarCredenciales(dni, pass);
                if (usuario == null)
                {
                    _view.MostrarError("DNI o contraseña incorrectos, o usuario inactivo.");
                    _view.LimpiarCampos();
                    return;
                }

                // Crear el nuevo formulario principal con pestañas
                var frmPrincipalTabs = new FrmPrincipalConTabs(usuario);
                
                // Obtener referencia al login
                var loginForm = _view as System.Windows.Forms.Form;
                
                // Mostrar la ventana principal
                frmPrincipalTabs.Show();
                
                // Ocultar el login
                if (loginForm != null)
                {
                    loginForm.WindowState = System.Windows.Forms.FormWindowState.Minimized;
                    loginForm.Hide();
                }
                
                // Configurar que cuando se cierre la ventana principal, se cierre la aplicación
                frmPrincipalTabs.FormClosed += (s, e) => {
                    System.Windows.Forms.Application.Exit();
                };
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                // Error específico de base de datos
                string mensaje = "Error de conexión a la base de datos.\n\n";
                
                if (ex.Number == 4060) // Cannot open database
                {
                    mensaje += "La base de datos 'SALC' no existe o no es accesible.\n\n";
                    mensaje += "Pasos para solucionar:\n";
                    mensaje += "1. Abrir SQL Server Management Studio (SSMS)\n";
                    mensaje += "2. Ejecutar el script: SALC/Docs/estructura-salc.sql\n";
                    mensaje += "3. Ejecutar el script: SALC/Docs/lote-salc.sql\n";
                    mensaje += "4. Verificar la cadena de conexión en App.config";
                }
                else if (ex.Number == 18456) // Login failed
                {
                    mensaje += "Error de autenticación de Windows.\n";
                    mensaje += "Verificar permisos del usuario en SQL Server.";
                }
                else
                {
                    mensaje += $"Error SQL ({ex.Number}): {ex.Message}";
                }

                _view.MostrarError(mensaje);
            }
            catch (Exception ex)
            {
                // Error general
                _view.MostrarError($"Error inesperado: {ex.Message}");
            }
        }
    }
}

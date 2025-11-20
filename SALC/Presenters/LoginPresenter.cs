using System;
using SALC.BLL;
using SALC.Presenters.ViewsContracts;
using SALC.Views;
using SALC.Infraestructura;
using SALC.Infraestructura.Exceptions;

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
            try
            {
                // Validación de entrada
                if (!int.TryParse(_view.DniTexto, out var dni))
                {
                    _view.MostrarError("El DNI debe ser numérico.");
                    return;
                }
                
                var pass = _view.Contrasenia;
                if (string.IsNullOrWhiteSpace(pass))
                {
                    _view.MostrarError("Ingrese la contraseña.");
                    return;
                }

                ExceptionHandler.LogInfo($"Intento de acceso al sistema - DNI: {dni}", "Login");

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

                ExceptionHandler.LogInfo($"Acceso concedido - Usuario: {usuario.Nombre} {usuario.Apellido}, Rol: {usuario.IdRol}", "Login");
            }
            catch (SalcDatabaseException dbEx)
            {
                // Error específico de base de datos con mensaje amigable ya configurado
                _view.MostrarError(dbEx.UserFriendlyMessage);
                ExceptionHandler.LogWarning($"Error de BD en login: {dbEx.Message}", "Login");
            }
            catch (SalcValidacionException valEx)
            {
                // Error de validación
                _view.MostrarError(valEx.UserFriendlyMessage);
            }
            catch (SalcException salcEx)
            {
                // Otras excepciones SALC
                _view.MostrarError(salcEx.UserFriendlyMessage);
                ExceptionHandler.LogWarning($"Error en login: {salcEx.Message}", "Login");
            }
            catch (Exception ex)
            {
                // Error general no esperado
                var mensajeUsuario = ExceptionHandler.ManejarExcepcion(ex, "Login", mostrarAlUsuario: false);
                _view.MostrarError(mensajeUsuario);
            }
        }
    }
}

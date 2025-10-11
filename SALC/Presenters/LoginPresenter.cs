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
            if (!int.TryParse(_view.DniTexto, out var dni))
            {
                _view.MostrarError("DNI inválido.");
                return;
            }
            var pass = _view.Contrasenia;
            if (string.IsNullOrWhiteSpace(pass))
            {
                _view.MostrarError("Ingrese la contraseña.");
                return;
            }

            var usuario = _authService.ValidarCredenciales(dni, pass);
            if (usuario == null)
            {
                _view.MostrarError("Credenciales inválidas o usuario inactivo.");
                return;
            }

            // Routing por rol: 1=Admin, 2=Médico, 3=Asistente (según lote/ERS)
            System.Windows.Forms.Form next;
            switch (usuario.IdRol)
            {
                case 1:
                    next = new Views.PanelAdministrador.FrmPanelAdministrador();
                    // Crear presenter del panel admin para cablear eventos (backup, probar conexión, etc.)
                    var pav = (Views.PanelAdministrador.FrmPanelAdministrador)next;
                    var adminPresenter = new PanelAdministradorPresenter(pav);
                    break;
                case 2:
                    next = new Views.PanelMedico.FrmPanelMedico();
                    var pmv = (Views.PanelMedico.FrmPanelMedico)next;
                    var medicoPresenter = new PanelMedicoPresenter(pmv, usuario.Dni);
                    break;
                case 3: next = new Views.PanelAsistente.FrmPanelAsistente(); break;
                default:
                    _view.MostrarError("Rol de usuario no reconocido.");
                    return;
            }

            // Abrir como hijo MDI si corresponde
            if (next != null)
            {
                // La vista concreta (FrmLogin) cierra la ventana actual
                // y el FrmPrincipal ya está configurado como MDI container
                // Delega apertura a la vista concreta (ya implementado por botones de simulación)
                if (next is System.Windows.Forms.Form f && _view is System.Windows.Forms.Form lf && lf.MdiParent != null)
                {
                    f.MdiParent = lf.MdiParent;
                    f.Show();
                    lf.Close();
                }
            }
        }
    }
}

using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;
using SALC.Infraestructura;

namespace SALC.Presenters
{
    public class PanelAdministradorPresenter
    {
        private readonly IPanelAdministradorView _view;

        public PanelAdministradorPresenter(IPanelAdministradorView view)
        {
            _view = view;
            _view.ProbarConexionClick += (s, e) => OnProbarConexion();
        }

        private void OnProbarConexion()
        {
            var r = DbHealth.ProbarConexion();
            if (r.Exito)
                MessageBox.Show("Base de datos: " + r.Detalle, "Conexión", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Error de conexión: " + r.Detalle, "Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

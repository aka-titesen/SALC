using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gestión de usuarios según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para RF-02: ABM de Usuarios
    /// </summary>
    public interface IVistaGestionUsuarios
    {
        #region Eventos de la Vista (en español)
        event EventHandler CargarUsuarios;
        event EventHandler<int> EliminarUsuario;
        event EventHandler<Models.Usuario> GuardarUsuario;
        event EventHandler<int> EditarUsuario;
        event EventHandler<string> BuscarUsuario;
        event EventHandler<int> RestablecerContrasena;
        event EventHandler<int> CambiarEstadoUsuario;
        event EventHandler CreacionSolicitada;
        event EventHandler EdicionSolicitada;
        event EventHandler EliminacionSolicitada;
        event EventHandler CierreSolicitado;
        #endregion

        #region Propiedades de la Vista (en español)
        List<Models.Usuario> Usuarios { get; set; }
        Models.Usuario UsuarioSeleccionado { get; set; }
        bool EstaEditando { get; set; }
        List<Models.Rol> RolesDisponibles { get; set; }
        List<Models.Medico> MedicosDisponibles { get; set; }
        #endregion

        #region Métodos de la Vista (en español)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void LimpiarFormulario();
        void CargarDatosUsuario(Models.Usuario usuario);
        void ActualizarListaUsuarios();
        void HabilitarFormulario(bool habilitado);
        void MostrarCamposEspecificos(string tipoRol);
        #endregion
    }
}
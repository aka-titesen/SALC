using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de notificaciones seg�n ERS v2.7
    /// Implementa el patr�n MVP (Model-View-Presenter) para RF-08: Generar y Enviar Informe
    /// </summary>
    public interface IVistaNotificaciones
    {
        #region Eventos de la Vista (en espa�ol)
        event EventHandler EnvioSolicitado;
        event EventHandler CierreSolicitado;
        event EventHandler<int> EnviarNotificacionPaciente;
        event EventHandler<string> EnviarNotificacionEmail;
        event EventHandler<string> EnviarNotificacionSms;
        #endregion

        #region Propiedades de la Vista (en espa�ol)
        string MensajeNotificacion { get; set; }
        string DestinatarioEmail { get; set; }
        string DestinatarioTelefono { get; set; }
        #endregion

        #region M�todos de la Vista (en espa�ol)
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void CargarDatosPaciente(Models.Paciente paciente);
        void LimpiarFormulario();
        void HabilitarEnvioEmail(bool habilitado);
        void HabilitarEnvioSms(bool habilitado);
        #endregion
    }
}
using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de gestión de copias de seguridad según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para RF-10: Gestionar Backups
    /// Solo disponible para usuarios Administrador
    /// </summary>
    public interface IVistaGestionCopiasSeguridad
    {
        #region Eventos de Copia de Seguridad (en español)
        event EventHandler EjecutarCopiaSeguridad;
        event EventHandler<string> RestaurarDesdeCopia;
        event EventHandler CargarListaCopiasSeguridad;
        event EventHandler<int> EliminarCopiaSeguridad;
        event EventHandler LimpiarCopiasAntiguas;
        #endregion

        #region Eventos de Programación (en español)
        event EventHandler<TimeSpan> ProgramarCopiaDiaria;
        event EventHandler DeshabilitarCopiaProgramada;
        #endregion

        #region Eventos de Navegación (en español)
        event EventHandler ExplorarRutaCopias;
        event EventHandler CierreSolicitado;
        #endregion

        #region Propiedades (en español)
        List<InformacionCopiaSeguridad> CopiasDisponibles { get; set; }
        string RutaCopiasSeguridad { get; set; }
        bool CopiaEnProgreso { get; set; }
        bool RestauracionEnProgreso { get; set; }
        TimeSpan HoraCopiaProgramada { get; set; }
        bool CopiaAutomaticaHabilitada { get; set; }
        int DiasRetencion { get; set; }
        #endregion

        #region Métodos de la Vista (en español)
        void CargarDatosCopiasSeguridad(List<InformacionCopiaSeguridad> copias);
        void MostrarResultadoCopia(ResultadoCopiaSeguridad resultado);
        void MostrarResultadoRestauracion(ResultadoCopiaSeguridad resultado);
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        void ActualizarProgreso(string estado, int porcentaje);
        void HabilitarControles(bool habilitado);
        #endregion
    }

    #region Clases de Apoyo (en español)
    /// <summary>
    /// Información de una copia de seguridad
    /// </summary>
    public class InformacionCopiaSeguridad
    {
        public string NombreArchivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public long TamanoBytes { get; set; }
        public string RutaCompleta { get; set; }
        public string Descripcion { get; set; }
    }

    /// <summary>
    /// Resultado de una operación de copia de seguridad
    /// </summary>
    public class ResultadoCopiaSeguridad
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string RutaArchivo { get; set; }
        public TimeSpan DuracionOperacion { get; set; }
        public Exception Error { get; set; }
    }
    #endregion
}
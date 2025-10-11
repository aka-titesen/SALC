using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de administración de catálogos según ERS v2.7
    /// Implementa el patrón MVP (Model-View-Presenter) para RF-04: ABM de Catálogos
    /// Cubre: obras sociales, tipos de análisis, métricas, estados, roles
    /// </summary>
    public interface IVistaAdministracionCatalogos
    {
        #region Eventos para Obras Sociales (en español)
        event EventHandler CargarObrasSociales;
        event EventHandler<Models.ObraSocial> GuardarObraSocial;
        event EventHandler<int> EditarObraSocial;
        event EventHandler<int> EliminarObraSocial;
        #endregion

        #region Eventos para Tipos de Análisis (en español)
        event EventHandler CargarTiposAnalisis;
        event EventHandler<Models.TipoAnalisis> GuardarTipoAnalisis;
        event EventHandler<int> EditarTipoAnalisis;
        event EventHandler<int> EliminarTipoAnalisis;
        #endregion

        #region Eventos para Métricas (en español)
        event EventHandler CargarMetricas;
        event EventHandler<Models.Metrica> GuardarMetrica;
        event EventHandler<int> EditarMetrica;
        event EventHandler<int> EliminarMetrica;
        #endregion

        #region Eventos para Estados y Roles (en español)
        event EventHandler CargarEstados;
        event EventHandler CargarRoles;
        #endregion

        #region Eventos de Navegación (en español)
        event EventHandler MostrarTabObrasSociales;
        event EventHandler MostrarTabTiposAnalisis;
        event EventHandler MostrarTabMetricas;
        event EventHandler MostrarTabEstados;
        event EventHandler MostrarTabRoles;
        #endregion

        #region Propiedades de Datos (en español)
        List<Models.ObraSocial> ObrasSociales { get; set; }
        List<Models.TipoAnalisis> TiposAnalisis { get; set; }
        List<Models.Metrica> Metricas { get; set; }
        List<Models.EstadoAnalisis> EstadosAnalisis { get; set; }
        List<Models.Rol> Roles { get; set; }
        #endregion

        #region Propiedades de Edición (en español)
        Models.ObraSocial ObraSocialSeleccionada { get; set; }
        Models.TipoAnalisis TipoAnalisisSeleccionado { get; set; }
        Models.Metrica MetricaSeleccionada { get; set; }
        bool EstaEditando { get; set; }
        string CatalogoActual { get; set; }
        #endregion

        #region Métodos de la Vista (en español)
        void CargarDatosObrasSociales(List<Models.ObraSocial> obras);
        void CargarDatosTiposAnalisis(List<Models.TipoAnalisis> tipos);
        void CargarDatosMetricas(List<Models.Metrica> metricas);
        void CargarDatosEstados(List<Models.EstadoAnalisis> estados);
        void CargarDatosRoles(List<Models.Rol> roles);

        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        
        void LimpiarFormularioObraSocial();
        void LimpiarFormularioTipoAnalisis();
        void LimpiarFormularioMetrica();
        
        void CargarObraSocialParaEdicion(Models.ObraSocial obra);
        void CargarTipoAnalisisParaEdicion(Models.TipoAnalisis tipo);
        void CargarMetricaParaEdicion(Models.Metrica metrica);
        #endregion
    }
}
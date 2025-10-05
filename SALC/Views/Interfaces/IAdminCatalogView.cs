// Views/Interfaces/IAdminCatalogView.cs
using System;
using System.Collections.Generic;
using SALC.Models;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interface para la gestión de catálogos del sistema (Administrador)
    /// Cubre: obras sociales, tipos de análisis, métricas, estados, roles
    /// </summary>
    public interface IAdminCatalogView
    {
        // Eventos para Obras Sociales (RF-13)
        event EventHandler LoadObrasSociales;
        event EventHandler<ObraSocial> SaveObraSocial;
        event EventHandler<int> EditObraSocial;
        event EventHandler<int> DeleteObraSocial;

        // Eventos para Tipos de Análisis (RF-11)
        event EventHandler LoadTiposAnalisis;
        event EventHandler<TipoAnalisis> SaveTipoAnalisis;
        event EventHandler<int> EditTipoAnalisis;
        event EventHandler<int> DeleteTipoAnalisis;

        // Eventos para Métricas (RF-12)
        event EventHandler LoadMetricas;
        event EventHandler<Metrica> SaveMetrica;
        event EventHandler<int> EditMetrica;
        event EventHandler<int> DeleteMetrica;

        // Eventos para Estados y Roles
        event EventHandler LoadEstados;
        event EventHandler LoadEstadosUsuario;
        event EventHandler LoadRoles;

        // Eventos de navegación entre catálogos
        event EventHandler ShowObrasSocialesTab;
        event EventHandler ShowTiposAnalisisTab;
        event EventHandler ShowMetricasTab;
        event EventHandler ShowEstadosTab;
        event EventHandler ShowRolesTab;

        // Propiedades para datos
        List<ObraSocial> ObrasSociales { get; set; }
        List<TipoAnalisis> TiposAnalisis { get; set; }
        List<Metrica> Metricas { get; set; }
        List<Estado> Estados { get; set; }
        List<EstadoUsuario> EstadosUsuario { get; set; }
        List<Rol> Roles { get; set; }

        // Propiedades para edición
        ObraSocial SelectedObraSocial { get; set; }
        TipoAnalisis SelectedTipoAnalisis { get; set; }
        Metrica SelectedMetrica { get; set; }

        // Métodos de la interfaz
        void LoadObrasSocialesData(List<ObraSocial> obras);
        void LoadTiposAnalisisData(List<TipoAnalisis> tipos);
        void LoadMetricasData(List<Metrica> metricas);
        void LoadEstadosData(List<Estado> estados);
        void LoadEstadosUsuarioData(List<EstadoUsuario> estadosUsuario);
        void LoadRolesData(List<Rol> roles);

        void ShowMessage(string title, string message, System.Windows.Forms.MessageBoxIcon icon);
        void ShowError(string error);
        
        void ClearObraSocialForm();
        void ClearTipoAnalisisForm();
        void ClearMetricaForm();
        
        void LoadObraSocialForEdit(ObraSocial obra);
        void LoadTipoAnalisisForEdit(TipoAnalisis tipo);
        void LoadMetricaForEdit(Metrica metrica);

        bool IsEditing { get; set; }
        string CurrentCatalog { get; set; }
    }
}
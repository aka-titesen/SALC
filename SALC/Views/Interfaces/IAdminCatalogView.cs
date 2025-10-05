// Views/Interfaces/IAdminCatalogView.cs
using System;
using System.Collections.Generic;
using SALC.Models;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interface para la gesti�n de cat�logos del sistema (Administrador)
    /// Cubre: obras sociales, tipos de an�lisis, m�tricas, estados, roles
    /// </summary>
    public interface IAdminCatalogView
    {
        // Eventos para Obras Sociales (RF-13)
        event EventHandler LoadObrasSociales;
        event EventHandler<ObraSocial> SaveObraSocial;
        event EventHandler<int> EditObraSocial;
        event EventHandler<int> DeleteObraSocial;

        // Eventos para Tipos de An�lisis (RF-11)
        event EventHandler LoadTiposAnalisis;
        event EventHandler<TipoAnalisis> SaveTipoAnalisis;
        event EventHandler<int> EditTipoAnalisis;
        event EventHandler<int> DeleteTipoAnalisis;

        // Eventos para M�tricas (RF-12)
        event EventHandler LoadMetricas;
        event EventHandler<Metrica> SaveMetrica;
        event EventHandler<int> EditMetrica;
        event EventHandler<int> DeleteMetrica;

        // Eventos para Estados y Roles
        event EventHandler LoadEstados;
        event EventHandler LoadEstadosUsuario;
        event EventHandler LoadRoles;

        // Eventos de navegaci�n entre cat�logos
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

        // Propiedades para edici�n
        ObraSocial SelectedObraSocial { get; set; }
        TipoAnalisis SelectedTipoAnalisis { get; set; }
        Metrica SelectedMetrica { get; set; }

        // M�todos de la interfaz
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
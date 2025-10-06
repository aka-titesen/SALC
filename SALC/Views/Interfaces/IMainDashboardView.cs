using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    public interface IMainDashboardView
    {
        // Header y usuario
        void SetHeaderTitle(string title);
        void SetUserInfo(string displayName, string role);

        // Definir features disponibles (Presenter -> View)
        void SetAvailableFeatures(IReadOnlyCollection<AppFeature> features);

        // Eventos comunes (View -> Presenter / app)
        event EventHandler LogoutRequested;
        event EventHandler PatientsRequested;
        event EventHandler StudiesRequested;
        event EventHandler ResultsRequested;
        event EventHandler RecepcionMuestrasRequested;  // Nuevo evento para asistentes (RF-17)
        event EventHandler ReportsRequested;
        event EventHandler NotificationsRequested;
        event EventHandler HistoryRequested;
        
        // EVENTOS ADMINISTRACI�N DE USUARIOS (Solo Administrador)
        event EventHandler UserManagementRequested;      // ABM usuarios internos
        event EventHandler PatientsAdminRequested;       // ABM pacientes (vista admin)
        event EventHandler ExternalDoctorsRequested;     // ABM doctores externos
        
        // EVENTOS ADMINISTRACI�N DE CAT�LOGOS (Solo Administrador)
        event EventHandler AnalysisTypesRequested;       // ABM tipo_analisis
        event EventHandler MetricsRequested;             // ABM metrica
        event EventHandler InsuranceRequested;           // ABM obra_social
        event EventHandler StatesRequested;              // ABM estado y estado_usuario
        event EventHandler RolesRequested;               // ABM rol
        
        // EVENTOS CONFIGURACI�N Y SISTEMA (Solo Administrador)
        // TODO: Eventos deshabilitados temporalmente - no soportados en ERS v1.0
        // event EventHandler SystemConfigRequested;        // Configuraci�n general
        event EventHandler BackupsRequested;             // Backup y restore
        // event EventHandler SecurityRequested;            // Auditor�a y logs
        // event EventHandler AuditRequested;               // Logs de accesos

        void ShowMessage(string title, string message);
    }
}

using System;
using System.Collections.Generic;
using SALC;

namespace SALC.Views.Interfaces
{
    public interface IMainDashboardView
    {
        // Header y usuario
        void SetHeaderTitle(string title);
        void SetUserInfo(string displayName, string role);

        // Definir features disponibles (Presenter -> View)
        void SetAvailableFeatures(IReadOnlyCollection<SALC.AppFeature> features);

        // Eventos comunes (View -> Presenter / app)
        event EventHandler LogoutRequested;
        event EventHandler PatientsRequested;
        event EventHandler StudiesRequested;
        event EventHandler ResultsRequested;
        event EventHandler ReportsRequested;
        event EventHandler NotificationsRequested;
        event EventHandler HistoryRequested;
        event EventHandler AppointmentsRequested; // Turnos
        event EventHandler UserManagementRequested;
        event EventHandler SystemConfigRequested;
        event EventHandler BackupsRequested;
        event EventHandler SecurityRequested;

        void ShowMessage(string title, string message);
    }
}

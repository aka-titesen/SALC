// Views/Interfaces/ISocialWorksView.cs
using System;
using System.Collections.Generic;
using SALC.Models;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interface para la vista de gestión de obras sociales
    /// </summary>
    public interface ISocialWorksView
    {
        // Eventos
        event EventHandler LoadSocialWorks;
        event EventHandler<int> DeleteSocialWork;
        event EventHandler<ObraSocial> SaveSocialWork;
        event EventHandler<int> EditSocialWork;
        event EventHandler<string> SearchSocialWork;

        // Propiedades
        List<ObraSocial> SocialWorks { get; set; }
        ObraSocial SelectedSocialWork { get; set; }
        bool IsEditing { get; set; }

        // Métodos
        void ShowMessage(string message);
        void ShowError(string error);
        void ClearForm();
        void LoadSocialWorkData(ObraSocial obraSocial);
    }
}
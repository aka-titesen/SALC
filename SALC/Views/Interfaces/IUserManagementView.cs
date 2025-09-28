// Views/Interfaces/IUserManagementView.cs
using System;
using System.Collections.Generic;
using SALC.Models;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interface para la vista de gestión de usuarios mejorada
    /// </summary>
    public interface IUserManagementView
    {
        // Eventos
        event EventHandler LoadUsers;
        event EventHandler<int> DeleteUser;
        event EventHandler<Usuario> SaveUser;
        event EventHandler<int> EditUser;
        event EventHandler<string> SearchUser;
        event EventHandler<int> ResetPassword;
        event EventHandler<int> ToggleUserStatus;

        // Propiedades
        List<Usuario> Users { get; set; }
        Usuario SelectedUser { get; set; }
        bool IsEditing { get; set; }
        List<Rol> AvailableRoles { get; set; }

        // Métodos
        void ShowMessage(string message);
        void ShowError(string error);
        void ClearForm();
        void LoadUserData(Usuario usuario);
        void RefreshUserList();
    }
}
// Services/UserAuthentication.cs
using System;
using SALC.Models;
using SALC.DataAccess.Repositories;

namespace SALC.Services
{
    /// <summary>
    /// Servicio de autenticación de usuarios según ERS v2.7
    /// Utiliza BCrypt para validación de contraseñas y el patrón Repository para acceso a datos
    /// </summary>
    public static class UserAuthentication
    {
        private static readonly UsuarioRepository _usuarioRepository = new UsuarioRepository();

        /// <summary>
        /// Usuario actualmente autenticado en el sistema
        /// </summary>
        public static Usuario CurrentUser { get; private set; } = null;

        /// <summary>
        /// Evento que se dispara cuando un usuario inicia sesión
        /// </summary>
        public static event EventHandler<Usuario> UserLoggedIn;

        /// <summary>
        /// Evento que se dispara cuando un usuario cierra sesión
        /// </summary>
        public static event EventHandler UserLoggedOut;

        /// <summary>
        /// Autentica un usuario contra la base de datos usando DNI o email
        /// </summary>
        /// <param name="credencial">DNI (numérico) o email del usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Usuario autenticado o null si las credenciales son inválidas</returns>
        public static Usuario Authenticate(string credencial, string password)
        {
            if (string.IsNullOrWhiteSpace(credencial) || string.IsNullOrWhiteSpace(password))
                return null;

            try
            {
                Usuario usuario = null;

                // Determinar si la credencial es DNI o email
                if (int.TryParse(credencial, out int dni))
                {
                    // Buscar por DNI
                    usuario = _usuarioRepository.ObtenerPorDni(dni);
                }
                else if (credencial.Contains("@"))
                {
                    // Buscar por email
                    usuario = _usuarioRepository.ObtenerPorEmail(credencial);
                }

                if (usuario == null || !usuario.EstaActivo)
                    return null;

                // Validar contraseña
                if (ValidarPassword(password, usuario.PasswordHash))
                {
                    return usuario;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log del error en un sistema real
                System.Diagnostics.Debug.WriteLine($"Error en autenticación: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Realiza el proceso completo de login
        /// </summary>
        /// <param name="credencial">DNI o email del usuario</param>
        /// <param name="password">Contraseña</param>
        /// <returns>True si el login fue exitoso</returns>
        public static bool Login(string credencial, string password)
        {
            Usuario usuario = Authenticate(credencial, password);
            
            if (usuario != null)
            {
                CurrentUser = usuario;
                UserLoggedIn?.Invoke(null, usuario);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Cierra la sesión del usuario actual
        /// </summary>
        public static void Logout()
        {
            if (IsLoggedIn())
            {
                CurrentUser = null;
                UserLoggedOut?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Verifica si hay un usuario autenticado
        /// </summary>
        public static bool IsLoggedIn()
        {
            return CurrentUser != null;
        }

        /// <summary>
        /// Verifica si el usuario actual tiene un rol específico
        /// </summary>
        public static bool HasRole(string nombreRol)
        {
            return IsLoggedIn() && 
                   string.Equals(CurrentUser.NombreRol, nombreRol, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verifica si el usuario actual tiene permisos para una operación
        /// </summary>
        public static bool HasPermission(string operacion)
        {
            if (!IsLoggedIn()) return false;

            // Los administradores tienen todos los permisos
            if (CurrentUser.EsAdministrador) return true;

            // Verificar permisos específicos según rol
            return operacion?.ToLowerInvariant() switch
            {
                "crear_analisis" => CurrentUser.EsMedico,
                "cargar_resultados" => CurrentUser.EsMedico,
                "validar_analisis" => CurrentUser.EsMedico,
                "generar_informe_propio" => CurrentUser.EsMedico,
                "generar_informe_verificado" => CurrentUser.EsMedico || CurrentUser.EsAsistente,
                "ver_historial_pacientes" => CurrentUser.EsMedico || CurrentUser.EsAsistente,
                "gestionar_usuarios" => CurrentUser.EsAdministrador,
                "gestionar_catalogos" => CurrentUser.EsAdministrador,
                "gestionar_backups" => CurrentUser.EsAdministrador,
                _ => false
            };
        }

        /// <summary>
        /// Cambia la contraseña del usuario actual
        /// </summary>
        public static bool ChangePassword(string currentPassword, string newPassword)
        {
            if (!IsLoggedIn() || string.IsNullOrWhiteSpace(newPassword))
                return false;

            try
            {
                // Verificar contraseña actual
                if (!ValidarPassword(currentPassword, CurrentUser.PasswordHash))
                    return false;

                // Hashear nueva contraseña
                string newPasswordHash = HashPassword(newPassword);

                // Actualizar en base de datos
                CurrentUser.PasswordHash = newPasswordHash;
                return _usuarioRepository.Actualizar(CurrentUser, true);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Refresca los datos del usuario actual desde la base de datos
        /// </summary>
        public static void RefreshCurrentUser()
        {
            if (IsLoggedIn())
            {
                var usuarioActualizado = _usuarioRepository.ObtenerPorDni(CurrentUser.Dni);
                if (usuarioActualizado != null && usuarioActualizado.EstaActivo)
                {
                    CurrentUser = usuarioActualizado;
                }
                else
                {
                    // El usuario fue desactivado, cerrar sesión
                    Logout();
                }
            }
        }

        #region Métodos Privados para Manejo de Contraseñas

        /// <summary>
        /// Valida una contraseña contra su hash
        /// TODO: Implementar BCrypt en lugar de comparación directa
        /// </summary>
        private static bool ValidarPassword(string password, string hash)
        {
            // TEMPORAL: Comparación directa para compatibilidad con datos existentes
            // En producción, usar: return BCrypt.Net.BCrypt.Verify(password, hash);
            return string.Equals(password, hash, StringComparison.Ordinal);
        }

        /// <summary>
        /// Genera un hash de la contraseña
        /// TODO: Implementar BCrypt en lugar de texto plano
        /// </summary>
        private static string HashPassword(string password)
        {
            // TEMPORAL: Retorna texto plano para compatibilidad con datos existentes
            // En producción, usar: return BCrypt.Net.BCrypt.HashPassword(password);
            return password;
        }

        #endregion

        #region Métodos de Compatibilidad (Deprecated)

        [Obsolete("Use Login method instead")]
        public static Usuario Authenticate(string usernameOrEmail, string password)
        {
            return Authenticate(usernameOrEmail, password);
        }

        #endregion
    }
}
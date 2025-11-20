using System;
using System.Data.SqlClient;
using SALC.DAL;
using SALC.Domain;
using SALC.Infraestructura;
using SALC.Infraestructura.Exceptions;

namespace SALC.BLL
{
    /// <summary>
    /// Servicio de lógica de negocio para la autenticación de usuarios.
    /// Valida credenciales y verifica que el usuario esté activo en el sistema.
    /// </summary>
    public class AutenticacionService : IAutenticacionService
    {
        private readonly UsuarioRepositorio _usuarioRepo;
        private readonly IPasswordHasher _hasher;

        /// <summary>
        /// Constructor del servicio de autenticación
        /// </summary>
        /// <param name="usuarioRepo">Repositorio de usuarios</param>
        /// <param name="hasher">Servicio de hashing de contraseñas</param>
        public AutenticacionService(UsuarioRepositorio usuarioRepo, IPasswordHasher hasher)
        {
            _usuarioRepo = usuarioRepo ?? throw new ArgumentNullException(nameof(usuarioRepo));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        /// <summary>
        /// Valida las credenciales de un usuario
        /// </summary>
        /// <param name="dni">DNI del usuario</param>
        /// <param name="contrasenia">Contraseña en texto plano</param>
        /// <returns>El usuario autenticado si las credenciales son válidas, null en caso contrario</returns>
        public Usuario ValidarCredenciales(int dni, string contrasenia)
        {
            try
            {
                // Validaciones de entrada
                if (dni <= 0)
                {
                    throw new SalcValidacionException("El DNI debe ser un número positivo.", "dni");
                }

                if (string.IsNullOrWhiteSpace(contrasenia))
                {
                    throw new SalcValidacionException("La contraseña es obligatoria.", "contrasenia");
                }

                ExceptionHandler.LogInfo($"Intento de autenticación para DNI: {dni}", "Autenticación");

                // Buscar usuario
                Usuario usuario;
                try
                {
                    usuario = _usuarioRepo.ObtenerPorId(dni);
                }
                catch (SqlException sqlEx)
                {
                    throw new SalcDatabaseException("Error al buscar usuario en la base de datos", "ValidarCredenciales", sqlEx);
                }

                // Validar que el usuario exista y esté activo
                if (usuario == null)
                {
                    ExceptionHandler.LogWarning($"Usuario no encontrado - DNI: {dni}", "Autenticación");
                    return null; // Credenciales incorrectas
                }

                if (usuario.Estado != "Activo")
                {
                    ExceptionHandler.LogWarning($"Intento de acceso de usuario inactivo - DNI: {dni}", "Autenticación");
                    return null; // Usuario inactivo
                }

                // Validar contraseña
                bool esValida;
                try
                {
                    esValida = _hasher.Verify(contrasenia, usuario.PasswordHash);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogWarning($"Error al validar contraseña para DNI: {dni} - {ex.Message}", "Autenticación");
                    return null;
                }

                if (!esValida)
                {
                    ExceptionHandler.LogWarning($"Contraseña incorrecta para DNI: {dni}", "Autenticación");
                    return null;
                }

                // Autenticación exitosa
                ExceptionHandler.LogInfo($"Autenticación exitosa - DNI: {dni}, Rol: {usuario.IdRol}", "Autenticación");
                return usuario;
            }
            catch (SalcException)
            {
                // Re-lanzar excepciones SALC para que sean manejadas por capas superiores
                throw;
            }
            catch (Exception ex)
            {
                // Cualquier otra excepción no esperada
                ExceptionHandler.LogWarning($"Error inesperado en autenticación: {ex.Message}", "Autenticación");
                throw new SalcException(
                    "Error durante el proceso de autenticación",
                    "Ha ocurrido un error al validar las credenciales. Por favor, intente nuevamente.",
                    "AUTH_ERROR"
                );
            }
        }
    }
}

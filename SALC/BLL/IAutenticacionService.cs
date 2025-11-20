using SALC.Domain;

namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para el servicio de autenticación de usuarios.
    /// Define las operaciones para validar credenciales de acceso al sistema.
    /// </summary>
    public interface IAutenticacionService
    {
        /// <summary>
        /// Valida las credenciales de un usuario
        /// </summary>
        /// <param name="dni">DNI del usuario</param>
        /// <param name="contrasenia">Contraseña en texto plano</param>
        /// <returns>Usuario autenticado si las credenciales son válidas, null en caso contrario</returns>
        Usuario ValidarCredenciales(int dni, string contrasenia);
    }
}

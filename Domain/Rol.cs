namespace SALC.Domain
{
    /// <summary>
    /// Representa un rol de usuario en el sistema.
    /// Define los permisos y capacidades de cada tipo de usuario.
    /// </summary>
    public class Rol
    {
        /// <summary>
        /// Identificador único del rol
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Nombre descriptivo del rol (Administrador, Médico, Asistente)
        /// </summary>
        public string NombreRol { get; set; }
    }
}

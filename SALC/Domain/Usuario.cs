namespace SALC.Domain
{
    /// <summary>
    /// Representa un usuario del sistema SALC.
    /// Los usuarios pueden tener diferentes roles: Administrador, Médico o Asistente.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// DNI del usuario (Documento Nacional de Identidad). Actúa como identificador único
        /// </summary>
        public int Dni { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Apellido del usuario
        /// </summary>
        public string Apellido { get; set; }

        /// <summary>
        /// Dirección de correo electrónico del usuario
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Contraseña hasheada del usuario. Nunca debe almacenarse en texto plano
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Identificador del rol del usuario (1: Administrador, 2: Médico, 3: Asistente)
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Estado del usuario. Valores válidos: 'Activo' o 'Inactivo'
        /// </summary>
        public string Estado { get; set; }
    }
}

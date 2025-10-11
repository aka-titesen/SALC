namespace SALC.Common.Enums
{
    /// <summary>
    /// Enumeración que define los roles de usuario en el sistema SALC.
    /// Corresponde a los valores de la tabla 'roles' en la base de datos.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Administrador del sistema - Acceso completo a todas las funcionalidades
        /// </summary>
        Administrador = 1,

        /// <summary>
        /// Médico - Puede crear, cargar resultados y validar análisis
        /// </summary>
        Medico = 2,

        /// <summary>
        /// Asistente - Puede visualizar y generar reportes de análisis validados
        /// </summary>
        Asistente = 3
    }
}

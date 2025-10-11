namespace SALC.Common.Constants
{
    /// <summary>
    /// Constantes utilizadas en todo el sistema SALC.
    /// Centraliza valores constantes para facilitar el mantenimiento.
    /// </summary>
    public static class SALCConstants
    {
        /// <summary>
        /// Constantes relacionadas con la base de datos
        /// </summary>
        public static class Database
        {
            public const string ConnectionStringName = "SALCDatabase";
            public const int CommandTimeout = 30;
        }

        /// <summary>
        /// Constantes de validación
        /// </summary>
        public static class Validation
        {
            public const int MinPasswordLength = 6;
            public const int MaxNameLength = 50;
            public const int MaxEmailLength = 100;
            public const int DniMinValue = 1000000;
            public const int DniMaxValue = 99999999;
        }

        /// <summary>
        /// Constantes de la interfaz de usuario
        /// </summary>
        public static class UI
        {
            public const string AppTitle = "Sistema de Administración de Laboratorio Clínico (SALC)";
            public const string DefaultDateFormat = "dd/MM/yyyy";
            public const string DefaultDateTimeFormat = "dd/MM/yyyy HH:mm";
        }

        /// <summary>
        /// Mensajes del sistema
        /// </summary>
        public static class Messages
        {
            public const string LoginRequired = "Debe iniciar sesión para acceder al sistema.";
            public const string AccessDenied = "No tiene permisos para realizar esta acción.";
            public const string RecordSaved = "El registro se ha guardado correctamente.";
            public const string RecordDeleted = "El registro se ha eliminado correctamente.";
            public const string ConfirmDelete = "¿Está seguro que desea eliminar este registro?";
            public const string UnexpectedError = "Ha ocurrido un error inesperado. Contacte al administrador.";
        }

        /// <summary>
        /// Constantes de roles
        /// </summary>
        public static class Roles
        {
            public const string Administrador = "Administrador";
            public const string Medico = "Médico";
            public const string Asistente = "Asistente";
        }

        /// <summary>
        /// Constantes de estados de análisis
        /// </summary>
        public static class EstadosAnalisis
        {
            public const string SinVerificar = "Sin verificar";
            public const string Verificado = "Verificado";
        }
    }
}

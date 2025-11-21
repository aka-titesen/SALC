namespace SALC.Domain
{
    /// <summary>
    /// Clase que define constantes para los estados utilizados en el sistema.
    /// Centraliza los valores de estado para mantener consistencia en toda la aplicación.
    /// </summary>
    public static class EstadosConstantes
    {
        /// <summary>
        /// Estado que indica que una entidad está activa y disponible
        /// </summary>
        public const string Activo = "Activo";

        /// <summary>
        /// Estado que indica que una entidad está inactiva (baja lógica)
        /// </summary>
        public const string Inactivo = "Inactivo";

        /// <summary>
        /// Constantes para los estados específicos de análisis clínicos
        /// </summary>
        public static class Analisis
        {
            /// <summary>
            /// Estado que indica que el análisis fue creado pero aún no ha sido verificado por un médico
            /// </summary>
            public const int SinVerificar = 1;

            /// <summary>
            /// Estado que indica que el análisis ha sido firmado y verificado por un médico
            /// </summary>
            public const int Verificado = 2;

            /// <summary>
            /// Estado que indica que el análisis ha sido anulado (baja lógica)
            /// </summary>
            public const int Anulado = 3;
        }

        /// <summary>
        /// Verifica si un estado de tipo string es válido
        /// </summary>
        /// <param name="estado">Estado a validar</param>
        /// <returns>True si el estado es Activo o Inactivo, false en caso contrario</returns>
        public static bool EsEstadoValido(string estado)
        {
            return estado == Activo || estado == Inactivo;
        }

        /// <summary>
        /// Verifica si un estado de análisis es válido
        /// </summary>
        /// <param name="estado">Identificador del estado a validar</param>
        /// <returns>True si el estado es uno de los estados válidos de análisis, false en caso contrario</returns>
        public static bool EsEstadoAnalisisValido(int estado)
        {
            return estado == Analisis.SinVerificar || 
                   estado == Analisis.Verificado || 
                   estado == Analisis.Anulado;
        }
    }
}
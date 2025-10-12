namespace SALC.Domain
{
    /// <summary>
    /// Clase con constantes para los estados utilizados en el sistema
    /// </summary>
    public static class EstadosConstantes
    {
        // Estados para entidades con baja lógica
        public const string Activo = "Activo";
        public const string Inactivo = "Inactivo";

        // Estados específicos para análisis
        public static class Analisis
        {
            public const int SinVerificar = 1;
            public const int Verificado = 2;
            public const int Anulado = 3; // Baja lógica para análisis
        }

        // Métodos de utilidad para validar estados
        public static bool EsEstadoValido(string estado)
        {
            return estado == Activo || estado == Inactivo;
        }

        public static bool EsEstadoAnalisisValido(int estado)
        {
            return estado == Analisis.SinVerificar || 
                   estado == Analisis.Verificado || 
                   estado == Analisis.Anulado;
        }
    }
}
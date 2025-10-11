namespace SALC.Common.Enums
{
    /// <summary>
    /// Enumeración que define los estados posibles de un análisis.
    /// Corresponde a los valores de la tabla 'estados_analisis' en la base de datos.
    /// </summary>
    public enum AnalisisState
    {
        /// <summary>
        /// Análisis creado pero sin verificar por un médico
        /// </summary>
        SinVerificar = 1,

        /// <summary>
        /// Análisis verificado y firmado por un médico
        /// </summary>
        Verificado = 2
    }
}

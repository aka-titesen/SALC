namespace SALC.Domain
{
    /// <summary>
    /// Representa el resultado de una métrica específica dentro de un análisis.
    /// Relaciona un análisis con una métrica y almacena el valor medido.
    /// </summary>
    public class AnalisisMetrica
    {
        /// <summary>
        /// Identificador del análisis al que pertenece esta métrica
        /// </summary>
        public int IdAnalisis { get; set; }

        /// <summary>
        /// Identificador de la métrica que se está midiendo
        /// </summary>
        public int IdMetrica { get; set; }

        /// <summary>
        /// Valor numérico del resultado obtenido para esta métrica
        /// </summary>
        public decimal Resultado { get; set; }

        /// <summary>
        /// Observaciones adicionales sobre este resultado específico
        /// </summary>
        public string Observaciones { get; set; }
    }
}

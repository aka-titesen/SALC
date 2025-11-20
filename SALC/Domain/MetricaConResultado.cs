namespace SALC.Domain
{
    /// <summary>
    /// Representa una métrica junto con su resultado para la edición de análisis.
    /// Combina la información de la métrica con los valores obtenidos en un análisis específico.
    /// </summary>
    public class MetricaConResultado
    {
        /// <summary>
        /// Identificador de la métrica
        /// </summary>
        public int IdMetrica { get; set; }

        /// <summary>
        /// Nombre de la métrica
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Unidad de medida de la métrica
        /// </summary>
        public string UnidadMedida { get; set; }

        /// <summary>
        /// Valor mínimo de referencia normal
        /// </summary>
        public decimal? ValorMinimo { get; set; }

        /// <summary>
        /// Valor máximo de referencia normal
        /// </summary>
        public decimal? ValorMaximo { get; set; }

        /// <summary>
        /// Resultado obtenido en el análisis. Null si aún no se ha registrado
        /// </summary>
        public decimal? Resultado { get; set; }

        /// <summary>
        /// Observaciones sobre el resultado de esta métrica
        /// </summary>
        public string Observaciones { get; set; }

        /// <summary>
        /// Crea una instancia de MetricaConResultado a partir de una Metrica y opcionalmente un AnalisisMetrica
        /// </summary>
        /// <param name="metrica">Métrica base</param>
        /// <param name="analisisMetrica">Resultado del análisis (opcional)</param>
        /// <returns>Nueva instancia de MetricaConResultado</returns>
        public static MetricaConResultado Desde(Metrica metrica, AnalisisMetrica analisisMetrica = null)
        {
            return new MetricaConResultado
            {
                IdMetrica = metrica.IdMetrica,
                Nombre = metrica.Nombre,
                UnidadMedida = metrica.UnidadMedida,
                ValorMinimo = metrica.ValorMinimo,
                ValorMaximo = metrica.ValorMaximo,
                Resultado = analisisMetrica?.Resultado,
                Observaciones = analisisMetrica?.Observaciones
            };
        }

        /// <summary>
        /// Convierte esta instancia a un AnalisisMetrica para su persistencia en la base de datos
        /// </summary>
        /// <param name="idAnalisis">Identificador del análisis al que pertenece</param>
        /// <returns>Nueva instancia de AnalisisMetrica</returns>
        public AnalisisMetrica ToAnalisisMetrica(int idAnalisis)
        {
            return new AnalisisMetrica
            {
                IdAnalisis = idAnalisis,
                IdMetrica = IdMetrica,
                Resultado = Resultado ?? 0,
                Observaciones = Observaciones
            };
        }
    }
}
namespace SALC.Domain
{
    /// <summary>
    /// Representa una métrica con su resultado para la edición de análisis.
    /// No es un DTO, sino una composición de entidades del dominio.
    /// </summary>
    public class MetricaConResultado
    {
        public int IdMetrica { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public decimal? ValorMinimo { get; set; }
        public decimal? ValorMaximo { get; set; }
        public decimal? Resultado { get; set; }
        public string Observaciones { get; set; }

        /// <summary>
        /// Constructor desde Metrica y AnalisisMetrica
        /// </summary>
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
        /// Convierte a AnalisisMetrica para persistencia
        /// </summary>
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
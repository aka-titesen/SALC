namespace SALC.Domain
{
    /// <summary>
    /// Representa una métrica o parámetro que se mide en un análisis clínico.
    /// Por ejemplo: Glóbulos rojos, Glucosa en sangre, Colesterol total, etc.
    /// </summary>
    public class Metrica
    {
        /// <summary>
        /// Identificador único de la métrica
        /// </summary>
        public int IdMetrica { get; set; }

        /// <summary>
        /// Nombre de la métrica
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Unidad de medida de la métrica (mg/dL, mmol/L, etc.)
        /// </summary>
        public string UnidadMedida { get; set; }

        /// <summary>
        /// Valor máximo de referencia normal para esta métrica. Null si no aplica
        /// </summary>
        public decimal? ValorMaximo { get; set; }

        /// <summary>
        /// Valor mínimo de referencia normal para esta métrica. Null si no aplica
        /// </summary>
        public decimal? ValorMinimo { get; set; }

        /// <summary>
        /// Estado de la métrica para baja lógica. Valores válidos: 'Activo' o 'Inactivo'
        /// </summary>
        public string Estado { get; set; } = "Activo";
    }
}

namespace SALC.Domain
{
    /// <summary>
    /// Representa la relación entre un tipo de análisis y las métricas que lo componen.
    /// Define qué parámetros se deben medir para cada tipo de análisis.
    /// </summary>
    public class TipoAnalisisMetrica
    {
        /// <summary>
        /// Identificador del tipo de análisis
        /// </summary>
        public int IdTipoAnalisis { get; set; }

        /// <summary>
        /// Identificador de la métrica asociada al tipo de análisis
        /// </summary>
        public int IdMetrica { get; set; }
        
        /// <summary>
        /// Descripción del tipo de análisis (propiedad de navegación para facilitar la visualización)
        /// </summary>
        public string DescripcionTipoAnalisis { get; set; }

        /// <summary>
        /// Nombre de la métrica (propiedad de navegación para facilitar la visualización)
        /// </summary>
        public string NombreMetrica { get; set; }

        /// <summary>
        /// Unidad de medida de la métrica (propiedad de navegación para facilitar la visualización)
        /// </summary>
        public string UnidadMedidaMetrica { get; set; }
    }
}
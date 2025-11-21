namespace SALC.Domain
{
    /// <summary>
    /// Representa un tipo o categoría de análisis clínico que se puede realizar.
    /// Por ejemplo: Hemograma completo, Glucemia, etc.
    /// </summary>
    public class TipoAnalisis
    {
        /// <summary>
        /// Identificador único del tipo de análisis
        /// </summary>
        public int IdTipoAnalisis { get; set; }

        /// <summary>
        /// Descripción del tipo de análisis
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Estado del tipo de análisis para baja lógica. Valores válidos: 'Activo' o 'Inactivo'
        /// </summary>
        public string Estado { get; set; } = "Activo";
    }
}

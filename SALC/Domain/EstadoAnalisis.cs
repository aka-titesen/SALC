namespace SALC.Domain
{
    /// <summary>
    /// Representa un estado posible de un análisis clínico.
    /// Los estados típicos son: Sin verificar, Verificado, Anulado.
    /// </summary>
    public class EstadoAnalisis
    {
        /// <summary>
        /// Identificador único del estado
        /// </summary>
        public int IdEstado { get; set; }

        /// <summary>
        /// Descripción del estado del análisis
        /// </summary>
        public string Descripcion { get; set; }
    }
}

namespace SALC.Domain
{
    /// <summary>
    /// Representa una obra social o cobertura médica.
    /// </summary>
    public class ObraSocial
    {
        /// <summary>
        /// Identificador único de la obra social
        /// </summary>
        public int IdObraSocial { get; set; }

        /// <summary>
        /// CUIT de la obra social (Código Único de Identificación Tributaria)
        /// </summary>
        public string Cuit { get; set; }

        /// <summary>
        /// Nombre de la obra social
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Estado de la obra social para baja lógica. Valores válidos: 'Activo' o 'Inactivo'
        /// </summary>
        public string Estado { get; set; } = "Activo";
    }
}

namespace SALC.Domain
{
    public class TipoAnalisisMetrica
    {
        public int IdTipoAnalisis { get; set; }
        public int IdMetrica { get; set; }
        
        // Propiedades navegacionales para facilitar la visualización
        public string DescripcionTipoAnalisis { get; set; }
        public string NombreMetrica { get; set; }
        public string UnidadMedidaMetrica { get; set; }
    }
}
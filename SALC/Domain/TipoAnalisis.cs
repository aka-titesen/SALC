namespace SALC.Domain
{
    public class TipoAnalisis
    {
        public int IdTipoAnalisis { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; } = "Activo"; // Baja l�gica: "Activo" | "Inactivo"
    }
}

namespace SALC.Domain
{
    public class TipoAnalisis
    {
        public int IdTipoAnalisis { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; } = "Activo"; // Baja lógica: "Activo" | "Inactivo"
    }
}

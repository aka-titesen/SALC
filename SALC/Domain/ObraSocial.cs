namespace SALC.Domain
{
    public class ObraSocial
    {
        public int IdObraSocial { get; set; }
        public string Cuit { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; } = "Activo"; // Baja l�gica: "Activo" | "Inactivo"
    }
}

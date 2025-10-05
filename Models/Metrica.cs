// Models/Metrica.cs
namespace SALC.Models
{
    /// <summary>
    /// Representa una métrica de análisis en el sistema SALC
    /// </summary>
    public class Metrica
    {
        public int IdMetrica { get; set; }
        public string Nombre { get; set; }
        public string Unidad { get; set; }
        public decimal? ValorMinimo { get; set; }  // ✅ AGREGAR
        public decimal? ValorMaximo { get; set; }  // ✅ AGREGAR
    }
}
// Models/Metrica.cs
namespace SALC.Models
{
    /// <summary>
    /// Representa una m�trica para an�lisis en el sistema SALC
    /// </summary>
    public class Metrica
    {
        public int IdMet { get; set; }
        public string Nombre { get; set; }
        public string Unidad { get; set; }
        public decimal ValorMinimo { get; set; }
        public decimal ValorMaximo { get; set; }
    }
}
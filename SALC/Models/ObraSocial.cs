// Models/ObraSocial.cs
namespace SALC.Models
{
    /// <summary>
    /// Representa una obra social en el sistema SALC
    /// </summary>
    public class ObraSocial
    {
        public int IdObraSocial { get; set; }
        public string Cuit { get; set; }
        public string Nombre { get; set; }
    }
}
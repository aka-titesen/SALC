// Models/AnalisisMetrica.cs
namespace SALC.Models
{
    /// <summary>
    /// Representa la relaci�n entre un an�lisis y sus m�tricas con valores
    /// </summary>
    public class AnalisisMetrica
    {
        public int IdAnalisis { get; set; }
        public int IdMetrica { get; set; }
        public decimal Valor { get; set; }
        
        // Propiedades de navegaci�n (opcional)
        public Analisis Analisis { get; set; }
        public Metrica Metrica { get; set; }
    }
}
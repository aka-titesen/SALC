// Models/AnalisisMetrica.cs
using System;

// Models/AnalisisMetrica.cs
namespace SALC.Models
{
    /// <summary>
    /// Representa la relación entre un análisis y sus métricas con valores
    /// </summary>
    public class AnalisisMetrica
    {
        public int IdAnalisis { get; set; }
        public int IdMetrica { get; set; }
        public decimal Valor { get; set; }
        
        // Propiedades de navegación (opcional)
        public Analisis Analisis { get; set; }
        public Metrica Metrica { get; set; }
    }
}
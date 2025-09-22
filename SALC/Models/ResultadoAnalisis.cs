// Models/ResultadoAnalisis.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa el resultado de un análisis en el sistema SALC
    /// </summary>
    public class ResultadoAnalisis
    {
        public int IdAnalisis { get; set; }
        public int IdMet { get; set; }
        public decimal Valor { get; set; }
        public DateTime FechaCarga { get; set; }
    }
}
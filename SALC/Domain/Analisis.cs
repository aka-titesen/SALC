using System;

namespace SALC.Domain
{
    public class Analisis
    {
        public int IdAnalisis { get; set; }
        public int IdTipoAnalisis { get; set; }
        public int IdEstado { get; set; }
        public int DniPaciente { get; set; }
        public int DniCarga { get; set; }
        public int? DniFirma { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaFirma { get; set; }
        public string Observaciones { get; set; }
    }
}

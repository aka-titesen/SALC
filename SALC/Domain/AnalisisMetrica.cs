namespace SALC.Domain
{
    public class AnalisisMetrica
    {
        public int IdAnalisis { get; set; }
        public int IdMetrica { get; set; }
        public decimal Resultado { get; set; }
        public string Observaciones { get; set; }
    }
}

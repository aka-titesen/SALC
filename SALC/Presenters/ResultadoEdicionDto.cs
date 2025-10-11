namespace SALC.Presenters
{
    public class ResultadoEdicionDto
    {
        public int IdMetrica { get; set; }
        public string Nombre { get; set; }
        public string Unidad { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal? Resultado { get; set; }
        public string Observaciones { get; set; }
    }
}

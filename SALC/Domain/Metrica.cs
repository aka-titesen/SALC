namespace SALC.Domain
{
    public class Metrica
    {
        public int IdMetrica { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public decimal? ValorMaximo { get; set; }
        public decimal? ValorMinimo { get; set; }
    }
}

// Models/Doctor.cs
namespace SALC.Models
{
    /// <summary>
    /// Representa un doctor en el sistema SALC
    /// </summary>
    public class Doctor
    {
        public int Dni { get; set; }
        public int NroMatricula { get; set; }
        public string Especialidad { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
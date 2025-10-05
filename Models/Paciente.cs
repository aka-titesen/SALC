// Models/Paciente.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un paciente en el sistema SALC
    /// </summary>
    public class Paciente
    {
        public int Dni { get; set; }  // ✅ CORREGIDO: era "NroDoc"
        public string TipoDoc { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public int? IdObraSocial { get; set; }  // ✅ CORREGIDO: ahora nullable
        
        // Propiedades de navegación (opcional)
        public ObraSocial ObraSocial { get; set; }
        
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
using System;

namespace SALC.Domain
{
    public class Paciente
    {
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNac { get; set; }
        public char Sexo { get; set; } // 'M' | 'F' | 'X'
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int? IdObraSocial { get; set; }
    }
}

namespace SALC.Domain
{
    public class Usuario
    {
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int IdRol { get; set; }
        public string Estado { get; set; } // 'Activo' | 'Inactivo'
    }
}

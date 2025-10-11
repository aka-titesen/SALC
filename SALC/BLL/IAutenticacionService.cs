using SALC.Domain;

namespace SALC.BLL
{
    public interface IAutenticacionService
    {
        Usuario ValidarCredenciales(int dni, string contrasenia);
    }
}

using System.Collections.Generic;

namespace SALC.DAL
{
    public interface IRepositorioBase<T>
    {
        T ObtenerPorId(object id);
        IEnumerable<T> ObtenerTodos();
        void Crear(T entidad);
        void Actualizar(T entidad);
        void Eliminar(object id);
    }
}

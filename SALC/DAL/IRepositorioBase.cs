using System.Collections.Generic;

namespace SALC.DAL
{
    /// <summary>
    /// Interfaz base para todos los repositorios del sistema.
    /// Define las operaciones CRUD estándar que deben implementar los repositorios.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que maneja el repositorio</typeparam>
    public interface IRepositorioBase<T>
    {
        /// <summary>
        /// Obtiene una entidad por su identificador
        /// </summary>
        /// <param name="id">Identificador de la entidad</param>
        /// <returns>La entidad encontrada o null si no existe</returns>
        T ObtenerPorId(object id);

        /// <summary>
        /// Obtiene todas las entidades del repositorio
        /// </summary>
        /// <returns>Colección de todas las entidades</returns>
        IEnumerable<T> ObtenerTodos();

        /// <summary>
        /// Crea una nueva entidad en el repositorio
        /// </summary>
        /// <param name="entidad">Entidad a crear</param>
        void Crear(T entidad);

        /// <summary>
        /// Actualiza una entidad existente
        /// </summary>
        /// <param name="entidad">Entidad con los datos actualizados</param>
        void Actualizar(T entidad);

        /// <summary>
        /// Elimina una entidad del repositorio (puede ser física o lógica según implementación)
        /// </summary>
        /// <param name="id">Identificador de la entidad a eliminar</param>
        void Eliminar(object id);
    }
}

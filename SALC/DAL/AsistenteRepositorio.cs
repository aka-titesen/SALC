using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos específicos de asistentes de laboratorio.
    /// Gestiona las operaciones CRUD sobre la tabla de asistentes en la base de datos.
    /// </summary>
    public class AsistenteRepositorio : IRepositorioBase<Asistente>
    {
        /// <summary>
        /// Crea un nuevo asistente en la base de datos
        /// </summary>
        /// <param name="a">Asistente a crear</param>
        public void Crear(Asistente a)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("INSERT INTO asistentes (dni, dni_supervisor, fecha_ingreso) VALUES (@dni, @sup, @ing)", cn))
            {
                cmd.Parameters.AddWithValue("@dni", a.Dni);
                cmd.Parameters.AddWithValue("@sup", a.DniSupervisor);
                cmd.Parameters.AddWithValue("@ing", a.FechaIngreso);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza los datos específicos de un asistente
        /// </summary>
        /// <param name="a">Asistente con los datos actualizados</param>
        public void Actualizar(Asistente a)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE asistentes SET dni_supervisor=@sup, fecha_ingreso=@ing WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", a.Dni);
                cmd.Parameters.AddWithValue("@sup", a.DniSupervisor);
                cmd.Parameters.AddWithValue("@ing", a.FechaIngreso);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina un asistente de la base de datos
        /// </summary>
        /// <param name="id">DNI del asistente a eliminar</param>
        public void Eliminar(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("DELETE FROM asistentes WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene un asistente por su DNI
        /// </summary>
        /// <param name="id">DNI del asistente</param>
        /// <returns>El asistente encontrado o null si no existe</returns>
        public Asistente ObtenerPorId(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT dni, dni_supervisor, fecha_ingreso FROM asistentes WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        return new Asistente
                        {
                            Dni = rd.GetInt32(0),
                            DniSupervisor = rd.GetInt32(1),
                            FechaIngreso = rd.GetDateTime(2)
                        };
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene todos los asistentes del sistema
        /// </summary>
        /// <returns>Colección de todos los asistentes</returns>
        public IEnumerable<Asistente> ObtenerTodos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT dni, dni_supervisor, fecha_ingreso FROM asistentes", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return new Asistente
                        {
                            Dni = rd.GetInt32(0),
                            DniSupervisor = rd.GetInt32(1),
                            FechaIngreso = rd.GetDateTime(2)
                        };
                }
            }
        }
    }
}

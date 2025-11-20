using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos específicos de médicos.
    /// Gestiona las operaciones CRUD sobre la tabla de médicos en la base de datos.
    /// </summary>
    public class MedicoRepositorio : IRepositorioBase<Medico>
    {
        /// <summary>
        /// Crea un nuevo médico en la base de datos
        /// </summary>
        /// <param name="m">Médico a crear</param>
        public void Crear(Medico m)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("INSERT INTO medicos (dni, nro_matricula, especialidad) VALUES (@dni, @mat, @esp)", cn))
            {
                cmd.Parameters.AddWithValue("@dni", m.Dni);
                cmd.Parameters.AddWithValue("@mat", m.NroMatricula);
                cmd.Parameters.AddWithValue("@esp", m.Especialidad);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza los datos específicos de un médico
        /// </summary>
        /// <param name="m">Médico con los datos actualizados</param>
        public void Actualizar(Medico m)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE medicos SET nro_matricula=@mat, especialidad=@esp WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", m.Dni);
                cmd.Parameters.AddWithValue("@mat", m.NroMatricula);
                cmd.Parameters.AddWithValue("@esp", m.Especialidad);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina un médico de la base de datos
        /// </summary>
        /// <param name="id">DNI del médico a eliminar</param>
        public void Eliminar(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("DELETE FROM medicos WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene un médico por su DNI
        /// </summary>
        /// <param name="id">DNI del médico</param>
        /// <returns>El médico encontrado o null si no existe</returns>
        public Medico ObtenerPorId(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT dni, nro_matricula, especialidad FROM medicos WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        return new Medico
                        {
                            Dni = rd.GetInt32(0),
                            NroMatricula = rd.GetInt32(1),
                            Especialidad = rd.GetString(2)
                        };
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene todos los médicos del sistema
        /// </summary>
        /// <returns>Colección de todos los médicos</returns>
        public IEnumerable<Medico> ObtenerTodos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT dni, nro_matricula, especialidad FROM medicos", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return new Medico
                        {
                            Dni = rd.GetInt32(0),
                            NroMatricula = rd.GetInt32(1),
                            Especialidad = rd.GetString(2)
                        };
                }
            }
        }
    }
}

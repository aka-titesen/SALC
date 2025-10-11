using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class MedicoRepositorio : IRepositorioBase<Medico>
    {
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

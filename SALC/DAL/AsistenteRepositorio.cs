using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class AsistenteRepositorio : IRepositorioBase<Asistente>
    {
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

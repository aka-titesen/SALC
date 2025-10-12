using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class PacienteRepositorio : IRepositorioBase<Paciente>
    {
        public void Crear(Paciente p)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"INSERT INTO pacientes
                (dni, nombre, apellido, fecha_nac, sexo, email, telefono, id_obra_social, estado)
                VALUES (@dni, @nombre, @apellido, @fecha_nac, @sexo, @email, @telefono, @id_obra_social, @estado)", cn))
            {
                cmd.Parameters.AddWithValue("@dni", p.Dni);
                cmd.Parameters.AddWithValue("@nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@apellido", p.Apellido);
                cmd.Parameters.AddWithValue("@fecha_nac", p.FechaNac);
                cmd.Parameters.AddWithValue("@sexo", p.Sexo);
                cmd.Parameters.AddWithValue("@email", (object)p.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@telefono", (object)p.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id_obra_social", (object)p.IdObraSocial ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@estado", p.Estado ?? "Activo");
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Actualizar(Paciente p)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"UPDATE pacientes SET
                nombre=@nombre, apellido=@apellido, fecha_nac=@fecha_nac, sexo=@sexo, email=@email, telefono=@telefono, id_obra_social=@id_obra_social, estado=@estado
                WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", p.Dni);
                cmd.Parameters.AddWithValue("@nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@apellido", p.Apellido);
                cmd.Parameters.AddWithValue("@fecha_nac", p.FechaNac);
                cmd.Parameters.AddWithValue("@sexo", p.Sexo);
                cmd.Parameters.AddWithValue("@email", (object)p.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@telefono", (object)p.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id_obra_social", (object)p.IdObraSocial ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@estado", p.Estado ?? "Activo");
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(object id)
        {
            // Baja lógica - cambiar estado a "Inactivo"
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE pacientes SET estado='Inactivo' WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Paciente ObtenerPorId(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT dni, nombre, apellido, fecha_nac, sexo, email, telefono, id_obra_social, estado
                FROM pacientes WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        return Map(rd);
                }
            }
            return null;
        }

        public IEnumerable<Paciente> ObtenerTodos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT dni, nombre, apellido, fecha_nac, sexo, email, telefono, id_obra_social, estado FROM pacientes", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return Map(rd);
                }
            }
        }

        // Método para obtener solo pacientes activos
        public IEnumerable<Paciente> ObtenerActivos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT dni, nombre, apellido, fecha_nac, sexo, email, telefono, id_obra_social, estado FROM pacientes WHERE estado = 'Activo'", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return Map(rd);
                }
            }
        }

        private Paciente Map(IDataRecord rd)
        {
            return new Paciente
            {
                Dni = rd.GetInt32(0),
                Nombre = rd.GetString(1),
                Apellido = rd.GetString(2),
                FechaNac = rd.GetDateTime(3),
                Sexo = rd.GetString(4)[0],
                Email = rd.IsDBNull(5) ? null : rd.GetString(5),
                Telefono = rd.IsDBNull(6) ? null : rd.GetString(6),
                IdObraSocial = rd.IsDBNull(7) ? (int?)null : rd.GetInt32(7),
                Estado = rd.GetString(8)
            };
        }
    }
}

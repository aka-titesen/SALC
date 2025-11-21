using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos de pacientes.
    /// Gestiona las operaciones CRUD sobre la tabla de pacientes en la base de datos.
    /// </summary>
    public class PacienteRepositorio : IRepositorioBase<Paciente>
    {
        /// <summary>
        /// Crea un nuevo paciente en la base de datos
        /// </summary>
        /// <param name="p">Paciente a crear</param>
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

        /// <summary>
        /// Actualiza los datos de un paciente existente
        /// </summary>
        /// <param name="p">Paciente con los datos actualizados</param>
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

        /// <summary>
        /// Elimina un paciente mediante baja lógica, cambiando su estado a Inactivo
        /// </summary>
        /// <param name="id">DNI del paciente a eliminar</param>
        public void Eliminar(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE pacientes SET estado='Inactivo' WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene un paciente por su DNI
        /// </summary>
        /// <param name="id">DNI del paciente</param>
        /// <returns>El paciente encontrado o null si no existe</returns>
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

        /// <summary>
        /// Obtiene todos los pacientes del sistema
        /// </summary>
        /// <returns>Colección de todos los pacientes</returns>
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

        /// <summary>
        /// Obtiene todos los pacientes con estado Activo
        /// </summary>
        /// <returns>Colección de pacientes activos</returns>
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

        /// <summary>
        /// Mapea un registro de la base de datos a un objeto Paciente
        /// </summary>
        /// <param name="rd">Registro leído de la base de datos</param>
        /// <returns>Instancia de Paciente con los datos del registro</returns>
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

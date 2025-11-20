using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos de análisis clínicos.
    /// Gestiona las operaciones CRUD sobre la tabla de análisis en la base de datos.
    /// </summary>
    public class AnalisisRepositorio : IRepositorioBase<Analisis>
    {
        private const int EstadoSinVerificar = 1;
        private const int EstadoVerificado = 2;
        private const int EstadoAnulado = 3;

        /// <summary>
        /// Crea un nuevo análisis en la base de datos y retorna el objeto creado con su ID asignado
        /// </summary>
        /// <param name="a">Análisis a crear</param>
        /// <returns>El análisis creado con su identificador asignado por la base de datos</returns>
        public Analisis CrearYDevolver(Analisis a)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"INSERT INTO analisis
                (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma, observaciones)
                OUTPUT INSERTED.id_analisis
                VALUES (@tipo, @estado, @dni_paciente, @dni_carga, @dni_firma, @fecha_creacion, @fecha_firma, @obs)", cn))
            {
                cmd.Parameters.AddWithValue("@tipo", a.IdTipoAnalisis);
                cmd.Parameters.AddWithValue("@estado", a.IdEstado);
                cmd.Parameters.AddWithValue("@dni_paciente", a.DniPaciente);
                cmd.Parameters.AddWithValue("@dni_carga", a.DniCarga);
                cmd.Parameters.AddWithValue("@dni_firma", (object)a.DniFirma ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@fecha_creacion", a.FechaCreacion);
                cmd.Parameters.AddWithValue("@fecha_firma", (object)a.FechaFirma ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@obs", (object)a.Observaciones ?? DBNull.Value);
                cn.Open();
                a.IdAnalisis = (int)cmd.ExecuteScalar();
                return a;
            }
        }

        /// <summary>
        /// Crea un nuevo análisis en la base de datos
        /// </summary>
        /// <param name="entidad">Análisis a crear</param>
        public void Crear(Analisis entidad) => CrearYDevolver(entidad);

        /// <summary>
        /// Actualiza un análisis existente en la base de datos
        /// </summary>
        /// <param name="a">Análisis con los datos actualizados</param>
        public void Actualizar(Analisis a)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"UPDATE analisis SET
                id_tipo_analisis=@tipo, id_estado=@estado, dni_paciente=@dni_paciente, dni_carga=@dni_carga,
                dni_firma=@dni_firma, fecha_creacion=@fecha_creacion, fecha_firma=@fecha_firma, observaciones=@obs
                WHERE id_analisis=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", a.IdAnalisis);
                cmd.Parameters.AddWithValue("@tipo", a.IdTipoAnalisis);
                cmd.Parameters.AddWithValue("@estado", a.IdEstado);
                cmd.Parameters.AddWithValue("@dni_paciente", a.DniPaciente);
                cmd.Parameters.AddWithValue("@dni_carga", a.DniCarga);
                cmd.Parameters.AddWithValue("@dni_firma", (object)a.DniFirma ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@fecha_creacion", a.FechaCreacion);
                cmd.Parameters.AddWithValue("@fecha_firma", (object)a.FechaFirma ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@obs", (object)a.Observaciones ?? DBNull.Value);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina un análisis mediante baja lógica, cambiando su estado a Anulado
        /// </summary>
        /// <param name="id">Identificador del análisis a eliminar</param>
        public void Eliminar(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE analisis SET id_estado=@estado WHERE id_analisis=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", (int)id);
                cmd.Parameters.AddWithValue("@estado", EstadoAnulado);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene un análisis específico por su identificador
        /// </summary>
        /// <param name="id">Identificador del análisis</param>
        /// <returns>El análisis encontrado o null si no existe</returns>
        public Analisis ObtenerPorId(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT id_analisis, id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma, observaciones
                FROM analisis WHERE id_analisis=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", (int)id);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read()) return Map(rd);
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene todos los análisis del sistema
        /// </summary>
        /// <returns>Colección de todos los análisis</returns>
        public IEnumerable<Analisis> ObtenerTodos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT id_analisis, id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma, observaciones FROM analisis", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los análisis no anulados del sistema
        /// </summary>
        /// <returns>Colección de análisis activos</returns>
        public IEnumerable<Analisis> ObtenerActivos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT id_analisis, id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma, observaciones 
                FROM analisis WHERE id_estado != @estadoAnulado", cn))
            {
                cmd.Parameters.AddWithValue("@estadoAnulado", EstadoAnulado);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los análisis creados por un médico específico
        /// </summary>
        /// <param name="dni">DNI del médico</param>
        /// <returns>Colección de análisis del médico</returns>
        public IEnumerable<Analisis> ObtenerPorMedicoCarga(int dni)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT id_analisis, id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma, observaciones
                FROM analisis WHERE dni_carga=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", dni);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        /// <summary>
        /// Obtiene los análisis activos (no anulados) creados por un médico específico
        /// </summary>
        /// <param name="dni">DNI del médico</param>
        /// <returns>Colección de análisis activos del médico</returns>
        public IEnumerable<Analisis> ObtenerActivosPorMedicoCarga(int dni)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT id_analisis, id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma, observaciones
                FROM analisis WHERE dni_carga=@dni AND id_estado != @estadoAnulado", cn))
            {
                cmd.Parameters.AddWithValue("@dni", dni);
                cmd.Parameters.AddWithValue("@estadoAnulado", EstadoAnulado);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los análisis de un paciente específico
        /// </summary>
        /// <param name="dniPaciente">DNI del paciente</param>
        /// <returns>Colección de análisis del paciente</returns>
        public IEnumerable<Analisis> ObtenerPorPaciente(int dniPaciente)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT id_analisis, id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma, observaciones
                FROM analisis WHERE dni_paciente=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", dniPaciente);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        /// <summary>
        /// Obtiene los análisis activos (no anulados) de un paciente específico
        /// </summary>
        /// <param name="dniPaciente">DNI del paciente</param>
        /// <returns>Colección de análisis activos del paciente</returns>
        public IEnumerable<Analisis> ObtenerActivosPorPaciente(int dniPaciente)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT id_analisis, id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma, observaciones
                FROM analisis WHERE dni_paciente=@dni AND id_estado != @estadoAnulado", cn))
            {
                cmd.Parameters.AddWithValue("@dni", dniPaciente);
                cmd.Parameters.AddWithValue("@estadoAnulado", EstadoAnulado);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        /// <summary>
        /// Mapea un registro de la base de datos a un objeto Analisis
        /// </summary>
        /// <param name="rd">Registro leído de la base de datos</param>
        /// <returns>Instancia de Analisis con los datos del registro</returns>
        private Analisis Map(IDataRecord rd)
        {
            return new Analisis
            {
                IdAnalisis = rd.GetInt32(0),
                IdTipoAnalisis = rd.GetInt32(1),
                IdEstado = rd.GetInt32(2),
                DniPaciente = rd.GetInt32(3),
                DniCarga = rd.GetInt32(4),
                DniFirma = rd.IsDBNull(5) ? (int?)null : rd.GetInt32(5),
                FechaCreacion = rd.GetDateTime(6),
                FechaFirma = rd.IsDBNull(7) ? (DateTime?)null : rd.GetDateTime(7),
                Observaciones = rd.IsDBNull(8) ? null : rd.GetString(8)
            };
        }
    }
}

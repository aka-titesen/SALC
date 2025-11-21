using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos del historial de copias de seguridad.
    /// Gestiona las operaciones de registro y consulta del historial de backups.
    /// </summary>
    public class BackupRepositorio
    {
        #region Historial de Backups

        /// <summary>
        /// Inserta un nuevo registro en el historial de backups
        /// </summary>
        /// <param name="historial">Datos del backup a registrar</param>
        public void InsertarHistorial(HistorialBackup historial)
        {
            const string sql = @"
                INSERT INTO historial_backup 
                    (fecha_hora, ruta_archivo, tamano_archivo, estado, observaciones, dni_usuario)
                VALUES 
                    (@FechaHora, @RutaArchivo, @TamanoArchivo, @Estado, @Observaciones, @DniUsuario)";

            using (var conexion = DbConexion.CrearConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@FechaHora", historial.FechaHora);
                comando.Parameters.AddWithValue("@RutaArchivo", historial.RutaArchivo ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@TamanoArchivo", historial.TamanoArchivo);
                comando.Parameters.AddWithValue("@Estado", historial.Estado);
                comando.Parameters.AddWithValue("@Observaciones", historial.Observaciones ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@DniUsuario", historial.DniUsuario);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene el historial de backups ordenado por fecha descendente
        /// </summary>
        /// <param name="limite">Cantidad máxima de registros a retornar</param>
        /// <returns>Lista con el historial de backups</returns>
        public List<HistorialBackup> ObtenerHistorial(int limite = 50)
        {
            var historial = new List<HistorialBackup>();

            const string sql = @"
                SELECT TOP (@Limite) 
                    id, fecha_hora, ruta_archivo, tamano_archivo, estado, observaciones, dni_usuario
                FROM historial_backup
                ORDER BY fecha_hora DESC";

            using (var conexion = DbConexion.CrearConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Limite", limite);
                conexion.Open();

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        historial.Add(new HistorialBackup
                        {
                            Id = reader.GetInt32(0),
                            FechaHora = reader.GetDateTime(1),
                            RutaArchivo = reader.IsDBNull(2) ? null : reader.GetString(2),
                            TamanoArchivo = reader.GetInt64(3),
                            Estado = reader.GetString(4),
                            Observaciones = reader.IsDBNull(5) ? null : reader.GetString(5),
                            DniUsuario = reader.GetInt32(6)
                        });
                    }
                }
            }

            return historial;
        }

        /// <summary>
        /// Obtiene el último backup exitoso registrado en el sistema
        /// </summary>
        /// <returns>Información del último backup exitoso o null si no hay registros</returns>
        public HistorialBackup ObtenerUltimoBackup()
        {
            const string sql = @"
                SELECT TOP 1 
                    id, fecha_hora, ruta_archivo, tamano_archivo, estado, observaciones, dni_usuario
                FROM historial_backup
                WHERE estado = 'Exitoso'
                ORDER BY fecha_hora DESC";

            using (var conexion = DbConexion.CrearConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                conexion.Open();

                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new HistorialBackup
                        {
                            Id = reader.GetInt32(0),
                            FechaHora = reader.GetDateTime(1),
                            RutaArchivo = reader.IsDBNull(2) ? null : reader.GetString(2),
                            TamanoArchivo = reader.GetInt64(3),
                            Estado = reader.GetString(4),
                            Observaciones = reader.IsDBNull(5) ? null : reader.GetString(5),
                            DniUsuario = reader.GetInt32(6)
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Elimina registros del historial más antiguos que el período de retención especificado
        /// </summary>
        /// <param name="diasRetencion">Cantidad de días de retención de registros</param>
        public void LimpiarHistorialAntiguo(int diasRetencion)
        {
            const string sql = @"
                DELETE FROM historial_backup 
                WHERE fecha_hora < DATEADD(day, -@DiasRetencion, GETDATE())";

            using (var conexion = DbConexion.CrearConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@DiasRetencion", diasRetencion);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene todas las rutas únicas de archivos de backup del historial.
        /// Útil para la limpieza de archivos físicos.
        /// </summary>
        /// <returns>Lista de rutas de archivos de backup</returns>
        public List<string> ObtenerRutasBackups()
        {
            var rutas = new List<string>();

            const string sql = @"
                SELECT DISTINCT ruta_archivo 
                FROM historial_backup 
                WHERE ruta_archivo IS NOT NULL";

            using (var conexion = DbConexion.CrearConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                conexion.Open();

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rutas.Add(reader.GetString(0));
                    }
                }
            }

            return rutas;
        }

        #endregion
    }
}
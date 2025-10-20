using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class BackupRepositorio
    {
        #region Historial de Backups

        public void InsertarHistorial(HistorialBackup historial)
        {
            const string sql = @"
                INSERT INTO historial_backup (fecha_hora, ruta_archivo, tamano_archivo, estado, observaciones, tipo_backup, dni_usuario)
                VALUES (@FechaHora, @RutaArchivo, @TamanoArchivo, @Estado, @Observaciones, @TipoBackup, @DniUsuario)";

            using (var conexion = DbConexion.CrearConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@FechaHora", historial.FechaHora);
                comando.Parameters.AddWithValue("@RutaArchivo", historial.RutaArchivo ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@TamanoArchivo", historial.TamanoArchivo);
                comando.Parameters.AddWithValue("@Estado", historial.Estado);
                comando.Parameters.AddWithValue("@Observaciones", historial.Observaciones ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@TipoBackup", historial.TipoBackup);
                comando.Parameters.AddWithValue("@DniUsuario", historial.DniUsuario.HasValue ? (object)historial.DniUsuario.Value : DBNull.Value);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public List<HistorialBackup> ObtenerHistorial(int limite = 50)
        {
            var historial = new List<HistorialBackup>();

            const string sql = @"
                SELECT TOP (@Limite) id, fecha_hora, ruta_archivo, tamano_archivo, estado, observaciones, tipo_backup, dni_usuario
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
                            TipoBackup = reader.GetString(6),
                            DniUsuario = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                        });
                    }
                }
            }

            return historial;
        }

        public HistorialBackup ObtenerUltimoBackup()
        {
            const string sql = @"
                SELECT TOP 1 id, fecha_hora, ruta_archivo, tamano_archivo, estado, observaciones, tipo_backup, dni_usuario
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
                            TipoBackup = reader.GetString(6),
                            DniUsuario = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                        };
                    }
                }
            }

            return null;
        }

        public void LimpiarHistorialAntiguo(int diasRetencion)
        {
            const string sql = "DELETE FROM historial_backup WHERE fecha_hora < DATEADD(day, -@DiasRetencion, GETDATE())";

            using (var conexion = DbConexion.CrearConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@DiasRetencion", diasRetencion);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        #endregion

        #region Configuración de Backups

        public ConfiguracionBackup ObtenerConfiguracion()
        {
            const string sql = @"
                SELECT id, backup_automatico_habilitado, hora_programada, dias_semana, ruta_destino, 
                       dias_retencion, ultima_ejecucion, fecha_modificacion, dni_usuario_modificacion
                FROM configuracion_backup
                WHERE id = 1";

            using (var conexion = DbConexion.CrearConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                conexion.Open();

                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ConfiguracionBackup
                        {
                            Id = reader.GetInt32(0),
                            BackupAutomaticoHabilitado = reader.GetBoolean(1),
                            HoraProgramada = reader.IsDBNull(2) ? null : reader.GetString(2),
                            DiasSemana = reader.IsDBNull(3) ? null : reader.GetString(3),
                            RutaDestino = reader.IsDBNull(4) ? null : reader.GetString(4),
                            DiasRetencion = reader.GetInt32(5),
                            UltimaEjecucion = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                            FechaModificacion = reader.GetDateTime(7),
                            DniUsuarioModificacion = reader.GetInt32(8)
                        };
                    }
                }
            }

            // Si no existe configuración, crear una por defecto
            return CrearConfiguracionPorDefecto();
        }

        public void ActualizarConfiguracion(ConfiguracionBackup config)
        {
            // Verificar si existe el registro
            const string sqlExiste = "SELECT COUNT(*) FROM configuracion_backup WHERE id = 1";
            
            using (var conexion = DbConexion.CrearConexion())
            {
                conexion.Open();
                
                using (var comando = new SqlCommand(sqlExiste, conexion))
                {
                    var existe = (int)comando.ExecuteScalar() > 0;

                    string sql;
                    if (existe)
                    {
                        sql = @"
                            UPDATE configuracion_backup SET 
                                backup_automatico_habilitado = @BackupAutomaticoHabilitado,
                                hora_programada = @HoraProgramada,
                                dias_semana = @DiasSemana,
                                ruta_destino = @RutaDestino,
                                dias_retencion = @DiasRetencion,
                                fecha_modificacion = @FechaModificacion,
                                dni_usuario_modificacion = @DniUsuarioModificacion
                            WHERE id = 1";
                    }
                    else
                    {
                        sql = @"
                            INSERT INTO configuracion_backup 
                                (id, backup_automatico_habilitado, hora_programada, dias_semana, ruta_destino, 
                                 dias_retencion, fecha_modificacion, dni_usuario_modificacion)
                            VALUES (1, @BackupAutomaticoHabilitado, @HoraProgramada, @DiasSemana, @RutaDestino, 
                                    @DiasRetencion, @FechaModificacion, @DniUsuarioModificacion)";
                    }

                    using (var cmdUpdate = new SqlCommand(sql, conexion))
                    {
                        cmdUpdate.Parameters.AddWithValue("@BackupAutomaticoHabilitado", config.BackupAutomaticoHabilitado);
                        cmdUpdate.Parameters.AddWithValue("@HoraProgramada", config.HoraProgramada ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@DiasSemana", config.DiasSemana ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@RutaDestino", config.RutaDestino ?? (object)DBNull.Value);
                        cmdUpdate.Parameters.AddWithValue("@DiasRetencion", config.DiasRetencion);
                        cmdUpdate.Parameters.AddWithValue("@FechaModificacion", config.FechaModificacion);
                        cmdUpdate.Parameters.AddWithValue("@DniUsuarioModificacion", config.DniUsuarioModificacion);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
            }
        }

        public void ActualizarUltimaEjecucion(DateTime fechaEjecucion)
        {
            const string sql = "UPDATE configuracion_backup SET ultima_ejecucion = @UltimaEjecucion WHERE id = 1";

            using (var conexion = DbConexion.CrearConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@UltimaEjecucion", fechaEjecucion);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private ConfiguracionBackup CrearConfiguracionPorDefecto()
        {
            var config = new ConfiguracionBackup
            {
                Id = 1,
                BackupAutomaticoHabilitado = false,
                HoraProgramada = "02:00",
                DiasSemana = "1,2,3,4,5", // Lunes a Viernes
                RutaDestino = @"C:\Backups\SALC",
                DiasRetencion = 30,
                FechaModificacion = DateTime.Now,
                DniUsuarioModificacion = 0
            };

            ActualizarConfiguracion(config);
            return config;
        }

        #endregion
    }
}
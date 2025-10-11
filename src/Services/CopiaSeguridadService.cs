using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gestión de copia de seguridad con convención 'Service'.
    /// </summary>
    public class CopiaSeguridadService
    {
        private readonly string _cadenaConexion;
        private readonly ConfiguracionSistemaService _servicioConfig;

        public CopiaSeguridadService()
        {
            _cadenaConexion = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_cadenaConexion))
                throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada.");

            _servicioConfig = new ConfiguracionSistemaService();
        }

        public class InformacionBackup
        {
            public string NombreArchivo { get; set; }
            public DateTime FechaCreacion { get; set; }
            public long TamanoBytes { get; set; }
            public string TamanoMB => $"{TamanoBytes / 1024.0 / 1024.0:F2} MB";
            public string RutaCompleta { get; set; }
        }

        public class ResultadoBackup
        {
            public bool Exito { get; set; }
            public string Mensaje { get; set; }
            public string RutaArchivoBackup { get; set; }
            public TimeSpan Duracion { get; set; }
        }

        public ResultadoBackup EjecutarBackupCompleto(string rutaPersonalizada = null)
        {
            var inicio = DateTime.Now;
            var resultado = new ResultadoBackup { Exito = false };

            try
            {
                var config = _servicioConfig.ObtenerConfiguracionSistema();
                string carpeta = rutaPersonalizada ?? config.RutaBackups;
                if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

                string nombre = $"SALC_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string ruta = Path.Combine(carpeta, nombre);

                string sql = @"BACKUP DATABASE [SALC] TO DISK = @ruta WITH FORMAT, INIT, NAME='SALC-Backup Completo', DESCRIPTION='Backup automático SALC', SKIP, NOREWIND, NOUNLOAD, COMPRESSION, STATS=10";

                using (var cn = new SqlConnection(_cadenaConexion))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.CommandTimeout = 300;
                        cmd.Parameters.AddWithValue("@ruta", ruta);
                        cmd.ExecuteNonQuery();
                    }
                }

                resultado.Exito = true;
                resultado.RutaArchivoBackup = ruta;
                resultado.Mensaje = $"Backup completado en: {ruta}";
                resultado.Duracion = DateTime.Now - inicio;
            }
            catch (Exception ex)
            {
                resultado.Exito = false;
                resultado.Mensaje = $"Error durante el backup: {ex.Message}";
                resultado.Duracion = DateTime.Now - inicio;
                throw;
            }

            return resultado;
        }

        public List<InformacionBackup> ObtenerBackupsDisponibles()
        {
            var lista = new List<InformacionBackup>();
            var config = _servicioConfig.ObtenerConfiguracionSistema();

            var dir = new DirectoryInfo(config.RutaBackups);
            if (!dir.Exists) return lista;

            foreach (var f in dir.GetFiles("SALC_*.bak").OrderByDescending(f => f.CreationTime))
            {
                lista.Add(new InformacionBackup
                {
                    NombreArchivo = f.Name,
                    FechaCreacion = f.CreationTime,
                    TamanoBytes = f.Length,
                    RutaCompleta = f.FullName
                });
            }
            return lista;
        }

        public ResultadoBackup RestaurarDesdeBackup(string rutaArchivo, bool forzarReemplazo = false)
        {
            var inicio = DateTime.Now;
            var resultado = new ResultadoBackup { Exito = false };

            try
            {
                if (!File.Exists(rutaArchivo))
                    throw new FileNotFoundException($"No existe el archivo de backup: {rutaArchivo}");

                string sql = @"RESTORE DATABASE [SALC] FROM DISK = @ruta WITH FILE = 1, NOUNLOAD, REPLACE, STATS = 10";

                using (var cn = new SqlConnection(_cadenaConexion))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.CommandTimeout = 600;
                        cmd.Parameters.AddWithValue("@ruta", rutaArchivo);
                        cmd.ExecuteNonQuery();
                    }
                }

                resultado.Exito = true;
                resultado.Mensaje = "Restauración completada";
                resultado.Duracion = DateTime.Now - inicio;
            }
            catch (Exception ex)
            {
                resultado.Exito = false;
                resultado.Mensaje = $"Error durante la restauración: {ex.Message}";
                resultado.Duracion = DateTime.Now - inicio;
                throw;
            }

            return resultado;
        }
    }
}

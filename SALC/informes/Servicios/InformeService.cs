using SALC.informes.Infraestructura; // GeneradorPdf
using SALC.Infraestructura;          // DbConexion (tu clase de conexión)
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SALC.Informes.Servicios
{
    public interface IInformeService
    {
        /// Genera el PDF del análisis y devuelve la ruta del archivo.
        string GenerarPdfDeAnalisis(int idAnalisis);
    }

    public class InformeService : IInformeService
    {
        public string GenerarPdfDeAnalisis(int idAnalisis)
        {
            // ----- 1) Cabecera -----
            string nombre = "", apellido = "", tipoAnalisis = "", estado = "", observaciones = "";
            DateTime fechaCreacion = DateTime.Now;
            DateTime? fechaFirma = null;

            using (var cn = DbConexion.CrearConexion())
            {
                cn.Open();

                const string sqlCabecera = @"
SELECT  p.nombre, p.apellido,
        t.descripcion      AS tipo_analisis,
        e.descripcion      AS estado,
        a.fecha_creacion,
        a.fecha_firma,
        a.observaciones
FROM    analisis a
JOIN    pacientes p        ON p.dni = a.dni_paciente
JOIN    tipos_analisis t   ON t.id_tipo_analisis = a.id_tipo_analisis
JOIN    estados_analisis e ON e.id_estado = a.id_estado
WHERE   a.id_analisis = @id;";

                using (var cmd = new SqlCommand(sqlCabecera, cn))
                {
                    cmd.Parameters.AddWithValue("@id", idAnalisis);
                    using var rd = cmd.ExecuteReader();
                    if (!rd.Read()) throw new InvalidOperationException("No se encontró el análisis.");

                    nombre = rd.GetString(rd.GetOrdinal("nombre"));
                    apellido = rd.GetString(rd.GetOrdinal("apellido"));
                    tipoAnalisis = rd.GetString(rd.GetOrdinal("tipo_analisis"));
                    estado = rd.GetString(rd.GetOrdinal("estado"));
                    fechaCreacion = rd.GetDateTime(rd.GetOrdinal("fecha_creacion"));

                    int ixFirma = rd.GetOrdinal("fecha_firma");
                    fechaFirma = rd.IsDBNull(ixFirma) ? (DateTime?)null : rd.GetDateTime(ixFirma);

                    int ixObs = rd.GetOrdinal("observaciones");
                    observaciones = rd.IsDBNull(ixObs) ? "" : rd.GetString(ixObs);
                }

                // ----- 2) Métricas -----
                var metricas = new Dictionary<string, (decimal valor, string unidad)>();
                const string sqlMetricas = @"
SELECT  m.nombre, am.resultado, m.unidad_medida
FROM    analisis_metrica am
JOIN    metricas m ON m.id_metrica = am.id_metrica
WHERE   am.id_analisis = @id
ORDER BY m.nombre;";

                using (var cmd = new SqlCommand(sqlMetricas, cn))
                {
                    cmd.Parameters.AddWithValue("@id", idAnalisis);
                    using var rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        string met = rd.GetString(0);
                        decimal val = rd.GetDecimal(1);
                        string uni = rd.GetString(2);
                        metricas[met] = (val, uni);
                    }
                }

                // ----- 3) PDF -----
                return GeneradorPdf.GenerarInformeAnalisis(
                    nombrePaciente: nombre,
                    apellidoPaciente: apellido,
                    tipoAnalisis: tipoAnalisis,
                    fechaCreacion: fechaCreacion,
                    estado: estado,
                    fechaFirma: fechaFirma,
                    metricas: metricas,
                    observaciones: observaciones
                );
            }
        }
    }
}

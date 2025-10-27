using SALC.informes.Infraestructura; // GeneradorPdf
using SALC.Infraestructura;          // DbConexion
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SALC.Informes.Servicios
{
    public interface IInformeService
    {
        string GenerarPdfDeAnalisis(int idAnalisis);
    }

    public class InformeService : IInformeService
    {
        public string GenerarPdfDeAnalisis(int idAnalisis)
        {
            // Datos a rellenar desde la BD
            string nombre = "", apellido = "", tipoAnalisis = "", estado = "", observaciones = "";
            DateTime fechaCreacion = DateTime.Now;
            DateTime? fechaFirma = null;

            using (SqlConnection cn = DbConexion.CrearConexion())
            {
                cn.Open();

                // ===== 1) CABECERA =====
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

                using (SqlCommand cmd = new SqlCommand(sqlCabecera, cn))
                {
                    cmd.Parameters.AddWithValue("@id", idAnalisis);

                    using (SqlDataReader rdCabecera = cmd.ExecuteReader())
                    {
                        if (!rdCabecera.Read())
                            throw new InvalidOperationException("No se encontró el análisis.");

                        nombre = rdCabecera.GetString(rdCabecera.GetOrdinal("nombre"));
                        apellido = rdCabecera.GetString(rdCabecera.GetOrdinal("apellido"));
                        tipoAnalisis = rdCabecera.GetString(rdCabecera.GetOrdinal("tipo_analisis"));
                        estado = rdCabecera.GetString(rdCabecera.GetOrdinal("estado"));
                        fechaCreacion = rdCabecera.GetDateTime(rdCabecera.GetOrdinal("fecha_creacion"));

                        int ixFirma = rdCabecera.GetOrdinal("fecha_firma");
                        fechaFirma = rdCabecera.IsDBNull(ixFirma) ? (DateTime?)null
                                                                   : rdCabecera.GetDateTime(ixFirma);

                        int ixObs = rdCabecera.GetOrdinal("observaciones");
                        observaciones = rdCabecera.IsDBNull(ixObs) ? "" : rdCabecera.GetString(ixObs);
                    }
                }

                // ===== 2) MÉTRICAS =====
                var metricas = new Dictionary<string, (decimal valor, string unidad)>();

                const string sqlMetricas = @"
SELECT  m.nombre, am.resultado, m.unidad_medida
FROM    analisis_metrica am
JOIN    metricas m ON m.id_metrica = am.id_metrica
WHERE   am.id_analisis = @id
ORDER BY m.nombre;";

                using (SqlCommand cmd = new SqlCommand(sqlMetricas, cn))
                {
                    cmd.Parameters.AddWithValue("@id", idAnalisis);

                    using (SqlDataReader rdMetricas = cmd.ExecuteReader())
                    {
                        while (rdMetricas.Read())
                        {
                            string met = rdMetricas.GetString(0);
                            decimal val = rdMetricas.GetDecimal(1);
                            string uni = rdMetricas.GetString(2);
                            metricas[met] = (val, uni);
                        }
                    }
                }

                // ===== 3) PDF =====
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

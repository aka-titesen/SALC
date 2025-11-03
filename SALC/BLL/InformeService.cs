using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using SALC.Infraestructura;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace SALC.BLL
{
    /// <summary>
    /// Servicio de generación de informes PDF siguiendo el patrón MVP de 3 capas.
    /// Implementa RF-08: Generar y Enviar Informe
    /// </summary>
    public class InformeService : IInformeService
    {
        public string GenerarPdfDeAnalisis(int idAnalisis)
        {
            // Variables para almacenar los datos del análisis
            string nombrePaciente = "";
            string apellidoPaciente = "";
            int dniPaciente = 0;
            string tipoAnalisis = "";
            string estado = "";
            string observaciones = "";
            DateTime fechaCreacion = DateTime.Now;
            DateTime? fechaFirma = null;
            string medicoFirmaNombre = "";
            string medicoFirmaApellido = "";
            string medicoMatricula = "";
            string medicoEspecialidad = "";

            using (SqlConnection cn = DbConexion.CrearConexion())
            {
                cn.Open();

                // ===== 1) CABECERA DEL ANÁLISIS =====
                const string sqlCabecera = @"
SELECT  p.dni, p.nombre, p.apellido,
        t.descripcion      AS tipo_analisis,
        e.descripcion      AS estado,
        a.fecha_creacion,
        a.fecha_firma,
        a.observaciones,
        a.dni_firma
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
                            throw new InvalidOperationException("No se encontró el análisis solicitado.");

                        dniPaciente = rdCabecera.GetInt32(rdCabecera.GetOrdinal("dni"));
                        nombrePaciente = rdCabecera.GetString(rdCabecera.GetOrdinal("nombre"));
                        apellidoPaciente = rdCabecera.GetString(rdCabecera.GetOrdinal("apellido"));
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

                // Validar que el análisis esté verificado
                if (estado != "Verificado")
                {
                    throw new InvalidOperationException("Solo se pueden generar informes de análisis verificados.");
                }

                // ===== 2) DATOS DEL MÉDICO QUE FIRMÓ =====
                const string sqlMedico = @"
SELECT  u.nombre, u.apellido, m.nro_matricula, m.especialidad
FROM    usuarios u
JOIN    medicos m ON m.dni = u.dni
JOIN    analisis a ON a.dni_firma = u.dni
WHERE   a.id_analisis = @id;";

                using (SqlCommand cmd = new SqlCommand(sqlMedico, cn))
                {
                    cmd.Parameters.AddWithValue("@id", idAnalisis);

                    using (SqlDataReader rdMedico = cmd.ExecuteReader())
                    {
                        if (rdMedico.Read())
                        {
                            medicoFirmaNombre = rdMedico.GetString(0);
                            medicoFirmaApellido = rdMedico.GetString(1);
                            medicoMatricula = rdMedico.GetInt32(2).ToString();
                            medicoEspecialidad = rdMedico.GetString(3);
                        }
                    }
                }

                // ===== 3) MÉTRICAS Y RESULTADOS =====
                var metricas = new List<MetricaResultado>();

                const string sqlMetricas = @"
SELECT  m.nombre, am.resultado, m.unidad_medida, m.valor_minimo, m.valor_maximo, am.observaciones
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
                            string nombreMetrica = rdMetricas.GetString(0);
                            decimal resultado = rdMetricas.GetDecimal(1);
                            string unidad = rdMetricas.GetString(2);

                            decimal? valorMin = rdMetricas.IsDBNull(3) ? (decimal?)null : rdMetricas.GetDecimal(3);
                            decimal? valorMax = rdMetricas.IsDBNull(4) ? (decimal?)null : rdMetricas.GetDecimal(4);
                            string obsMetrica = rdMetricas.IsDBNull(5) ? "" : rdMetricas.GetString(5);

                            metricas.Add(new MetricaResultado
                            {
                                Nombre = nombreMetrica,
                                Resultado = resultado,
                                Unidad = unidad,
                                ValorMinimo = valorMin,
                                ValorMaximo = valorMax,
                                Observaciones = obsMetrica
                            });
                        }
                    }
                }

                // ===== 4) SOLICITAR UBICACIÓN PARA GUARDAR EL PDF =====
                string rutaArchivo = null;
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
                    saveDialog.Title = "Guardar Informe de Análisis";
                    saveDialog.FileName = $"Informe_{apellidoPaciente}_{nombrePaciente}_DNI{dniPaciente}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    saveDialog.DefaultExt = "pdf";
                    saveDialog.AddExtension = true;

                    if (saveDialog.ShowDialog() != DialogResult.OK)
                    {
                        return null; // Usuario canceló
                    }

                    rutaArchivo = saveDialog.FileName;
                }

                // ===== 5) GENERAR EL DOCUMENTO PDF =====
                Document doc = CrearDocumentoPdf(
                    nombrePaciente, apellidoPaciente, dniPaciente,
                    tipoAnalisis, fechaCreacion, fechaFirma,
                    medicoFirmaNombre, medicoFirmaApellido, medicoMatricula, medicoEspecialidad,
                    metricas, observaciones, idAnalisis);

                // ===== 6) RENDERIZAR Y GUARDAR =====
                PdfDocumentRenderer renderer = new PdfDocumentRenderer
                {
                    Document = doc
                };
                renderer.RenderDocument();
                renderer.PdfDocument.Save(rutaArchivo);

                return rutaArchivo;
            }
        }

        /// <summary>
        /// Crea el documento PDF con formato profesional
        /// </summary>
        private Document CrearDocumentoPdf(
            string nombrePaciente, string apellidoPaciente, int dniPaciente,
            string tipoAnalisis, DateTime fechaCreacion, DateTime? fechaFirma,
            string medicoNombre, string medicoApellido, string medicoMatricula, string medicoEspecialidad,
            List<MetricaResultado> metricas, string observaciones, int idAnalisis)
        {
            Document doc = new Document();
            Section seccion = doc.AddSection();

            // Configurar márgenes
            seccion.PageSetup.LeftMargin = "2.5cm";
            seccion.PageSetup.RightMargin = "2.5cm";
            seccion.PageSetup.TopMargin = "2cm";
            seccion.PageSetup.BottomMargin = "2cm";

            // ===== ENCABEZADO =====
            Paragraph titulo = seccion.AddParagraph("INFORME DE ANÁLISIS CLÍNICO");
            titulo.Format.Font.Size = 18;
            titulo.Format.Font.Bold = true;
            titulo.Format.Alignment = ParagraphAlignment.Center;
            titulo.Format.SpaceAfter = "0.5cm";

            Paragraph subtitulo = seccion.AddParagraph("Sistema de Administración de Laboratorio Clínico - SALC");
            subtitulo.Format.Font.Size = 10;
            subtitulo.Format.Font.Italic = true;
            subtitulo.Format.Alignment = ParagraphAlignment.Center;
            subtitulo.Format.SpaceAfter = "1cm";

            // ===== INFORMACIÓN DEL ANÁLISIS =====
            Paragraph infoAnalisis = seccion.AddParagraph($"Análisis N° {idAnalisis}");
            infoAnalisis.Format.Font.Size = 10;
            infoAnalisis.Format.Font.Bold = true;
            infoAnalisis.Format.SpaceAfter = "0.3cm";

            // ===== DATOS DEL PACIENTE =====
            Paragraph seccionPaciente = seccion.AddParagraph("DATOS DEL PACIENTE");
            seccionPaciente.Format.Font.Size = 12;
            seccionPaciente.Format.Font.Bold = true;
            seccionPaciente.Format.SpaceAfter = "0.3cm";
            seccionPaciente.Format.Borders.Bottom.Width = 1;

            seccion.AddParagraph($"Apellido y Nombre: {apellidoPaciente}, {nombrePaciente}");
            seccion.AddParagraph($"DNI: {dniPaciente}");
            seccion.AddParagraph().Format.SpaceAfter = "0.5cm";

            // ===== DATOS DEL ANÁLISIS =====
            Paragraph seccionAnalisisDetalle = seccion.AddParagraph("INFORMACIÓN DEL ANÁLISIS");
            seccionAnalisisDetalle.Format.Font.Size = 12;
            seccionAnalisisDetalle.Format.Font.Bold = true;
            seccionAnalisisDetalle.Format.SpaceAfter = "0.3cm";
            seccionAnalisisDetalle.Format.Borders.Bottom.Width = 1;

            seccion.AddParagraph($"Tipo de Análisis: {tipoAnalisis}");
            seccion.AddParagraph($"Fecha de Realización: {fechaCreacion:dd/MM/yyyy HH:mm}");
            if (fechaFirma.HasValue)
                seccion.AddParagraph($"Fecha de Verificación: {fechaFirma:dd/MM/yyyy HH:mm}");
            seccion.AddParagraph().Format.SpaceAfter = "0.5cm";

            // ===== RESULTADOS =====
            Paragraph seccionResultados = seccion.AddParagraph("RESULTADOS");
            seccionResultados.Format.Font.Size = 12;
            seccionResultados.Format.Font.Bold = true;
            seccionResultados.Format.SpaceAfter = "0.3cm";
            seccionResultados.Format.Borders.Bottom.Width = 1;

            // Tabla de métricas
            Table tabla = seccion.AddTable();
            tabla.Borders.Width = 0.75;

            // Columnas
            Column col1 = tabla.AddColumn("7cm");
            Column col2 = tabla.AddColumn("2.5cm");
            Column col3 = tabla.AddColumn("2cm");
            Column col4 = tabla.AddColumn("3cm");

            // Encabezado de la tabla
            Row header = tabla.AddRow();
            header.Shading.Color = Colors.LightGray;
            header.HeadingFormat = true;
            header.Format.Font.Bold = true;
            header.Cells[0].AddParagraph("Métrica");
            header.Cells[1].AddParagraph("Resultado");
            header.Cells[2].AddParagraph("Unidad");
            header.Cells[3].AddParagraph("Valores de Ref.");

            // Datos de las métricas
            foreach (var metrica in metricas)
            {
                Row row = tabla.AddRow();
                row.Cells[0].AddParagraph(metrica.Nombre);
                
                // Resultado
                Paragraph pResultado = row.Cells[1].AddParagraph(metrica.Resultado.ToString("0.00"));
                pResultado.Format.Alignment = ParagraphAlignment.Right;
                
                // Verificar si el resultado está fuera de rango
                bool fueraDeRango = false;
                if (metrica.ValorMinimo.HasValue && metrica.Resultado < metrica.ValorMinimo.Value)
                    fueraDeRango = true;
                if (metrica.ValorMaximo.HasValue && metrica.Resultado > metrica.ValorMaximo.Value)
                    fueraDeRango = true;
                
                if (fueraDeRango)
                {
                    pResultado.Format.Font.Bold = true;
                    pResultado.Format.Font.Color = Colors.Red;
                }

                row.Cells[2].AddParagraph(metrica.Unidad);

                // Valores de referencia
                string valoresRef = "";
                if (metrica.ValorMinimo.HasValue && metrica.ValorMaximo.HasValue)
                    valoresRef = $"{metrica.ValorMinimo:0.00} - {metrica.ValorMaximo:0.00}";
                else if (metrica.ValorMinimo.HasValue)
                    valoresRef = $"> {metrica.ValorMinimo:0.00}";
                else if (metrica.ValorMaximo.HasValue)
                    valoresRef = $"< {metrica.ValorMaximo:0.00}";
                else
                    valoresRef = "N/A";
                
                Paragraph pValRef = row.Cells[3].AddParagraph(valoresRef);
                pValRef.Format.Font.Size = 9;
            }

            seccion.AddParagraph().Format.SpaceAfter = "0.5cm";

            // ===== OBSERVACIONES =====
            if (!string.IsNullOrWhiteSpace(observaciones))
            {
                Paragraph seccionObservaciones = seccion.AddParagraph("OBSERVACIONES");
                seccionObservaciones.Format.Font.Size = 12;
                seccionObservaciones.Format.Font.Bold = true;
                seccionObservaciones.Format.SpaceAfter = "0.3cm";

                Paragraph obsTexto = seccion.AddParagraph(observaciones);
                obsTexto.Format.Font.Size = 10;
                seccion.AddParagraph().Format.SpaceAfter = "0.5cm";
            }

            // ===== FIRMA DEL MÉDICO =====
            seccion.AddParagraph().Format.SpaceAfter = "1.5cm";

            Paragraph seccionFirma = seccion.AddParagraph("VERIFICADO POR");
            seccionFirma.Format.Font.Size = 12;
            seccionFirma.Format.Font.Bold = true;
            seccionFirma.Format.SpaceAfter = "0.3cm";
            seccionFirma.Format.Borders.Bottom.Width = 1;

            seccion.AddParagraph($"Dr./Dra. {medicoNombre} {medicoApellido}");
            seccion.AddParagraph($"Matrícula N°: {medicoMatricula}");
            seccion.AddParagraph($"Especialidad: {medicoEspecialidad}");

            // ===== PIE DE PÁGINA =====
            seccion.AddParagraph().Format.SpaceAfter = "1cm";
            Paragraph pie = seccion.AddParagraph($"Documento generado el {DateTime.Now:dd/MM/yyyy HH:mm}");
            pie.Format.Font.Size = 8;
            pie.Format.Font.Italic = true;
            pie.Format.Alignment = ParagraphAlignment.Center;
            pie.Format.Font.Color = Colors.Gray;

            return doc;
        }

        /// <summary>
        /// Clase auxiliar para transportar datos de métricas
        /// </summary>
        private class MetricaResultado
        {
            public string Nombre { get; set; }
            public decimal Resultado { get; set; }
            public string Unidad { get; set; }
            public decimal? ValorMinimo { get; set; }
            public decimal? ValorMaximo { get; set; }
            public string Observaciones { get; set; }
        }
    }
}

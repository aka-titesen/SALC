using System;
using System.Collections.Generic;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace SALC.informes.Infraestructura
{
    public static class GeneradorPdf
    {
        public static string GenerarInformeAnalisis(
            string nombrePaciente,
            string apellidoPaciente,
            string tipoAnalisis,
            DateTime fechaCreacion,
            string estado,
            DateTime? fechaFirma,
            Dictionary<string, (decimal valor, string unidad)> metricas,
            string observaciones = "")
        {
            // Crear documento
            Document doc = new Document();
            Section seccion = doc.AddSection();

            // Encabezado
            Paragraph titulo = seccion.AddParagraph("Informe de Análisis Clínico");
            titulo.Format.Font.Size = 16;
            titulo.Format.Font.Bold = true;
            titulo.Format.Alignment = ParagraphAlignment.Center;
            seccion.AddParagraph();

            // Datos del paciente
            seccion.AddParagraph($"Paciente: {nombrePaciente} {apellidoPaciente}");
            seccion.AddParagraph($"Tipo de Análisis: {tipoAnalisis}");
            seccion.AddParagraph($"Estado: {estado}");
            seccion.AddParagraph($"Fecha de Creación: {fechaCreacion:dd/MM/yyyy HH:mm}");
            if (fechaFirma.HasValue)
                seccion.AddParagraph($"Fecha de Firma: {fechaFirma:dd/MM/yyyy HH:mm}");
            seccion.AddParagraph();

            // Tabla de métricas
            Table tabla = seccion.AddTable();
            tabla.Borders.Width = 0.75;
            tabla.AddColumn("8cm");
            tabla.AddColumn("3cm");
            tabla.AddColumn("3cm");

            Row header = tabla.AddRow();
            header.Shading.Color = Colors.LightGray;
            header.Cells[0].AddParagraph("Métrica");
            header.Cells[1].AddParagraph("Resultado");
            header.Cells[2].AddParagraph("Unidad");

            foreach (var m in metricas)
            {
                Row row = tabla.AddRow();
                row.Cells[0].AddParagraph(m.Key);
                row.Cells[1].AddParagraph(m.Value.valor.ToString("0.00"));
                row.Cells[2].AddParagraph(m.Value.unidad);
            }

            seccion.AddParagraph();
            if (!string.IsNullOrEmpty(observaciones))
            {
                Paragraph obs = seccion.AddParagraph("Observaciones:");
                obs.Format.Font.Bold = true;
                seccion.AddParagraph(observaciones);
            }

            // Crear carpeta si no existe
            string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Informes");
            Directory.CreateDirectory(carpeta);

            string nombreArchivo = $"{nombrePaciente}_{apellidoPaciente}_{tipoAnalisis}_{fechaCreacion:yyyyMMddHHmm}.pdf";
            string ruta = Path.Combine(carpeta, nombreArchivo);

            // Renderizar PDF
            PdfDocumentRenderer renderer = new PdfDocumentRenderer
            {
                Document = doc
            };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(ruta);

            return ruta;
        }
    }
}

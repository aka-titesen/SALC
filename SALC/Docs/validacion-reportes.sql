-- =====================================================
-- Script de Validación de Reportes Administrativos
-- Sistema: SALC - Sistema de Administración de Laboratorio Clínico
-- =====================================================

USE SALC;
GO

PRINT '=== VALIDACIÓN DE REPORTES ADMINISTRATIVOS ===';
PRINT '';

-- =====================================================
-- 1. REPORTE DE PRODUCTIVIDAD DE MÉDICOS
-- =====================================================
PRINT '--- 1. REPORTE DE PRODUCTIVIDAD DE MÉDICOS ---';
PRINT 'Ranking de análisis creados y verificados por médico';
PRINT '';

DECLARE @FechaDesde DATE = '2024-01-01';
DECLARE @FechaHasta DATE = '2024-12-31';

SELECT 
    u.dni AS DNI,
    u.nombre + ' ' + u.apellido AS NombreMedico,
    COUNT(a.id_analisis) AS TotalCreados,
    SUM(CASE WHEN a.id_estado = 2 THEN 1 ELSE 0 END) AS TotalVerificados,
    CAST(
        CASE 
            WHEN COUNT(a.id_analisis) > 0 
            THEN (SUM(CASE WHEN a.id_estado = 2 THEN 1 ELSE 0 END) * 100.0 / COUNT(a.id_analisis))
            ELSE 0 
        END 
    AS DECIMAL(5,2)) AS PorcentajeVerificacion
FROM 
    usuarios u
INNER JOIN 
    medicos m ON u.dni = m.dni
LEFT JOIN 
    analisis a ON u.dni = a.dni_carga 
    AND a.fecha_creacion BETWEEN @FechaDesde AND @FechaHasta
    AND a.id_estado != 3  -- Excluye análisis anulados
WHERE 
    u.id_rol = 2 AND u.estado = 'Activo'
GROUP BY 
    u.dni, u.nombre, u.apellido
ORDER BY 
    TotalVerificados DESC, TotalCreados DESC;

PRINT '';
PRINT 'Interpretación:';
PRINT '- TotalCreados: Análisis que el médico creó en el período';
PRINT '- TotalVerificados: Análisis que el médico firmó/validó';
PRINT '- PorcentajeVerificacion: Eficiencia en la firma de análisis';
PRINT '';

-- =====================================================
-- 2. REPORTE DE FACTURACIÓN POR OBRA SOCIAL
-- =====================================================
PRINT '--- 2. REPORTE DE FACTURACIÓN POR OBRA SOCIAL ---';
PRINT 'Distribución de análisis según obra social';
PRINT '';

SELECT 
    ISNULL(os.nombre, 'Privado') AS ObraSocial,
    COUNT(a.id_analisis) AS CantidadAnalisis,
    CAST(COUNT(a.id_analisis) * 100.0 / SUM(COUNT(a.id_analisis)) OVER() AS DECIMAL(5,2)) AS Porcentaje
FROM 
    analisis a
INNER JOIN 
    pacientes p ON a.dni_paciente = p.dni
LEFT JOIN 
    obras_sociales os ON p.id_obra_social = os.id_obra_social
WHERE 
    a.fecha_creacion BETWEEN @FechaDesde AND @FechaHasta
    AND a.id_estado != 3  -- Excluye análisis anulados
GROUP BY 
    os.nombre
ORDER BY 
    CantidadAnalisis DESC;

PRINT '';
PRINT 'Interpretación:';
PRINT '- ObraSocial: Nombre de la obra social (Privado = sin obra social)';
PRINT '- CantidadAnalisis: Número total de análisis realizados';
PRINT '- Porcentaje: Proporción respecto al total del período';
PRINT '';

-- =====================================================
-- 3. REPORTE DE DEMANDA (TOP ANÁLISIS)
-- =====================================================
PRINT '--- 3. REPORTE DE DEMANDA (TOP 10 ANÁLISIS) ---';
PRINT 'Tipos de análisis más solicitados';
PRINT '';

SELECT TOP (10)
    ta.descripcion AS TipoAnalisis,
    COUNT(a.id_analisis) AS CantidadSolicitados,
    CAST(COUNT(a.id_analisis) * 100.0 / SUM(COUNT(a.id_analisis)) OVER() AS DECIMAL(5,2)) AS PorcentajeDemanda
FROM 
    analisis a
INNER JOIN 
    tipos_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
WHERE 
    a.fecha_creacion BETWEEN @FechaDesde AND @FechaHasta
    AND a.id_estado != 3  -- Excluye análisis anulados
GROUP BY 
    ta.descripcion
ORDER BY 
    CantidadSolicitados DESC;

PRINT '';
PRINT 'Interpretación:';
PRINT '- TipoAnalisis: Nombre del tipo de análisis';
PRINT '- CantidadSolicitados: Veces que fue solicitado';
PRINT '- PorcentajeDemanda: Proporción del Top 10';
PRINT '';

-- =====================================================
-- ESTADÍSTICAS ADICIONALES
-- =====================================================
PRINT '--- ESTADÍSTICAS GENERALES DEL PERÍODO ---';
PRINT '';

SELECT 
    'Análisis Totales' AS Metrica,
    COUNT(*) AS Valor
FROM analisis
WHERE fecha_creacion BETWEEN @FechaDesde AND @FechaHasta
    AND id_estado != 3

UNION ALL

SELECT 
    'Análisis Verificados' AS Metrica,
    COUNT(*) AS Valor
FROM analisis
WHERE fecha_creacion BETWEEN @FechaDesde AND @FechaHasta
    AND id_estado = 2

UNION ALL

SELECT 
    'Análisis Pendientes' AS Metrica,
    COUNT(*) AS Valor
FROM analisis
WHERE fecha_creacion BETWEEN @FechaDesde AND @FechaHasta
    AND id_estado = 1

UNION ALL

SELECT 
    'Médicos Activos' AS Metrica,
    COUNT(DISTINCT dni) AS Valor
FROM usuarios
WHERE id_rol = 2 AND estado = 'Activo'

UNION ALL

SELECT 
    'Pacientes Atendidos' AS Metrica,
    COUNT(DISTINCT dni_paciente) AS Valor
FROM analisis
WHERE fecha_creacion BETWEEN @FechaDesde AND @FechaHasta
    AND id_estado != 3;

PRINT '';
PRINT '=== VALIDACIÓN COMPLETADA ===';
PRINT 'Si los resultados muestran datos, las consultas funcionan correctamente.';
PRINT 'Si aparecen valores en 0, verifique que existan análisis en el rango de fechas.';
GO

# ?? Módulo de Reportes para Administrador - SALC

## Descripción General

El módulo de reportes proporciona análisis estadísticos y visualizaciones gráficas para ayudar al administrador a tomar decisiones basadas en datos sobre la gestión del laboratorio.

---

## ?? Reportes Disponibles

### 1. **Reporte de Productividad (Top Médicos)**

**Objetivo:** Identificar el rendimiento de cada médico en términos de análisis creados y verificados.

**Visualización:** Gráfico de barras comparativo

**Decisiones que ayuda a tomar:**
- ¿Quién está sobrecargado de trabajo?
- ¿Quién necesita capacitación o soporte?
- ¿Quién es más eficiente en la verificación de análisis?

**Datos mostrados:**
- Análisis Creados (azul)
- Análisis Verificados (verde)
- Por cada médico activo en el sistema

---

### 2. **Reporte de Facturación por Obra Social**

**Objetivo:** Visualizar la distribución de análisis según la obra social del paciente.

**Visualización:** Gráfico de torta (pie chart)

**Decisiones que ayuda a tomar:**
- ¿Qué obra social genera más trabajo?
- ¿Vale la pena renegociar contratos con obras sociales específicas?
- ¿Qué porcentaje de pacientes son privados?

**Datos mostrados:**
- Porcentaje de análisis por obra social
- Cantidad absoluta de análisis
- Pacientes sin obra social aparecen como "Privado"

---

### 3. **Reporte de Demanda (Top Análisis)**

**Objetivo:** Identificar los tipos de análisis más solicitados.

**Visualización:** Gráfico de barras horizontales (Top 10)

**Decisiones que ayuda a tomar:**
- ¿Qué reactivos debo tener siempre en stock?
- ¿En qué áreas necesito más personal especializado?
- ¿Qué equipamiento es más crítico mantener operativo?

**Datos mostrados:**
- Top 10 tipos de análisis más solicitados
- Cantidad de veces que fue solicitado cada tipo

---

## ?? Cómo Usar el Módulo

### Acceso al Módulo

1. Iniciar sesión como **Administrador**
2. Navegar a la pestaña **"Backups y Reportes"**
3. Hacer clic en el botón **"?? Abrir Módulo de Reportes"**

### Generación de Reportes

1. **Seleccionar rango de fechas:**
   - Configurar "Fecha Desde"
   - Configurar "Fecha Hasta"

2. **Generar el reporte deseado:**
   - Clic en **"?? Productividad Médicos"** (gráfico de barras)
   - Clic en **"?? Facturación por Obra Social"** (gráfico de torta)
   - Clic en **"?? Top Análisis Solicitados"** (gráfico de barras horizontales)

3. **Interpretar resultados:**
   - Los gráficos se actualizan automáticamente
   - El título muestra el período analizado
   - La leyenda identifica cada serie de datos

---

## ??? Arquitectura Técnica

### Patrón MVP de 3 Capas

```
???????????????????????????????????????????????????
?  VISTA (UI)                                     ?
?  - FrmReportesAdmin.cs                          ?
?  - Implementa IReportesAdminView                ?
?  - Controles DateTimePicker, Button, Chart      ?
???????????????????????????????????????????????????
                   ? Eventos
                   ?
???????????????????????????????????????????????????
?  PRESENTER                                      ?
?  - ReportesAdminPresenter.cs                    ?
?  - Orquesta la lógica de presentación           ?
?  - Valida fechas                                ?
???????????????????????????????????????????????????
                   ? Llama a
                   ?
???????????????????????????????????????????????????
?  LÓGICA DE NEGOCIO (BLL)                        ?
?  - ReportesService.cs                           ?
?  - IReportesService.cs                          ?
?  - Validaciones de negocio                      ?
???????????????????????????????????????????????????
                   ? Consulta
                   ?
???????????????????????????????????????????????????
?  ACCESO A DATOS (DAL)                           ?
?  - ReportesRepositorio.cs                       ?
?  - Consultas SQL con GROUP BY                   ?
?  - Uso de SqlParameter (seguridad)              ?
???????????????????????????????????????????????????
```

---

## ?? Archivos Creados

### Capa de Lógica de Negocio (BLL)
- `SALC/BLL/IReportesService.cs` - Interfaz del servicio
- `SALC/BLL/ReportesService.cs` - Implementación del servicio
- Clases de datos:
  - `ReporteProductividad`
  - `ReporteFacturacion`
  - `ReporteDemanda`
  - `ReporteAlerta` (para médicos - futuro)
  - `ReporteCargaTrabajo` (para médicos - futuro)

### Capa de Acceso a Datos (DAL)
- `SALC/DAL/ReportesRepositorio.cs` - Consultas SQL agregadas

### Capa de Presentación (Presenters)
- `SALC/Presenters/ViewsContracts/IReportesAdminView.cs` - Interfaz de la vista
- `SALC/Presenters/ReportesAdminPresenter.cs` - Presentador

### Capa de Vista (UI)
- `SALC/Views/PanelAdministrador/FrmReportesAdmin.cs` - Formulario con gráficos

---

## ?? Seguridad

- Todas las consultas SQL usan **SqlParameter** para prevenir inyección SQL
- Validación de rangos de fechas en el Presenter
- Solo usuarios con rol **Administrador** pueden acceder

---

## ?? Consultas SQL Ejecutadas

### Productividad de Médicos
```sql
SELECT 
    u.dni,
    u.nombre + ' ' + u.apellido AS NombreMedico,
    COUNT(a.id_analisis) AS TotalCreados,
    SUM(CASE WHEN a.id_estado = 2 THEN 1 ELSE 0 END) AS TotalVerificados
FROM usuarios u
INNER JOIN medicos m ON u.dni = m.dni
LEFT JOIN analisis a ON u.dni = a.dni_carga 
    AND a.fecha_creacion BETWEEN @desde AND @hasta
    AND a.id_estado != 3  -- Excluye anulados
WHERE u.id_rol = 2 AND u.estado = 'Activo'
GROUP BY u.dni, u.nombre, u.apellido
ORDER BY TotalVerificados DESC, TotalCreados DESC
```

### Facturación por Obra Social
```sql
SELECT 
    ISNULL(os.nombre, 'Privado') AS NombreObraSocial,
    COUNT(a.id_analisis) AS Cantidad,
    CAST(COUNT(a.id_analisis) * 100.0 / SUM(COUNT(a.id_analisis)) OVER() AS DECIMAL(5,2)) AS Porcentaje
FROM analisis a
INNER JOIN pacientes p ON a.dni_paciente = p.dni
LEFT JOIN obras_sociales os ON p.id_obra_social = os.id_obra_social
WHERE a.fecha_creacion BETWEEN @desde AND @hasta
    AND a.id_estado != 3  -- Excluye anulados
GROUP BY os.nombre
ORDER BY Cantidad DESC
```

### Top Análisis Solicitados
```sql
SELECT TOP (10)
    ta.descripcion AS TipoAnalisis,
    COUNT(a.id_analisis) AS Cantidad
FROM analisis a
INNER JOIN tipos_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
WHERE a.fecha_creacion BETWEEN @desde AND @hasta
    AND a.id_estado != 3  -- Excluye anulados
GROUP BY ta.descripcion
ORDER BY Cantidad DESC
```

---

## ?? Personalización de Gráficos

Los gráficos utilizan `System.Windows.Forms.DataVisualization.Charting`:

- **Colores coherentes:** Azul para creados, Verde para verificados
- **Leyendas claras:** Posicionadas a la derecha
- **Etiquetas de datos:** Visible en los puntos importantes
- **Títulos dinámicos:** Muestran el período seleccionado

---

## ?? Futuras Mejoras

- Exportar reportes a PDF
- Exportar datos a Excel
- Reportes programados por email
- Dashboard con múltiples KPIs
- Filtros adicionales (por médico específico, por obra social, etc.)
- Comparación de períodos (mes vs mes anterior)

---

## ?? Soporte

Para dudas sobre el uso o funcionamiento del módulo de reportes, consultar la documentación técnica en `/SALC/Docs/ERS-SALC_IEEEv2.9.md`

---

**Versión:** 1.0  
**Fecha:** 2024  
**Proyecto:** Sistema de Administración de Laboratorio Clínico (SALC)

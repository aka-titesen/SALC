# ?? Casos de Prueba: Módulo de Reportes Administrativos

## Objetivo
Validar que el módulo de reportes funcione correctamente en diferentes escenarios y con distintos volúmenes de datos.

---

## ?? Casos de Prueba Funcionales

### CP-001: Reporte de Productividad - Caso Normal

**Precondiciones:**
- Sistema con al menos 3 médicos activos
- Análisis creados y verificados en el último mes

**Pasos:**
1. Iniciar sesión como Administrador
2. Ir a "Backups y Reportes" ? "?? Abrir Módulo de Reportes"
3. Configurar:
   - Fecha Desde: Primer día del mes actual
   - Fecha Hasta: Día actual
4. Clic en "?? Productividad Médicos"

**Resultado Esperado:**
- ? Gráfico de barras con 2 series (Creados, Verificados)
- ? Cada médico activo aparece en el eje X
- ? Barras azules (creados) y verdes (verificados)
- ? Médicos ordenados por cantidad verificada (descendente)
- ? Título muestra el período correcto

---

### CP-002: Reporte de Productividad - Sin Datos

**Precondiciones:**
- Médicos activos en el sistema
- NO hay análisis en el rango de fechas seleccionado

**Pasos:**
1. Configurar un rango de fechas en el futuro (ej: próximo año)
2. Clic en "?? Productividad Médicos"

**Resultado Esperado:**
- ? Mensaje: "No hay datos disponibles para el período seleccionado."
- ? Gráfico no se actualiza (mantiene estado anterior)

---

### CP-003: Reporte de Productividad - Médico sin Análisis

**Precondiciones:**
- Al menos 1 médico activo SIN análisis creados

**Pasos:**
1. Generar reporte con rango amplio (últimos 6 meses)

**Resultado Esperado:**
- ? El médico aparece en el gráfico
- ? Ambas barras tienen valor 0
- ? No se produce error

---

### CP-004: Facturación por Obra Social - Con Privados

**Precondiciones:**
- Análisis de pacientes CON obra social
- Análisis de pacientes SIN obra social (privados)

**Pasos:**
1. Configurar rango de fechas con datos mixtos
2. Clic en "?? Facturación por Obra Social"

**Resultado Esperado:**
- ? Gráfico de torta con porciones de colores
- ? Existe una porción "Privado" para pacientes sin obra social
- ? Porcentajes suman 100%
- ? Etiquetas muestran porcentaje y cantidad

---

### CP-005: Facturación por Obra Social - Solo Privados

**Precondiciones:**
- Todos los pacientes del período NO tienen obra social

**Pasos:**
1. Filtrar por rango con solo pacientes privados
2. Generar reporte

**Resultado Esperado:**
- ? Gráfico de torta con 1 sola porción "Privado" = 100%
- ? No se produce error

---

### CP-006: Top Análisis - Más de 10 Tipos

**Precondiciones:**
- Existen más de 10 tipos de análisis diferentes solicitados

**Pasos:**
1. Configurar rango amplio
2. Clic en "?? Top Análisis Solicitados"

**Resultado Esperado:**
- ? Gráfico muestra exactamente 10 barras
- ? Ordenadas de mayor a menor cantidad
- ? Los tipos menos solicitados NO aparecen

---

### CP-007: Top Análisis - Menos de 10 Tipos

**Precondiciones:**
- Solo existen 5 tipos de análisis solicitados

**Pasos:**
1. Generar reporte

**Resultado Esperado:**
- ? Gráfico muestra las 5 barras disponibles
- ? No hay barras vacías o en 0

---

### CP-008: Validación de Fechas - Desde > Hasta

**Precondiciones:**
- Sistema funcionando normalmente

**Pasos:**
1. Configurar:
   - Fecha Desde: 31/12/2024
   - Fecha Hasta: 01/01/2024
2. Clic en cualquier botón de reporte

**Resultado Esperado:**
- ? Mensaje de error: "La fecha 'Desde' no puede ser mayor a la fecha 'Hasta'"
- ? Reporte NO se genera

---

### CP-009: Rendimiento - Volumen Alto

**Precondiciones:**
- Base de datos con > 10,000 análisis

**Pasos:**
1. Configurar rango de 1 año
2. Generar los 3 reportes consecutivamente

**Resultado Esperado:**
- ? Cada reporte tarda < 5 segundos
- ? Gráficos se renderizan correctamente
- ? No hay errores de memoria

---

### CP-010: Cambio de Reporte - Actualización Dinámica

**Precondiciones:**
- Sistema con datos

**Pasos:**
1. Generar "Productividad Médicos" (gráfico de barras)
2. Sin cerrar la ventana, clic en "Facturación" (gráfico de torta)
3. Clic en "Top Análisis" (barras horizontales)

**Resultado Esperado:**
- ? Cada gráfico reemplaza al anterior
- ? Título se actualiza correctamente
- ? Leyendas corresponden al gráfico actual
- ? No quedan restos del gráfico anterior

---

## ??? Casos de Prueba de Seguridad

### CS-001: Inyección SQL - Parámetros Maliciosos

**Objetivo:** Verificar que no sea vulnerable a SQL Injection

**Pasos:**
1. Modificar manualmente el código para pasar valores maliciosos
2. Ejemplo: `FechaDesde = "2024-01-01'; DROP TABLE analisis; --"`

**Resultado Esperado:**
- ? Error de conversión de tipo (fecha inválida)
- ? NO se ejecuta el comando DROP
- ? Sistema sigue funcionando

**Nota:** Esto NO debería ser posible desde la UI normal.

---

### CS-002: Control de Acceso - Rol Médico

**Pasos:**
1. Iniciar sesión como Médico
2. Intentar acceder al panel de administrador

**Resultado Esperado:**
- ? El botón "?? Abrir Módulo de Reportes" NO está visible
- ? No se puede navegar al formulario

---

### CS-003: Control de Acceso - Rol Asistente

**Pasos:**
1. Iniciar sesión como Asistente
2. Intentar acceder al panel de administrador

**Resultado Esperado:**
- ? El panel de administrador NO es accesible
- ? El módulo de reportes NO está disponible

---

## ?? Casos de Prueba de Datos

### CD-001: Análisis Anulados - Exclusión

**Precondiciones:**
- Existen análisis con `id_estado = 3` (Anulados)

**Pasos:**
1. Contar análisis anulados en el rango: `SELECT COUNT(*) FROM analisis WHERE id_estado = 3`
2. Generar cualquier reporte

**Resultado Esperado:**
- ? Los análisis anulados NO aparecen en ningún reporte
- ? Los totales excluyen los anulados

---

### CD-002: Médicos Inactivos - Exclusión

**Precondiciones:**
- Existen médicos con `estado = 'Inactivo'` que tienen análisis históricos

**Pasos:**
1. Generar reporte de productividad

**Resultado Esperado:**
- ? Médicos inactivos NO aparecen en el gráfico
- ? Sus análisis históricos tampoco se cuentan

---

### CD-003: Precisión de Porcentajes

**Precondiciones:**
- 3 obras sociales con: 100, 50, 50 análisis respectivamente

**Pasos:**
1. Generar reporte de facturación

**Resultado Esperado:**
- ? Obra 1: 50.00%
- ? Obra 2: 25.00%
- ? Obra 3: 25.00%
- ? TOTAL = 100.00%

---

## ??? Casos de Prueba de UI/UX

### UI-001: Adaptación de Gráficos - Nombres Largos

**Precondiciones:**
- Médico con nombre muy largo (> 30 caracteres)

**Pasos:**
1. Generar reporte de productividad

**Resultado Esperado:**
- ? Etiquetas del eje X rotadas -45° para legibilidad
- ? No hay solapamiento de texto
- ? Todo el texto es visible

---

### UI-002: Leyenda - Visibilidad

**Pasos:**
1. Generar reporte con muchas series/categorías

**Resultado Esperado:**
- ? Leyenda posicionada a la derecha
- ? Todos los elementos son legibles
- ? No se superpone con el gráfico

---

### UI-003: Responsividad - Pantallas Pequeñas

**Pasos:**
1. Cambiar resolución a 1024x768
2. Abrir módulo de reportes

**Resultado Esperado:**
- ? Formulario es completamente visible
- ? Botones no se salen del área visible
- ? Scroll vertical disponible si es necesario

---

## ?? Casos de Prueba de Integración

### INT-001: Integración con Panel Administrador

**Pasos:**
1. Desde panel admin, abrir reportes
2. Cerrar formulario de reportes
3. Volver a abrir

**Resultado Esperado:**
- ? Formulario se abre/cierra correctamente
- ? Panel admin sigue funcionando
- ? Fechas se reinician al valor por defecto

---

### INT-002: Cambio de Usuario - Cierre de Sesión

**Pasos:**
1. Abrir módulo de reportes
2. Cerrar sesión desde otro panel
3. Intentar generar un reporte

**Resultado Esperado:**
- ? Se cierra toda la aplicación
- ? O se redirige al login

---

## ?? Checklist de Validación Final

- [ ] Todos los gráficos se visualizan correctamente
- [ ] Los porcentajes suman 100% en facturación
- [ ] No hay errores en la consola de Visual Studio
- [ ] Los filtros de fecha funcionan
- [ ] Los mensajes de error son claros
- [ ] Solo administradores pueden acceder
- [ ] Las consultas SQL usan parámetros
- [ ] Los análisis anulados están excluidos
- [ ] El rendimiento es aceptable (< 5 seg)
- [ ] La documentación está completa

---

## ?? Ejecución de Pruebas

### Preparación del Entorno
```sql
-- Ejecutar script de validación
USE SALC;
GO
-- Ver SALC/Docs/validacion-reportes.sql
```

### Datos de Prueba Recomendados
- Al menos 5 médicos activos
- Al menos 100 análisis en diferentes estados
- Al menos 3 obras sociales diferentes
- Al menos 5 tipos de análisis
- Rango de fechas: últimos 3 meses

---

**Documento:** Casos de Prueba del Módulo de Reportes  
**Versión:** 1.0  
**Estado:** Listo para Ejecución  
**Autor:** GitHub Copilot

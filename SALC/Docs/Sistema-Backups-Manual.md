# Sistema de Backups para SALC - Estructura Consolidada

## Descripción General

El sistema de backups de SALC proporciona una solución completa para realizar respaldos de la base de datos tanto de forma manual como automática, con un historial detallado y configuración flexible.

## Arquitectura Consolidada

### ?? **Estructura de Archivos**

```
SALC/
??? Domain/
?   ??? BackupEntities.cs          # Todas las entidades de backup
??? DAL/
?   ??? BackupRepositorio.cs       # Todas las operaciones de BD
??? BLL/
?   ??? BackupService.cs           # Servicio completo de backups
?   ??? IBackupService.cs          # Interfaz del servicio
??? Views/PanelAdministrador/Backups/
?   ??? FrmGestionBackups.cs       # Formulario principal
?   ??? FrmGestionBackups.Designer.cs
??? Docs/
    ??? script-backups.sql         # Script de BD
    ??? Sistema-Backups-Manual.md  # Esta documentación
```

### ?? **Separación de Responsabilidades**

#### **Domain (BackupEntities.cs)**
- `HistorialBackup`: Registro de backups realizados
- `ConfiguracionBackup`: Configuración del sistema
- `BackupResult`: Resultado de operaciones

#### **DAL (BackupRepositorio.cs)**
- **Historial**: `InsertarHistorial()`, `ObtenerHistorial()`, `ObtenerUltimoBackup()`
- **Configuración**: `ObtenerConfiguracion()`, `ActualizarConfiguracion()`
- **Limpieza**: `LimpiarHistorialAntiguo()`

#### **BLL (BackupService.cs)**
- **Ejecución**: `EjecutarBackup()`, `EjecutarBackupAutomatico()`
- **Gestión**: `ObtenerHistorialBackups()`, `LimpiarBackupsAntiguos()`
- **Configuración**: `ObtenerConfiguracion()`, `ActualizarConfiguracion()`
- **Programación**: `ProgramarBackupAutomatico()`, `EliminarTareaProgramada()`
- **Utilidades**: `FormatearTamanoArchivo()`

#### **Views (FrmGestionBackups.cs)**
- Formulario único con 3 pestañas
- Organizado en carpeta `Views/PanelAdministrador/Backups/`

## Características Principales

### ? **Funcionalidades Implementadas**

1. **Backup Manual**
   - Ejecución inmediata con interfaz gráfica
   - Selección de ubicación y nombre del archivo
   - Prueba de configuración

2. **Backup Automático**
   - Programación por días de la semana
   - Integración con Programador de Tareas de Windows
   - Ejecución silenciosa en segundo plano

3. **Historial Detallado**
   - Registro completo de todos los backups
   - Vista del último backup exitoso
   - Información de tamaño, fecha, estado

4. **Gestión Inteligente**
   - Limpieza automática de archivos antiguos
   - Configuración de retención personalizable
   - Validaciones completas

## Instalación

### 1. Ejecutar Script de Base de Datos

```sql
-- Ejecutar en SQL Server Management Studio
sqlcmd -S localhost -E -i "SALC\Docs\script-backups.sql"
```

### 2. Permisos Necesarios

- **SQL Server**: `db_backupoperator` o `sysadmin`
- **Sistema**: Administrador local (para tareas programadas)
- **Archivos**: Escritura en carpeta de destino

## Uso del Sistema

### Acceso
1. Iniciar sesión como **Administrador**
2. Panel de Administrador ? **"Ejecutar Backup"**

### Pestañas Disponibles

#### ?? **Backup Manual**
- Estado del último backup
- Ejecutar backup inmediato
- Probar configuración

#### ?? **Configuración**
- Habilitar backup automático
- Días y hora de ejecución
- Ruta de destino
- Política de retención

#### ?? **Historial**
- Lista completa de backups
- Códigos de color por estado
- Limpieza de registros antiguos

## Estructura Técnica

### Base de Datos

```sql
-- Tablas creadas
historial_backup (
    id, fecha_hora, ruta_archivo, tamano_archivo, 
    estado, observaciones, tipo_backup, dni_usuario
)

configuracion_backup (
    id, backup_automatico_habilitado, hora_programada, 
    dias_semana, ruta_destino, dias_retencion, 
    ultima_ejecucion, fecha_modificacion, dni_usuario_modificacion
)
```

### Clases Principales

- **`BackupEntities`**: Todas las entidades del dominio
- **`BackupRepositorio`**: Acceso unificado a datos
- **`BackupService`**: Servicio completo con toda la lógica
- **`FrmGestionBackups`**: Interfaz completa de usuario

## Ventajas de la Estructura Consolidada

### ? **Organización Mejorada**
- Un archivo por capa lógica
- Responsabilidades claramente definidas  
- Fácil mantenimiento y extensión

### ? **Menos Complejidad**
- Menos archivos para gestionar
- Dependencias simplificadas  
- Estructura más intuitiva

### ? **Mejor Rendimiento**
- Menos instanciación de objetos
- Reutilización de conexiones
- Operaciones más eficientes

### ? **Facilidad de Testing**
- Una sola interfaz por capa
- Mocking más sencillo  
- Tests más enfocados

## Programación Automática

### Funcionamiento
1. Configuración ? `ProgramarBackupAutomatico()`
2. Crea tarea: `SALC_BackupAutomatico`
3. Ejecuta: `SALC.exe /backup /auto`
4. Procesa sin UI y registra resultado

### Verificación
- Programador de Tareas ? `SALC_BackupAutomatico`
- Event Log ? Aplicaciones ? SALC

## Mantenimiento

### Tareas Recomendadas
1. **Verificar backups** semanalmente
2. **Probar restauración** mensualmente
3. **Revisar espacio** regularmente
4. **Actualizar retención** según necesidades

### Monitoreo
- Pestaña "Backup Manual" ? Estado último backup
- Pestaña "Historial" ? Patrones de error
- Carpeta destino ? Espacio usado

---

**Versión**: 2.0 (Consolidada)  
**Estructura**: 1 archivo por capa + organización en carpetas  
**Mantenibilidad**: Mejorada significativamente
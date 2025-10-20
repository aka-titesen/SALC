# Sistema de Backups para SALC - Estructura Consolidada

## Descripci�n General

El sistema de backups de SALC proporciona una soluci�n completa para realizar respaldos de la base de datos tanto de forma manual como autom�tica, con un historial detallado y configuraci�n flexible.

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
    ??? Sistema-Backups-Manual.md  # Esta documentaci�n
```

### ?? **Separaci�n de Responsabilidades**

#### **Domain (BackupEntities.cs)**
- `HistorialBackup`: Registro de backups realizados
- `ConfiguracionBackup`: Configuraci�n del sistema
- `BackupResult`: Resultado de operaciones

#### **DAL (BackupRepositorio.cs)**
- **Historial**: `InsertarHistorial()`, `ObtenerHistorial()`, `ObtenerUltimoBackup()`
- **Configuraci�n**: `ObtenerConfiguracion()`, `ActualizarConfiguracion()`
- **Limpieza**: `LimpiarHistorialAntiguo()`

#### **BLL (BackupService.cs)**
- **Ejecuci�n**: `EjecutarBackup()`, `EjecutarBackupAutomatico()`
- **Gesti�n**: `ObtenerHistorialBackups()`, `LimpiarBackupsAntiguos()`
- **Configuraci�n**: `ObtenerConfiguracion()`, `ActualizarConfiguracion()`
- **Programaci�n**: `ProgramarBackupAutomatico()`, `EliminarTareaProgramada()`
- **Utilidades**: `FormatearTamanoArchivo()`

#### **Views (FrmGestionBackups.cs)**
- Formulario �nico con 3 pesta�as
- Organizado en carpeta `Views/PanelAdministrador/Backups/`

## Caracter�sticas Principales

### ? **Funcionalidades Implementadas**

1. **Backup Manual**
   - Ejecuci�n inmediata con interfaz gr�fica
   - Selecci�n de ubicaci�n y nombre del archivo
   - Prueba de configuraci�n

2. **Backup Autom�tico**
   - Programaci�n por d�as de la semana
   - Integraci�n con Programador de Tareas de Windows
   - Ejecuci�n silenciosa en segundo plano

3. **Historial Detallado**
   - Registro completo de todos los backups
   - Vista del �ltimo backup exitoso
   - Informaci�n de tama�o, fecha, estado

4. **Gesti�n Inteligente**
   - Limpieza autom�tica de archivos antiguos
   - Configuraci�n de retenci�n personalizable
   - Validaciones completas

## Instalaci�n

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
1. Iniciar sesi�n como **Administrador**
2. Panel de Administrador ? **"Ejecutar Backup"**

### Pesta�as Disponibles

#### ?? **Backup Manual**
- Estado del �ltimo backup
- Ejecutar backup inmediato
- Probar configuraci�n

#### ?? **Configuraci�n**
- Habilitar backup autom�tico
- D�as y hora de ejecuci�n
- Ruta de destino
- Pol�tica de retenci�n

#### ?? **Historial**
- Lista completa de backups
- C�digos de color por estado
- Limpieza de registros antiguos

## Estructura T�cnica

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
- **`BackupService`**: Servicio completo con toda la l�gica
- **`FrmGestionBackups`**: Interfaz completa de usuario

## Ventajas de la Estructura Consolidada

### ? **Organizaci�n Mejorada**
- Un archivo por capa l�gica
- Responsabilidades claramente definidas  
- F�cil mantenimiento y extensi�n

### ? **Menos Complejidad**
- Menos archivos para gestionar
- Dependencias simplificadas  
- Estructura m�s intuitiva

### ? **Mejor Rendimiento**
- Menos instanciaci�n de objetos
- Reutilizaci�n de conexiones
- Operaciones m�s eficientes

### ? **Facilidad de Testing**
- Una sola interfaz por capa
- Mocking m�s sencillo  
- Tests m�s enfocados

## Programaci�n Autom�tica

### Funcionamiento
1. Configuraci�n ? `ProgramarBackupAutomatico()`
2. Crea tarea: `SALC_BackupAutomatico`
3. Ejecuta: `SALC.exe /backup /auto`
4. Procesa sin UI y registra resultado

### Verificaci�n
- Programador de Tareas ? `SALC_BackupAutomatico`
- Event Log ? Aplicaciones ? SALC

## Mantenimiento

### Tareas Recomendadas
1. **Verificar backups** semanalmente
2. **Probar restauraci�n** mensualmente
3. **Revisar espacio** regularmente
4. **Actualizar retenci�n** seg�n necesidades

### Monitoreo
- Pesta�a "Backup Manual" ? Estado �ltimo backup
- Pesta�a "Historial" ? Patrones de error
- Carpeta destino ? Espacio usado

---

**Versi�n**: 2.0 (Consolidada)  
**Estructura**: 1 archivo por capa + organizaci�n en carpetas  
**Mantenibilidad**: Mejorada significativamente
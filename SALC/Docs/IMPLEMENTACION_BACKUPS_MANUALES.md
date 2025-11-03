# Implementación de Copias de Seguridad Manuales - SALC

## Resumen

Se ha implementado un sistema de copias de seguridad **manual** para la base de datos del sistema SALC, siguiendo el patrón arquitectónico **MVP (Model-View-Presenter)** de 3 capas.

## Características Principales

### ? Funcionalidad Implementada

1. **Ejecución Manual de Backups**
   - El administrador puede ejecutar copias de seguridad cuando lo necesite
   - Selección libre de ubicación y nombre del archivo .bak
   - Feedback visual del progreso con cursor de espera
   - Confirmación detallada al finalizar (ubicación, tamaño, fecha)

2. **Historial de Backups**
   - Registro completo de todas las copias ejecutadas
   - Almacena: fecha/hora, ruta, tamaño, estado, observaciones y usuario ejecutor
   - Consulta del último backup exitoso
   - Capacidad de limpieza de registros antiguos

3. **Gestión de Archivos**
   - Creación automática de directorios si no existen
   - Validación de permisos y espacio en disco
   - Limpieza de archivos físicos antiguos

4. **Interfaz de Usuario**
   - Pestaña dedicada "Backups" en el Panel de Administrador
   - Botón "Ejecutar Copia de Seguridad Ahora" con descripción clara
   - Botón "Probar Conexión a Base de Datos" para diagnóstico
   - Información contextual para el usuario

### ? Funcionalidad Eliminada

- **Backups automáticos programados** (eliminado completamente)
- **Configuración de horarios y frecuencias**
- **Integración con Programador de Tareas de Windows**
- **Tabla de configuración en base de datos**

## Arquitectura

### Patrón MVP de 3 Capas

```
???????????????????????????????????????????????????????????????
?                    CAPA DE PRESENTACIÓN                      ?
?  ?????????????????????????????????????????????????????????? ?
?  ? FrmPanelAdministrador.cs (View)                        ? ?
?  ? - Interfaz gráfica con controles de usuario            ? ?
?  ? - Dispara eventos cuando el usuario hace clic          ? ?
?  ?????????????????????????????????????????????????????????? ?
?                            ?                                 ?
?  ?????????????????????????????????????????????????????????? ?
?  ? PanelAdministradorPresenter.cs (Presenter)             ? ?
?  ? - Lógica de presentación                               ? ?
?  ? - Maneja eventos de la vista                           ? ?
?  ? - Actualiza la vista con datos formateados             ? ?
?  ?????????????????????????????????????????????????????????? ?
???????????????????????????????????????????????????????????????
                            ?
???????????????????????????????????????????????????????????????
?                 CAPA DE LÓGICA DE NEGOCIO                    ?
?  ?????????????????????????????????????????????????????????? ?
?  ? IBackupService.cs (Interfaz)                           ? ?
?  ? BackupService.cs (Servicio)                            ? ?
?  ? - Validaciones de negocio                              ? ?
?  ? - Orquestación de operaciones                          ? ?
?  ? - Manejo de errores de alto nivel                      ? ?
?  ?????????????????????????????????????????????????????????? ?
???????????????????????????????????????????????????????????????
                            ?
???????????????????????????????????????????????????????????????
?                   CAPA DE ACCESO A DATOS                     ?
?  ?????????????????????????????????????????????????????????? ?
?  ? BackupRepositorio.cs (Repositorio)                     ? ?
?  ? - Comandos SQL parametrizados                          ? ?
?  ? - Interacción directa con SQL Server                   ? ?
?  ? - CRUD del historial de backups                        ? ?
?  ?????????????????????????????????????????????????????????? ?
???????????????????????????????????????????????????????????????
                            ?
???????????????????????????????????????????????????????????????
?                         BASE DE DATOS                        ?
?                     SQL Server 2022                          ?
?                   Tabla: historial_backup                    ?
???????????????????????????????????????????????????????????????
```

## Estructura de Archivos

### Dominio (Domain Layer)
- **`SALC\Domain\Backup.cs`**
  - `HistorialBackup`: Entidad que representa un registro de backup
  - `BackupResult`: DTO para resultados de operaciones

### Capa de Lógica de Negocio (BLL)
- **`SALC\BLL\IBackupService.cs`**
  - Contrato de la interfaz del servicio
  - Define operaciones: `EjecutarBackupManual`, `ObtenerHistorialBackups`, etc.

- **`SALC\BLL\BackupService.cs`**
  - Implementación del servicio
  - Validaciones de negocio
  - Ejecución del comando `BACKUP DATABASE` de SQL Server
  - Gestión del historial y archivos

### Capa de Acceso a Datos (DAL)
- **`SALC\DAL\BackupRepositorio.cs`**
  - CRUD para tabla `historial_backup`
  - Consultas con SqlParameter para prevenir SQL Injection
  - Métodos: `InsertarHistorial`, `ObtenerHistorial`, `ObtenerUltimoBackup`, etc.

### Capa de Presentación (UI)
- **`SALC\Presenters\PanelAdministradorPresenter.cs`**
  - Método `OnEjecutarBackup()`: Maneja el evento del botón
  - Muestra diálogos, gestiona cursores, formatea mensajes

- **`SALC\Presenters\ViewsContracts\IPanelAdministradorView.cs`**
  - Contrato de la vista con eventos
  - `event EventHandler EjecutarBackupClick`

- **`SALC\Views\PanelAdministrador\FrmPanelAdministrador.cs`**
  - Implementación de la interfaz gráfica
  - Pestaña "Backups" con controles y eventos

### Scripts de Base de Datos
- **`SALC\Docs\Migraciones y Lotes\v2.0\20251021_MIGRATION_SIMPLIFY_BACKUP_MANUAL_ONLY.sql`**
  - Script de migración para crear/actualizar la tabla `historial_backup`
  - Elimina tablas antiguas de configuración automática
  - Crea índice para optimizar consultas por fecha

## Flujo de Ejecución

### Caso de Uso: Administrador Ejecuta Backup Manual

1. **Usuario hace clic en "Ejecutar Copia de Seguridad Ahora"**
   ```
   FrmPanelAdministrador ? Dispara evento EjecutarBackupClick
   ```

2. **Presenter captura el evento**
   ```csharp
   PanelAdministradorPresenter.OnEjecutarBackup()
   - Muestra SaveFileDialog para seleccionar ubicación
   - Cambia cursor a WaitCursor
   ```

3. **Servicio ejecuta la lógica de negocio**
   ```csharp
   BackupService.EjecutarBackupManual(ruta, dniUsuario)
   - Valida parámetros (ruta, DNI)
   - Crea directorio si no existe
   - Obtiene nombre de BD del connection string
   ```

4. **Repositorio ejecuta el comando SQL**
   ```sql
   BACKUP DATABASE [SALC] TO DISK = @ruta WITH INIT, STATS = 5
   ```

5. **Se registra en el historial**
   ```csharp
   BackupRepositorio.InsertarHistorial(historial)
   - INSERT INTO historial_backup (...)
   ```

6. **Se muestra confirmación al usuario**
   ```
   MessageBox con ubicación, tamaño y fecha del backup
   ```

## Base de Datos

### Tabla: historial_backup

```sql
CREATE TABLE historial_backup (
    id             INT IDENTITY(1,1) PRIMARY KEY,
    fecha_hora     DATETIME2 NOT NULL DEFAULT GETDATE(),
    ruta_archivo   NVARCHAR(500) NULL,
    tamano_archivo BIGINT NOT NULL DEFAULT 0,
    estado         NVARCHAR(20) NOT NULL, -- 'Exitoso' o 'Error'
    observaciones  NVARCHAR(MAX) NULL,
    dni_usuario    INT NOT NULL,
    
    CONSTRAINT FK_historial_backup_usuario 
        FOREIGN KEY (dni_usuario) REFERENCES usuarios(dni),
    CONSTRAINT CHK_historial_backup_estado 
        CHECK (estado IN ('Exitoso', 'Error'))
);

CREATE INDEX IX_historial_backup_fecha_hora 
    ON historial_backup(fecha_hora DESC);
```

### Campos Principales

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id` | INT IDENTITY | Identificador único autoincrementable |
| `fecha_hora` | DATETIME2 | Fecha y hora de ejecución del backup |
| `ruta_archivo` | NVARCHAR(500) | Ruta completa del archivo .bak generado |
| `tamano_archivo` | BIGINT | Tamaño en bytes del archivo de backup |
| `estado` | NVARCHAR(20) | 'Exitoso' o 'Error' |
| `observaciones` | NVARCHAR(MAX) | Detalles adicionales o mensajes de error |
| `dni_usuario` | INT | DNI del administrador que ejecutó el backup |

## Mejores Prácticas Aplicadas

### 1. **Separación de Responsabilidades (SOLID - S)**
- Vista: Solo renderiza y captura eventos
- Presenter: Solo lógica de presentación
- Service: Solo lógica de negocio
- Repository: Solo acceso a datos

### 2. **Prevención de SQL Injection**
```csharp
comando.Parameters.AddWithValue("@ruta", rutaArchivoBak);
```
Todas las consultas usan `SqlParameter`

### 3. **Manejo Robusto de Errores**
- Try-catch en cada capa
- Registro en historial incluso si falla
- Mensajes descriptivos al usuario
- Finally para garantizar limpieza de recursos

### 4. **Validaciones en Capas**
- **UI**: SaveFileDialog valida formato .bak
- **Service**: Valida parámetros, permisos, espacio
- **DAL**: Constraints de BD (CHECK, FK)

### 5. **Usabilidad**
- Cursor de espera durante operación larga
- Mensajes informativos con iconos apropiados
- Valores predeterminados inteligentes (nombre con timestamp)
- Ubicación inicial en Documentos del usuario

### 6. **Rendimiento**
- `CommandTimeout = 0` para backups grandes
- Índice en `fecha_hora` para consultas rápidas
- Limpieza de archivos antiguos en background

## Instalación y Configuración

### Requisitos Previos
- SQL Server 2022 (o compatible)
- .NET Framework 4.7.2
- Permisos de escritura en disco para backups
- Usuario de BD con permisos `BACKUP DATABASE`

### Pasos de Instalación

1. **Ejecutar Script de Migración**
   ```sql
   -- En SQL Server Management Studio o similar
   -- Abrir y ejecutar:
   SALC\Docs\Migraciones y Lotes\v2.0\20251021_MIGRATION_SIMPLIFY_BACKUP_MANUAL_ONLY.sql
   ```

2. **Compilar el Proyecto**
   ```bash
   cd C:\Users\Facundo\source\repos\SALC
   msbuild SALC.sln /t:Build /p:Configuration=Release
   ```

3. **Verificar Connection String**
   - Abrir `App.config`
   - Verificar que `connectionStrings` apunte a la BD correcta

4. **Ejecutar como Administrador**
   - El usuario debe tener rol "Administrador" en el sistema

## Uso del Sistema

### Para Administradores

1. **Iniciar sesión** en SALC
2. Ir al **Panel de Administrador**
3. Seleccionar pestaña **"Backups"**
4. Hacer clic en **"Ejecutar Copia de Seguridad Ahora"**
5. **La ubicación predeterminada ya tiene permisos** - se consulta automáticamente de SQL Server
6. Hacer clic en "Guardar" (o cambiar ubicación si lo desea)
7. Esperar confirmación (puede tardar varios minutos en BDs grandes)
8. Verificar que el archivo .bak se creó correctamente

### ?? Ubicación Automática

El sistema **consulta automáticamente a SQL Server** su carpeta predeterminada de backups, que típicamente es:

```
C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\Backup\SALC\
```

Esta ubicación **siempre tiene los permisos necesarios** porque es la carpeta configurada por SQL Server durante la instalación.

### Recomendaciones

- **Frecuencia**: Ejecutar backups diariamente o según política del laboratorio
- **Ubicación**: La predeterminada ya funciona - no necesita cambiarla
- **Nomenclatura**: El sistema genera automáticamente nombres con fecha y hora
- **Verificación**: Probar restauración periódicamente
- **Espacio**: Verificar espacio en disco disponible

## Solución de Problemas

### ? No hay problemas de permisos

El sistema ahora **consulta automáticamente** la ruta de backups configurada en SQL Server mediante:

```sql
EXEC master.dbo.xp_instance_regread 
    N'HKEY_LOCAL_MACHINE',
    N'Software\Microsoft\MSSQLServer\MSSQLServer',
    N'BackupDirectory'
```

Esta ruta **siempre tiene los permisos correctos** porque es configurada por SQL Server.

### Si aún así hay errores

1. **Use la ubicación predeterminada**: No cambie la ruta en el diálogo de guardar
2. **Verifique espacio en disco**: Asegúrese de tener suficiente espacio
3. **Verifique que SQL Server esté en ejecución**: Use el botón "Probar Conexión"

### Error: "No se pudo conectar a la base de datos"

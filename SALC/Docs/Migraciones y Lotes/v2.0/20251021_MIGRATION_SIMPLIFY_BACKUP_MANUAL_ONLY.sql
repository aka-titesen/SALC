----------------------------------------------------------------------
-- SALC Database Script - Simplificación de Backups
-- Fecha: 2025-01-21
-- Descripción:
--   Elimina la funcionalidad de backups automáticos.
--   Solo mantiene el historial de backups manuales ejecutados por administradores.
----------------------------------------------------------------------

USE [SALC];
GO

-- 1. ELIMINAR tablas existentes de backups si existen
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'backup_historial') AND type in (N'U'))
    DROP TABLE backup_historial;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'backup_configuracion') AND type in (N'U'))
    DROP TABLE backup_configuracion;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'historial_backup') AND type in (N'U'))
    DROP TABLE historial_backup;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'configuracion_backup') AND type in (N'U'))
    DROP TABLE configuracion_backup;
GO

-- 2. CREAR tabla simplificada para historial de backups MANUALES
CREATE TABLE historial_backup (
    id             INT IDENTITY(1,1) CONSTRAINT PK_historial_backup PRIMARY KEY,
    fecha_hora     DATETIME2 NOT NULL DEFAULT GETDATE(),
    ruta_archivo   NVARCHAR(500) NULL,
    tamano_archivo BIGINT NOT NULL DEFAULT 0,
    estado         NVARCHAR(20) NOT NULL, -- 'Exitoso' o 'Error'
    observaciones  NVARCHAR(MAX) NULL,
    dni_usuario    INT NOT NULL,          -- Administrador que ejecutó el backup manual

    CONSTRAINT FK_historial_backup_usuario FOREIGN KEY (dni_usuario) REFERENCES usuarios(dni),
    CONSTRAINT CHK_historial_backup_estado CHECK (estado IN ('Exitoso', 'Error'))
);
GO

-- 3. CREAR índice para mejorar consultas por fecha
CREATE INDEX IX_historial_backup_fecha_hora ON historial_backup(fecha_hora DESC);
GO

-- 4. COMENTARIOS sobre la tabla
EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'Historial de copias de seguridad manuales ejecutadas por administradores del sistema', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE',  @level1name=N'historial_backup';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'DNI del administrador que ejecutó la copia de seguridad manual', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE',  @level1name=N'historial_backup',
    @level2type=N'COLUMN', @level2name=N'dni_usuario';
GO

PRINT 'Migración completada: Sistema de backups configurado solo para operación MANUAL';
GO

-- Script para agregar tablas de gestión de backups
-- Versión: 1.0
-- Fecha: $(date)

USE [SALC];
GO

-- Tabla para el historial de backups realizados
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'historial_backup')
BEGIN
    CREATE TABLE historial_backup (
        id                  INT IDENTITY(1,1) CONSTRAINT PK_historial_backup PRIMARY KEY,
        fecha_hora          DATETIME2 NOT NULL DEFAULT GETDATE(),
        ruta_archivo        NVARCHAR(500) NULL,
        tamano_archivo      BIGINT NOT NULL DEFAULT 0,
        estado              NVARCHAR(20) NOT NULL, -- 'Exitoso', 'Error'
        observaciones       NVARCHAR(MAX) NULL,
        tipo_backup         NVARCHAR(20) NOT NULL, -- 'Manual', 'Automatico'
        dni_usuario         INT NULL, -- Usuario que ejecutó el backup manual
        
        CONSTRAINT CHK_historial_backup_estado CHECK (estado IN ('Exitoso', 'Error')),
        CONSTRAINT CHK_historial_backup_tipo CHECK (tipo_backup IN ('Manual', 'Automatico')),
        CONSTRAINT FK_historial_backup_usuario FOREIGN KEY (dni_usuario) REFERENCES usuarios(dni)
    );
    
    -- Índice para optimizar consultas por fecha
    CREATE INDEX IX_historial_backup_fecha_hora ON historial_backup(fecha_hora DESC);
    
    PRINT 'Tabla historial_backup creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla historial_backup ya existe.';
END
GO

-- Tabla para la configuración de backups automáticos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'configuracion_backup')
BEGIN
    CREATE TABLE configuracion_backup (
        id                          INT NOT NULL CONSTRAINT PK_configuracion_backup PRIMARY KEY,
        backup_automatico_habilitado BIT NOT NULL DEFAULT 0,
        hora_programada             NVARCHAR(5) NULL, -- Formato HH:mm
        dias_semana                 NVARCHAR(20) NULL, -- Días separados por coma: "1,2,3,4,5"
        ruta_destino                NVARCHAR(500) NULL,
        dias_retencion              INT NOT NULL DEFAULT 30,
        ultima_ejecucion            DATETIME2 NULL,
        fecha_modificacion          DATETIME2 NOT NULL DEFAULT GETDATE(),
        dni_usuario_modificacion    INT NOT NULL,
        
        CONSTRAINT CHK_configuracion_backup_dias_retencion CHECK (dias_retencion > 0),
        CONSTRAINT FK_configuracion_backup_usuario FOREIGN KEY (dni_usuario_modificacion) REFERENCES usuarios(dni)
    );
    
    -- Insertar configuración por defecto
    INSERT INTO configuracion_backup (
        id, backup_automatico_habilitado, hora_programada, dias_semana, 
        ruta_destino, dias_retencion, fecha_modificacion, dni_usuario_modificacion
    ) VALUES (
        1, 0, '02:00', '1,2,3,4,5', 
        'C:\Backups\SALC', 30, GETDATE(), 
        (SELECT TOP 1 dni FROM usuarios WHERE id_rol = 1) -- Primer administrador
    );
    
    PRINT 'Tabla configuracion_backup creada exitosamente con configuración por defecto.';
END
ELSE
BEGIN
    PRINT 'La tabla configuracion_backup ya existe.';
END
GO

PRINT 'Script de backups completado exitosamente.';
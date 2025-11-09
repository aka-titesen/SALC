USE [SALC];
GO

-- Tabla para la CONFIGURACIÓN de los backups. Solo tendrá una fila.
CREATE TABLE backup_configuracion (
    id                                INT CONSTRAINT PK_backup_configuracion PRIMARY KEY DEFAULT 1,
    es_automatico_habilitado          BIT NOT NULL DEFAULT 0,
    hora_programada                   TIME NOT NULL DEFAULT '02:00:00',
    dias_semana_programado            NVARCHAR(15) NOT NULL DEFAULT '1,2,3,4,5', -- Lunes=1, Martes=2, etc.
    ruta_destino                      NVARCHAR(500) NOT NULL,
    dias_retencion                    INT NOT NULL DEFAULT 7,
    
    CONSTRAINT CHK_backup_configuracion_id CHECK (id = 1) -- Asegura que solo haya una fila
);
GO

-- Tabla para el HISTORIAL de cada backup realizado.
CREATE TABLE backup_historial (
    id             INT IDENTITY(1,1) CONSTRAINT PK_backup_historial PRIMARY KEY,
    fecha_hora     DATETIME2 NOT NULL DEFAULT GETDATE(),
    ruta_archivo   NVARCHAR(500) NOT NULL,
    tamano_bytes   BIGINT NOT NULL,
    tipo           NVARCHAR(20) NOT NULL, -- 'Manual' o 'Automatico'
    estado         NVARCHAR(20) NOT NULL, -- 'Exitoso' o 'Error'
    mensaje_error  NVARCHAR(MAX) NULL,    -- Solo se llena si el estado es 'Error'
    dni_usuario    INT NULL,              -- Para saber qué admin lo hizo manualmente

    CONSTRAINT FK_backup_historial_usuario FOREIGN KEY (dni_usuario) REFERENCES usuarios(dni),
    CONSTRAINT CHK_backup_historial_tipo CHECK (tipo IN ('Manual', 'Automatico')),
    CONSTRAINT CHK_backup_historial_estado CHECK (estado IN ('Exitoso', 'Error'))
);
GO

-- Insertamos la fila de configuración inicial.
INSERT INTO backup_configuracion (ruta_destino) VALUES ('C:\Backups\SALC');
GO
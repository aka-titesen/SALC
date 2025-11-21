-- 0. CREACIÓN DE BASE DE DATOS
CREATE DATABASE [SALC];
GO

USE [SALC];
GO

PRINT '--- 0. Base de datos [SALC] creada y seleccionada ---';
GO

-- 1. CREACIÓN DE TABLAS
-- 1.1 ROLES
CREATE TABLE roles (
    id_rol     INT IDENTITY(1,1) CONSTRAINT PK_roles PRIMARY KEY,
    nombre_rol NVARCHAR(50) NOT NULL CONSTRAINT UQ_roles_nombre UNIQUE
);

-- 1.2 ESTADOS_ANALISIS
CREATE TABLE estados_analisis (
    id_estado   INT IDENTITY(1,1) CONSTRAINT PK_estados_analisis PRIMARY KEY,
    descripcion NVARCHAR(50) NOT NULL CONSTRAINT UQ_estados_analisis_descripcion UNIQUE
);

-- 1.3 OBRA_SOCIAL
CREATE TABLE obras_sociales (
    id_obra_social INT IDENTITY(1,1) CONSTRAINT PK_obras_sociales PRIMARY KEY,
    cuit           NVARCHAR(13) NOT NULL CONSTRAINT UQ_obras_sociales_cuit UNIQUE,
    nombre         NVARCHAR(50) NOT NULL,
    estado         NVARCHAR(20) NOT NULL DEFAULT 'Activo',
    CONSTRAINT CHK_obras_sociales_cuit CHECK (LEN(cuit) BETWEEN 10 AND 13),
    CONSTRAINT CHK_obras_sociales_estado CHECK (estado IN ('Activo', 'Inactivo'))
);

-- 1.4 TIPO_ANALISIS
CREATE TABLE tipos_analisis (
    id_tipo_analisis INT IDENTITY(1,1) CONSTRAINT PK_tipos_analisis PRIMARY KEY,
    descripcion      NVARCHAR(100) NOT NULL,
    estado           NVARCHAR(20) NOT NULL DEFAULT 'Activo',
    CONSTRAINT CHK_tipos_analisis_estado CHECK (estado IN ('Activo', 'Inactivo'))
);

-- 1.5 METRICAS
CREATE TABLE metricas (
    id_metrica    INT IDENTITY(1,1) CONSTRAINT PK_metricas PRIMARY KEY,
    nombre        NVARCHAR(100) NOT NULL,
    unidad_medida NVARCHAR(20) NOT NULL,
    valor_maximo  DECIMAL(10, 2) NULL,
    valor_minimo  DECIMAL(10, 2) NULL,
    estado        NVARCHAR(20) NOT NULL DEFAULT 'Activo',
    CONSTRAINT CHK_metricas_valores CHECK (valor_minimo <= valor_maximo),
    CONSTRAINT CHK_metricas_estado CHECK (estado IN ('Activo', 'Inactivo'))
);

-- 1.6 TIPO_ANALISIS_METRICA (Tabla de enlace)
CREATE TABLE tipo_analisis_metrica (
    id_tipo_analisis INT NOT NULL,
    id_metrica       INT NOT NULL,
    CONSTRAINT PK_tipo_analisis_metrica PRIMARY KEY (id_tipo_analisis, id_metrica),
    CONSTRAINT FK_tam_tipo_analisis FOREIGN KEY (id_tipo_analisis) REFERENCES tipos_analisis(id_tipo_analisis) ON DELETE CASCADE,
    CONSTRAINT FK_tam_metricas FOREIGN KEY (id_metrica) REFERENCES metricas(id_metrica) ON DELETE CASCADE
);

-- 1.7 USUARIOS (Tabla Principal)
CREATE TABLE usuarios (
    dni             INT CONSTRAINT PK_usuarios PRIMARY KEY,
    nombre          NVARCHAR(50) NOT NULL,
    apellido        NVARCHAR(50) NOT NULL,
    email           NVARCHAR(100) NOT NULL CONSTRAINT UQ_usuarios_email UNIQUE,
    password_hash   NVARCHAR(255) NOT NULL,
    id_rol          INT NOT NULL,
    estado          NVARCHAR(20) NOT NULL,

    CONSTRAINT FK_usuarios_roles FOREIGN KEY (id_rol) REFERENCES roles(id_rol),
    CONSTRAINT CHK_usuarios_dni CHECK (dni > 0),
    CONSTRAINT CHK_usuarios_estado CHECK (estado IN ('Activo', 'Inactivo')),
    CONSTRAINT CHK_usuarios_email CHECK (email LIKE '%_@__%.__%')
);

-- 1.8 MEDICOS (Extensión)
CREATE TABLE medicos (
    dni           INT CONSTRAINT PK_medicos PRIMARY KEY,
    nro_matricula INT NOT NULL CONSTRAINT UQ_medicos_matricula UNIQUE,
    especialidad  NVARCHAR(50) NOT NULL,
    
    CONSTRAINT FK_medicos_usuarios FOREIGN KEY (dni) REFERENCES usuarios(dni) ON DELETE CASCADE
);

-- 1.9 ASISTENTES (Extensión)
CREATE TABLE asistentes (
    dni             INT CONSTRAINT PK_asistentes PRIMARY KEY,
    dni_supervisor  INT NOT NULL, 
    fecha_ingreso   DATE NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_asistentes_usuarios FOREIGN KEY (dni) REFERENCES usuarios(dni) ON DELETE CASCADE,
    CONSTRAINT FK_asistentes_supervisor FOREIGN KEY (dni_supervisor) REFERENCES medicos(dni)
);

-- 1.10 PACIENTES
CREATE TABLE pacientes (
    dni             INT CONSTRAINT PK_pacientes PRIMARY KEY,
    nombre          NVARCHAR(50) NOT NULL,
    apellido        NVARCHAR(50) NOT NULL,
    fecha_nac       DATE NOT NULL,
    sexo            CHAR(1) NOT NULL,
    email           NVARCHAR(100) NULL,
    telefono        NVARCHAR(20) NULL,
    id_obra_social  INT NULL,
    estado          NVARCHAR(20) NOT NULL DEFAULT 'Activo',

    CONSTRAINT FK_pacientes_obras_sociales FOREIGN KEY (id_obra_social) REFERENCES obras_sociales(id_obra_social),
    CONSTRAINT CHK_pacientes_sexo CHECK (sexo IN ('M', 'F', 'X')),
    CONSTRAINT CHK_pacientes_fecha_nac CHECK (fecha_nac <= GETDATE()),
    CONSTRAINT CHK_pacientes_estado CHECK (estado IN ('Activo', 'Inactivo'))
);

-- 1.11 ANALISIS
CREATE TABLE analisis (
    id_analisis      INT IDENTITY(1,1) CONSTRAINT PK_analisis PRIMARY KEY,
    id_tipo_analisis INT NOT NULL,
    id_estado        INT NOT NULL,
    dni_paciente     INT NOT NULL,
    dni_carga        INT NOT NULL,
    dni_firma        INT NULL,
    fecha_creacion   DATETIME2 NOT NULL DEFAULT GETDATE(),
    fecha_firma      DATETIME2 NULL,
    observaciones    NVARCHAR(MAX) NULL,

    CONSTRAINT FK_analisis_tipos_analisis FOREIGN KEY (id_tipo_analisis) REFERENCES tipos_analisis(id_tipo_analisis),
    CONSTRAINT FK_analisis_estados_analisis FOREIGN KEY (id_estado) REFERENCES estados_analisis(id_estado),
    CONSTRAINT FK_analisis_pacientes FOREIGN KEY (dni_paciente) REFERENCES pacientes(dni),
    CONSTRAINT FK_analisis_medico_carga FOREIGN KEY (dni_carga) REFERENCES medicos(dni),
    CONSTRAINT FK_analisis_medico_firma FOREIGN KEY (dni_firma) REFERENCES medicos(dni)
);

-- 1.12 ANALISIS_METRICA
CREATE TABLE analisis_metrica (
    id_analisis   INT NOT NULL,
    id_metrica    INT NOT NULL,
    resultado     DECIMAL(10, 2) NOT NULL,
    observaciones NVARCHAR(500) NULL,
    CONSTRAINT PK_analisis_metrica PRIMARY KEY (id_analisis, id_metrica),
    CONSTRAINT FK_analisis_metrica_analisis FOREIGN KEY (id_analisis) REFERENCES analisis(id_analisis) ON DELETE CASCADE,
    CONSTRAINT FK_analisis_metrica_metricas FOREIGN KEY (id_metrica) REFERENCES metricas(id_metrica)
);
GO

-- 1.13 TABLA DE BACKUP SIMPLIFICADA
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

PRINT '--- 1. Todas las tablas creadas ---';
GO

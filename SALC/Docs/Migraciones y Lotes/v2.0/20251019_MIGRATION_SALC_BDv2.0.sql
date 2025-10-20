----------------------------------------------------------------------
-- SALC Database Script - v2.0
-- Cambios:
-- 1. Se añade la tabla 'tipo_analisis_metrica' para crear una
--    relación N:N entre tipos de análisis y las métricas que los
--    componen.
----------------------------------------------------------------------

-----------------------------------------------------------
-- 1. CREACIÓN DE BASE DE DATOS
-----------------------------------------------------------
-- CREATE DATABASE [SALC];
-- GO
-- USE [SALC];
-- GO

-----------------------------------------------------------
-- 2. TABLAS DE CATÁLOGO (LOOKUP TABLES)
-----------------------------------------------------------

-- 2.1 ROLES
CREATE TABLE roles (
    id_rol     INT IDENTITY(1,1) CONSTRAINT PK_roles PRIMARY KEY,
    nombre_rol NVARCHAR(50) NOT NULL CONSTRAINT UQ_roles_nombre UNIQUE
);

-- 2.2 ESTADOS_ANALISIS
CREATE TABLE estados_analisis (
    id_estado   INT IDENTITY(1,1) CONSTRAINT PK_estados_analisis PRIMARY KEY,
    descripcion NVARCHAR(50) NOT NULL CONSTRAINT UQ_estados_analisis_descripcion UNIQUE
);

-- 2.3 OBRA_SOCIAL
CREATE TABLE obras_sociales (
    id_obra_social INT IDENTITY(1,1) CONSTRAINT PK_obras_sociales PRIMARY KEY,
    cuit           NVARCHAR(13) NOT NULL CONSTRAINT UQ_obras_sociales_cuit UNIQUE,
    nombre         NVARCHAR(50) NOT NULL,
    estado         NVARCHAR(20) NOT NULL DEFAULT 'Activo',
    CONSTRAINT CHK_obras_sociales_cuit CHECK (LEN(cuit) BETWEEN 10 AND 13),
    CONSTRAINT CHK_obras_sociales_estado CHECK (estado IN ('Activo', 'Inactivo'))
);

-- 2.4 TIPO_ANALISIS
CREATE TABLE tipos_analisis (
    id_tipo_analisis INT IDENTITY(1,1) CONSTRAINT PK_tipos_analisis PRIMARY KEY,
    descripcion      NVARCHAR(100) NOT NULL,
    estado           NVARCHAR(20) NOT NULL DEFAULT 'Activo',
    CONSTRAINT CHK_tipos_analisis_estado CHECK (estado IN ('Activo', 'Inactivo'))
);

-- 2.5 METRICAS
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

-- 2.6 TIPO_ANALISIS_METRICA (NUEVA TABLA DE ENLACE)
-- Define qué métricas componen cada tipo de análisis. Relación N:N.
CREATE TABLE tipo_analisis_metrica (
    id_tipo_analisis INT NOT NULL,
    id_metrica       INT NOT NULL,
    CONSTRAINT PK_tipo_analisis_metrica PRIMARY KEY (id_tipo_analisis, id_metrica),
    CONSTRAINT FK_tam_tipo_analisis FOREIGN KEY (id_tipo_analisis) REFERENCES tipos_analisis(id_tipo_analisis) ON DELETE CASCADE,
    CONSTRAINT FK_tam_metricas FOREIGN KEY (id_metrica) REFERENCES metricas(id_metrica) ON DELETE CASCADE
);


-----------------------------------------------------------
-- 3. TABLAS DE USUARIOS (NORMALIZADAS)
-----------------------------------------------------------

-- 3.1 USUARIOS (Tabla Principal)
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

-- 3.2 MEDICOS (Tabla de Extensión)
CREATE TABLE medicos (
    dni             INT CONSTRAINT PK_medicos PRIMARY KEY,
    nro_matricula   INT NOT NULL CONSTRAINT UQ_medicos_matricula UNIQUE,
    especialidad    NVARCHAR(50) NOT NULL,
    
    CONSTRAINT FK_medicos_usuarios FOREIGN KEY (dni) REFERENCES usuarios(dni) ON DELETE CASCADE
);

-- 3.3 ASISTENTES (Tabla de Extensión)
CREATE TABLE asistentes (
    dni             INT CONSTRAINT PK_asistentes PRIMARY KEY,
    dni_supervisor  INT NOT NULL, 
    fecha_ingreso   DATE NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_asistentes_usuarios FOREIGN KEY (dni) REFERENCES usuarios(dni) ON DELETE CASCADE,
    CONSTRAINT FK_asistentes_supervisor FOREIGN KEY (dni_supervisor) REFERENCES medicos(dni)
);

-----------------------------------------------------------
-- 4. TABLA DE PACIENTES
-----------------------------------------------------------

-- 4.1 PACIENTES
CREATE TABLE pacientes (
    dni              INT CONSTRAINT PK_pacientes PRIMARY KEY,
    nombre           NVARCHAR(50) NOT NULL,
    apellido         NVARCHAR(50) NOT NULL,
    fecha_nac        DATE NOT NULL,
    sexo             CHAR(1) NOT NULL,
    email            NVARCHAR(100) NULL,
    telefono         NVARCHAR(20) NULL,
    id_obra_social   INT NULL,
    estado           NVARCHAR(20) NOT NULL DEFAULT 'Activo',

    CONSTRAINT FK_pacientes_obras_sociales FOREIGN KEY (id_obra_social) REFERENCES obras_sociales(id_obra_social),
    CONSTRAINT CHK_pacientes_sexo CHECK (sexo IN ('M', 'F', 'X')),
    CONSTRAINT CHK_pacientes_fecha_nac CHECK (fecha_nac <= GETDATE()),
    CONSTRAINT CHK_pacientes_estado CHECK (estado IN ('Activo', 'Inactivo'))
);

-----------------------------------------------------------
-- 5. TABLAS DE ANÁLISIS Y RESULTADOS
-----------------------------------------------------------

-- 5.1 ANALISIS
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

-- 5.2 ANALISIS_METRICA
CREATE TABLE analisis_metrica (
    id_analisis INT NOT NULL,
    id_metrica  INT NOT NULL,
    resultado   DECIMAL(10, 2) NOT NULL,
    observaciones NVARCHAR(500) NULL,
    CONSTRAINT PK_analisis_metrica PRIMARY KEY (id_analisis, id_metrica),
    CONSTRAINT FK_analisis_metrica_analisis FOREIGN KEY (id_analisis) REFERENCES analisis(id_analisis) ON DELETE CASCADE,
    CONSTRAINT FK_analisis_metrica_metricas FOREIGN KEY (id_metrica) REFERENCES metricas(id_metrica)
);
GO

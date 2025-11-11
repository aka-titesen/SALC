/******************************************************************************
 SCRIPT TOTAL SALC (v1.0)
 
 ESTE SCRIPT CREA LA BASE DE DATOS, CREA TODAS LAS TABLAS
 Y LAS PUEBLA CON DATOS DE PRUEBA REALISTAS.
 
 EJECUTAR SOBRE UN SSMS LIMPIO (SIN LA BD 'SALC' EXISTENTE).
******************************************************************************/

-----------------------------------------------------------
-- 0. CREACIÓN DE BASE DE DATOS
-----------------------------------------------------------
CREATE DATABASE [SALC];
GO

USE [SALC];
GO

PRINT '--- 0. Base de datos [SALC] creada y seleccionada ---';
GO

-----------------------------------------------------------
-- 1. CREACIÓN DE TABLAS
-----------------------------------------------------------

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

PRINT '--- 1. Todas las tablas creadas (incluyendo migración de backup) ---';
GO

-----------------------------------------------------------
-- 2. INSERCIÓN DE DATOS (SEED)
-----------------------------------------------------------

-- 2.1 CARGA DE CATÁLOGOS BÁSICOS
INSERT INTO dbo.roles (nombre_rol) VALUES
('Administrador'),  -- ID ROL = 1
('Medico'),         -- ID ROL = 2 (Sin acento)
('Asistente');       -- ID ROL = 3
GO

INSERT INTO dbo.estados_analisis (descripcion) VALUES
('Sin verificar'),  -- ID ESTADO = 1
('Verificado');     -- ID ESTADO = 2
GO

INSERT INTO dbo.obras_sociales (cuit, nombre, estado) VALUES
('30-50000000-1', 'OSDE', 'Activo'),
('30-50000001-2', 'Swiss Medical', 'Activo'),
('30-50000002-3', 'Galeno', 'Activo'),
('30-50000003-4', 'IOMA', 'Activo'),
('30-50000004-5', 'PAMI', 'Activo'),
('30-50000005-6', 'OSECAC', 'Activo'),
('30-50000006-7', 'Unión Personal (UP)', 'Activo'),
('30-50000007-8', 'OSPe', 'Activo'),
('30-50000008-9', 'OSUTHGRA', 'Inactivo'),
('30-50000009-0', 'Sancor Salud', 'Activo');
GO

PRINT '--- 2.1 Catálogos cargados (Roles, Estados, OS) ---';
GO

-- 2.2 CARGA DE MÉTRICAS Y TIPOS DE ANÁLISIS

-- Métricas
INSERT INTO dbo.metricas (nombre, unidad_medida, valor_maximo, valor_minimo, estado) VALUES
('Recuento Globulos Rojos', 'millones/µL', 5.90, 4.20, 'Activo'),  -- ID 1
('Recuento Globulos Blancos', 'miles/µL', 11.00, 4.50, 'Activo'),   -- ID 2
('Hemoglobina', 'g/dL', 17.50, 13.50, 'Activo'),                   -- ID 3
('Hematocrito', '%', 53.00, 41.00, 'Activo'),                       -- ID 4
('Plaquetas', 'miles/µL', 450.00, 150.00, 'Activo'),                -- ID 5
('Colesterol Total', 'mg/dL', 200.00, 0.00, 'Activo'),             -- ID 6
('Colesterol HDL', 'mg/dL', 60.00, 40.00, 'Activo'),                -- ID 7
('Colesterol LDL', 'mg/dL', 130.00, 0.00, 'Activo'),                -- ID 8
('Trigliceridos', 'mg/dL', 150.00, 0.00, 'Activo'),                 -- ID 9
('Glucosa en Ayunas', 'mg/dL', 100.00, 70.00, 'Activo'),            -- ID 10
('Hemoglobina Glicosilada (HbA1c)', '%', 5.60, 4.00, 'Activo'),      -- ID 11
('Urea', 'mg/dL', 50.00, 10.00, 'Activo'),                          -- ID 12
('Creatinina', 'mg/dL', 1.20, 0.60, 'Activo'),                      -- ID 13
('Acido Urico', 'mg/dL', 7.00, 2.50, 'Activo'),                     -- ID 14
('TGO (AST)', 'U/L', 40.00, 5.00, 'Activo'),                        -- ID 15
('TGP (ALT)', 'U/L', 56.00, 7.00, 'Activo'),                        -- ID 16
('Bilirrubina Total', 'mg/dL', 1.20, 0.20, 'Activo'),               -- ID 17
('Fosfatasa Alcalina (FAL)', 'U/L', 147.00, 44.00, 'Activo'),        -- ID 18
('TSH', 'µUI/mL', 4.00, 0.40, 'Activo'),                             -- ID 19
('T4 Libre', 'ng/dL', 1.80, 0.80, 'Activo'),                        -- ID 20
('Sodio (Na)', 'mEq/L', 145.00, 135.00, 'Activo'),                  -- ID 21
('Potasio (K)', 'mEq/L', 5.00, 3.50, 'Activo'),                     -- ID 22
('Cloro (Cl)', 'mEq/L', 107.00, 98.00, 'Activo'),                   -- ID 23
('Tiempo de Protrombina (TP)', 'seg', 13.5, 11.0, 'Activo'),        -- ID 24
('KPTT', 'seg', 35.0, 25.0, 'Activo');                               -- ID 25
GO

-- Tipos de Análisis
INSERT INTO dbo.tipos_analisis (descripcion, estado) VALUES
('Hemograma Completo', 'Activo'),   -- ID 1
('Perfil Lipidico', 'Activo'),      -- ID 2
('Perfil Glucemico', 'Activo'),     -- ID 3
('Funcion Renal (Uremia)', 'Activo'), -- ID 4
('Hepatograma Basico', 'Activo'),   -- ID 5
('Perfil Tiroideo Basico', 'Activo'), -- ID 6
('Ionograma Plasmatico', 'Activo'), -- ID 7
('Coagulograma', 'Activo');          -- ID 8
GO

-- Relaciones Tipo <> Métrica
INSERT INTO dbo.tipo_analisis_metrica (id_tipo_analisis, id_metrica) VALUES
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5),
(2, 6), (2, 7), (2, 8), (2, 9),
(3, 10), (3, 11),
(4, 12), (4, 13), (4, 14),
(5, 15), (5, 16), (5, 17), (5, 18),
(6, 19), (6, 20),
(7, 21), (7, 22), (7, 23),
(8, 24), (8, 25);
GO

PRINT '--- 2.2 Métricas y Tipos de Análisis cargados ---';
GO

-- 2.3 CARGA DE USUARIOS
-- Contraseña para todos: 'salc123'

-- Administradores (5) - Rol ID 1
INSERT INTO dbo.usuarios (dni, nombre, apellido, email, password_hash, id_rol, estado) VALUES
(25111111, 'Admin', 'Global', 'admin@salc.com', 'salc123', 1, 'Activo'),
(25111112, 'Laura', 'Campos', 'laura.campos@salc.com', 'salc123', 1, 'Activo'),
(25111113, 'Martín', 'Herrera', 'martin.herrera@salc.com', 'salc123', 1, 'Activo'),
(25111114, 'Sistema', 'Integracion', 'sys.integracion@salc.com', 'salc123', 1, 'Inactivo'),
(25111115, 'Julia', 'Gómez', 'julia.gomez@salc.com', 'salc123', 1, 'Activo');
GO

-- Médicos (20) - Rol ID 2
INSERT INTO dbo.usuarios (dni, nombre, apellido, email, password_hash, id_rol, estado) VALUES
(30100101, 'Carlos', 'Bianchi', 'carlos.bianchi@salc.com', 'salc123', 2, 'Activo'),
(30100102, 'Ana', 'Fernández', 'ana.fernandez@salc.com', 'salc123', 2, 'Activo'),
(30100103, 'Ricardo', 'Mollo', 'ricardo.mollo@salc.com', 'salc123', 2, 'Activo'),
(30100104, 'Silvia', 'Perez', 'silvia.perez@salc.com', 'salc123', 2, 'Activo'),
(30100105, 'Esteban', 'Quito', 'esteban.quito@salc.com', 'salc123', 2, 'Activo'),
(30100106, 'Valeria', 'Lynch', 'valeria.lynch@salc.com', 'salc123', 2, 'Activo'),
(30100107, 'Diego', 'Torres', 'diego.torres@salc.com', 'salc123', 2, 'Activo'),
(30100108, 'Julieta', 'Venegas', 'julieta.venegas@salc.com', 'salc123', 2, 'Activo'),
(30100109, 'Andrés', 'Calamaro', 'andres.calamaro@salc.com', 'salc123', 2, 'Activo'),
(30100110, 'Fabiana', 'Cantilo', 'fabiana.cantilo@salc.com', 'salc123', 2, 'Activo'),
(30100111, 'Gustavo', 'Cerati', 'gustavo.cerati@salc.com', 'salc123', 2, 'Inactivo'),
(30100112, 'Charly', 'García', 'charly.garcia@salc.com', 'salc123', 2, 'Activo'),
(30100113, 'Fito', 'Paez', 'fito.paez@salc.com', 'salc123', 2, 'Activo'),
(30100114, 'Mercedes', 'Sosa', 'mercedes.sosa@salc.com', 'salc123', 2, 'Activo'),
(30100115, 'Luis Alberto', 'Spinetta', 'luis.spinetta@salc.com', 'salc123', 2, 'Inactivo'),
(30100116, 'Leon', 'Gieco', 'leon.gieco@salc.com', 'salc123', 2, 'Activo'),
(30100117, 'Maria Elena', 'Walsh', 'maria.walsh@salc.com', 'salc123', 2, 'Activo'),
(30100118, 'Atahualpa', 'Yupanqui', 'atahualpa.yupanqui@salc.com', 'salc123', 2, 'Activo'),
(30100119, 'Sandro', 'De America', 'sandro.deamerica@salc.com', 'salc123', 2, 'Activo'),
(30100120, 'Tita', 'Merello', 'tita.merello@salc.com', 'salc123', 2, 'Activo');

INSERT INTO dbo.medicos (dni, nro_matricula, especialidad) VALUES
(30100101, 10001, 'Hematología'),
(30100102, 10002, 'Endocrinología'),
(30100103, 10003, 'Bioquímica Clínica'),
(30100104, 10004, 'Hematología'),
(30100105, 10005, 'Nefrología'),
(30100106, 10006, 'Bioquímica Clínica'),
(30100107, 10007, 'Bioquímica Clínica'),
(30100108, 10008, 'Hematología'),
(30100109, 10009, 'Endocrinología'),
(30100110, 10010, 'Nefrología'),
(30100111, 10011, 'Bioquímica Clínica'),
(30100112, 10012, 'Hematología'),
(30100113, 10013, 'Bioquímica Clínica'),
(30100114, 10014, 'Endocrinología'),
(30100115, 10015, 'Nefrología'),
(30100116, 10016, 'Bioquímica Clínica'),
(30100117, 10017, 'Hematología'),
(30100118, 10018, 'Bioquímica Clínica'),
(30100119, 10019, 'Endocrinología'),
(30100120, 10020, 'Nefrología');
GO

-- Asistentes (10) - Rol ID 3
INSERT INTO dbo.usuarios (dni, nombre, apellido, email, password_hash, id_rol, estado) VALUES
(40100201, 'Maria', 'Becerra', 'maria.becerra@salc.com', 'salc123', 3, 'Activo'),
(40100202, 'Duki', 'Lombardo', 'duki.lombardo@salc.com', 'salc123', 3, 'Activo'),
(40100203, 'Nicki', 'Nicole', 'nicki.nicole@salc.com', 'salc123', 3, 'Activo'),
(40100204, 'Tiago', 'PZK', 'tiago.pzk@salc.com', 'salc123', 3, 'Activo'),
(40100205, 'L-Gante', 'RKT', 'l-gante.rkt@salc.com', 'salc123', 3, 'Inactivo'),
(40100206, 'Wos', 'Valentin', 'wos.valentin@salc.com', 'salc123', 3, 'Activo'),
(40100207, 'Trueno', 'Peligro', 'trueno.peligro@salc.com', 'salc123', 3, 'Activo'),
(40100208, 'Bizarrap', 'Gonzalo', 'bizarrap.gonzalo@salc.com', 'salc123', 3, 'Activo'),
(40100209, 'Nathy', 'Peluso', 'nathy.peluso@salc.com', 'salc123', 3, 'Activo'),
(40100210, 'Cazzu', 'Jefa', 'cazzu.jefa@salc.com', 'salc123', 3, 'Activo');

INSERT INTO dbo.asistentes (dni, dni_supervisor, fecha_ingreso) VALUES
(40100201, 30100101, '2023-01-15'), -- Supervisor: Carlos Bianchi
(40100202, 30100102, '2023-02-20'), -- Supervisor: Ana Fernández
(40100203, 30100101, '2023-03-10'), -- Supervisor: Carlos Bianchi
(40100204, 30100103, '2023-04-05'), -- Supervisor: Ricardo Mollo
(40100205, 30100104, '2023-05-12'), -- Supervisor: Silvia Perez
(40100206, 30100103, '2023-06-18'), -- Supervisor: Ricardo Mollo
(40100207, 30100105, '2023-07-22'), -- Supervisor: Esteban Quito
(40100208, 30100106, '2023-08-30'), -- Supervisor: Valeria Lynch
(40100209, 30100107, '2023-09-14'), -- Supervisor: Diego Torres
(40100210, 30100108, '2023-10-25'); -- Supervisor: Julieta Venegas
GO

PRINT '--- 2.3 Usuarios (Admin, Médicos, Asistentes) cargados ---';
GO

-- 2.4 CARGA DE PACIENTES
INSERT INTO dbo.pacientes (dni, nombre, apellido, fecha_nac, sexo, email, telefono, id_obra_social, estado) VALUES
(10000001, 'Lionel', 'Messi', '1987-06-24', 'M', 'lionel.messi@paciente.com', '+54 9 341 1010101', 1, 'Activo'),
(10000002, 'Diego', 'Maradona', '1960-10-30', 'M', 'diego.maradona@paciente.com', '+54 9 11 1010102', 2, 'Inactivo'),
(10000003, 'Mirtha', 'Legrand', '1927-02-23', 'F', 'mirtha.legrand@paciente.com', '+54 9 11 1010103', 3, 'Activo'),
(10000004, 'Susana', 'Gimenez', '1944-01-29', 'F', 'susana.gimenez@paciente.com', '+54 9 11 1010104', 1, 'Activo'),
(10000005, 'Ricardo', 'Darin', '1957-01-16', 'M', 'ricardo.darin@paciente.com', '+54 9 11 1010105', 4, 'Activo'),
(10000006, 'Guillermo', 'Francella', '1955-02-14', 'M', 'guillermo.francella@paciente.com', '+54 9 11 1010106', 5, 'Activo'),
(10000007, 'Norma', 'Aleandro', '1936-05-02', 'F', 'norma.aleandro@paciente.com', '+54 9 11 1010107', 6, 'Activo'),
(10000008, 'Quino', 'Lavado', '1932-07-17', 'M', 'quino.lavado@paciente.com', '+54 9 261 1010108', 7, 'Inactivo'),
(10000009, 'Jorge Luis', 'Borges', '1899-08-24', 'M', 'jorge.borges@paciente.com', '+54 9 11 1010109', 8, 'Inactivo'),
(10000010, 'Julio', 'Cortazar', '1914-08-26', 'M', 'julio.cortazar@paciente.com', '+54 9 11 1010110', 10, 'Inactivo'),
(20000011, 'Maria', 'Gomez', '1990-05-15', 'F', 'maria.gomez@paciente.com', '+54 9 11 2020211', 1, 'Activo'),
(20000012, 'Juan', 'Perez', '1985-11-30', 'M', 'juan.perez@paciente.com', '+54 9 11 2020212', 2, 'Activo'),
(20000013, 'Ana', 'Lopez', '1992-02-10', 'F', 'ana.lopez@paciente.com', '+54 9 351 2020213', 3, 'Activo'),
(20000014, 'Carlos', 'Garcia', '1978-07-20', 'M', 'carlos.garcia@paciente.com', '+54 9 341 2020214', 4, 'Activo'),
(20000015, 'Lucia', 'Fernandez', '2001-12-05', 'F', 'lucia.fernandez@paciente.com', '+54 9 11 2020215', 5, 'Activo'),
(20000016, 'Matias', 'Rodriguez', '1995-03-25', 'M', 'matias.rodriguez@paciente.com', '+54 9 221 2020216', 6, 'Activo'),
(20000017, 'Sofia', 'Martinez', '1989-09-18', 'F', 'sofia.martinez@paciente.com', '+54 9 11 2020217', 7, 'Activo'),
(20000018, 'Diego', 'Gonzalez', '1982-01-01', 'M', 'diego.gonzalez@paciente.com', '+54 9 11 2020218', 8, 'Activo'),
(20000019, 'Valentina', 'Sanchez', '1999-06-08', 'F', 'valentina.sanchez@paciente.com', '+54 9 261 2020219', 10, 'Activo'),
(20000020, 'Agustin', 'Romero', '1991-04-12', 'M', 'agustin.romero@paciente.com', '+54 9 11 2020220', 1, 'Activo'),
(30000021, 'Camila', 'Diaz', '1998-10-22', 'F', 'camila.diaz@paciente.com', '+54 9 11 3030321', 2, 'Activo'),
(30000022, 'Nicolas', 'Alvarez', '1975-08-14', 'M', 'nicolas.alvarez@paciente.com', '+54 9 351 3030322', 3, 'Activo'),
(30000023, 'Julieta', 'Torres', '2003-04-30', 'F', 'julieta.torres@paciente.com', '+54 9 11 3030323', 4, 'Activo'),
(30000024, 'Santiago', 'Ruiz', '1969-11-02', 'M', 'santiago.ruiz@paciente.com', '+54 9 341 3030324', 5, 'Activo'),
(30000025, 'Martina', 'Suarez', '1993-01-28', 'F', 'martina.suarez@paciente.com', '+54 9 11 3030325', 6, 'Activo'),
(30000026, 'Micaela', 'Jimenez', '1987-07-17', 'F', 'micaela.jimenez@paciente.com', '+54 9 221 3030326', 7, 'Activo'),
(30000027, 'Facundo', 'Gimenez', '1994-03-03', 'M', 'facundo.gimenez@paciente.com', '+54 9 11 3030327', 8, 'Activo'),
(30000028, 'Paula', 'Moreno', '2000-09-09', 'F', 'paula.moreno@paciente.com', '+54 9 261 3030328', 10, 'Activo'),
(30000029, 'Ezequiel', 'Vazquez', '1980-12-19', 'M', 'ezequiel.vazquez@paciente.com', '+54 9 11 3030329', 1, 'Activo'),
(30000030, 'Florencia', 'Castro', '1996-05-27', 'F', 'florencia.castro@paciente.com', '+54 9 351 3030330', 2, 'Activo'),
(40000031, 'Sebastian', 'Ortiz', '1983-10-11', 'M', 'sebastian.ortiz@paciente.com', '+54 9 11 4040431', 3, 'Activo'),
(40000032, 'Daniela', 'Medina', '1997-02-07', 'F', 'daniela.medina@paciente.com', '+54 9 341 4040432', 4, 'Activo'),
(40000033, 'Hernan', 'Dominguez', '1973-06-21', 'M', 'hernan.dominguez@paciente.com', '+54 9 11 4040433', 5, 'Activo'),
(40000034, 'Carla', 'Soto', '1991-08-03', 'F', 'carla.soto@paciente.com', '+54 9 221 4040434', 6, 'Activo'),
(40000035, 'Francisco', 'Benitez', '1986-04-16', 'M', 'francisco.benitez@paciente.com', '+54 9 11 4040435', 7, 'Activo'),
(40000036, 'Victoria', 'Ramos', '2002-11-23', 'F', 'victoria.ramos@paciente.com', '+54 9 261 4040436', 8, 'Activo'),
(40000037, 'Alejandro', 'Ayala', '1979-01-09', 'M', 'alejandro.ayala@paciente.com', '+54 9 11 4040437', 10, 'Activo'),
(40000038, 'Rocio', 'Peralta', '1990-07-31', 'F', 'rocio.peralta@paciente.com', '+54 9 351 4040438', 1, 'Activo'),
(40000039, 'Lucas', 'Molina', '1988-03-14', 'M', 'lucas.molina@paciente.com', '+54 9 11 4040439', 2, 'Activo'),
(40000040, 'Jazmin', 'Luna', '1994-09-02', 'F', 'jazmin.luna@paciente.com', '+54 9 341 4040440', 3, 'Activo'),
(50000041, 'Ivan', 'Vega', '1977-05-06', 'M', 'ivan.vega@paciente.com', '+54 9 11 5050541', 4, 'Activo'),
(50000042, 'Abril', 'Rios', '1999-08-18', 'F', 'abril.rios@paciente.com', '+54 9 221 5050542', 5, 'Activo'),
(50000043, 'Bruno', 'Acosta', '1984-12-24', 'M', 'bruno.acosta@paciente.com', '+54 9 11 5050543', 6, 'Activo'),
(50000044, 'Cintia', 'Ledesma', '1992-10-07', 'F', 'cintia.ledesma@paciente.com', '+54 9 261 5050544', 7, 'Activo'),
(50000045, 'Damian', 'Quiroga', '1970-02-13', 'M', 'damian.quiroga@paciente.com', '+54 9 11 5050545', 8, 'Activo'),
(50000046, 'Eliana', 'Paz', '1981-11-09', 'F', 'eliana.paz@paciente.com', '+54 9 351 5050546', 10, 'Activo'),
(50000047, 'Federico', 'Blanco', '1993-07-29', 'M', 'federico.blanco@paciente.com', '+54 9 11 5050547', 1, 'Activo'),
(50000048, 'Gabriela', 'Bustos', '1976-04-01', 'F', 'gabriela.bustos@paciente.com', '+54 9 341 5050548', 2, 'Activo'),
(50000049, 'Hector', 'Campos', '1965-08-27', 'M', 'hector.campos@paciente.com', '+54 9 11 5050549', 3, 'Activo'),
(50000050, 'Irene', 'Sosa', '1995-12-31', 'F', 'irene.sosa@paciente.com', '+54 9 221 5050550', 4, 'Activo');
GO

PRINT '--- 2.4 Pacientes (50) cargados ---';
GO

-- 2.5 CARGA DE ANÁLISIS Y RESULTADOS (40)

SET NOCOUNT ON;
DECLARE @idAnalisis INT;

-- 20 ANÁLISIS SIN VERIFICAR (Estado 1)
-- 1
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (1, 1, 20000011, 30100101, GETDATE()-3); -- Tipo 1 (Hemo), Pac: Gomez, Med: Bianchi
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 1, 4.50), (@idAnalisis, 2, 8.20), (@idAnalisis, 3, 14.1), (@idAnalisis, 4, 42.0), (@idAnalisis, 5, 250);

-- 2
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (2, 1, 20000012, 30100102, GETDATE()-3); -- Tipo 2 (Lipid), Pac: Perez, Med: Fernandez
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 6, 210.0), (@idAnalisis, 7, 45.0), (@idAnalisis, 8, 140.0), (@idAnalisis, 9, 130.0);

-- 3
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (3, 1, 20000013, 30100103, GETDATE()-2); -- Tipo 3 (Gluco), Pac: Lopez, Med: Mollo
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 10, 95.0), (@idAnalisis, 11, 5.5);

-- 4
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (4, 1, 20000014, 30100104, GETDATE()-2); -- Tipo 4 (Renal), Pac: Garcia, Med: Perez
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 12, 40.0), (@idAnalisis, 13, 1.1), (@idAnalisis, 14, 6.5);

-- 5
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (5, 1, 20000015, 30100105, GETDATE()-1); -- Tipo 5 (Hepato), Pac: Fernandez, Med: Quito
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 15, 30.0), (@idAnalisis, 16, 35.0), (@idAnalisis, 17, 0.8), (@idAnalisis, 18, 100.0);

-- 6
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (6, 1, 20000016, 30100106, GETDATE()-1); -- Tipo 6 (Tiro), Pac: Rodriguez, Med: Lynch
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 19, 2.5), (@idAnalisis, 20, 1.2);

-- 7
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (7, 1, 20000017, 30100107, GETDATE()-1); -- Tipo 7 (Iono), Pac: Martinez, Med: Torres
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 21, 140.0), (@idAnalisis, 22, 4.1), (@idAnalisis, 23, 102.0);

-- 8
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (8, 1, 20000018, 30100108, GETDATE()); -- Tipo 8 (Coag), Pac: Gonzalez, Med: Venegas
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 24, 12.5), (@idAnalisis, 25, 30.0);

-- 9
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (1, 1, 20000019, 30100109, GETDATE()); -- Tipo 1 (Hemo), Pac: Sanchez, Med: Calamaro
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 1, 4.80), (@idAnalisis, 2, 6.50), (@idAnalisis, 3, 15.0), (@idAnalisis, 4, 45.0), (@idAnalisis, 5, 300);

-- 10
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (2, 1, 20000020, 30100110, GETDATE()-4); -- Tipo 2 (Lipid), Pac: Romero, Med: Cantilo
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 6, 230.0), (@idAnalisis, 7, 38.0), (@idAnalisis, 8, 160.0), (@idAnalisis, 9, 180.0);

-- 11
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (4, 1, 30000021, 30100112, GETDATE()-5); -- Tipo 4 (Renal), Pac: Diaz, Med: Garcia
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 12, 55.0), (@idAnalisis, 13, 1.3), (@idAnalisis, 14, 7.2);

-- 12
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (5, 1, 30000022, 30100113, GETDATE()-6); -- Tipo 5 (Hepato), Pac: Alvarez, Med: Paez
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 15, 45.0), (@idAnalisis, 16, 60.0), (@idAnalisis, 17, 1.1), (@idAnalisis, 18, 150.0);

-- 13
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (1, 1, 30000023, 30100114, GETDATE()-7); -- Tipo 1 (Hemo), Pac: Torres, Med: Sosa
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 1, 4.10), (@idAnalisis, 2, 5.0), (@idAnalisis, 3, 12.5), (@idAnalisis, 4, 38.0), (@idAnalisis, 5, 180);

-- 14
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (6, 1, 30000024, 30100116, GETDATE()-8); -- Tipo 6 (Tiro), Pac: Ruiz, Med: Gieco
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 19, 5.1), (@idAnalisis, 20, 0.7);

-- 15
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (7, 1, 30000025, 30100117, GETDATE()-9); -- Tipo 7 (Iono), Pac: Suarez, Med: Walsh
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 21, 134.0), (@idAnalisis, 22, 3.4), (@idAnalisis, 23, 97.0);

-- 16
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (3, 1, 10000001, 30100101, GETDATE()-10); -- Tipo 3 (Gluco), Pac: Messi, Med: Bianchi
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 10, 88.0), (@idAnalisis, 11, 5.1);

-- 17
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (1, 1, 10000003, 30100102, GETDATE()-11); -- Tipo 1 (Hemo), Pac: Legrand, Med: Fernandez
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 1, 4.30), (@idAnalisis, 2, 7.10), (@idAnalisis, 3, 13.8), (@idAnalisis, 4, 41.5), (@idAnalisis, 5, 220);

-- 18
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (2, 1, 10000004, 30100103, GETDATE()-12); -- Tipo 2 (Lipid), Pac: Gimenez, Med: Mollo
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 6, 190.0), (@idAnalisis, 7, 55.0), (@idAnalisis, 8, 120.0), (@idAnalisis, 9, 110.0);

-- 19
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (4, 1, 10000005, 30100104, GETDATE()-13); -- Tipo 4 (Renal), Pac: Darin, Med: Perez
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 12, 35.0), (@idAnalisis, 13, 0.9), (@idAnalisis, 14, 5.0);

-- 20
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, fecha_creacion)
VALUES (8, 1, 10000006, 30100105, GETDATE()-14); -- Tipo 8 (Coag), Pac: Francella, Med: Quito
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 24, 12.0), (@idAnalisis, 25, 31.0);


-- 20 ANÁLISIS VERIFICADOS (Estado 2)
-- REGLA: dni_firma = dni_carga

-- 21
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (1, 2, 40000031, 30100118, 30100118, GETDATE()-10, GETDATE()-8); -- Pac: Ortiz, Med: Yupanqui
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 1, 5.10), (@idAnalisis, 2, 9.0), (@idAnalisis, 3, 16.0), (@idAnalisis, 4, 48.0), (@idAnalisis, 5, 280);

-- 22
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (2, 2, 40000032, 30100119, 30100119, GETDATE()-10, GETDATE()-8); -- Pac: Medina, Med: Sandro
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 6, 250.0), (@idAnalisis, 7, 35.0), (@idAnalisis, 8, 170.0), (@idAnalisis, 9, 210.0);

-- 23
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (3, 2, 40000033, 30100120, 30100120, GETDATE()-9, GETDATE()-7); -- Pac: Dominguez, Med: Merello
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 10, 105.0), (@idAnalisis, 11, 5.8);

-- 24
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (4, 2, 40000034, 30100101, 30100101, GETDATE()-9, GETDATE()-7); -- Pac: Soto, Med: Bianchi
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 12, 25.0), (@idAnalisis, 13, 0.8), (@idAnalisis, 14, 4.5);

-- 25
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (5, 2, 40000035, 30100102, 30100102, GETDATE()-8, GETDATE()-6); -- Pac: Benitez, Med: Fernandez
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 15, 22.0), (@idAnalisis, 16, 20.0), (@idAnalisis, 17, 0.5), (@idAnalisis, 18, 80.0);

-- 26
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (6, 2, 40000036, 30100103, 30100103, GETDATE()-8, GETDATE()-6); -- Pac: Ramos, Med: Mollo
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 19, 1.5), (@idAnalisis, 20, 1.0);

-- 27
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (7, 2, 40000037, 30100104, 30100104, GETDATE()-7, GETDATE()-5); -- Pac: Ayala, Med: Perez
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 21, 142.0), (@idAnalisis, 22, 4.5), (@idAnalisis, 23, 100.0);

-- 28
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (8, 2, 40000038, 30100105, 30100105, GETDATE()-7, GETDATE()-5); -- Pac: Peralta, Med: Quito
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 24, 13.0), (@idAnalisis, 25, 33.0);

-- 29
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (1, 2, 40000039, 30100106, 30100106, GETDATE()-6, GETDATE()-4); -- Pac: Molina, Med: Lynch
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 1, 4.90), (@idAnalisis, 2, 7.80), (@idAnalisis, 3, 15.5), (@idAnalisis, 4, 46.0), (@idAnalisis, 5, 290);

-- 30
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (2, 2, 40000040, 30100107, 30100107, GETDATE()-6, GETDATE()-4); -- Pac: Luna, Med: Torres
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 6, 185.0), (@idAnalisis, 7, 52.0), (@idAnalisis, 8, 115.0), (@idAnalisis, 9, 90.0);

-- 31
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (1, 2, 10000001, 30100108, 30100108, GETDATE()-20, GETDATE()-18); -- Pac: Messi, Med: Venegas
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 1, 5.20), (@idAnalisis, 2, 5.50), (@idAnalisis, 3, 16.2), (@idAnalisis, 4, 49.0), (@idAnalisis, 5, 200);

-- 32
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (5, 2, 10000003, 30100109, 30100109, GETDATE()-20, GETDATE()-18); -- Pac: Legrand, Med: Calamaro
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 15, 28.0), (@idAnalisis, 16, 25.0), (@idAnalisis, 17, 0.7), (@idAnalisis, 18, 110.0);

-- 33
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (1, 2, 50000041, 30100110, 30100110, GETDATE()-15, GETDATE()-13); -- Pac: Vega, Med: Cantilo
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 1, 4.60), (@idAnalisis, 2, 8.10), (@idAnalisis, 3, 14.3), (@idAnalisis, 4, 43.0), (@idAnalisis, 5, 260);

-- 34
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (2, 2, 50000042, 30100112, 30100112, GETDATE()-15, GETDATE()-13); -- Pac: Rios, Med: Garcia
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 6, 215.0), (@idAnalisis, 7, 42.0), (@idAnalisis, 8, 145.0), (@idAnalisis, 9, 140.0);

-- 35
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (3, 2, 50000043, 30100113, 30100113, GETDATE()-14, GETDATE()-12); -- Pac: Acosta, Med: Paez
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 10, 92.0), (@idAnalisis, 11, 5.3);

-- 36
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (4, 2, 50000044, 30100114, 30100114, GETDATE()-14, GETDATE()-12); -- Pac: Ledesma, Med: Sosa
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 12, 42.0), (@idAnalisis, 13, 1.0), (@idAnalisis, 14, 6.8);

-- 37
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (5, 2, 50000045, 30100116, 30100116, GETDATE()-13, GETDATE()-11); -- Pac: Quiroga, Med: Gieco
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 15, 33.0), (@idAnalisis, 16, 38.0), (@idAnalisis, 17, 0.9), (@idAnalisis, 18, 120.0);

-- 38
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (6, 2, 50000046, 30100117, 30100117, GETDATE()-13, GETDATE()-11); -- Pac: Paz, Med: Walsh
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 19, 3.1), (@idAnalisis, 20, 1.1);

-- 39
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (7, 2, 50000047, 30100118, 30100118, GETDATE()-12, GETDATE()-10); -- Pac: Blanco, Med: Yupanqui
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 21, 138.0), (@idAnalisis, 22, 3.9), (@idAnalisis, 23, 99.0);

-- 40
INSERT INTO dbo.analisis (id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma)
VALUES (8, 2, 50000048, 30100119, 30100119, GETDATE()-12, GETDATE()-10); -- Pac: Bustos, Med: Sandro
SET @idAnalisis = SCOPE_IDENTITY();
INSERT INTO dbo.analisis_metrica (id_analisis, id_metrica, resultado) VALUES
(@idAnalisis, 24, 12.8), (@idAnalisis, 25, 32.0);

SET NOCOUNT OFF;
GO

PRINT '--- 2.5 Análisis (40) y Resultados cargados ---';
GO

PRINT '--- SCRIPT TOTAL COMPLETADO CON ÉXITO ---';
GO
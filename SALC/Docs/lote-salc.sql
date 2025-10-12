/* ====================================================================
   SALC – LOTE DE DATOS DE EJEMPLO – v2.8
   - Adaptado a la estructura de BD v2.8 con baja lógica implementada.
   - Incluye el estado 'Anulado' para análisis.
   - Incluye ejemplos de registros 'Inactivos' en tablas maestras.
   - Contraseñas hasheadas (ejemplo BCrypt).
   ==================================================================== */
USE [SALC];
GO

-- 1. ROLES ----------------------------------------------
INSERT INTO roles (nombre_rol) VALUES
('Administrador'),
('Médico'),
('Asistente');
GO

-- 2. ESTADOS DE ANÁLISIS ---------------------------------
INSERT INTO estados_analisis (descripcion) VALUES
('Sin verificar'),
('Verificado'),
('Anulado'); -- ESTADO PARA BAJA LÓGICA DE ANÁLISIS
GO

-- 3. OBRAS SOCIALES -------------------------------------
INSERT INTO obras_sociales (cuit, nombre, estado) VALUES
('30500000010','OSDE', 'Activo'),
('30650000020','Swiss Medical', 'Activo'),
('30990000040','Sin Obra Social', 'Activo'),
('30770000030','PAMI', 'Inactivo'); -- EJEMPLO DE BAJA LÓGICA
GO

-- 4. TIPOS DE ANÁLISIS ----------------------------------
INSERT INTO tipos_analisis (descripcion, estado) VALUES
('Hemograma Completo', 'Activo'),
('Glucosa en Ayunas', 'Activo'),
('Perfil Lipídico Completo', 'Activo'),
('Análisis de Orina Completo', 'Activo');
GO

-- 5. MÉTRICAS -------------------------------------------
INSERT INTO metricas (nombre, unidad_medida, valor_minimo, valor_maximo, estado) VALUES
('Glucosa','mg/dL',70,110, 'Activo'),
('Colesterol Total','mg/dL',0,200, 'Activo'),
('HDL','mg/dL',40,60, 'Activo'),
('LDL','mg/dL',0,100, 'Activo'),
('Triglicéridos','mg/dL',0,150, 'Activo'),
('Hemoglobina','g/dL',12,18, 'Activo'),
('Leucocitos','10³/µL',4,11, 'Activo'),
('Plaquetas','10³/µL',150,450, 'Activo');
GO

-- 6. USUARIOS (Datos comunes) ---------------------------
-- Las contraseñas son un HASH generado por la aplicación (ej. BCrypt).
INSERT INTO usuarios (dni, nombre, apellido, id_rol, email, password_hash, estado) VALUES
(30000001,'Juan','Pérez',1,'jperez@lab.com','$2a$12$URP1nbn2iSYn5/cEFwcaMeN8N.8SR1TaL3FMwFvYthH6c7DAfxWWm', 'Activo'),
(30000002,'María','González',2,'mgonzalez@lab.com','$2a$12$xR2X4M31NjPdlYXefWGhVO/mhEY4wEcHGtrIT09zPWFyjcH49k7.q', 'Activo'),
(30000003,'Sofía','Gundisalvo',2,'sofiagun@lab.com','$2a$12$o7V3ylw3QSIO2qk2QagJKubqaJFVoomZv3lTo3mVjeUuCTz6xTUsO', 'Activo'),
(30000004,'Carlos','Ramírez',3,'cramirez@lab.com','$2a$12$y98vRL/bjbV.U2.DPSpciOLaqSBPNxQk.0uY.fBe.FZDdP3KVKm8a', 'Activo'),
(30000005,'Erika','Miralles',3,'erikamir@lab.com','$2a$12$PTg6.zjtrOe1NcGegfAl/elM95qtmTOEmHrAetH39m7lQw5wXKq9.', 'Activo'),
(30000006,'Pedro','Alonso',2,'palonso@lab.com','$2a$12$abcdefghijklmnopqrstuv.wxyz', 'Inactivo'); -- EJEMPLO USUARIO INACTIVO
GO

-- 7. MEDICOS (Datos específicos) ------------------------
INSERT INTO medicos (dni, nro_matricula, especialidad) VALUES
(30000002, 25478, 'Clínica Médica'),
(30000003, 36589, 'Bioquímica Clínica'),
(30000006, 12345, 'Cardiología'); -- MÉDICO INACTIVO
GO

-- 8. ASISTENTES (Datos específicos) ---------------------
INSERT INTO asistentes (dni, dni_supervisor, fecha_ingreso) VALUES
(30000004, 30000002, GETDATE()), -- Supervisado por María González
(30000005, 30000003, GETDATE()); -- Supervisado por Sofía Gundisalvo
GO

-- 9. PACIENTES ------------------------------------------
INSERT INTO pacientes (dni, nombre, apellido, sexo, fecha_nac, telefono, email, id_obra_social, estado) VALUES
(40000005,'Ana','López','F','1985-06-15','3415123456','analopez@gmail.com',1, 'Activo'),
(40000006,'Luis','Martínez','M','1990-11-22','3415223456','luis.martinez@gmail.com',2, 'Activo'),
(40000007,'Sofía','Fernández','F','1978-03-08','3415323456','sofia.fernandez@gmail.com',3, 'Activo'),
(40000008,'Jorge','García','M','1975-01-20','3415423456','jgarcia@email.com',1, 'Inactivo'); -- EJEMPLO PACIENTE INACTIVO
GO

-- 10. ANÁLISIS ------------------------------------------
INSERT INTO analisis (id_tipo_analisis, id_estado, dni_paciente, observaciones, dni_carga, dni_firma, fecha_creacion, fecha_firma) VALUES
(1, 2, 40000005, 'Control anual', 30000002, 30000002, GETDATE()-2, GETDATE()-1), -- Estado: Verificado
(2, 1, 40000006, 'Pre-operatorio', 30000002, NULL, GETDATE()-1, NULL),           -- Estado: Sin verificar
(3, 1, 40000007, 'Chequeo general', 30000003, NULL, GETDATE(), NULL),            -- Estado: Sin verificar
(1, 3, 40000005, 'Duplicado por error', 30000003, NULL, GETDATE()-3, NULL);      -- Estado: Anulado (BAJA LÓGICA)
GO

-- 11. RESULTADOS DE ANÁLISIS ----------------------------
INSERT INTO analisis_metrica (id_analisis, id_metrica, resultado) VALUES
-- Resultados para el Análisis 1 (Hemograma de Ana López)
(1, 6, 13.2), (1, 7, 5.8), (1, 8, 250),
-- Resultados para el Análisis 2 (Glucosa de Luis Martínez)
(2, 1, 95),
-- Resultados para el Análisis 3 (Perfil Lipídico de Sofía Fernández)
(3, 2, 180), (3, 3, 45), (3, 4, 110), (3, 5, 125);
-- NOTA: El análisis anulado (ID 4) no tiene resultados cargados.
GO

PRINT 'Lote de datos de ejemplo v2.8 (con baja lógica) cargado correctamente.';
GO
/* ====================================================================
   SALC – LOTE DE DATOS DE EJEMPLO – v2.9
   - Adaptado a la estructura de BD v2.9.
   - Se añade la población de la tabla 'tipo_analisis_metrica'.
   - Se incluyen contraseñas en texto plano en comentarios para pruebas.
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
('Hemograma Completo', 'Activo'),       -- ID = 1
('Glucosa en Ayunas', 'Activo'),      -- ID = 2
('Perfil Lipídico Completo', 'Activo'), -- ID = 3
('Análisis de Orina Completo', 'Activo'); -- ID = 4
GO

-- 5. MÉTRICAS -------------------------------------------
INSERT INTO metricas (nombre, unidad_medida, valor_minimo, valor_maximo, estado) VALUES
('Glucosa','mg/dL',70,110, 'Activo'),             -- ID = 1
('Colesterol Total','mg/dL',0,200, 'Activo'),      -- ID = 2
('HDL','mg/dL',40,60, 'Activo'),                   -- ID = 3
('LDL','mg/dL',0,100, 'Activo'),                   -- ID = 4
('Triglicéridos','mg/dL',0,150, 'Activo'),         -- ID = 5
('Hemoglobina','g/dL',12,18, 'Activo'),            -- ID = 6
('Leucocitos','10³/µL',4,11, 'Activo'),            -- ID = 7
('Plaquetas','10³/µL',150,450, 'Activo');         -- ID = 8
GO

-- 6. ASOCIACIÓN TIPO ANÁLISIS Y MÉTRICAS ----------
INSERT INTO tipo_analisis_metrica (id_tipo_analisis, id_metrica) VALUES
-- Hemograma Completo (ID 1) se compone de:
(1, 6), -- Hemoglobina
(1, 7), -- Leucocitos
(1, 8), -- Plaquetas
-- Glucosa en Ayunas (ID 2) se compone de:
(2, 1), -- Glucosa
-- Perfil Lipídico Completo (ID 3) se compone de:
(3, 2), -- Colesterol Total
(3, 3), -- HDL
(3, 4), -- LDL
(3, 5); -- Triglicéridos
GO

-- 7. USUARIOS (Datos comunes) ---------------------------
INSERT INTO usuarios (dni, nombre, apellido, id_rol, email, password_hash, estado) VALUES
(30000001,'Juan','Pérez',1,'jperez@lab.com','$2a$12$URP1nbn2iSYn5/cEFwcaMeN8N.8SR1TaL3FMwFvYthH6c7DAfxWWm', 'Activo'), -- Pass: admin123
(30000002,'María','González',2,'mgonzalez@lab.com','$2a$12$xR2X4M31NjPdlYXefWGhVO/mhEY4wEcHGtrIT09zPWFyjcH49k7.q', 'Activo'), -- Pass: medico123
(30000003,'Sofía','Gundisalvo',2,'sofiagun@lab.com','$2a$12$o7V3ylw3QSIO2qk2QagJKubqaJFVoomZv3lTo3mVjeUuCTz6xTUsO', 'Activo'), -- Pass: medico456
(30000004,'Carlos','Ramírez',3,'cramirez@lab.com','$2a$12$y98vRL/bjbV.U2.DPSpciOLaqSBPNxQk.0uY.fBe.FZDdP3KVKm8a', 'Activo'), -- Pass: asistente123
(30000005,'Erika','Miralles',3,'erikamir@lab.com','$2a$12$PTg6.zjtrOe1NcGegfAl/elM95qtmTOEmHrAetH39m7lQw5wXKq9.', 'Activo'), -- Pass: asistente456
(30000006,'Pedro','Alonso',2,'palonso@lab.com','$2a$12$gL.k3F83N9D6GvV07sYVUO.b2hV2fJ/7O4nSTspG/3a/jY.2023.K', 'Inactivo'); -- Pass: pedro123
GO

-- 8. MEDICOS (Datos específicos) ------------------------
INSERT INTO medicos (dni, nro_matricula, especialidad) VALUES
(30000002, 25478, 'Clínica Médica'),
(30000003, 36589, 'Bioquímica Clínica'),
(30000006, 12345, 'Cardiología'); -- MÉDICO INACTIVO
GO

-- 9. ASISTENTES (Datos específicos) ---------------------
INSERT INTO asistentes (dni, dni_supervisor, fecha_ingreso) VALUES
(30000004, 30000002, GETDATE()), -- Supervisado por María González
(30000005, 30000003, GETDATE()); -- Supervisado por Sofía Gundisalvo
GO

-- 10. PACIENTES ------------------------------------------
INSERT INTO pacientes (dni, nombre, apellido, sexo, fecha_nac, telefono, email, id_obra_social, estado) VALUES
(40000005,'Ana','López','F','1985-06-15','3415123456','analopez@gmail.com',1, 'Activo'),
(40000006,'Luis','Martínez','M','1990-11-22','3415223456','luis.martinez@gmail.com',2, 'Activo'),
(40000007,'Sofía','Fernández','F','1978-03-08','3415323456','sofia.fernandez@gmail.com',3, 'Activo'),
(40000008,'Jorge','García','M','1975-01-20','3415423456','jgarcia@email.com',1, 'Inactivo');
GO

-- 11. ANÁLISIS ------------------------------------------
INSERT INTO analisis (id_tipo_analisis, id_estado, dni_paciente, observaciones, dni_carga, dni_firma, fecha_creacion, fecha_firma) VALUES
(1, 2, 40000005, 'Control anual', 30000002, 30000002, GETDATE()-2, GETDATE()-1), -- Estado: Verificado
(2, 1, 40000006, 'Pre-operatorio', 30000002, NULL, GETDATE()-1, NULL),           -- Estado: Sin verificar
(3, 1, 40000007, 'Chequeo general', 30000003, NULL, GETDATE(), NULL),            -- Estado: Sin verificar
(1, 3, 40000005, 'Duplicado por error', 30000003, NULL, GETDATE()-3, NULL);      -- Estado: Anulado (BAJA LÓGICA)
GO

-- 12. RESULTADOS DE ANÁLISIS ----------------------------
INSERT INTO analisis_metrica (id_analisis, id_metrica, resultado) VALUES
-- Resultados para el Análisis 1 (Hemograma de Ana López) -> Métricas 6, 7, 8
(1, 6, 13.2), (1, 7, 5.8), (1, 8, 250),
-- Resultados para el Análisis 2 (Glucosa de Luis Martínez) -> Métrica 1
(2, 1, 95),
-- Resultados para el Análisis 3 (Perfil Lipídico de Sofía Fernández) -> Métricas 2, 3, 4, 5
(3, 2, 180), (3, 3, 45), (3, 4, 110), (3, 5, 125);
-- NOTA: El análisis anulado (ID 4) no tiene resultados cargados.
GO

PRINT 'Lote de datos de ejemplo v2.9 (con passwords) cargado correctamente.';
GO


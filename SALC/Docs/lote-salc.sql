/* ====================================================================
   SALC – LOTE DE DATOS DE EJEMPLO – v2.7
   - Adaptado a la estructura de BD normalizada (usuarios/medicos/asistentes).
   - Contraseñas no hasheadas (ejemplo BCrypt es lo que deberíamos usar).
   - Eliminadas entidades obsoletas (doctor_externo, estado_usuario).
   - Orden de ejecución: Catálogos -> Usuarios -> Pacientes -> Análisis.
   ==================================================================== */
USE [SALC];
GO

-- 1. ROLES ----------------------------------------------
-- Los roles deben coincidir con la lógica de la aplicación.
INSERT INTO roles (nombre_rol) VALUES
('Administrador'),
('Médico'),
('Asistente');
GO

-- 2. ESTADOS DE ANÁLISIS ---------------------------------
INSERT INTO estados_analisis (descripcion) VALUES
('Sin verificar'),
('Verificado');
GO

-- 3. OBRAS SOCIALES -------------------------------------
INSERT INTO obras_sociales (cuit, nombre) VALUES
('30500000010','OSDE'),
('30650000020','Swiss Medical'),
('30990000040','Sin Obra Social');
GO

-- 4. TIPOS DE ANÁLISIS ----------------------------------
INSERT INTO tipos_analisis (descripcion) VALUES
('Hemograma Completo'),
('Glucosa en Ayunas'),
('Perfil Lipídico Completo'),
('Análisis de Orina Completo');
GO

-- 5. MÉTRICAS -------------------------------------------
INSERT INTO metricas (nombre, unidad_medida, valor_minimo, valor_maximo) VALUES
('Glucosa','mg/dL',70,110),
('Colesterol Total','mg/dL',0,200),
('HDL','mg/dL',40,60),
('LDL','mg/dL',0,100),
('Triglicéridos','mg/dL',0,150),
('Hemoglobina','g/dL',12,18),
('Leucocitos','10³/µL',4,11),
('Plaquetas','10³/µL',150,450);
GO

-- 6. USUARIOS (Datos comunes) ---------------------------
-- Las contraseñas son un HASH generado por la aplicación (ej. BCrypt).
-- El texto es solo un ejemplo, no la contraseña real.
INSERT INTO usuarios (dni, nombre, apellido, id_rol, email, password_hash, estado) VALUES
--Contrseña plana: "9ffe8/5<7X1}"
(30000001,'Juan','Pérez',1,'jperez@lab.com','$2a$12$URP1nbn2iSYn5/cEFwcaMeN8N.8SR1TaL3FMwFvYthH6c7DAfxWWm', 'Activo'),     -- Rol: Administrador
-- Contraseña plana: "5kA5GM^087C`"
(30000002,'María','González',2,'mgonzalez@lab.com','$2a$12$xR2X4M31NjPdlYXefWGhVO/mhEY4wEcHGtrIT09zPWFyjcH49k7.q', 'Activo'),  -- Rol: Médico
-- Contraseña plana: "0Z1AiD'-65-*"
(30000003,'Sofía','Gundisalvo',2,'sofiagun@lab.com','$2a$12$o7V3ylw3QSIO2qk2QagJKubqaJFVoomZv3lTo3mVjeUuCTz6xTUsO', 'Activo'),   -- Rol: Médico
-- Contraseña plana: "&8A2Ve9&#Xmi"
(30000004,'Carlos','Ramírez',3,'cramirez@lab.com','$2a$12$y98vRL/bjbV.U2.DPSpciOLaqSBPNxQk.0uY.fBe.FZDdP3KVKm8a', 'Activo'),     -- Rol: Asistente
-- Contraseña plana: "GGeI((h71485"
(30000005,'Erika','Miralles',3,'erikamir@lab.com','$2a$12$PTg6.zjtrOe1NcGegfAl/elM95qtmTOEmHrAetH39m7lQw5wXKq9.', 'Activo');      -- Rol: Asistente
GO

-- 7. MEDICOS (Datos específicos) ------------------------
INSERT INTO medicos (dni, nro_matricula, especialidad) VALUES
(30000002, 25478, 'Clínica Médica'),
(30000003, 36589, 'Bioquímica Clínica');
GO

-- 8. ASISTENTES (Datos específicos) ---------------------
INSERT INTO asistentes (dni, dni_supervisor, fecha_ingreso) VALUES
(30000004, 30000002, GETDATE()), -- Supervisado por María González
(30000005, 30000003, GETDATE()); -- Supervisado por Sofía Gundisalvo
GO

-- 9. PACIENTES ------------------------------------------
INSERT INTO pacientes (dni, nombre, apellido, sexo, fecha_nac, telefono, email, id_obra_social) VALUES
(40000005,'Ana','López','F','1985-06-15','3415123456','analopez@gmail.com',1),
(40000006,'Luis','Martínez','M','1990-11-22','3415223456','luis.martinez@gmail.com',2),
(40000007,'Sofía','Fernández','F','1978-03-08','3415323456','sofia.fernandez@gmail.com',3);
GO

-- 10. ANÁLISIS ------------------------------------------
-- dni_carga es el médico que crea el análisis.
-- dni_firma es el médico que lo valida (solo para estados 'Verificado').
INSERT INTO analisis (id_tipo_analisis, id_estado, dni_paciente, observaciones, dni_carga, dni_firma, fecha_creacion, fecha_firma) VALUES
(1, 2, 40000005, 'Control anual', 30000002, 30000002, GETDATE()-2, GETDATE()-1), -- Creado y Firmado por María
(2, 1, 40000006, 'Pre-operatorio', 30000002, NULL, GETDATE()-1, NULL),           -- Creado por María, pendiente de firma
(3, 1, 40000007, 'Chequeo general', 30000003, NULL, GETDATE(), NULL);            -- Creado por Sofía, pendiente de firma
GO

-- 11. RESULTADOS DE ANÁLISIS ----------------------------
INSERT INTO analisis_metrica (id_analisis, id_metrica, resultado) VALUES
-- Resultados para el Análisis 1 (Hemograma de Ana López)
(1, 6, 13.2), -- Hemoglobina
(1, 7, 5.8),  -- Leucocitos
(1, 8, 250),  -- Plaquetas
-- Resultados para el Análisis 2 (Glucosa de Luis Martínez)
(2, 1, 95),   -- Glucosa
-- Resultados para el Análisis 3 (Perfil Lipídico de Sofía Fernández)
(3, 2, 180),  -- Colesterol Total
(3, 3, 45),   -- HDL
(3, 4, 110),  -- LDL
(3, 5, 125);  -- Triglicéridos
GO

PRINT 'Lote de datos de ejemplo cargado correctamente en la base de datos SALC.';
GO

-- ========================================================
-- Script para verificar contraseñas que necesitan migración
-- Este script NO modifica la base de datos, solo muestra qué usuarios necesitan migración
-- Las contraseñas se migrarán automáticamente cuando los usuarios inicien sesión
-- ========================================================

USE SALC;
GO

-- Mostrar usuarios con contraseñas en texto plano (que NO comienzan con $2)
SELECT 
    dni,
    nombre,
    apellido,
    email,
    CASE 
        WHEN password_hash LIKE '$2a$%' OR 
             password_hash LIKE '$2b$%' OR 
             password_hash LIKE '$2x$%' OR 
             password_hash LIKE '$2y$%' 
        THEN 'BCrypt Hash ?'
        ELSE 'TEXTO PLANO - Necesita migración'
    END AS estado_password,
    LEN(password_hash) AS longitud_hash
FROM usuarios
ORDER BY 
    CASE 
        WHEN password_hash LIKE '$2a$%' OR 
             password_hash LIKE '$2b$%' OR 
             password_hash LIKE '$2x$%' OR 
             password_hash LIKE '$2y$%' 
        THEN 1
        ELSE 0
    END,
    dni;

-- Resumen
SELECT 
    CASE 
        WHEN password_hash LIKE '$2a$%' OR 
             password_hash LIKE '$2b$%' OR 
             password_hash LIKE '$2x$%' OR 
             password_hash LIKE '$2y$%' 
        THEN 'Con BCrypt Hash'
        ELSE 'En Texto Plano'
    END AS tipo,
    COUNT(*) AS cantidad
FROM usuarios
GROUP BY 
    CASE 
        WHEN password_hash LIKE '$2a$%' OR 
             password_hash LIKE '$2b$%' OR 
             password_hash LIKE '$2x$%' OR 
             password_hash LIKE '$2y$%' 
        THEN 'Con BCrypt Hash'
        ELSE 'En Texto Plano'
    END;

PRINT '';
PRINT '========================================================';
PRINT 'NOTA IMPORTANTE:';
PRINT 'Las contraseñas en texto plano se migrarán automáticamente';
PRINT 'a BCrypt cuando cada usuario inicie sesión.';
PRINT '';
PRINT 'El sistema soporta ambos formatos durante la migración.';
PRINT '========================================================';
GO

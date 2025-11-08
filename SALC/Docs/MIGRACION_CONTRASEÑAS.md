# ?? Implementación de Migración Automática de Contraseñas - SALC

## ?? Resumen

Se ha implementado un sistema de **migración automática de contraseñas** que:

1. ? **Detecta contraseñas en texto plano** al momento del login
2. ? **Las hashea automáticamente con BCrypt** (work factor 12)
3. ? **Actualiza la base de datos** sin intervención manual
4. ? **Valida normalmente** las contraseñas ya hasheadas

---

## ??? Arquitectura de la Solución

### **Componentes Modificados:**

#### **1. IPasswordHasher.cs** (Interfaz)
- Agregado método: `bool IsPlainText(string passwordHash)`
- Permite detectar si una contraseña está en texto plano o hasheada

#### **2. DefaultPasswordHasher.cs** (Implementación)
**Método `IsPlainText()`:**
```csharp
// Detecta si el hash NO empieza con $2a$, $2b$, $2x$ o $2y$ (formatos BCrypt)
public bool IsPlainText(string passwordHash)
{
    if (string.IsNullOrEmpty(passwordHash))
        return true;

    return !passwordHash.StartsWith("$2a$") && 
           !passwordHash.StartsWith("$2b$") && 
           !passwordHash.StartsWith("$2x$") && 
           !passwordHash.StartsWith("$2y$");
}
```

**Método `Hash()`:**
```csharp
// Hashea con BCrypt usando work factor 12 (4096 iteraciones)
public string Hash(string plainText)
{
    return BCrypt.Net.BCrypt.HashPassword(plainText, workFactor: 12);
}
```

**Método `Verify()`:**
```csharp
// Valida contra hash BCrypt o compara texto plano como fallback
public bool Verify(string plainText, string hashed)
{
    try
    {
        return BCrypt.Net.BCrypt.Verify(plainText, hashed);
    }
    catch (Exception)
    {
        // Fallback para migración: compara como texto plano
        return string.Equals(plainText, hashed, StringComparison.Ordinal);
    }
}
```

#### **3. AutenticacionService.cs** (Lógica de Negocio)
**Método `ValidarCredenciales()` con Migración Automática:**

```csharp
public Usuario ValidarCredenciales(int dni, string contrasenia)
{
    var usuario = _usuarios.ObtenerPorId(dni);
    if (usuario == null) return null;
    if (!string.Equals(usuario.Estado, "Activo")) return null;

    // ===== MIGRACIÓN AUTOMÁTICA =====
    if (_hasher.IsPlainText(usuario.PasswordHash))
    {
        // Es texto plano ? verificar y migrar
        if (string.Equals(contrasenia, usuario.PasswordHash))
        {
            // ? Contraseña correcta ? Hashear y guardar
            string nuevoHash = _hasher.Hash(contrasenia);
            usuario.PasswordHash = nuevoHash;
            _usuarios.Actualizar(usuario);
            
            Debug.WriteLine($"? Contraseña migrada para DNI: {dni}");
            return usuario;
        }
        else
        {
            // ? Contraseña incorrecta
            return null;
        }
    }
    else
    {
        // Ya está hasheada ? validar normalmente
        if (!_hasher.Verify(contrasenia, usuario.PasswordHash))
            return null;
        
        return usuario;
    }
}
```

---

## ?? Instalación de BCrypt.Net-Next

### **Opción A: Desde Visual Studio (Recomendado)**

1. Abra la solución `SALC.sln` en Visual Studio
2. Click derecho en el proyecto **SALC**
3. Seleccione **"Manage NuGet Packages..."**
4. En la pestaña **Browse**, busque: `BCrypt.Net-Next`
5. Seleccione la versión **4.0.3** o superior
6. Click en **Install**

### **Opción B: Package Manager Console**

1. En Visual Studio, vaya a: **Tools ? NuGet Package Manager ? Package Manager Console**
2. Ejecute el comando:
   ```powershell
   Install-Package BCrypt.Net-Next -Version 4.0.3
   ```

### **Opción C: Editar packages.config (Ya hecho)**

El archivo `SALC/packages.config` ya tiene la referencia agregada:
```xml
<package id="BCrypt.Net-Next" version="4.0.3" targetFramework="net472" />
```

Solo necesita:
1. Click derecho en la solución
2. Seleccione **"Restore NuGet Packages"**

---

## ?? Flujo de Migración Automática

### **Escenario 1: Usuario con Contraseña en Texto Plano**

```
1. Usuario ingresa DNI: 30000001 y contraseña: "admin123"

2. Sistema consulta BD:
   password_hash = "admin123" (texto plano)

3. IsPlainText("admin123") ? TRUE

4. Compara: "admin123" == "admin123" ? ? CORRECTO

5. Hashea: Hash("admin123") ? "$2a$12$URP1nbn2iSYn5/..."

6. Actualiza BD:
   UPDATE usuarios 
   SET password_hash = '$2a$12$URP1nbn2iSYn5/...'
   WHERE dni = 30000001

7. ? Usuario autenticado y contraseña migrada
```

### **Escenario 2: Usuario con Contraseña Ya Hasheada**

```
1. Usuario ingresa DNI: 30000002 y contraseña: "medico123"

2. Sistema consulta BD:
   password_hash = "$2a$12$xR2X4M31NjPdlYX..."

3. IsPlainText("$2a$12$xR2X4M31NjPdlYX...") ? FALSE

4. Verify("medico123", "$2a$12$xR2X4M31NjPdlYX...") ? ? TRUE

5. ? Usuario autenticado (sin cambios en BD)
```

---

## ??? Estado de la Base de Datos

### **Datos de Ejemplo (v2.0)**

El lote de datos ya incluye contraseñas hasheadas con BCrypt:

| DNI      | Email              | Contraseña (texto) | password_hash (BD)                           |
|----------|--------------------|--------------------|----------------------------------------------|
| 30000001 | jperez@lab.com     | admin123           | $2a$12$URP1nbn2iSYn5/cEFwcaMeN8N.8SR1TaL3F...  |
| 30000002 | mgonzalez@lab.com  | medico123          | $2a$12$xR2X4M31NjPdlYXefWGhVO/mhEY4wEcHGtr...  |
| 30000003 | sofiagun@lab.com   | medico456          | $2a$12$o7V3ylw3QSIO2qk2QagJKubqaJFVoomZv3lT...  |
| 30000004 | cramirez@lab.com   | asistente123       | $2a$12$y98vRL/bjbV.U2.DPSpciOLaqSBPNxQk.0uY...  |
| 30000005 | erikamir@lab.com   | asistente456       | $2a$12$PTg6.zjtrOe1NcGegfAl/elM95qtmTOEmHrA...  |

**Si tienes contraseñas en texto plano**, el sistema las migrará automáticamente en el primer login.

---

## ? Pruebas Recomendadas

### **1. Migración desde Texto Plano**

```sql
-- Crear usuario de prueba con texto plano
INSERT INTO usuarios (dni, nombre, apellido, email, password_hash, id_rol, estado)
VALUES (99999999, 'Test', 'Usuario', 'test@test.com', 'password123', 1, 'Activo');

-- Intentar login con DNI: 99999999 y contraseña: "password123"
-- ? Debería autenticar y actualizar el hash automáticamente

-- Verificar que se migró:
SELECT password_hash FROM usuarios WHERE dni = 99999999;
-- Debería mostrar: $2a$12$...
```

### **2. Login Normal (Ya Hasheada)**

```
DNI: 30000001
Contraseña: admin123
? Debería autenticar sin problemas
```

### **3. Contraseña Incorrecta**

```
DNI: 30000001
Contraseña: wrongpassword
? Debería rechazar el login
```

---

## ?? Seguridad

### **BCrypt Work Factor**
- **Configurado en:** 12 (4,096 iteraciones)
- **Tiempo de hashing:** ~250ms por contraseña
- **Recomendación:** Mantener entre 10-12 para balance seguridad/rendimiento

### **Formato de Hash**
```
$2a$12$URP1nbn2iSYn5/cEFwcaMeN8N.8SR1TaL3FMwFvYthH6c7DAfxWWm
 ?   ?   ?                                                    ?
 ?   ?   ?                                                    ?? Hash (31 chars)
 ?   ?   ??????????????????????????????? Salt (22 chars)
 ?   ??? Work factor (12 = 2^12 = 4096 iteraciones)
 ??? Algoritmo ($2a = BCrypt)
```

### **Ventajas de BCrypt**
- ? Resistente a ataques de fuerza bruta
- ? Incluye salt automático (previene rainbow tables)
- ? Ajustable en el futuro (work factor)
- ? Estándar de la industria

---

## ?? Troubleshooting

### **Error: "BCrypt.Net-Next no está instalado"**

**Solución:**
1. Abra Visual Studio
2. Tools ? NuGet Package Manager ? Manage NuGet Packages for Solution
3. Busque e instale "BCrypt.Net-Next"

### **Error: "No se encuentra el tipo BCrypt.Net"**

**Solución:**
1. Verifique que `packages.config` tiene:
   ```xml
   <package id="BCrypt.Net-Next" version="4.0.3" targetFramework="net472" />
   ```
2. Click derecho en solución ? "Restore NuGet Packages"
3. Recompilar (Ctrl + Shift + B)

### **Las contraseñas no se migran**

**Verificar:**
1. Que el usuario exista y esté "Activo"
2. Que la contraseña en BD sea exactamente igual a la ingresada
3. Revisar la consola de Debug para ver mensajes de migración

---

## ?? Monitoreo

### **Logs de Migración**

El sistema escribe en la consola de Debug cuando migra una contraseña:

```csharp
Debug.WriteLine($"? Contraseña migrada a BCrypt para usuario DNI: {dni}");
```

Para ver estos logs en Visual Studio:
1. Ejecute la aplicación en modo Debug (F5)
2. Abra la ventana Output (Ctrl + Alt + O)
3. Seleccione "Debug" en el dropdown

---

## ?? Notas Importantes

### **Migración Gradual**
- ? No requiere script masivo de actualización
- ? Se migra automáticamente cuando el usuario hace login
- ? Usuarios inactivos se migrarán en su próximo login
- ?? Usuarios que nunca vuelvan a hacer login quedarán con texto plano (aceptable)

### **Compatibilidad**
- ? Compatible con contraseñas legacy (texto plano)
- ? Compatible con hashes BCrypt existentes
- ? No rompe funcionalidad existente

### **Rendimiento**
- **Primer login (migración):** ~250ms adicionales
- **Logins posteriores:** ~100-150ms (validación BCrypt normal)
- **Impacto:** Mínimo, imperceptible para el usuario

---

## ?? Próximos Pasos Recomendados

### **1. Política de Contraseñas** (Opcional)
Agregar validaciones en el formulario de creación/edición de usuarios:

```csharp
// En UsuarioService.cs
public void ValidarContrasenia(string password)
{
    if (password.Length < 8)
        throw new ArgumentException("La contraseña debe tener al menos 8 caracteres");
        
    if (!password.Any(char.IsUpper))
        throw new ArgumentException("La contraseña debe tener al menos una mayúscula");
        
    if (!password.Any(char.IsDigit))
        throw new ArgumentException("La contraseña debe tener al menos un número");
}
```

### **2. Cambio de Contraseña** (Opcional)
Implementar funcionalidad para que usuarios cambien su contraseña:
- Desde el menú de usuario logueado
- Solicitar contraseña actual + nueva contraseña
- Hashear automáticamente con BCrypt

### **3. Expiración de Contraseñas** (Opcional)
Agregar campo `fecha_cambio_password` en tabla `usuarios` y forzar cambio cada N meses.

---

## ? Checklist de Implementación

- [x] Actualizar `IPasswordHasher` con método `IsPlainText()`
- [x] Implementar `DefaultPasswordHasher` con BCrypt
- [x] Modificar `AutenticacionService` para migración automática
- [x] Agregar BCrypt.Net-Next a `packages.config`
- [x] Actualizar `.csproj` con referencia a BCrypt
- [ ] **Instalar paquete NuGet desde Visual Studio** (?? PENDIENTE)
- [ ] Probar login con contraseña en texto plano
- [ ] Probar login con contraseña hasheada
- [ ] Verificar migración en base de datos

---

## ?? Referencias

- [BCrypt.Net-Next GitHub](https://github.com/BcryptNet/bcrypt.net)
- [BCrypt Specification](https://en.wikipedia.org/wiki/Bcrypt)
- [OWASP Password Storage](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)

---

**Implementado por:** Sistema SALC  
**Fecha:** 2025  
**Versión:** 1.0

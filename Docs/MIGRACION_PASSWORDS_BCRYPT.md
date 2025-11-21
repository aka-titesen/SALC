# Migración Automática de Contraseñas a BCrypt

## ?? Resumen

El sistema SALC ahora incluye **migración automática** de contraseñas desde texto plano a hashes BCrypt seguros.

---

## ? Funcionalidades Implementadas

### 1. **Detección Inteligente de Formato**
- El sistema detecta automáticamente si una contraseña está hasheada o en texto plano
- Método `IsPlainText()` verifica los prefijos BCrypt válidos: `$2a$`, `$2b$`, `$2x$`, `$2y$`

### 2. **Migración Automática en Login**
- Cuando un usuario inicia sesión con contraseña en texto plano:
  1. ? Se valida la contraseña (comparación directa)
  2. ? Se genera un hash BCrypt de la contraseña
  3. ? Se actualiza automáticamente en la base de datos
  4. ? Se registra en logs la migración exitosa
  5. ? El usuario continúa sin interrupciones

### 3. **Hasheo Automático en Creación/Actualización**
- Al crear usuarios nuevos: contraseña se hashea automáticamente
- Al actualizar usuarios: solo se hashea si está en texto plano
- Previene doble hasheo accidental

### 4. **Verificación Dual**
```csharp
public bool Verify(string plainText, string hashed)
{
    // Si es hash BCrypt ? usa BCrypt.Verify()
    if (hashed.StartsWith("$2a$") || hashed.StartsWith("$2b$") || ...)
        return BCrypt.Net.BCrypt.Verify(plainText, hashed);
    
    // Si es texto plano ? comparación directa (solo para migración)
    else
        return string.Equals(plainText, hashed);
}
```

---

## ?? Archivos Modificados

### 1. `BLL\AutenticacionService.cs`
**Cambios:**
- Detecta contraseñas en texto plano durante el login
- Migra automáticamente a BCrypt tras validación exitosa
- Registra migración en logs

**Código clave:**
```csharp
// Verificar si necesita migración
if (esValida && _hasher.IsPlainText(usuario.PasswordHash))
{
    necesitaMigracion = true;
    ExceptionHandler.LogInfo($"Contraseña en texto plano detectada...", "Autenticación");
}

// Migrar a BCrypt
if (necesitaMigracion)
{
    var hashNuevo = _hasher.Hash(contrasenia);
    usuario.PasswordHash = hashNuevo;
    _usuarioRepo.Actualizar(usuario);
}
```

### 2. `BLL\UsuarioService.cs`
**Cambios:**
- Usa `_hasher.IsPlainText()` en lugar de `.StartsWith("$2")`
- Evita doble hasheo
- Log cuando hashea contraseñas

**Código clave:**
```csharp
// Hashear solo si es texto plano
if (!string.IsNullOrEmpty(usuario.PasswordHash) && _hasher.IsPlainText(usuario.PasswordHash))
{
    ExceptionHandler.LogInfo($"Hasheando contraseña...", "ActualizarUsuario");
    usuario.PasswordHash = _hasher.Hash(usuario.PasswordHash);
}
```

### 3. `BLL\DefaultPasswordHasher.cs`
**Cambios:**
- Método `Verify()` mejorado con soporte dual
- Verificación explícita de prefijos BCrypt
- Comentarios de advertencia sobre texto plano

**Código clave:**
```csharp
public bool Verify(string plainText, string hashed)
{
    if (hashed.StartsWith("$2a$") || hashed.StartsWith("$2b$") || 
        hashed.StartsWith("$2x$") || hashed.StartsWith("$2y$"))
    {
        return BCrypt.Net.BCrypt.Verify(plainText, hashed);
    }
    else
    {
        // Comparación directa para migración
        return string.Equals(plainText, hashed, StringComparison.Ordinal);
    }
}
```

---

## ?? Casos de Uso

### Caso 1: Usuario Existente con Contraseña en Texto Plano
**Escenario:** Usuario con DNI `30100101` y password `"salc123"` en texto plano en la BD

**Flujo:**
1. Usuario ingresa DNI: `30100101` y contraseña: `salc123`
2. Sistema busca usuario en BD
3. Detecta que `password_hash = "salc123"` (texto plano)
4. Valida: `"salc123" == "salc123"` ? ? válido
5. Genera hash: `$2a$12$...` (BCrypt)
6. Actualiza BD: `password_hash = "$2a$12$..."`
7. **Log:** `"Contraseña migrada a BCrypt exitosamente - DNI: 30100101"`
8. Usuario ingresa al sistema normalmente

### Caso 2: Creación de Usuario Nuevo
**Escenario:** Administrador crea usuario con contraseña `"nuevaPass123"`

**Flujo:**
1. Administrador completa formulario con password: `"nuevaPass123"`
2. `UsuarioService.CrearUsuario()` ejecuta
3. Detecta: `IsPlainText("nuevaPass123")` ? `true`
4. Hashea: `Hash("nuevaPass123")` ? `"$2a$12$..."`
5. Inserta en BD con hash BCrypt
6. **Resultado:** Usuario creado con contraseña segura desde el inicio

### Caso 3: Actualización de Usuario
**Escenario:** Cambiar email de un usuario sin cambiar contraseña

**Flujo:**
1. Usuario ya tiene hash BCrypt: `"$2a$12$abc..."`
2. Al actualizar: `IsPlainText("$2a$12$abc...")` ? `false`
3. **NO se hashea nuevamente** (evita corrupción)
4. Solo se actualiza el email
5. Contraseña permanece intacta

---

## ?? Verificación de Estado

### Script SQL para Diagnóstico
Ubicación: `Database/05-migrar-passwords-bcrypt.sql`

Ejecutar para ver:
- Usuarios con contraseñas en texto plano
- Usuarios con BCrypt hash
- Resumen de cantidades

**Ejemplo de salida:**
```
dni      | nombre  | estado_password
---------|---------|---------------------------
30100101 | Carlos  | TEXTO PLANO - Necesita migración
30100102 | Ana     | BCrypt Hash ?
25111112 | Laura   | BCrypt Hash ?

Resumen:
Con BCrypt Hash: 2
En Texto Plano: 1
```

---

## ?? Seguridad

### Fortalezas
? Hashes BCrypt con work factor 12 (muy seguro)
? Migración transparente sin interrumpir usuarios
? Logging completo de migraciones
? No expone contraseñas en logs
? Previene doble hasheo

### Consideraciones
?? Durante la migración, el sistema acepta texto plano solo para validación
?? Una vez migrado, solo acepta BCrypt
?? No hay rollback automático (migración es permanente)

---

## ?? Logs

### Ejemplo de Logs Generados

**Login exitoso con migración:**
```
[INFO] Intento de autenticación para DNI: 30100101
[INFO] Contraseña en texto plano detectada para DNI: 30100101. Se migrará automáticamente.
[INFO] Contraseña migrada a BCrypt exitosamente - DNI: 30100101
[INFO] Autenticación exitosa - DNI: 30100101, Rol: 2
```

**Actualización de usuario:**
```
[INFO] Hasheando contraseña para usuario - DNI: 40100201
[INFO] Actualizando usuario - DNI: 40100201, Rol: 3
[INFO] Usuario actualizado exitosamente - DNI: 40100201
```

---

## ?? Testing

### Pruebas Recomendadas

1. **Login con texto plano:**
   - Usuario: `30100101`, Password: `salc123`
   - Verificar migración en logs
   - Verificar segundo login (ya con BCrypt)

2. **Crear usuario nuevo:**
   - Crear con password: `"test123"`
   - Verificar hash en BD comienza con `$2`
   - Verificar login exitoso

3. **Actualizar sin cambiar password:**
   - Editar email de usuario
   - Verificar que hash no cambia

4. **Actualizar cambiando password:**
   - Cambiar a `"newPass456"`
   - Verificar nuevo hash BCrypt
   - Verificar login con nueva password

---

## ? Checklist de Validación

- [ ] Todos los usuarios de prueba pueden iniciar sesión
- [ ] Contraseñas en texto plano se migran automáticamente
- [ ] Usuarios nuevos se crean con BCrypt desde el inicio
- [ ] Logs muestran migraciones exitosas
- [ ] No hay doble hasheo en actualizaciones
- [ ] `IsPlainText()` detecta correctamente formato BCrypt
- [ ] Script SQL muestra estado actual de passwords

---

## ?? Despliegue

### Pasos para Producción

1. **Backup de Base de Datos:**
   ```sql
   BACKUP DATABASE SALC TO DISK = 'C:\Backups\SALC_pre_migracion.bak'
   ```

2. **Compilar y Desplegar:**
   ```
   Build ? Rebuild Solution
   ```

3. **Verificar Logs:**
   - Revisar `Logs\salc.log` después del primer login de cada usuario

4. **Monitorear Migración:**
   - Ejecutar script SQL de verificación periódicamente
   - Confirmar que todos los usuarios migran exitosamente

---

## ?? Soporte

Si encuentras problemas:

1. Revisa los logs en `Logs\salc.log`
2. Ejecuta el script SQL de diagnóstico
3. Verifica que BCrypt.Net-Next v4.0.3 esté instalado
4. Consulta la sección de errores comunes abajo

### Errores Comunes

**Error: "BCrypt.Net-Next no está instalado"**
- Solución: `Install-Package BCrypt.Net-Next -Version 4.0.3`

**Error: "Error al validar contraseña"**
- Verificar que la contraseña en BD no esté corrupta
- Revisar logs para más detalles

**Usuarios no pueden iniciar sesión:**
- Ejecutar script SQL para verificar formato de passwords
- Revisar que `AutenticacionService` esté usando el hasher correcto

---

## ?? Referencias

- BCrypt.Net-Next: https://github.com/BcryptNet/bcrypt.net
- BCrypt Algorithm: https://en.wikipedia.org/wiki/Bcrypt
- OWASP Password Storage: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html

---

**Fecha de Implementación:** 2025-01-03
**Versión:** SALC v1.0
**Estado:** ? Producción Ready

# Sistema de Gestión de Excepciones - SALC

## Visión General

El sistema SALC ahora cuenta con un sistema robusto y completo de gestión de excepciones que incluye:

1. **Jerarquía de Excepciones Personalizadas**
2. **Sistema de Logging**
3. **Manejo Centralizado de Excepciones**
4. **Mensajes Amigables para el Usuario**

## 1. Jerarquía de Excepciones

### SalcException (Base)
Excepción base para todas las excepciones personalizadas del sistema.

**Propiedades:**
- `ErrorCode`: Código único de error
- `UserFriendlyMessage`: Mensaje amigable para mostrar al usuario
- `ShouldLog`: Indica si debe registrarse en el log

**Uso:**
```csharp
throw new SalcException(
    "Error técnico detallado",
    "Mensaje amigable para el usuario",
    "ERROR_CODE"
);
```

### SalcValidacionException
Para errores de validación de datos o entrada del usuario.

**Propiedades adicionales:**
- `Campo`: Campo que falló la validación

**Uso:**
```csharp
if (string.IsNullOrWhiteSpace(nombre))
{
    throw new SalcValidacionException("El nombre es obligatorio.", "nombre");
}
```

### SalcDatabaseException
Para errores relacionados con la base de datos.

**Propiedades adicionales:**
- `SqlErrorNumber`: Número de error de SQL Server
- `Operacion`: Nombre de la operación que falló

**Características:**
- Convierte automáticamente códigos de error SQL en mensajes amigables
- Mapea errores comunes (timeout, conexión, constraints, etc.)

**Uso:**
```csharp
try
{
    // Operación de BD
}
catch (SqlException sqlEx)
{
    throw new SalcDatabaseException("Error al guardar datos", "GuardarPaciente", sqlEx);
}
```

### SalcBusinessException
Para violaciones de reglas de negocio.

**Uso:**
```csharp
if (analisis.IdEstado == EstadoVerificado)
{
    throw new SalcBusinessException("No se puede modificar un análisis verificado.");
}
```

### SalcAuthorizationException
Para errores de permisos o autorización.

**Propiedades adicionales:**
- `Operacion`: Operación a la que se intentó acceder
- `RolRequerido`: Rol necesario para la operación

**Uso:**
```csharp
if (usuario.IdRol != RolAdministrador)
{
    throw new SalcAuthorizationException("Eliminar Usuario", "Administrador");
}
```

## 2. Sistema de Logging

### Logger (Singleton)

Sistema de logging ligero basado en archivos de texto.

**Características:**
- Archivos diarios en formato: `SALC_yyyyMMdd.log`
- Ubicación: `[Directorio App]/Logs/`
- Thread-safe (usando locks)
- Rotación automática diaria
- Niveles de severidad: Debug, Info, Warning, Error, Fatal

**Uso:**
```csharp
using SALC.Infraestructura.Logging;

var logger = Logger.Instance;

logger.Info("Usuario autenticado correctamente");
logger.Warning("Intentos de acceso fallidos: 3");
logger.Error("Error al conectar con BD", exception);
logger.Fatal("Error crítico del sistema", exception);
logger.Debug("Valor de variable X: " + x);
```

**Formato de Log:**
```
[2025-01-20 14:30:45.123] [INFO] Usuario autenticado correctamente
[2025-01-20 14:30:50.456] [ERROR] Error al crear análisis
Exception: SqlException
Message: Cannot insert duplicate key
StackTrace: ...
```

**Limpieza de Logs:**
```csharp
// Eliminar logs de más de 30 días
Logger.Instance.LimpiarLogsAntiguos(30);
```

## 3. ExceptionHandler (Centralizado)

Clase estática para manejo centralizado de excepciones en toda la aplicación.

### Método Principal: ManejarExcepcion

```csharp
using SALC.Infraestructura;

try
{
    // Código que puede fallar
}
catch (Exception ex)
{
    ExceptionHandler.ManejarExcepcion(ex, "NombreOperacion", mostrarAlUsuario: true);
}
```

**Parámetros:**
- `ex`: Excepción a manejar
- `contexto`: Nombre de la operación/contexto (para logs)
- `mostrarAlUsuario`: Si debe mostrar MessageBox al usuario

**Funcionalidad:**
1. Identifica el tipo de excepción
2. Genera mensaje técnico para logs
3. Genera mensaje amigable para usuario
4. Registra en el log si corresponde
5. Muestra MessageBox si se solicita
6. Retorna el mensaje amigable

### Métodos Especializados

#### Validación
```csharp
ExceptionHandler.ManejarValidacion("El DNI debe ser numérico", "dni");
```

#### Error de Base de Datos
```csharp
catch (SqlException sqlEx)
{
    ExceptionHandler.ManejarErrorBaseDatos(sqlEx, "CrearPaciente");
}
```

#### Regla de Negocio
```csharp
ExceptionHandler.ManejarReglaNegocio("El análisis ya fue verificado");
```

#### No Autorizado
```csharp
ExceptionHandler.ManejarNoAutorizado("Eliminar Usuario", "Administrador");
```

### Logging Directo

```csharp
ExceptionHandler.LogInfo("Operación completada exitosamente", "GuardarPaciente");
ExceptionHandler.LogWarning("Valor fuera de rango detectado", "ValidarMetrica");
ExceptionHandler.LogDebug("Variable X = " + x, "CalcularResultado");
```

### Ejecutar con Manejo Automático

```csharp
// Para acciones (void)
ExceptionHandler.EjecutarConManejo(() => 
{
    // Código que puede fallar
    servicio.GuardarDatos();
}, "GuardarDatos");

// Para funciones (con retorno)
var resultado = ExceptionHandler.EjecutarConManejo(() => 
{
    return servicio.ObtenerDatos();
}, "ObtenerDatos", valorPorDefecto: null);
```

## 4. Patrones de Uso por Capa

### BLL (Business Logic Layer)

```csharp
public Paciente CrearPaciente(Paciente paciente)
{
    try
    {
        // Validaciones
        if (paciente.Dni <= 0)
            throw new SalcValidacionException("El DNI debe ser válido.", "dni");
        
        if (string.IsNullOrWhiteSpace(paciente.Nombre))
            throw new SalcValidacionException("El nombre es obligatorio.", "nombre");

        // Reglas de negocio
        var existente = _repo.ObtenerPorDni(paciente.Dni);
        if (existente != null)
            throw new SalcBusinessException("Ya existe un paciente con ese DNI.");

        ExceptionHandler.LogInfo($"Creando paciente: {paciente.Dni}", "CrearPaciente");

        // Operación
        var resultado = _repo.Crear(paciente);

        ExceptionHandler.LogInfo($"Paciente creado exitosamente: {resultado.Dni}", "CrearPaciente");

        return resultado;
    }
    catch (SqlException sqlEx)
    {
        throw new SalcDatabaseException("Error al crear paciente", "CrearPaciente", sqlEx);
    }
    catch (SalcException)
    {
        throw; // Re-lanzar excepciones SALC
    }
    catch (Exception ex)
    {
        ExceptionHandler.LogWarning($"Error inesperado: {ex.Message}", "CrearPaciente");
        throw new SalcException(
            "Error al crear paciente",
            "No se pudo crear el paciente. Por favor, intente nuevamente.",
            "CREATE_PACIENTE_ERROR"
        );
    }
}
```

### Presenters

```csharp
private void OnGuardar()
{
    try
    {
        var dni = int.Parse(_view.DniTexto);
        var paciente = new Paciente { Dni = dni, ... };

        _service.CrearPaciente(paciente);

        _view.MostrarMensaje("Paciente creado exitosamente.");
        _view.LimpiarCampos();
    }
    catch (SalcValidacionException valEx)
    {
        _view.MostrarError(valEx.UserFriendlyMessage);
    }
    catch (SalcBusinessException bizEx)
    {
        _view.MostrarError(bizEx.UserFriendlyMessage);
    }
    catch (SalcDatabaseException dbEx)
    {
        _view.MostrarError(dbEx.UserFriendlyMessage);
    }
    catch (SalcException salcEx)
    {
        _view.MostrarError(salcEx.UserFriendlyMessage);
    }
    catch (Exception ex)
    {
        var mensaje = ExceptionHandler.ManejarExcepcion(ex, "GuardarPaciente", false);
        _view.MostrarError(mensaje);
    }
}
```

### DAL (Data Access Layer)

```csharp
public void Crear(Paciente paciente)
{
    try
    {
        using (var cn = DbConexion.CrearConexion())
        using (var cmd = new SqlCommand("INSERT INTO pacientes ...", cn))
        {
            cmd.Parameters.AddWithValue("@dni", paciente.Dni);
            // ...
            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
    catch (SqlException sqlEx)
    {
        // Dejar que la capa superior (BLL) maneje el SqlException
        // o convertirlo aquí a SalcDatabaseException
        throw;
    }
}
```

## 5. Mensajes de Error SQL Mapeados

El sistema mapea automáticamente códigos de error SQL comunes:

| Código SQL | Significado | Mensaje al Usuario |
|------------|-------------|-------------------|
| -1 | Timeout | La operación tardó demasiado tiempo. Intente nuevamente. |
| 2, 53 | Error de red | No se pudo conectar al servidor. Verifique su conexión. |
| 4060 | Base de datos no disponible | La base de datos no está disponible. Contacte al administrador. |
| 18456 | Error de autenticación | Error de autenticación. Contacte al administrador. |
| 547 | Violación de clave foránea | No se puede completar porque existen registros relacionados. |
| 2627, 2601 | Violación de restricción única | Ya existe un registro con los mismos datos. |
| 8152 | Truncamiento de string | Uno de los valores ingresados es demasiado largo. |

## 6. Mejores Prácticas

### ? HACER:

1. **Usar excepciones específicas en BLL:**
```csharp
if (dni <= 0)
    throw new SalcValidacionException("DNI inválido", "dni");
```

2. **Loguear operaciones importantes:**
```csharp
ExceptionHandler.LogInfo("Análisis creado: " + id, "CrearAnalisis");
```

3. **Convertir SqlException en capas superiores:**
```csharp
catch (SqlException sqlEx)
{
    throw new SalcDatabaseException("Error en BD", "Operacion", sqlEx);
}
```

4. **Mostrar mensajes amigables:**
```csharp
_view.MostrarError(salcEx.UserFriendlyMessage);
```

5. **Re-lanzar excepciones SALC:**
```csharp
catch (SalcException)
{
    throw; // Dejar que capas superiores manejen
}
```

### ? NO HACER:

1. **No mostrar ex.Message directamente:**
```csharp
// MAL
MessageBox.Show(ex.Message);

// BIEN
MessageBox.Show(salcEx.UserFriendlyMessage);
```

2. **No tragar excepciones:**
```csharp
// MAL
catch (Exception ex)
{
    // No hacer nada
}

// BIEN
catch (Exception ex)
{
    ExceptionHandler.ManejarExcepcion(ex, "Operacion");
}
```

3. **No usar Exception genérico cuando hay uno específico:**
```csharp
// MAL
throw new Exception("DNI inválido");

// BIEN
throw new SalcValidacionException("DNI inválido", "dni");
```

## 7. Integración con Aplicación Existente

El sistema está diseñado para integrarse gradualmente:

1. **Archivos ya actualizados:**
   - `BLL/AutenticacionService.cs`
   - `BLL/AnalisisService.cs`
   - `Presenters/LoginPresenter.cs`

2. **Para actualizar otros archivos:**
   - Agregar `using SALC.Infraestructura;`
   - Agregar `using SALC.Infraestructura.Exceptions;`
   - Reemplazar bloques try-catch genéricos con el nuevo sistema
   - Agregar logging en operaciones críticas

## 8. Ejemplo Completo End-to-End

```csharp
// DAL
public Paciente Crear(Paciente p)
{
    using (var cn = DbConexion.CrearConexion())
    using (var cmd = new SqlCommand("INSERT INTO pacientes..."))
    {
        // ... parámetros
        cn.Open();
        cmd.ExecuteNonQuery();
        return p;
    }
}

// BLL
public Paciente CrearPaciente(Paciente p)
{
    try
    {
        if (p.Dni <= 0)
            throw new SalcValidacionException("DNI inválido", "dni");

        ExceptionHandler.LogInfo($"Creando paciente: {p.Dni}", "CrearPaciente");
        
        var resultado = _repo.Crear(p);
        
        ExceptionHandler.LogInfo($"Paciente creado: {resultado.Dni}", "CrearPaciente");
        
        return resultado;
    }
    catch (SqlException sqlEx)
    {
        throw new SalcDatabaseException("Error al crear paciente", "CrearPaciente", sqlEx);
    }
}

// Presenter
private void OnGuardar()
{
    try
    {
        var p = new Paciente { Dni = int.Parse(_view.Dni), ... };
        _service.CrearPaciente(p);
        _view.MostrarMensaje("Paciente creado exitosamente.");
    }
    catch (SalcException salcEx)
    {
        _view.MostrarError(salcEx.UserFriendlyMessage);
    }
    catch (Exception ex)
    {
        ExceptionHandler.ManejarExcepcion(ex, "GuardarPaciente");
    }
}
```

## 9. Archivos de Log

Los archivos de log se encuentran en: `[Directorio de la aplicación]/Logs/`

Formato de archivo: `SALC_YYYYMMDD.log`

Ejemplo:
- `SALC_20250120.log`
- `SALC_20250121.log`

**Rotación:** Se crea un archivo nuevo cada día automáticamente.

**Limpieza:** Se recomienda ejecutar periódicamente:
```csharp
Logger.Instance.LimpiarLogsAntiguos(30); // Elimina logs de más de 30 días
```

## 10. Mantenimiento

### Agregar Nuevo Tipo de Excepción

1. Crear clase en `Infraestructura/Exceptions/`:
```csharp
public class MiNuevaException : SalcException
{
    public MiNuevaException(string message) : base(message)
    {
        ErrorCode = "MI_ERROR";
        UserFriendlyMessage = message;
    }
}
```

2. Agregar al proyecto (SALC.csproj)

3. Usar en el código:
```csharp
throw new MiNuevaException("Descripción del error");
```

### Personalizar Mensajes de Error SQL

Editar `SalcDatabaseException.ObtenerMensajeAmigable()`:
```csharp
case NUEVO_CODIGO:
    return "Mensaje amigable para este error";
```

---

**Versión del Sistema:** 1.0
**Última Actualización:** Enero 2025

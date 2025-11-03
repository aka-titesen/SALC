# ?? Configuración de Credenciales SMTP

## ?? IMPORTANTE: Protección de Credenciales

Este proyecto usa **archivos de secretos separados** para evitar subir credenciales a Git.

---

## ?? Configuración (Primera Vez)

### 1. El archivo `App.secrets.config` ya existe localmente

Abre el archivo: `SALC\App.secrets.config`

### 2. Edita con tus credenciales reales

Reemplaza los valores de ejemplo:

```xml
<appSettings>
  <add key="SMTP_Usuario" value="tu-email@gmail.com" />
  <add key="SMTP_Password" value="tu-contraseña-aquí" />
</appSettings>
```

### 3. Para Gmail: Generar Contraseña de Aplicación

**Gmail requiere contraseñas de aplicación (no tu contraseña normal):**

1. **Activa verificación en 2 pasos**
   - https://myaccount.google.com/security
   - Busca "Verificación en 2 pasos" y actívala

2. **Genera contraseña de aplicación**
   - https://myaccount.google.com/apppasswords
   - Selecciona: App = "Correo", Dispositivo = "Otro"
   - Nombre: "SALC Laboratorio"
   - **Copia la contraseña de 16 caracteres** (ej: `abcd efgh ijkl mnop`)

3. **Pega en App.secrets.config**
   ```xml
   <add key="SMTP_Usuario" value="tucorreo@gmail.com" />
   <add key="SMTP_Password" value="abcd efgh ijkl mnop" />
   ```

---

## ?? Seguridad Garantizada

### ? Archivo `App.secrets.config`:
- ? **NO** se sube a GitHub (protegido por `.gitignore`)
- ? Solo existe en tu computadora local
- ? Cada desarrollador tiene su propia copia

### ? Archivo `App.secrets.config.example`:
- ? **SÍ** se sube a GitHub (es solo una plantilla)
- ?? No contiene credenciales reales
- ?? Sirve como guía para el equipo

---

## ?? Verificar que Funciona

1. Guarda `App.secrets.config` con tus credenciales
2. Compila el proyecto
3. Ejecuta la aplicación
4. Inicia sesión como **Asistente**
5. Intenta enviar un informe por email
6. Verifica que el email llega al destinatario

---

## ?? Solución de Problemas

### Error: "Credenciales no configuradas"
? Verifica que `App.secrets.config` existe en `SALC\`
? Verifica que tenga las claves `SMTP_Usuario` y `SMTP_Password`

### Error: "Autenticación fallida" (Gmail)
? Usa **Contraseña de Aplicación**, NO tu contraseña normal
? Verifica que la verificación en 2 pasos esté activa
? Genera una nueva contraseña de aplicación

### El archivo aparece en `git status`
Si accidentalmente agregaste el archivo a Git:
```bash
git rm --cached SALC/App.secrets.config
git commit -m "Remove secrets from Git"
```

---

## ?? Para Otros Desarrolladores

Cuando tu equipo haga `git pull`:

1. Verán el archivo `App.secrets.config.example`
2. Deben copiarlo: `copy App.secrets.config.example App.secrets.config`
3. Cada uno edita su copia con sus credenciales
4. Cada uno mantiene su archivo local (NO se comparte)

---

## ?? Estructura de Archivos

```
SALC/
??? App.config                      ? Se sube (sin credenciales)
??? App.secrets.config              ?? NO se sube (tus credenciales)
??? App.secrets.config.example      ? Se sube (plantilla)
```

---

## ?? Más Información

- Documentación técnica: `SALC\Docs\IMPLEMENTACION_EMAIL.md`
- Contraseñas Gmail: https://support.google.com/accounts/answer/185833

# SALC - Sistema de Administración de Laboratorio Clínico

## Descripción
SALC es una aplicación de escritorio desarrollada en C# con Windows Forms para la gestión integral de laboratorios clínicos.

## Características Implementadas

### Sistema de Autenticación
- **Formulario de Login**: Interfaz moderna y profesional para el acceso al sistema
- **Validación de Credenciales**: Sistema de autenticación con roles de usuario
- **Gestión de Usuarios**: Control de acceso basado en roles

### Usuarios de Prueba
Para acceder al sistema, puede usar las siguientes credenciales:

| Usuario | Contraseña | Rol |
|---------|------------|-----|
| admin | admin123 | Administrador |
| clinico | clinico123 | Clínico |
| tecnico | tecnico123 | Técnico |

### Funcionalidades del Sistema
- **Gestión de Pacientes**: Registro y administración de información de pacientes
- **Gestión de Estudios**: Creación y seguimiento de órdenes de análisis
- **Carga de Resultados**: Ingreso de resultados de estudios con validación
- **Generación de Informes**: Creación de reportes en PDF
- **Sistema de Notificaciones**: Envío de notificaciones a pacientes
- **Copias de Seguridad**: Gestión automática de backups

## Estructura del Proyecto

```
SALC/
├── LoginForm.cs              # Formulario de login principal
├── LoginForm.Designer.cs     # Diseño del formulario de login
├── Form1.cs                  # Formulario principal de la aplicación
├── Form1.Designer.cs         # Diseño del formulario principal
├── UserAuthentication.cs     # Clase para manejo de autenticación
├── Program.cs                # Punto de entrada de la aplicación
└── SALC.csproj              # Archivo de proyecto
```

## Tecnologías Utilizadas
- **.NET Framework 4.7.2**
- **Windows Forms**
- **C#**
- **SQL Server** (para futuras implementaciones)

## Instalación y Uso

### Requisitos del Sistema
- Windows 7 o superior
- .NET Framework 4.7.2 o superior
- Mínimo 2GB RAM
- 500MB de espacio en disco

### Compilación
1. Abrir el archivo `SALC.sln` en Visual Studio
2. Restaurar los paquetes NuGet si es necesario
3. Compilar la solución (Ctrl+Shift+B)
4. Ejecutar la aplicación (F5)

### Uso del Sistema
1. Ejecutar la aplicación SALC.exe
2. Ingresar las credenciales de usuario
3. Navegar por los menús para acceder a las diferentes funcionalidades

## Arquitectura del Sistema

### Capa de Presentación
- **LoginForm**: Formulario de autenticación
- **Form1**: Formulario principal con menú de navegación

### Capa de Lógica de Negocio
- **UserAuthentication**: Manejo de autenticación y autorización
- **UserInfo**: Modelo de datos para información de usuario

### Características de Seguridad
- Validación de credenciales
- Control de acceso basado en roles
- Encriptación de contraseñas (SHA256)
- Gestión de sesiones de usuario

## Próximas Implementaciones
- Conexión a base de datos SQL Server
- Formularios para gestión de pacientes
- Formularios para gestión de estudios
- Sistema de generación de informes PDF
- Módulo de notificaciones
- Sistema de copias de seguridad

## Soporte
Para soporte técnico o consultas sobre el sistema, contactar al equipo de desarrollo.

---
**Versión**: 1.0.0  
**Fecha**: 2024  
**Desarrollado por**: Equipo SALC

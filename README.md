# SALC - Sistema de Análisis de Laboratorio Clínico

[![Estado de Documentación](https://img.shields.io/badge/documentation-100%25-brightgreen)](Docs/RESUMEN_DOCUMENTACION.md)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)](https://dotnet.microsoft.com/download/dotnet-framework/net472)
[![Arquitectura](https://img.shields.io/badge/architecture-MVP-orange)](https://es.wikipedia.org/wiki/Modelo%E2%80%93vista%E2%80%93presentador)
[![Licencia](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Contribuidores](https://img.shields.io/github/contributors/aka-titesen/SALC)](https://github.com/aka-titesen/SALC/graphs/contributors)

## Tabla de Contenidos

1. [Descripción](#descripción)

2. [Características](#características)

3. [Requisitos](#requisitos)

4. [Instalación](#instalación)

5. [Configuración](#configuración)

6. [Entorno de Desarrollo](#entorno-de-desarrollo)

7. [Uso](#uso)

8. [Estructura del Proyecto](#estructura-del-proyecto)

9. [Pruebas](#pruebas)

10. [Contribuir](#contribuir)

11. [Tecnologías Utilizadas](#tecnologías-utilizadas)

12. [Arquitectura](#arquitectura)

13. [Documentación](#documentación)

14. [Licencia](#licencia)

15. [Contacto](#contacto)

16. [Agradecimientos](#agradecimientos)

## Descripción

**SALC** (Sistema de Análisis de Laboratorio Clínico) es una aplicación de escritorio desarrollada en Windows Forms (.NET Framework 4.7.2) diseñada para gestionar de manera integral las operaciones de un laboratorio clínico.

El sistema permite la administración completa del flujo de trabajo de un laboratorio, desde el registro de pacientes hasta la generación de informes médicos, pasando por la carga de resultados y la firma digital de análisis. Está diseñado para tres roles principales:

- **Administradores**: Gestión de usuarios, catálogos y configuración del sistema

- **Médicos**: Creación de análisis, carga de resultados y firma digital

- **Asistentes de Laboratorio**: Gestión de pacientes y generación de informes

El proyecto fue desarrollado siguiendo el patrón arquitectónico MVP (Model-View-Presenter), con una clara separación de responsabilidades en capas, lo que garantiza un código mantenible, escalable y testeable.

## Características

### Gestión de Usuarios

- Autenticación segura con BCrypt

- Tres roles diferenciados (Administrador, Médico, Asistente)

- Gestión completa de usuarios (CRUD)

- Baja lógica de usuarios manteniendo integridad referencial

### Gestión de Pacientes

- Registro completo de pacientes con validaciones

- Asociación con obras sociales

- Baja lógica preservando historial

- Búsqueda y filtrado avanzado

### Análisis Clínicos

- Creación de análisis vinculados a tipos predefinidos

- Carga de resultados de métricas específicas

- Validación y firma digital de análisis

- Estados de análisis (Pendiente, Verificado, Anulado)

- Anulación de análisis con auditoría

### Catálogos Configurables

- Gestión de obras sociales

- Tipos de análisis personalizables

- Métricas con valores de referencia

- Relaciones dinámicas tipo análisis-métricas

- Baja lógica en todos los catálogos

### Reportes y Estadísticas

- Reportes de productividad de médicos

- Análisis de facturación por obra social

- Estadísticas de demanda de análisis

- Alertas de valores críticos

- Exportación a PDF

### Informes Médicos

- Generación automática de informes PDF

- Envío de informes por correo electrónico

- Plantillas profesionales con membrete

- Firma digital de médicos en informes

### Seguridad y Auditoría

- Sistema de logging integrado

- Manejo centralizado de excepciones

- Validaciones en todas las capas

- Trazabilidad de operaciones

- Backups manuales de base de datos

### Infraestructura Robusta

- Conexión a SQL Server con health checks

- Sistema de excepciones personalizadas

- Manejo de errores user-friendly

- Arquitectura en capas desacopladas

## Requisitos

### Software Necesario

- **Sistema Operativo**: Windows 10 o superior

- **Visual Studio**: 2019 o superior (Community, Professional o Enterprise)

- **SQL Server**: 2016 o superior (Express, Standard o Enterprise)

- **.NET Framework**: 4.7.2 o superior

- **SQL Server Management Studio** (SSMS): Recomendado para administración de BD

- **Git**: Para clonar el repositorio

### Librerías y Dependencias

- **BCrypt.Net-Next** v4.0.3 - Hashing de contraseñas

- **iTextSharp** v5.5.13.3 - Generación de PDFs

- **System.Configuration.ConfigurationManager** v6.0.0

- **System.Data.SqlClient** (incluido en .NET Framework)

### Requisitos de Hardware (Mínimos)

- **Procesador**: Intel Core i3 o equivalente

- **Memoria RAM**: 4 GB

- **Espacio en Disco**: 500 MB para la aplicación + espacio para base de datos

- **Resolución de Pantalla**: 1366x768 o superior

## Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/aka-titesen/SALC.git
cd SALC
```

### 2. Configurar la Base de Datos

#### Opción A: Usando SQL Server Management Studio (SSMS)

1. Abre SSMS y conéctate a tu instancia de SQL Server

2. Navega a la carpeta `Database` del proyecto

3. Ejecuta el script `estructura_salc_crear-tablas.sql` para crear la base de datos

4. Ejecuta el script `lote_salc_datos-ejemplos.sql` para cargar datos de pruebas

#### Opción B: Usando línea de comandos

```bash
# Navega a la carpeta de scripts
cd Database

# Ejecuta los scripts en orden
sqlcmd -S localhost -E -i salc_crear-base-datos.sql
sqlcmd -S localhost -d SALC -E -i estructura_salc_crear-tablas.sql
sqlcmd -S localhost -d SALC -E -i lote_salc_datos-ejemplos.sql
```

### 3. Restaurar Paquetes NuGet

Abre la solución en Visual Studio:

```bash
# Navega a la carpeta de la solución
cd SALC

# Abre la solución (esto abrirá Visual Studio)
start SALC.sln
```

En Visual Studio:

1. Click derecho en la solución → `Restore NuGet Packages`

2. O desde el menú: `Tools` → `NuGet Package Manager` → `Manage NuGet Packages for Solution`

### 4. Configurar la Cadena de Conexión

Edita el archivo `App.config`:

```xml
<connectionStrings>
  <add name="SALC" 
       connectionString="Server=localhost;Database=SALC;Integrated Security=true;" 
       providerName="System.Data.SqlClient"/>
</connectionStrings>
```

Ajusta los valores según tu configuración:

- `Server`: Nombre o IP de tu servidor SQL Server

- `Database`: Nombre de la base de datos (por defecto: SALC)

- `Integrated Security=true`: Para autenticación de Windows

- O usa `User Id=usuario;Password=contraseña;` para autenticación SQL

## Configuración

### Variables de Configuración

El archivo `App.config` contiene las siguientes configuraciones principales:

```xml
<configuration>
  <connectionStrings>
    <!-- Cadena de conexión a la base de datos -->
    <add name="SALC" 
         connectionString="Server=localhost;Database=SALC;Integrated Security=true;" 
         providerName="System.Data.SqlClient"/>
  </connectionStrings>
  
  <appSettings>
    <!-- Configuración de logging -->
    <add key="LogLevel" value="Info"/>
    <add key="LogPath" value="Logs\salc.log"/>
    
    <!-- Configuración de email (opcional) -->
    <add key="SmtpHost" value="smtp.gmail.com"/>
    <add key="SmtpPort" value="587"/>
    <add key="SmtpEnableSSL" value="true"/>
    <add key="SmtpUser" value="tu-email@gmail.com"/>
    <add key="SmtpPassword" value="tu-contraseña-de-aplicacion"/>
  </appSettings>
</configuration>
```

### Configuración de Email (Opcional)

Para habilitar el envío de informes por email:

1. Edita las claves `SmtpHost`, `SmtpPort`, `SmtpUser` y `SmtpPassword` en `App.config`

2. Para Gmail, necesitas crear una "Contraseña de aplicación":

   - Ve a tu cuenta de Google → Seguridad

   - Activa "Verificación en 2 pasos"

   - Genera una "Contraseña de aplicación"

   - Usa esa contraseña en `SmtpPassword`

### Usuarios de Prueba

El script de datos iniciales crea los siguientes usuarios de prueba:

| DNI      | Rol           | Email                   | Contraseña |
| -------- | ------------- | ----------------------- | ---------- |
| 25111112 | Administrador | laura.campos@salc.co    | salc123    |
| 30100101 | Médico        | carlos.bianchi@salc.com | salc123    |
| 30100102 | Médico        | ana.fernandez@salc.com  | salc123    |
| 40100201 | Asistente     | maria.becerra@salc.com  | salc123    |
| 40100206 | Asistente     | wos.valentin@salc.com   | salc123    |

**IMPORTANTE**: Cambia estas contraseñas en un entorno de producción.

## Entorno de Desarrollo

### Configuración del IDE

#### Visual Studio 2019/2022

1. **Instalar Workloads Necesarios**:

   - `.NET desktop development`

   - `Data storage and processing` (para herramientas de SQL Server)

2. **Extensiones Recomendadas**:

   - ReSharper (opcional, mejora la productividad)

   - CodeMaid (para limpieza de código)

   - GhostDoc (para generación de documentación XML)

3. **Configurar EditorConfig** (ya incluido en el proyecto):

   ```
   El archivo .editorconfig define el estilo de código del proyecto
   ```

### Restaurar Dependencias

#### Método 1: Visual Studio (Recomendado)

1. Abre la solución `SALC.sln` en Visual Studio

2. Visual Studio restaurará automáticamente los paquetes NuGet

3. Si no lo hace automáticamente:

   - Click derecho en la solución → `Restore NuGet Packages`

   - O presiona `Ctrl+Shift+B` para compilar (restaura automáticamente)

#### Método 2: Consola del Administrador de Paquetes

En Visual Studio:

```powershell
# Desde Tools → NuGet Package Manager → Package Manager Console
Update-Package -reinstall
```

#### Método 3: CLI de NuGet

```bash
# Navega a la carpeta del proyecto
cd SALC/SALC

# Restaura los paquetes
nuget restore

# O usando dotnet CLI
dotnet restore
```

#### Método 4: CLI con MSBuild

```bash
# Navega a la carpeta de la solución
cd SALC

# Restaura con MSBuild
msbuild /t:restore
```

### Verificar Dependencias Instaladas

Después de restaurar, verifica que todas las dependencias estén instaladas:

```bash
# Lista todos los paquetes instalados
nuget list -Source "C:\Users\[TuUsuario]\source\repos\SALC\packages"
```

Deberías ver:

- BCrypt.Net-Next.4.0.3

- iTextSharp.5.5.13.3

- System.Configuration.ConfigurationManager.6.0.0

### Compilar el Proyecto

#### Desde Visual Studio

```
Build → Build Solution (F7)
O
Build → Rebuild Solution (Ctrl+Shift+B)
```

#### Desde línea de comandos

```bash
# Compilación en modo Debug
msbuild SALC.sln /p:Configuration=Debug

# Compilación en modo Release
msbuild SALC.sln /p:Configuration=Release
```

### Ejecutar en Modo Debug

1. **En Visual Studio**:

   - Presiona `F5` para ejecutar con depuración

   - O presiona `Ctrl+F5` para ejecutar sin depuración

2. **Establecer Puntos de Interrupción**:

   - Click en el margen izquierdo del editor de código

   - O presiona `F9` en la línea deseada

3. **Variables de Entorno de Debug** (opcional):

   - Click derecho en el proyecto → Properties

   - Tab Debug → Environment variables

### Estructura de Carpetas de Desarrollo

```
SALC/
├── .vs/                    # Configuración de Visual Studio (no subir a Git)
├── bin/                    # Ejecutables compilados (no subir a Git)
├── obj/                    # Archivos de compilación (no subir a Git)
├── packages/               # Paquetes NuGet (no subir a Git)
├── Database/               # Scripts de base de datos
├── Docs/                   # Documentación del proyecto
├── BLL/                    # Capa de lógica de negocio
├── DAL/                    # Capa de acceso a datos
├── Domain/                 # Entidades del dominio
├── Infraestructura/        # Utilidades y servicios transversales
├── Presenters/             # Presentadores (patrón MVP)
├── Views/                  # Formularios de Windows Forms
├── Properties/             # Propiedades del proyecto
├── App.config              # Configuración de la aplicación
├── Program.cs              # Punto de entrada
└── SALC.csproj            # Archivo del proyecto
```

### Solución de Problemas Comunes

#### Error: "No se puede conectar a la base de datos"

```bash
# Verifica que SQL Server esté ejecutándose
sc query MSSQLSERVER

# Verifica la cadena de conexión en App.config
# Prueba la conexión desde SSMS
```

#### Error: "Package restore failed"

```bash
# Limpia la caché de NuGet
nuget locals all -clear

# Restaura nuevamente
nuget restore
```

#### Error: "Reference not found"

```bash
# Reinstala los paquetes
Update-Package -reinstall
```

#### Error de compilación "CS0234"

```bash
# Verifica que .NET Framework 4.7.2 esté instalado
# Reinstala las referencias del proyecto
```

## Uso

### Inicio de Sesión

1. Ejecuta la aplicación

2. Ingresa tu DNI (solo números, sin puntos ni guiones)

3. Ingresa tu contraseña

4. Click en "Acceder"

### Panel de Administrador

Como administrador puedes:

1. **Gestionar Usuarios**:

   - Crear nuevos usuarios (Administrador, Médico, Asistente)

   - Editar información de usuarios existentes

   - Dar de baja usuarios (baja lógica)

   - Ver detalles completos de usuarios

2. **Gestionar Catálogos**:

   - **Obras Sociales**: Crear, editar y desactivar obras sociales

   - **Tipos de Análisis**: Definir tipos de análisis disponibles

   - **Métricas**: Configurar métricas con valores de referencia

   - **Relaciones**: Asociar métricas a tipos de análisis

3. **Backups**:

   - Ejecutar backups manuales de la base de datos

   - Seleccionar ubicación de almacenamiento

   - Ver historial de backups

4. **Reportes**:

   - Productividad de médicos

   - Facturación por obra social

   - Demanda de tipos de análisis

5. **Salud del Sistema**:

   - Probar conexión a la base de datos

   - Ver estado del sistema

### Panel de Médico

Como médico puedes:

1. **Gestión de Pacientes**:

   - Ver listado de pacientes

   - Editar información de pacientes

   - Dar de baja pacientes

   - Buscar y filtrar pacientes

2. **Crear Análisis**:

   - Seleccionar paciente

   - Elegir tipo de análisis

   - Agregar observaciones generales

   - Crear el análisis (queda en estado Pendiente)

3. **Cargar Resultados**:

   - Buscar análisis pendientes

   - Cargar métricas correspondientes al tipo de análisis

   - Ingresar resultados numéricos

   - Agregar observaciones por métrica

   - Guardar resultados

4. **Validar y Firmar**:

   - Buscar análisis con resultados cargados

   - Revisar todos los resultados

   - Firmar digitalmente el análisis

   - El análisis pasa a estado Verificado

5. **Reportes Médicos**:

   - Ver alertas de valores críticos

   - Consultar carga de trabajo

### Panel de Asistente

Como asistente puedes:

1. **Gestión de Pacientes**:

   - Buscar pacientes

   - Ver listado de pacientes activos

   - Ver información completa de pacientes

2. **Historial de Pacientes**:

   - Seleccionar un paciente

   - Ver todos sus análisis (activos y anulados)

   - Consultar resultados de análisis

3. **Generar Informes**:

   - Seleccionar análisis verificado

   - Generar PDF del informe

   - Guardar PDF en ubicación deseada

   - Enviar informe por email al paciente

### Flujo de Trabajo Típico

```
1. Médico crea un análisis para un paciente
   ↓
2. Médico carga los resultados de las métricas
   ↓
3. Médico valida y firma el análisis digitalmente
   ↓
4. Asistente genera el informe PDF
   ↓
5. Asistente envía el informe por email al paciente
```

## Estructura del Proyecto

```
SALC/
│
├── BLL/                              # Capa de Lógica de Negocio
│   ├── AnalisisService.cs            # Servicio de análisis
│   ├── AutenticacionService.cs       # Servicio de autenticación
│   ├── BackupService.cs              # Servicio de backups
│   ├── CatalogoService.cs            # Servicio de catálogos
│   ├── DefaultPasswordHasher.cs      # Implementación de hashing BCrypt
│   ├── EmailService.cs               # Servicio de envío de emails
│   ├── InformeService.cs             # Servicio de generación de PDFs
│   ├── PacienteService.cs            # Servicio de pacientes
│   ├── ReportesService.cs            # Servicio de reportes
│   ├── UsuarioService.cs             # Servicio de usuarios
│   ├── IAnalisisService.cs           # Interfaces de servicios
│   ├── IAutenticacionService.cs
│   ├── IBackupService.cs
│   ├── ICatalogoService.cs
│   ├── IEmailService.cs
│   ├── IInformeService.cs
│   ├── IPasswordHasher.cs
│   ├── IPacienteService.cs
│   ├── IReportesService.cs
│   └── IUsuarioService.cs
│
├── DAL/                              # Capa de Acceso a Datos
│   ├── AnalisisRepositorio.cs        # Repositorio de análisis
│   ├── AnalisisMetricaRepositorio.cs # Repositorio de resultados
│   ├── AsistenteRepositorio.cs       # Repositorio de asistentes
│   ├── BackupRepositorio.cs          # Repositorio de backups
│   ├── CatalogoRepositorio.cs        # Repositorio de catálogos
│   ├── MedicoRepositorio.cs          # Repositorio de médicos
│   ├── ObraSocialRepositorio.cs      # Repositorio de obras sociales
│   ├── PacienteRepositorio.cs        # Repositorio de pacientes
│   ├── ReportesRepositorio.cs        # Repositorio de reportes
│   ├── TipoAnalisisMetricaRepositorio.cs # Repositorio de relaciones
│   ├── UsuarioRepositorio.cs         # Repositorio de usuarios
│   └── IRepositorioBase.cs           # Interfaz base de repositorios
│
├── Domain/                           # Capa de Dominio
│   ├── Analisis.cs                   # Entidad Análisis
│   ├── AnalisisMetrica.cs            # Entidad Resultado
│   ├── Asistente.cs                  # Entidad Asistente
│   ├── Backup.cs                     # Entidad Backup
│   ├── EstadoAnalisis.cs             # Entidad Estado
│   ├── EstadosConstantes.cs          # Constantes de estados
│   ├── Medico.cs                     # Entidad Médico
│   ├── Metrica.cs                    # Entidad Métrica
│   ├── MetricaConResultado.cs        # DTO para resultados
│   ├── ObraSocial.cs                 # Entidad Obra Social
│   ├── Paciente.cs                   # Entidad Paciente
│   ├── Rol.cs                        # Entidad Rol
│   ├── TipoAnalisis.cs               # Entidad Tipo de Análisis
│   ├── TipoAnalisisMetrica.cs        # Entidad Relación
│   └── Usuario.cs                    # Entidad Usuario
│
├── Infraestructura/                  # Capa de Infraestructura
│   ├── DbConexion.cs                 # Conexión a base de datos
│   ├── DbHealth.cs                   # Health check de BD
│   ├── ExceptionHandler.cs           # Manejador de excepciones
│   ├── Logger.cs                     # Sistema de logging
│   ├── SalcException.cs              # Excepciones base
│   ├── SalcAuthorizationException.cs # Excepciones de autorización
│   ├── SalcBusinessException.cs      # Excepciones de negocio
│   ├── SalcDatabaseException.cs      # Excepciones de BD
│   └── SalcValidacionException.cs    # Excepciones de validación
│
├── Presenters/                       # Presentadores (MVP)
│   ├── GestionPacientesAsistentePresenter.cs
│   ├── LoginPresenter.cs
│   ├── PanelAdministradorPresenter.cs
│   ├── PanelAsistentePresenter.cs
│   ├── PanelMedicoPresenter.cs
│   ├── ReportesAdminPresenter.cs
│   ├── ReportesMedicoPresenter.cs
│   └── ViewsContracts/              # Interfaces de vistas
│       ├── IGestionPacientesAsistenteView.cs
│       ├── ILoginView.cs
│       ├── IPanelAdministradorView.cs
│       ├── IPanelAsistenteView.cs
│       ├── IPanelMedicoView.cs
│       ├── IReportesAdminView.cs
│       └── IReportesMedicoView.cs
│
├── Views/                            # Vistas (Windows Forms)
│   ├── FrmLogin.cs                   # Formulario de login
│   ├── PanelAdministrador/           # Vistas del administrador
│   ├── PanelMedico/                  # Vistas del médico
│   └── PanelAsistente/               # Vistas del asistente
│
├── Database/                         # Scripts de base de datos
│   ├── 01-crear-base-datos.sql
│   ├── 02-crear-tablas.sql
│   ├── 03-datos-iniciales.sql
│   └── 04-usuarios-prueba.sql
│
├── Docs/                             # Documentación
│   ├── RESUMEN_DOCUMENTACION.md
│   └── DOCUMENTACION_COMPLETADA.md
│
├── Properties/                       # Propiedades del proyecto
│   └── AssemblyInfo.cs
│
├── App.config                        # Configuración de la aplicación
├── Program.cs                        # Punto de entrada
├── BCryptTest.cs                     # Utilidad de prueba
└── SALC.csproj                       # Archivo del proyecto
```

## Pruebas

### Pruebas de BCrypt

El proyecto incluye una utilidad para verificar el funcionamiento correcto del algoritmo BCrypt:

```csharp
// Desde el código
BCryptTest.TestBCrypt();
```

Esta prueba verifica que las contraseñas de los usuarios de prueba puedan ser validadas correctamente.

### Pruebas Manuales

1. **Login**:

   - Probar con credenciales válidas

   - Probar con credenciales inválidas

   - Verificar redirección según rol

2. **Gestión de Pacientes**:

   - Crear un paciente nuevo

   - Editar información de un paciente

   - Dar de baja un paciente

   - Buscar pacientes

3. **Flujo de Análisis**:

   - Crear un análisis

   - Cargar resultados

   - Firmar análisis

   - Generar informe PDF

4. **Catálogos**:

   - Crear obra social

   - Crear tipo de análisis

   - Crear métrica

   - Asociar métricas a tipos

### Pruebas de Conexión

```csharp
// Desde el panel de administrador
// Click en "Probar Conexión BD"
var resultado = DbHealth.ProbarConexion();
```

### Casos de Prueba Recomendados

1. **Validaciones**:

   - Formato de DNI correcto

   - Email válido

   - Fechas coherentes

   - Valores numéricos en métricas

   - Campos requeridos completos

2. **Seguridad**:

   - Contraseñas hasheadas en BD

   - Sesión por usuario

   - Validación de permisos por rol

   - Manejo de errores sin exponer información sensible

3. **Integridad Referencial**:

   - No se puede eliminar usuario con análisis

   - No se puede eliminar paciente con análisis

   - No se puede eliminar tipo de análisis en uso

   - Baja lógica preserva relaciones

## Contribuir

Si deseas contribuir a este proyecto, por favor sigue estos pasos:

### 1. Fork del Repositorio

Haz un fork del repositorio en GitHub.

### 2. Crear una Rama

```bash
git checkout -b feature/nueva-caracteristica
# o
git checkout -b fix/correccion-bug
```

### 3. Estándares de Código

- Seguir el patrón MVP existente

- Documentar todo el código con comentarios XML

- Usar PascalCase para clases y métodos

- Usar camelCase para variables privadas con prefijo `_`

### 4. Commits

Usa mensajes de commit descriptivos siguiendo Conventional Commits:

```bash
git commit -m "feat: agregar funcionalidad de exportación Excel"
git commit -m "fix: corregir validación de email"
git commit -m "docs: actualizar README con instrucciones"
```

Tipos de commits:

- `feat`: Nueva funcionalidad

- `fix`: Corrección de bug

- `docs`: Documentación

- `style`: Formato de código

- `refactor`: Refactorización

- `test`: Pruebas

- `chore`: Tareas de mantenimiento

### 5. Push y Pull Request

```bash
git push origin feature/nueva-caracteristica
```

Luego abre un Pull Request en GitHub con:

- Descripción clara de los cambios

- Screenshots si aplica

- Referencia a issues relacionados

### 6. Code Review

- Responde a comentarios del code review

- Realiza los cambios solicitados

- Mantén la conversación profesional

### Áreas de Contribución

- Mejoras en la UI/UX

- Optimización de consultas SQL

- Nuevos tipos de reportes

- Exportación a otros formatos

- Tests unitarios

- Documentación adicional

- Traducciones

## Tecnologías Utilizadas

### Frontend

- **Windows Forms**: Framework de UI para .NET

- **DevExpress** (opcional): Controles avanzados de UI

### Backend

- **.NET Framework**: 4.7.2

- **C#**: Lenguaje de programación principal

- **ADO.NET**: Acceso a datos

- **System.Data.SqlClient**: Provider de SQL Server

### Base de Datos

- **SQL Server**: 2016 o superior

- **T-SQL**: Lenguaje de consultas

### Seguridad

- **BCrypt.Net-Next**: v4.0.3 - Hashing de contraseñas

- **Autenticación**: Windows Authentication o SQL Authentication

### Generación de Documentos

- **iTextSharp**: v5.5.13.3 - Generación de PDFs

### Comunicaciones

- **System.Net.Mail**: Envío de emails

- **SMTP**: Protocolo de correo

### Arquitectura y Patrones

- **MVP**: Model-View-Presenter

- **Repository Pattern**: Para acceso a datos

- **Service Layer**: Para lógica de negocio

- **Dependency Injection**: Manual para desacoplamiento

### Herramientas de Desarrollo

- **Visual Studio**: IDE principal

- **SQL Server Management Studio**: Administración de BD

- **Git**: Control de versiones

- **NuGet**: Gestor de paquetes

### Librerías Adicionales

- **System.Configuration.ConfigurationManager**: v6.0.0

## Arquitectura

### Patrón MVP (Model-View-Presenter)

El proyecto sigue el patrón arquitectónico **MVP (Model-View-Presenter)** que separa:

```
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│    View     │ ◄─────► │  Presenter  │ ◄─────► │   Model     │
│ (UI Forms)  │         │ (Lógica de  │         │ (BLL + DAL) │
│             │         │  Presenta-  │         │             │
│             │         │    ción)    │         │             │
└─────────────┘         └─────────────┘         └─────────────┘
```

**Ventajas**:

- Testabilidad: La lógica está separada de la UI

- Mantenibilidad: Cambios en UI no afectan la lógica

- Reutilización: Los presentadores pueden usar diferentes vistas

### Arquitectura en Capas

```
┌──────────────────────────────────────┐
│         Presentation Layer           │  ← Views (Windows Forms)
├──────────────────────────────────────┤
│         Presenter Layer              │  ← Presenters (MVP)
├──────────────────────────────────────┤
│      Business Logic Layer (BLL)      │  ← Services, Business Rules
├──────────────────────────────────────┤
│      Data Access Layer (DAL)         │  ← Repositories, SQL
├──────────────────────────────────────┤
│         Domain Layer                 │  ← Entities, DTOs
├──────────────────────────────────────┤
│      Infrastructure Layer            │  ← Logger, Exceptions, DB
└──────────────────────────────────────┘
```

### Flujo de Datos

```
Usuario interactúa con View
         ↓
View dispara evento
         ↓
Presenter maneja evento
         ↓
Presenter llama a Service (BLL)
         ↓
Service aplica lógica de negocio
         ↓
Service llama a Repository (DAL)
         ↓
Repository ejecuta query SQL
         ↓
Repository retorna entidades (Domain)
         ↓
Service procesa y retorna a Presenter
         ↓
Presenter actualiza View
         ↓
View muestra resultados al Usuario
```

### Principios SOLID Aplicados

- **S**ingle Responsibility: Cada clase tiene una única responsabilidad

- **O**pen/Closed: Abierto a extensión, cerrado a modificación

- **L**iskov Substitution: Las interfaces definen contratos claros

- **I**nterface Segregation: Interfaces específicas por funcionalidad

- **D**ependency Inversion: Dependencia de abstracciones, no de implementaciones

## Documentación

El proyecto cuenta con **documentación XML completa** (100% de cobertura) en todos los archivos principales:

### Documentación Disponible

- **Comentarios XML**: Todos los métodos, clases y propiedades públicas

- **IntelliSense**: Ayuda contextual completa en Visual Studio

- **Especificación de Requisitos de Software**: Ver [ERS.md](SALC\Docs\ERS\ers-salc_ieee830v2.9.md)

- **Manual de Usuario**: Ver [Manual de Usuario.md](SALC\Docs\UserManual\manual_de_usuario.md)

### Generar Documentación HTML

Puedes generar documentación HTML usando herramientas como:

#### DocFX

```bash
# Instalar DocFX
choco install docfx

# Generar documentación
docfx metadata
docfx build
```

#### Sandcastle

```bash
# Descargar Sandcastle Help File Builder
# Configurar proyecto y generar
```

### Estándares de Documentación

- ✅ Descripciones claras y concisas

- ✅ Parámetros y retornos documentados

- ✅ Excepciones documentadas

## Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

```
MIT License

Copyright (c) 2025 SALC - Sistema de Análisis de Laboratorio Clínico

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## Contacto

### Desarrolladores

- **GitHub**: [@aka-titesen](https://github.com/aka-titesen)

- **Repositorio**: [https://github.com/aka-titesen/SALC](https://github.com/aka-titesen/SALC)

### Reporte de Bugs

Si encuentras un bug, por favor:

1. Verifica que no haya sido reportado previamente en [Issues](https://github.com/aka-titesen/SALC/issues)

2. Crea un nuevo issue con:

   - Título descriptivo

   - Pasos para reproducir

   - Comportamiento esperado vs actual

   - Screenshots si aplica

   - Versión del sistema operativo y SQL Server

### Solicitud de Funcionalidades

Para solicitar nuevas funcionalidades:

1. Abre un issue con el tag `enhancement`

2. Describe la funcionalidad deseada

3. Explica el caso de uso

4. Adjunta mockups si tienes (opcional)

## Agradecimientos

### Contribuidores

Gracias a todos los que han contribuido a este proyecto.

### Tecnologías y Recursos

- **Microsoft** por .NET Framework y SQL Server

- **BCrypt.Net** por la librería de hashing seguro

- **iTextSharp** por la generación de PDFs

- **Stack Overflow** por la comunidad de desarrolladores

- **GitHub** por la plataforma de colaboración

### Inspiración

- Metodología de desarrollo guiada por el patrón MVP

- Mejores prácticas de arquitectura en capas

- Estándares de la industria de software médico

---

## Estado del Proyecto

**Estado**: ✅ Producción Ready
**Última Actualización**: 2025
**Versión**: 1.0.0
**Documentación**: 100% Completa

---


<p align="center">
  Desarrollado con ❤️ para la gestión eficiente de laboratorios clínicos
</p>


<p align="center">
  <a href="https://github.com/aka-titesen/SALC">🏠 Inicio</a> •
  <a href="https://github.com/aka-titesen/SALC/issues">🐛 Reportar Bug</a> •
  <a href="https://github.com/aka-titesen/SALC/issues">✨ Nueva Funcionalidad</a>
</p>

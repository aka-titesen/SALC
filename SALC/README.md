# SALC - Sistema de Administración de Laboratorio Clínico

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?logo=.net)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoft-sql-server)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-Desktop-0078D4?logo=windows)
![Licencia](https://img.shields.io/badge/Licencia-Académico-blue)
![Estado](https://img.shields.io/badge/Estado-En%20Desarrollo-yellow)

## Tabla de Contenidos

1. [Descripción](#1-descripción)
2. [Características](#2-características)
3. [Requisitos](#3-requisitos)
4. [Instalación](#4-instalación)
5. [Configuración](#5-configuración)
6. [Uso](#6-uso)
7. [Estructura del Proyecto](#7-estructura-del-proyecto)
8. [Arquitectura](#8-arquitectura)
9. [Documentación](#9-documentación)
10. [Tecnologías Utilizadas](#10-tecnologías-utilizadas)
11. [Contribuir](#11-contribuir)
12. [Licencia](#12-licencia)
13. [Contacto](#13-contacto)
14. [Agradecimientos](#14-agradecimientos)

## 1. Descripción

SALC (Sistema de Administración de Laboratorio Clínico) es una **aplicación de escritorio desarrollada en Windows Forms** sobre **.NET Framework 4.7.2** que permite gestionar de manera integral la operativa interna de un laboratorio clínico. El sistema abarca desde la gestión de pacientes y usuarios hasta la creación, carga y validación de análisis clínicos, con generación automática de informes en formato PDF.

### Propósito

Optimizar la **trazabilidad**, **seguridad** y **eficiencia operativa** del laboratorio mediante:

- Control de acceso basado en roles (Administrador, Médico, Asistente)
- Seguimiento completo del ciclo de vida de los análisis clínicos
- Validación médica con firma digital
- Generación de reportes estadísticos para análisis de performance
- Gestión centralizada de catálogos y copias de seguridad

### Propuesta de Valor

El SALC resuelve la necesidad de **centralizar y digitalizar** los procesos del laboratorio, eliminando el trabajo manual en papel, reduciendo errores humanos y garantizando la integridad de los datos médicos mediante una arquitectura robusta de 3 capas con patrón MVP.

## 2. Características

### Gestión de Entidades

- **Usuarios**: Alta, Baja y Modificación (ABM) con tres roles diferenciados
- **Pacientes**: Registro completo con datos personales y obra social
- **Catálogos**: ABM de obras sociales, tipos de análisis y métricas clínicas
- **Relaciones Tipo-Métrica**: Configuración de qué métricas componen cada tipo de análisis

### Ciclo de Vida de Análisis

- **Creación**: El médico crea un análisis asociado a un paciente
- **Carga de Resultados**: Ingreso de valores numéricos para cada métrica
- **Validación**: Firma digital del médico que hace inmutables los resultados
- **Generación de Informes**: Exportación a PDF de análisis verificados

### Seguridad y Auditoría

- Autenticación por DNI/contraseña con hashing BCrypt
- Control de acceso granular según rol de usuario
- Trazabilidad completa: registro de quién crea, carga y valida cada análisis
- Prevención de inyección SQL mediante uso obligatorio de `SqlParameter`

### Reportes Estadísticos (BI)

- **Reportes Globales (Administrador)**: Análisis por estado, pacientes por obra social, usuarios por rol
- **Reportes Personales (Médico)**: Análisis propios agrupados por estado y tipo

### Operaciones Administrativas

- Gestión de copias de seguridad de la base de datos SQL Server
- Historial de backups con seguimiento de estado y observaciones

## 3. Requisitos

### Software

- **Sistema Operativo**: Windows 10/11 (x64)
- **Framework**: .NET Framework 4.7.2 Runtime
- **Base de Datos**: SQL Server 2022 (Developer/Express Edition)
- **Herramientas de Desarrollo** (para compilar):
  - Visual Studio 2019 o superior
  - SQL Server Management Studio (SSMS)

### Hardware (Cliente)

- **CPU**: 2 núcleos o superior
- **RAM**: 4 GB o superior
- **Disco**: 10 GB de espacio libre
- **Conectividad**: TCP/IP a la red local (puerto 1433 por defecto)

## 4. Instalación

### Paso 1: Clonar el Repositorio

```bash
git clone https://github.com/aka-titesen/SALC.git
cd SALC
```

### Paso 2: Configurar la Base de Datos

1. Abre **SQL Server Management Studio (SSMS)**
2. Conéctate a tu instancia de SQL Server 2022
3. Ejecuta el script de creación de estructura:
   - Ruta: `Database/estructura_salc_crear-tablas.sql`
   - Este script crea la base de datos `[SALC]` y todas las tablas necesarias

4. (Opcional) Ejecuta el script de datos de ejemplo:
   - Ruta: `Database/lote_salc_datos-ejemplos.sql`
   - Este script carga 50 pacientes, 35 usuarios (5 admin, 20 médicos, 10 asistentes), 40 análisis y catálogos completos

### Paso 3: Configurar la Cadena de Conexión

1. Abre el archivo `App.config` en la raíz del proyecto `SALC/SALC/`
2. Modifica la cadena de conexión según tu configuración de SQL Server:

```xml
<connectionStrings>
  <add name="SALCConnectionString" 
       connectionString="Data Source=NOMBRE_DE_TU_SERVIDOR;Initial Catalog=SALC;Integrated Security=True;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Ejemplo con autenticación de Windows:**
```xml
connectionString="Data Source=localhost;Initial Catalog=SALC;Integrated Security=True;"
```

**Ejemplo con usuario SQL:**
```xml
connectionString="Data Source=localhost;Initial Catalog=SALC;User Id=sa;Password=TuPassword;"
```

### Paso 4: Compilar y Ejecutar

1. Abre la solución `SALC.sln` en **Visual Studio**
2. Restaura los paquetes NuGet (click derecho en la solución ? Restore NuGet Packages)
3. Compila el proyecto: **Build** ? **Build Solution** (o `Ctrl+Shift+B`)
4. Ejecuta la aplicación: **Debug** ? **Start Debugging** (o `F5`)

## 5. Configuración

### Variables de Entorno Principales

La configuración principal se encuentra en `App.config`:

```xml
<appSettings>
  <!-- Configuración de la aplicación -->
  <add key="NombreEmpresa" value="SALC - Laboratorio Clínico"/>
  <add key="RutaBackups" value="C:\SALC\Backups"/>
  <add key="RutaInformesPDF" value="C:\SALC\Informes"/>
</appSettings>

<connectionStrings>
  <!-- Cadena de conexión a SQL Server -->
  <add name="SALCConnectionString" 
       connectionString="Data Source=TU_SERVIDOR;Initial Catalog=SALC;Integrated Security=True;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Configuración Inicial Recomendada

1. Crea las carpetas de trabajo si no existen:
   - `C:\SALC\Backups` (para copias de seguridad)
   - `C:\SALC\Informes` (para PDFs generados)

2. Verifica la conectividad con SQL Server:
   - Abre SSMS y conéctate con las mismas credenciales configuradas
   - Asegúrate de que la base de datos `[SALC]` existe

## 6. Uso

### Inicio de Sesión

Al ejecutar la aplicación, se mostrará la pantalla de login (`FrmLogin`). Ingresa las credenciales según tu rol:

| Rol | DNI | Contraseña | Permisos |
|-----|-----|------------|----------|
| **Administrador** | `25111111` | `salc123` | Gestión completa del sistema |
| **Médico** | `30100101` | `salc123` | Gestión de análisis y pacientes |
| **Asistente** | `40100201` | `salc123` | Alta de pacientes e informes |

### Navegación Principal

Una vez autenticado, accederás al formulario principal (`FrmPrincipal`) con interfaz MDI que contiene:

- **MenuStrip**: Menú superior con todas las funciones organizadas
- **ToolStrip**: Barra de herramientas con accesos rápidos a funciones comunes
- **Área de Trabajo**: Espacio central donde se abren las ventanas hijas

### Funcionalidades por Rol

#### Administrador

- **Gestión de Usuarios**: Menú Gestión ? Usuarios
  - Crear, modificar y eliminar usuarios (Médicos, Asistentes, otros Administradores)
  - Asignar roles y cambiar estados (Activo/Inactivo)
  
- **Gestión de Catálogos**: Menú Gestión ? Catálogos
  - Obras Sociales: ABM completo
  - Tipos de Análisis: ABM completo
  - Métricas Clínicas: ABM completo
  - Relación Tipo-Métrica: Configurar qué métricas componen cada tipo de análisis
  
- **Copias de Seguridad**: Menú Administración ? Backups
  - Ejecutar backup manual
  - Ver historial de backups
  
- **Reportes Estadísticos**: Menú Reportes ? Reportes Globales
  - Análisis por estado
  - Pacientes por obra social
  - Usuarios por rol

#### Médico

- **Gestión de Pacientes**: Menú Gestión ? Pacientes
  - Modificar datos de pacientes existentes
  - Realizar baja lógica (cambiar estado a "Inactivo")
  
- **Gestión de Análisis**: Menú Análisis
  - **Crear Análisis**: Seleccionar paciente, tipo de análisis y confirmar
  - **Cargar Resultados**: Ingresar valores numéricos para cada métrica del análisis
  - **Validar Análisis**: Firmar digitalmente el análisis (hace inmutables los resultados)
  
- **Historial de Pacientes**: Menú Pacientes ? Historial
  - Ver lista completa de pacientes
  - Acceder al historial detallado solo de pacientes asociados a sus propios análisis
  
- **Reportes Personales**: Menú Reportes ? Mis Reportes
  - Análisis cargados por él (agrupados por estado)
  - Análisis firmados por él (agrupados por tipo)

#### Asistente

- **Gestión de Pacientes**: Menú Gestión ? Pacientes
  - Alta de nuevos pacientes
  - Modificación de datos de pacientes existentes
  
- **Historial Completo**: Menú Pacientes ? Historial
  - Ver historial detallado de cualquier paciente del sistema
  
- **Generación de Informes**: Menú Informes ? Generar PDF
  - Seleccionar análisis verificado
  - Generar informe PDF con membrete y resultados
  - Opción de envío por email o teléfono

### Flujo de Trabajo Típico

1. **Asistente** registra un nuevo paciente que llega al laboratorio
2. **Médico** crea un análisis para ese paciente (ej. "Hemograma Completo")
3. Laboratorio procesa la muestra
4. **Médico** carga los resultados numéricos de cada métrica
5. **Médico** valida el análisis con su firma digital
6. Paciente vuelve a retirar resultados
7. **Asistente** genera el informe PDF y se lo entrega

## 7. Estructura del Proyecto

```
SALC/
??? SALC/                           # Proyecto principal (Windows Forms)
?   ??? Views/                      # Formularios (Vista - capa UI)
?   ?   ??? FrmLogin.cs
?   ?   ??? FrmPrincipal.cs        # Contenedor MDI
?   ?   ??? FrmGestionPacientes.cs
?   ?   ??? FrmGestionAnalisis.cs
?   ?   ??? FrmReportesAdmin.cs    # Reportes globales
?   ?   ??? FrmReportesMedico.cs   # Reportes filtrados
?   ??? Presenters/                 # Presentadores (lógica de UI - MVP)
?   ?   ??? LoginPresenter.cs
?   ?   ??? PacientePresenter.cs
?   ?   ??? AnalisisPresenter.cs
?   ??? Services/                   # Lógica de negocio (BLL)
?   ?   ??? UsuarioService.cs
?   ?   ??? PacienteService.cs
?   ?   ??? AnalisisService.cs
?   ?   ??? ReporteService.cs
?   ??? Repositories/               # Acceso a datos (DAL)
?   ?   ??? UsuarioRepository.cs
?   ?   ??? PacienteRepository.cs
?   ?   ??? AnalisisRepository.cs
?   ?   ??? DbConnection.cs        # Gestión centralizada de conexiones
?   ??? Models/                     # Entidades de negocio (POCOs)
?   ?   ??? Usuario.cs
?   ?   ??? Paciente.cs
?   ?   ??? Analisis.cs
?   ?   ??? Metrica.cs
?   ??? Utils/                      # Utilidades y helpers
?   ?   ??? PasswordHasher.cs      # BCrypt
?   ?   ??? PdfGenerator.cs        # Generación de informes
?   ?   ??? Validator.cs           # Validaciones comunes
?   ??? App.config                  # Configuración de la aplicación
??? Database/                       # Scripts SQL
?   ??? estructura_salc_crear-tablas.sql    # Creación de BD y tablas
?   ??? lote_salc_datos-ejemplos.sql        # Datos de ejemplo (seed)
??? Docs/                           # Documentación del proyecto
??? ERS/                        # Especificación de Requisitos
?   ??? ers-salc_ieee830v2.9.md
??? UserManual/                 # Manual de usuario
    ??? manual_de_usuario.md
```

## 8. Arquitectura

El sistema implementa una **arquitectura de 3 capas lógicas** con el patrón **Model-View-Presenter (MVP - Passive View)** para maximizar la separación de responsabilidades, testabilidad y mantenibilidad.

### Capas del Sistema

#### 1. Capa de Presentación (UI - User Interface)

**Componentes:**
- Formularios Windows Forms (`FrmXXX.cs`)
- Controles de usuario personalizados
- Presenters (`XXXPresenter.cs`)

**Responsabilidad:**
- Captura las interacciones del usuario (clicks, input de datos)
- Delega toda la lógica al Presenter
- Es completamente "pasiva": sin lógica de negocio
- Implementa interfaces `IView` para desacoplamiento

**Ejemplo:**
```csharp
// IFrmPacienteView.cs
public interface IFrmPacienteView
{
    string NombrePaciente { get; set; }
    string ApellidoPaciente { get; set; }
    event EventHandler GuardarClick;
    void MostrarMensaje(string mensaje);
}

// FrmPaciente.cs implementa IFrmPacienteView
```

#### 2. Capa de Lógica de Negocio (BLL - Business Logic Layer)

**Componentes:**
- Clases de Servicio (`XXXService.cs`)

**Responsabilidad:**
- Orquesta las reglas de negocio del laboratorio
- Realiza validaciones complejas (ej. verificar que un médico puede validar un análisis)
- Coordina operaciones entre la capa UI y la capa DAL
- No contiene código SQL ni de Windows Forms

**Ejemplo:**
```csharp
// PacienteService.cs
public class PacienteService
{
    private readonly PacienteRepository _repository;
    
    public void GuardarPaciente(Paciente paciente)
    {
        // Validaciones de negocio
        if (paciente.FechaNacimiento > DateTime.Now)
            throw new InvalidOperationException("Fecha de nacimiento inválida");
            
        // Delegación al repositorio
        _repository.Update(paciente);
    }
}
```

#### 3. Capa de Acceso a Datos (DAL - Data Access Layer)

**Componentes:**
- Clases de Repositorio (`XXXRepository.cs`)
- Clase `DbConnection` para gestión centralizada de conexiones

**Responsabilidad:**
- Encapsula toda la interacción con SQL Server mediante ADO.NET
- Ejecuta operaciones CRUD (Create, Read, Update, Delete)
- **Uso OBLIGATORIO de `SqlParameter`** para prevenir inyección SQL
- No contiene lógica de negocio

**Ejemplo:**
```csharp
// PacienteRepository.cs
public class PacienteRepository
{
    public void Update(Paciente paciente)
    {
        using (SqlConnection conn = DbConnection.GetConnection())
        {
            string query = "UPDATE pacientes SET nombre = @Nombre WHERE dni = @Dni";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Nombre", paciente.Nombre);
            cmd.Parameters.AddWithValue("@Dni", paciente.Dni);
            cmd.ExecuteNonQuery();
        }
    }
}
```

### Patrón Model-View-Presenter (MVP)

**View (Vista):**
- Es el formulario Windows Forms que implementa `IView`
- No contiene lógica de decisión
- Solo expone propiedades y eventos

**Presenter (Presentador):**
- Intermediario entre la Vista y el Modelo
- Contiene toda la lógica de presentación
- Se comunica con la BLL para obtener/enviar datos
- Actualiza la Vista a través de su interfaz

**Model (Modelo):**
- Entidades de negocio (POCOs - Plain Old CLR Objects)
- Solo contienen datos, sin lógica

### Flujo de Ejecución Completo

**Ejemplo: Guardar un Paciente**

```
1. Usuario hace click en "Guardar" en FrmPaciente (View)
   ?
2. Se dispara el evento GuardarClick
   ?
3. PacientePresenter.OnGuardarClick() recibe el evento
   ?
4. Presenter obtiene datos de la Vista: view.NombrePaciente, view.ApellidoPaciente
   ?
5. Presenter crea un objeto Paciente (Model)
   ?
6. Presenter invoca PacienteService.GuardarPaciente(paciente) (BLL)
   ?
7. Service realiza validaciones de negocio
   ?
8. Service invoca PacienteRepository.Update(paciente) (DAL)
   ?
9. Repository ejecuta SqlCommand con SqlParameter
   ?
10. Repository retorna a Service ? Service retorna a Presenter
    ?
11. Presenter notifica a la Vista: view.MostrarMensaje("Paciente guardado exitosamente")
```

### Principios de Diseño Aplicados

- **S**ingle Responsibility Principle (SOLID): Cada clase tiene una única responsabilidad
- **O**pen/Closed Principle: Extensible sin modificar código existente
- **L**iskov Substitution: Las interfaces permiten sustituir implementaciones
- **I**nterface Segregation: Interfaces específicas por funcionalidad
- **D**ependency Inversion: Las capas dependen de abstracciones, no de implementaciones concretas

## 9. Documentación

El proyecto cuenta con documentación exhaustiva organizada en la carpeta `Docs/`:

### Especificación de Requisitos de Software (ERS)

**Ubicación**: Ver [ERS.md](Docs/ERS/ers-salc_ieee830v2.9.md)

**Contenido**:
- Requisitos funcionales detallados (RF-01 a RF-13)
- Requisitos no funcionales (rendimiento, seguridad, usabilidad)
- Modelo de datos completo (diccionario de datos)
- Arquitectura del sistema y patrones de diseño
- Guía de estilo UX/UI
- Stack tecnológico

### Manual de Usuario

**Ubicación**: Ver [Manual de Usuario.md](Docs/UserManual/manual_de_usuario.md)

**Contenido**:
- Guía paso a paso para cada rol de usuario
- Capturas de pantalla de las interfaces principales
- Explicación de funcionalidades por módulo
- Guía rápida de botones y controles
- Casos de uso comunes

### Scripts de Base de Datos

**Ubicación**: `Database/`

1. **`estructura_salc_crear-tablas.sql`**: Script de creación de la base de datos
   - Crea la BD `[SALC]`
   - Define 13 tablas con todas sus restricciones
   - Incluye claves primarias, foráneas y checks
   - Comentarios explicativos

2. **`lote_salc_datos-ejemplos.sql`**: Script de datos de ejemplo (seed)
   - 50 pacientes de prueba
   - 35 usuarios (5 admin, 20 médicos, 10 asistentes)
   - 40 análisis clínicos (20 sin verificar, 20 verificados)
   - Catálogos completos (roles, obras sociales, tipos de análisis, métricas)

## 10. Tecnologías Utilizadas

### Stack Principal

- **Lenguaje**: C#
- **Framework**: .NET Framework 4.7.2
- **Interfaz de Usuario**: Windows Forms
- **Base de Datos**: SQL Server 2022
- **Acceso a Datos**: ADO.NET (`System.Data.SqlClient`)

### Bibliotecas y Paquetes

- **BCrypt.Net-Next**: Hashing seguro de contraseñas
- **iTextSharp** (o similar): Generación de informes PDF
- **System.Windows.Forms.DataVisualization.Charting**: Gráficos estadísticos para reportes BI

### Herramientas de Desarrollo

- **IDE**: Visual Studio 2019/2022
- **Gestor de BD**: SQL Server Management Studio (SSMS)
- **Control de Versiones**: Git / GitHub

### Arquitectura y Patrones

- **Patrón de Diseño**: Model-View-Presenter (MVP - Passive View)
- **Arquitectura**: 3 Capas (Presentación, Lógica de Negocio, Acceso a Datos)
- **Principios**: SOLID

## 11. Contribuir

Este proyecto es de carácter **académico**, desarrollado como Trabajo Práctico Integrador de la materia **Laboratorio de Computación II**.

### Guía para Contribuciones

Si deseas contribuir al proyecto, sigue estos pasos:

1. **Haz un fork** del repositorio en GitHub

2. **Clona** tu fork en tu máquina local:
   ```bash
   git clone https://github.com/TU_USUARIO/SALC.git
   cd SALC
   ```

3. **Crea una nueva rama** para tu feature:
   ```bash
   git checkout -b feature/nombre-descriptivo
   ```

4. **Realiza tus cambios** siguiendo las convenciones del proyecto:
   - Respeta la arquitectura MVP de 3 capas
   - Aplica principios SOLID
   - Usa `SqlParameter` SIEMPRE en consultas SQL
   - Documenta métodos públicos con comentarios XML (`///`)

5. **Asegúrate de que el código compila** sin errores ni advertencias

6. **Haz commit** de tus cambios con mensajes descriptivos:
   ```bash
   git commit -am 'feat: añade validación de fecha en PacienteService'
   ```

7. **Sube** tu rama al fork:
   ```bash
   git push origin feature/nombre-descriptivo
   ```

8. **Abre un Pull Request** desde GitHub hacia el repositorio original

### Convenciones de Código

- **Nomenclatura**: PascalCase para clases y métodos, camelCase para variables
- **Arquitectura**: Separación estricta de capas (UI, BLL, DAL)
- **Seguridad**: Nunca concatenar strings en consultas SQL
- **Validación**: Nivel UI (formato) + nivel BLL (negocio)

## 12. Licencia

Este proyecto es un **trabajo académico** desarrollado como parte de la materia **Laboratorio de Computación II** de la **Tecnicatura Universitaria en Programación** de la **Universidad Tecnológica Nacional - Facultad Regional Rosario**.

El código fuente está disponible con fines educativos y de consulta. Cualquier uso fuera del contexto académico debe ser consultado con los autores.

## 13. Contacto

- **Autor**: Facundo Fernández
- **Institución**: UTN-FRRo (Universidad Tecnológica Nacional - Facultad Regional Rosario)
- **Materia**: Laboratorio de Computación II
- **Carrera**: Tecnicatura Universitaria en Programación
- **Enlace del Proyecto**: [https://github.com/aka-titesen/SALC](https://github.com/aka-titesen/SALC)

Para consultas o sugerencias sobre el proyecto, abre un **Issue** en GitHub.

## 14. Agradecimientos

- A los **docentes de Laboratorio de Computación II** por la guía, supervisión y revisión continua del proyecto
- A la **comunidad de .NET Framework** por la extensa documentación y recursos disponibles
- A **Microsoft** por proveer herramientas robustas como Visual Studio y SQL Server
- A todos los **compañeros de cursada** que aportaron feedback durante el desarrollo

---

**Desarrollado con dedicación para la UTN-FRRo | 2024**

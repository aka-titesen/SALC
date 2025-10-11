# SALC — Sistema de Administración de Laboratorio Clínico (WinForms .NET Framework 4.7.2)

Aplicación de escritorio monolítica para gestionar Pacientes, Análisis y Usuarios de un laboratorio clínico. Implementa autenticación por DNI/contraseña (hash BCrypt), control de acceso por roles y ciclo de vida completo de análisis con generación de informes PDF.

Basado en la ERS v2.7 (ver `SALC/Docs/ERS-SALC_IEEEv2.7.md`) y modelo de datos SQL Server (scripts en `SALC/Docs`).

---

## Objetivos y alcance

- Gestión de entidades maestras: Pacientes, Obras Sociales, Tipos de Análisis y Métricas.
- Control de acceso por roles: Administrador, Médico y Asistente.
- Ciclo de vida de análisis: creación, carga de resultados, validación (firma), generación y envío de informe PDF.
- Administración del sistema: usuarios, catálogos y copias de seguridad.

Fuera de alcance: turnos/agenda, facturación/contabilidad, portal web, integración HIS/LIS (en esta versión).

---

## Roles y permisos (resumen)

- Administrador: ABM completo de todas las entidades. Gestión de usuarios y backups.
- Médico: crea análisis, carga resultados, valida (firma), genera y envía informes de sus análisis; ve historial detallado solo de pacientes asociados a análisis que él cargó.
- Asistente: ve todos los pacientes, puede ver historial detallado de cualquiera, genera y envía informes solo de análisis verificados. Cada asistente tiene un único médico supervisor.

---

## Arquitectura

- UI: Windows Forms (MDI) con MVP (Passive View). Formularios/controles son Vistas; la lógica de UI vive en Presenters.
- BLL: Servicios orquestan reglas de negocio y validaciones.
- DAL: Repositorios ADO.NET (System.Data.SqlClient) con consultas parametrizadas (SqlParameter). Prohibida la concatenación de strings para SQL.
- Entidades: POCOs.

Estructura en 3 capas lógicas siguiendo SOLID y separación estricta de responsabilidades.

---

## Stack tecnológico

- Lenguaje: C#
- Framework: .NET Framework 4.7.2
- UI: Windows Forms (MDI)
- BD: SQL Server 2022 (Developer/Express)
- Acceso a datos: ADO.NET (SqlClient)
- Hashing: BCrypt (ej. paquete NuGet BCrypt.Net-Next)

---

## Requisitos del entorno

- Windows 10/11 x64
- .NET Framework 4.7.2 Developer Pack
- SQL Server 2022 (local o remoto) con conectividad TCP/IP (puerto 1433 por defecto)
- Visual Studio 2019/2022 (recomendado) o CLI `dotnet` con targeting packs para .NET Framework

---

## Estructura del repositorio (resumen)

- `SALC.sln` — solución
- `SALC/` — proyecto WinForms (.NET Framework 4.7.2)
  - `Program.cs`, formularios, presenters, servicios y repositorios
  - `Docs/` — documentación y SQL
    - `ERS-SALC_IEEEv2.7.md` — Especificación de Requisitos de Software
    - `estructura-salc.sql` — DDL (tablas, claves, checks)
    - `lote-salc.sql` — datos de ejemplo

---

## Base de datos: creación y datos de ejemplo

1) Crear base y objetos
- Abrir `SALC/Docs/estructura-salc.sql` en SQL Server Management Studio (SSMS).
- Si deseas crear la BD automáticamente, descomenta las líneas `CREATE DATABASE/USE` del inicio o crea manualmente la BD `SALC` y usa `USE [SALC];`.
- Ejecuta el script completo para crear: `roles`, `estados_analisis`, `obras_sociales`, `tipos_analisis`, `metricas`, `usuarios`, `medicos`, `asistentes`, `pacientes`, `analisis`, `analisis_metrica`.

2) Cargar datos de ejemplo
- Ejecuta `SALC/Docs/lote-salc.sql` (tras crear la estructura). Incluye roles, catálogos, usuarios (con hashes placeholder), médicos/asistentes, pacientes, análisis y resultados.

Notas importantes:
- Los `password_hash` del lote son ejemplos y deben reemplazarse por hashes reales generados por la app.
- La integridad referencial asume el orden indicado (catálogos → usuarios → extensiones → pacientes → análisis → métricas).

---

## Configuración de la aplicación

- Connection string: en `App.config` del proyecto WinForms (`SALC/App.config`). Ajusta servidor/instancia y autenticación:

```xml
<configuration>
  <connectionStrings>
    <add name="SALC_Db"
         connectionString="Data Source=localhost;Initial Catalog=SALC;Integrated Security=True;TrustServerCertificate=True"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  <!-- ...resto de configuración -->
</configuration>
```

- Hashing BCrypt: agrega la referencia NuGet si aún no está incluida.
  - Visual Studio: Administrar paquetes NuGet → Buscar "BCrypt.Net-Next".
  - Alternativa CLI: `Install-Package BCrypt.Net-Next` (Package Manager Console).

- Seguridad de consultas: todas las operaciones DAL deben usar `SqlParameter`.

---

## Compilación y ejecución

Opción A — Visual Studio
- Abrir `SALC.sln`.
- Seleccionar configuración `Debug` (x64/x86 según necesidad) y compilar.
- Establecer el proyecto `SALC` como inicio y ejecutar.

Opción B — CLI (PowerShell)
- Requisitos: SDK con targeting packs para .NET Framework 4.7.2.
- Compilar solución:

```pwsh
# En la carpeta raíz del repo
 dotnet build SALC.sln -c Debug
```

- Ejecutar el binario generado:

```pwsh
# Ruta por defecto del ejecutable (Debug)
 .\SALC\bin\Debug\SALC.exe
```

---

## UX/UI (guía breve)

- MDI con `MenuStrip` + `ToolStrip` para acciones frecuentes.
- Formularios de gestión: ToolStrip superior (Nuevo/Editar/Eliminar/Buscar) y grilla `DataGridView` principal.
- Formularios de alta/edición: diálogos modales, campos agrupados (`GroupBox`/`TabControl`). Botones Aceptar/Cancelar en esquina inferior derecha.
- Estilos del sistema (SystemColors), tipografías estándar (Segoe UI / Microsoft Sans Serif 8.25–9pt).
- Validación visual con `ErrorProvider`, mensajes con `MessageBox`.

---

## Flujos funcionales clave

- Autenticación (RF-01): DNI + contraseña → verificación de hash BCrypt contra `usuarios.password_hash` → acceso según `id_rol`.
- ABM de Usuarios/Pacientes/Catálogos (RF-02/03/04): desde grillas, altas/ediciones/eliminaciones con validaciones. En catálogos se usan diálogos rápidos (InputBox) para alta/edición.
- Crear Análisis (RF-05): Médico selecciona paciente y tipo; se registra `dni_carga` y estado inicial.
- Cargar Resultados (RF-06): Médico edita `analisis_metrica` en estado "Sin verificar".
- Validar/Firmar (RF-07): cambia `id_estado` a "Verificado", registra `dni_firma` y `fecha_firma`; los resultados quedan inmutables.
- Generar/Enviar Informe PDF (RF-08): Médico (sus análisis) o Asistente (solo verificados).
- Historial (RF-09): Asistente ve todos; Médico ve detalle solo de pacientes asociados a análisis que él cargó.
- Backups (RF-10): interfaz para ejecutar respaldos manuales de la BD; la programación se sugiere realizarla con el Programador de Tareas de Windows (pendiente de integración nativa).

---

## Requisitos no funcionales (extracto)

- Rendimiento: grillas ≤ 3 s; informes PDF ≤ 5 s.
- Seguridad: contraseñas con BCrypt; consultas parametrizadas.
- Usabilidad: consistencia UI, navegación predecible, orden de `TabIndex` lógico.
- Disponibilidad: 99% en horario laboral.
- Mantenibilidad: MVP 3 capas, SOLID, código desacoplado y testeable.

---

## Buenas prácticas (aplicadas)

- Clase centralizada de conexión (DAL) para gestionar cadena de conexión y apertura/cierre.
- Validación en Presenter (formato/obligatoriedad) y en Services (reglas de negocio).
- Reutilización de componentes y separación clara UI/BLL/DAL.

---

## Scripts y utilidades

- DDL: `SALC/Docs/estructura-salc.sql`
- Datos ejemplo: `SALC/Docs/lote-salc.sql`
- Documentación: `SALC/Docs/ERS-SALC_IEEEv2.7.md`

---

## Desarrollo y contribución

- Estándares: seguir MVP (Passive View), SOLID y consultas parametrizadas.
- Nombrado sugerido: `FrmGestionPacientes`, `PacientesPresenter`, `PacienteService`, `PacienteRepository`, etc.
- Pull Requests: incluir breves notas de arquitectura y validaciones agregadas.

---

## Roadmap (sugerido)

- Generación de PDF y envío (email/SMS) productivo con plantillas.
- Módulo de backups con programación y verificación.
- Tests unitarios para Services y Repositorios (con DB de pruebas o mocks).
- Manejo avanzado de permisos y auditoría (trazas de cambios).

---

## Licencia

Si no se especifica lo contrario, el código se distribuye bajo la licencia elegida por el propietario del repositorio. Agrega aquí el archivo `LICENSE` si corresponde.

---

## Créditos

Proyecto académico/profesional “SALC” — ver documentación en `SALC/Docs`.

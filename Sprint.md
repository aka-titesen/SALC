# Plan de Sprints — SALC (Windows Forms .NET Framework 4.7.2)

Documento de planificación técnica con sprints, tareas, artefactos y criterios de aceptación para completar el proyecto siguiendo la ERS v2.7 y el modelo de datos provisto. Nombres y código en español, arquitectura MVP (3 capas) y ADO.NET con consultas parametrizadas.

Índice rápido:
- Sprint 1: Estructura base (arquitectura, patrones y buenas prácticas) — pocos archivos, unificados
- Sprint 2: Interfaces gráficas (UI) sin lógica
- Sprint 3: Modelado de datos (database-first) y repositorios ADO.NET
- Sprint 4: Conexión a base de datos (App.config + DbConexion)
- Sprint 5: Login (RF-01)
- Sprint 6: Lógica Administrador (RF-02, RF-03, RF-04, RF-10)
- Sprint 7: Lógica Médico (RF-05, RF-06, RF-07, RF-08 parcial)
- Sprint 8: Lógica Asistente (RF-08, RF-09)
- Sprint 9: Informes PDF y envío (RF-08)
- Sprint 10: RNF, endurecimiento, pruebas de humo y pulido final

Notas generales:
- Base de datos: SQL Server 2022, scripts en `SALC/Docs/`.
- Seguridad: contraseñas con BCrypt; todas las consultas con `SqlParameter` (sin concatenación de strings).
- UI: MDI, vistas compactas organizadas por panel (Administrador/Médico/Asistente), MVP (Passive View).
- No se define proveedor de “teléfono” (SMS/WhatsApp) sin confirmación. Se deja una tarea de decisión.

---

## Sprint 1 — Estructura base (arquitectura, patrones y buenas prácticas)

Objetivo: Armar la estructura del proyecto aplicando MVP 3 capas con pocos archivos unificados por responsabilidad. Preparar cimientos sin lógica de negocio completa.

Estructura de carpetas propuesta:
- `SALC/Domain/` — Modelos (POCOs) y contratos comunes
- `SALC/DAL/` — Acceso a datos (ADO.NET)
- `SALC/BLL/` — Servicios (reglas de negocio)
- `SALC/Presenters/` — Presenters y contratos de vistas
- `SALC/Views/` — WinForms (MDI y paneles por rol)
  - `Views/PanelAdministrador/`
  - `Views/PanelMedico/`
  - `Views/PanelAsistente/`
  - `Views/Compartidos/` (controles comunes)
- `SALC/Infraestructura/` — Configuración, utilidades (seguridad, helpers)

Tareas técnicas:
1) Definiciones base de dominio (POCOs) — crear 1 clase por entidad del modelo de datos:
   - `Usuario`, `Medico`, `Asistente`, `Paciente`, `ObraSocial`, `TipoAnalisis`, `Metrica`, `Analisis`, `AnalisisMetrica`, `Rol`, `EstadoAnalisis`.
2) Contratos y clases base DAL:
   - `DbConexion` (centraliza cadena de conexión y creación de `SqlConnection`).
   - `IRepositorioBase<T>` (CRUD genérico) y `RepositorioBase` (métodos utilitarios protegidos).
   - Repositorios a definir por entidad o funcionalidad (ver Sprint 3). Para compactar, catálogos se unifican en `CatalogoRepositorio` (obras sociales, tipos de análisis, métricas).
3) Servicios base BLL:
   - `IAutenticacionService`, `AutenticacionService` (interfaz + clase vacía con firma pública).
   - `IUsuarioService`, `UsuarioService`.
   - `IPacienteService`, `PacienteService`.
   - `ICatalogoService`, `CatalogoService` (unifica catálogos).
   - `IAnalisisService`, `AnalisisService`.
   - `IBackupService`, `BackupService`.
4) MVP base y vistas mínimas (sin lógica):
   - Contratos: `ILoginView`, `IPanelAdministradorView`, `IPanelMedicoView`, `IPanelAsistenteView`.
   - Presenters: `LoginPresenter`, `PanelAdministradorPresenter`, `PanelMedicoPresenter`, `PanelAsistentePresenter` (métodos vacíos con firmas).
   - Vistas: `FrmPrincipal` (MDI), `FrmLogin`, `FrmPanelAdministrador`, `FrmPanelMedico`, `FrmPanelAsistente`.
5) Normas de estilo y prácticas:
   - Nombrado en español, métodos y propiedades descriptivos.
   - Validaciones en Presenter; reglas de negocio en Services; DB en Repositorios.
   - Manejo de excepciones encapsulado en DAL/BLL con mensajes claros en UI.

Artefactos principales (crear/modificar):
- Nuevos archivos en `Domain/`, `DAL/`, `BLL/`, `Presenters/`, `Views/`, `Infraestructura/` según lo listado.

Estado de implementación: COMPLETADO

Cambios realizados (artefactos principales):
- Carpetas: `Domain/`, `DAL/`, `BLL/`, `Presenters/`, `Views/`, `Infraestructura/`.
- POCOs creados: Usuario, Medico, Asistente, Paciente, ObraSocial, TipoAnalisis, Metrica, Analisis, AnalisisMetrica, Rol, EstadoAnalisis.
- DAL: `IRepositorioBase<T>`, `Infraestructura/DbConexion`.
- BLL: Interfaces y stubs (`IAutenticacionService`, `IUsuarioService`, `IPacienteService`, `ICatalogoService`, `IAnalisisService`, `IBackupService`) y sus clases.
- Presenters: Login/Paneles (stubs) y contratos de vistas (`ILoginView`, `IPanelAdministradorView`, `IPanelMedicoView`, `IPanelAsistenteView`).
- Views: `FrmPrincipal` (MDI), `FrmLogin`, `FrmPanelAdministrador`, `FrmPanelMedico`, `FrmPanelAsistente` (stubs compilan y abren).
- `Program.cs` ajustado para iniciar `FrmPrincipal`.
- `SALC.csproj` actualizado para incluir nuevos archivos.

Criterios de aceptación:
- Compilación exitosa (Debug) de la solución.
- Inicio de la app en `FrmLogin` dentro de `FrmPrincipal` MDI sin errores.
- Clases e interfaces creadas con nombres en español, firmas públicas definidas (sin lógica).

Verificación rápida:
- Compila: OK.
- `FrmPrincipal` abre y muestra `FrmLogin` como hijo MDI: OK.

Definición de Hecho (DoD):
- Estructura de carpetas creada y comprometida.
- Clases y contratos base presentes, sin TODOs abiertos en firmas públicas.
- README actualizado (ya existente) hace referencia a arquitectura y estructura.

---

## Sprint 2 — Interfaces gráficas sin lógica

Objetivo: Crear todas las interfaces (formularios y controles) sin implementar la lógica. Vistas compactas y organizadas por panel.

Tareas técnicas:
1) Login (sin lógica):
   - `FrmLogin`: campos DNI, contraseña, botón Acceder, `ErrorProvider`.
2) Panel Administrador:
   - `FrmPanelAdministrador` (contenedor con `TabControl` para reducir número de formularios):
     - Tab Usuarios: grilla y barra de herramientas (Nuevo/Editar/Eliminar/Buscar).
     - Tab Pacientes: idem.
     - Tab Catálogos: secciones para `ObrasSociales`, `TiposAnalisis`, `Metricas`.
     - Tab Backups: UI para ejecutar/programar.
3) Panel Médico:
   - `FrmPanelMedico` (con `TabControl`):
     - Tab Crear Análisis.
     - Tab Carga de Resultados.
     - Tab Validación/Firma.
     - Tab Generar Informe (stub, sin PDF todavía).
4) Panel Asistente:
   - `FrmPanelAsistente` (con `TabControl`):
     - Tab Historial de Pacientes.
     - Tab Generar Informe Verificado.
5) Vistas compartidas:
   - Controles comunes en `Views/Compartidos/` (filtros, selección de paciente, selección de análisis, etc.) con firmas de eventos.

Artefactos principales:
- Formularios y UserControls con nombres y controles colocados, `TabIndex` lógico, sin lógica de negocio.
- Interfaces de vistas (`ILoginView`, etc.) con eventos y propiedades de acceso a campos.

Criterios de aceptación:
- Navegación desde `FrmLogin` a cada panel (sin validación real), solo simulación.
- Todas las vistas compilan, abren y no lanzan excepciones.
- Controles y etiquetas en español, consistentes con guía UI del ERS.

DoD:
- Diseño de formularios y tabs completo, sin lógica.
- Interfaces de vistas exponen eventos (ej. `AccederClick`, `NuevoClick`, etc.).

---

## Sprint 3 — Modelado de datos (database-first) y repositorios ADO.NET

Objetivo: Alinear POCOs con el esquema real de BD y crear repositorios parametrizados por entidad/funcionalidad.

Tareas técnicas:
1) Validación de esquema:
   - Verificar BD con `SALC/Docs/estructura-salc.sql` y datos de ejemplo `lote-salc.sql`.
2) POCOs definitivos:
   - Confirmar propiedades y tipos según tablas (incluye `EstadoAnalisis`, `Rol`).
3) Repositorios (ADO.NET, SqlParameter):
   - `UsuarioRepositorio` (incluye CRUD y consultas por email/DNI/rol/estado).
   - `MedicoRepositorio`, `AsistenteRepositorio` (extensiones 1:1 con `usuarios`).
   - `PacienteRepositorio`.
   - `CatalogoRepositorio` (obras sociales, tipos de análisis, métricas).
   - `AnalisisRepositorio` (incluye cambios de estado, firma, consultas por médico/paciente).
   - `AnalisisMetricaRepositorio` (inserción/modificación de resultados).
4) Transacciones y consistencia:
   - Operaciones que afecten múltiples tablas usan `SqlTransaction`.
5) Pruebas de acceso a datos (smoke):
   - Lectura de catálogos y un CRUD mínimo de paciente y usuario en base de ejemplo.

Artefactos principales:
- Implementaciones de repositorios en `DAL/`.
- Ajustes a POCOs en `Domain/`.

Criterios de aceptación:
- Todas las consultas usan `SqlParameter` (sin concatenación de strings).
- CRUD básico funcional contra la BD de ejemplo.
- Lectura de catálogos y pacientes ≤ 3 s (RNF-01, prueba manual).

DoD:
- Repositorios compilando, métodos principales implementados y probados manualmente.
- Manejo básico de excepciones y cierre de conexiones correcto (using/dispose).

---

## Sprint 4 — Conexión a la BD (App.config + DbConexion)

Objetivo: Configurar correctamente la cadena de conexión y centralizar la apertura/cierre de conexiones.

Tareas técnicas:
1) `App.config`: agregar `connectionStrings` con nombre `SALC_Db`.
2) `DbConexion`: leer cadena desde config, exponer `CrearConexion()`.
3) Prueba de conexión: método `ProbarConexion()` y botón en UI (solo Administrador) para verificar.

Criterios de aceptación:
- Conexión exitosa a la BD configurada.
- Errores de conexión muestran mensaje claro en UI.

DoD:
- `App.config` actualizado, `DbConexion` funcionando.

---

## Sprint 5 — Login (RF-01)

Objetivo: Implementar autenticación por DNI + contraseña con BCrypt y ruteo por rol.

Tareas técnicas:
1) `AutenticacionService.ValidarCredenciales(dni, contraseña)`:
   - Obtiene usuario por DNI, verifica `estado = 'Activo'`.
   - Compara hash con BCrypt.
   - Retorna DTO con `dni`, `id_rol`, `nombre`, `apellido`.
2) `LoginPresenter`:
   - Maneja evento Acceder, invoca servicio, rutea a `FrmPanelAdministrador`/`FrmPanelMedico`/`FrmPanelAsistente`.
3) Seguridad:
   - Almacenar únicamente hash.

Criterios de aceptación:
- Usuarios del lote ingresan según credenciales (hashes válidos pre-cargados o re-generados).
- Acceso denegado a usuarios `Inactivo`.

DoD:
- Flujo de login estable y mensajes de error adecuados.

---

## Sprint 6 — Lógica Administrador (RF-02, RF-03, RF-04, RF-10)

Objetivo: Implementar ABMs y backups para el rol Administrador.

Tareas técnicas:
1) Usuarios (usuarios/medicos/asistentes):
   - Alta/Edición/Baja con integridad referencial (transacciones para usuarios + extensiones).
   - Regeneración de `password_hash` al cambiar contraseña.
2) Pacientes:
   - ABM completo y búsqueda por DNI/Apellido.
3) Catálogos:
   - Obras Sociales, Tipos de Análisis, Métricas (ABM, validaciones de rangos en métricas).
4) Backups (UI y lógica):
   - Ejecutar backup manual (T-SQL `BACKUP DATABASE` a ruta configurable).
   - Programación simple (tareas de Windows) — registrar/guardar configuración.

Criterios de aceptación:
- ABMs funcionan y validan datos (UI y BLL) siguiendo ERS.
- Backup genera archivo `.bak` válido en ruta configurada.

DoD:
- Operaciones clave cubiertas, manejo de errores y confirmaciones en UI.

---

## Sprint 7 — Lógica Médico (RF-05, RF-06, RF-07, RF-08 parcial)

Objetivo: Implementar flujo completo del médico para análisis y resultados.

Tareas técnicas:
1) Crear Análisis (RF-05):
   - Selección de paciente y tipo de análisis, registrar `dni_carga` y estado inicial ("Sin verificar").
2) Cargar Resultados (RF-06):
   - CRUD sobre `analisis_metrica` mientras el análisis esté "Sin verificar".
3) Validar/Firmar (RF-07):
   - Cambiar estado a "Verificado", setear `dni_firma` y `fecha_firma`.
   - Bloquear edición de resultados luego de firmar.
4) Generar Informe (stub):
   - Preparar datos necesarios para PDF (sin render aún).

Criterios de aceptación:
- Un médico solo ve y manipula análisis creados por él (`dni_carga`).
- Resultados inmutables tras firma.

DoD:
- Flujo sin PDF operativo y validado con la BD de ejemplo.

---

## Sprint 8 — Lógica Asistente (RF-08, RF-09)

Objetivo: Implementar permisos y vistas del asistente.

Tareas técnicas:
1) Historial (RF-09):
   - Ver lista de todos los pacientes y detalle de análisis de cualquier paciente.
2) Generar Informe (RF-08):
   - Disponible solo si estado = "Verificado".

Criterios de aceptación:
- Asistente puede ver todo el historial; no puede modificar resultados ni estados.

DoD:
- Reglas de autorización validadas en BLL y Presenters.

---

## Sprint 9 — Informes PDF y envío (RF-08)

Objetivo: Generar informe PDF y enviarlo por email y teléfono.

Tareas técnicas:
1) Generación de PDF:
   - `InformeService`: plantilla simple con datos del paciente, análisis, resultados, rangos de referencia y firma del médico.
   - Evaluar dependencia (p.ej., iTextSharp para .NET Framework). Confirmar licencia antes de incluir.
2) Envío por email:
   - `NotificacionService` con `EmailNotificador` (System.Net.Mail `SmtpClient`).
   - Configuración SMTP en `App.config` (host, puerto, credenciales, SSL).
3) Envío por teléfono (decisión requerida):
   - Tarea de decisión: definir canal (SMS/WhatsApp) y proveedor.
   - Alternativa temporal: exportar PDF a carpeta y registrar "pendiente de envío externo".

Criterios de aceptación:
- Médicos y asistentes pueden generar PDF; asistentes solo para "Verificado".
- Envío por email funcional (con una casilla de pruebas).
- Para teléfono: o integración definida y operativa, o salida alternativa documentada.

DoD:
- PDF generado ≤ 5 s (RNF-01). Correos enviados y registrados en logs básicos.

---

## Sprint 10 — RNF, endurecimiento y pulido final

Objetivo: Cumplir RNF, mejorar robustez y experiencia.

Tareas técnicas:
1) Rendimiento:
   - Revisiones de índices en BD (dnis, claves foráneas, búsquedas frecuentes).
   - Paginación de grillas cuando corresponda.
2) Seguridad:
   - Validaciones de entrada exhaustivas en Presenter.
   - Revisar que todas las consultas usan `SqlParameter`.
3) Usabilidad/UI:
   - Consistencia visual, `TabIndex`, mensajes y confirmaciones.
4) Disponibilidad/Estabilidad:
   - Manejo de excepciones, reintentos mínimos de conexión, mensajes claros.
5) Documentación y pruebas:
   - Actualizar `README.md` y agregar notas de despliegue.
   - Pruebas de humo guiadas por RFs.

Criterios de aceptación:
- RNF cumplidos (rendimiento y seguridad verificados manualmente).
- Flujo integral del sistema operando sin errores.

DoD:
- Build limpio, smoke tests OK, documentación actualizada.

---

## Convenciones y lineamientos (aplican a todos los sprints)

- Nombres en español, descriptivos y alineados a ERS.
- MVP estricto: Vista (Form/UserControl) no contiene lógica de negocio.
- Servicios orquestan reglas; Repositorios encapsulan SQL.
- Todas las consultas parametrizadas (`SqlParameter`).
- Validaciones:
  - Presenter: formato y obligatoriedad (DNI numérico, email válido, rangos).
  - Service: reglas de negocio (permisos, estados, inmutabilidad post-firma).
- No se inventan reglas ni campos fuera de ERS/modelo de datos. Cualquier duda se eleva antes de implementar.

---

## Mapeo RF ↔ Sprints

- RF-01 (Login): Sprint 5
- RF-02 (ABM Usuarios): Sprint 6
- RF-03 (ABM Pacientes): Sprint 6
- RF-04 (ABM Catálogos): Sprint 6
- RF-05 (Crear Análisis): Sprint 7
- RF-06 (Cargar Resultados): Sprint 7
- RF-07 (Validar/Firmar): Sprint 7
- RF-08 (Generar/Enviar Informe): Sprints 7 (datos), 8 (permisos asistente), 9 (PDF+envío)
- RF-09 (Historial): Sprint 8
- RF-10 (Backups): Sprint 6

---

## Dependencias y decisiones pendientes

- Proveedor de envío por “teléfono” (SMS/WhatsApp). Requiere definición del cliente.
- Plantilla de PDF (branding, logo, formato). Se propone diseño simple inicialmente.

---

## Definición general de Hecho (DoD global)

- Compila en Debug, sin errores.
- Consulta a BD funcional con cadena en `App.config`.
- Acceso por roles cumple restricciones de visibilidad/edición.
- Consultas parametrizadas exclusivamente.
- Generación de PDF ≤ 5 s; carga de grillas ≤ 3 s en datos de ejemplo.
- Documentación de uso y despliegue actualizada.

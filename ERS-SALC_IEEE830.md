# **Especificación de Requisitos de Software**

[**Proyecto: Sistema de Administración de Laboratorio Clínico	2**](#salc:-sistema-de-administración-de-laboratorio-clínico)

[1\. Introducción	2](#introducción)

[1.1 Propósito	2](#heading=h.tr9qh7gy8o2l)

[1.2 Alcance	2](#alcance)

[1.3 Personal involucrado	3](#personal-involucrado)

[1.4 Definiciones, acrónimos y abreviaturas	4](#definiciones,-acrónimos-y-abreviaturas)

[1.5 Referencias	4](#referencias)

[1.6 Resumen	4](#resumen)

[2\. Descripción General	5](#descripción-general)

[2.1 Perspectiva del producto	5](#heading=h.101mr0j9a9et)

[2.2 Funcionalidades principales	5](#funcionalidades-principales)

[2.3 Características de los usuarios	6](#características-de-los-usuarios)

[2.4 Restricciones	7](#restricciones)

[2.5 Suposiciones y dependencias	7](#suposiciones-y-dependencias)

[2.6 Evolución previsible del sistema	8](#evolución-previsible-del-sistema)

[3\. Requisitos Específicos	8](#requisitos-específicos)

[3.1 Requisitos comunes de los interfaces	9](#requisitos-comunes-de-los-interfaces)

[3.1.1 Interfaces de usuario	9](#interfaces-de-usuario)

[3.1.2 Interfaces de hardware	9](#interfaces-de-hardware)

[3.1.3 Interfaces de software	9](#interfaces-de-software)

[3.1.4 Interfaces de comunicación	9](#interfaces-de-comunicación)

[3.2 Requisitos Funcionales	10](#requisitos-funcionales)

[3.3 Requisitos No Funcionales	12](#requisitos-no-funcionales)

[4\. Apéndices	13](#apéndices)

[4.1 Stack tecnológico	13](#heading=h.yqp5dwjxrm0j)

[Lenguaje y Framework Principal	13](#heading=h.1odod1wldvby)

[Base de Datos	13](#heading=h.3n50fcryl9f7)

[Librerías y Dependencias Principales	13](#heading=h.x73tntmrmd4r)

[Frontend (Interfaz de Usuario de Escritorio)	14](#heading=h.lvhzz1l7pup3)

[Control de Versiones y CI/CD	14](#heading=h.v9h8kot9y6qd)

[Pruebas y QA	14](#heading=h.i4ixf8oib3g2)

[Documentación y Estándares	15](#heading=h.hb100ej0phte)

[Requisitos de Entorno	15](#heading=h.gor55mm466tt)

[4.2 Arquitectura y Patrones de Diseño	15](#heading=h.omuqsz81etgr)

[Capas Definidas:	15](#heading=h.1ds5p6ijnm56)

[1.2 Patrones de Diseño Asociados	16](#heading=h.1411hfquqif)

[1.3 Principios de Diseño y Buenas Prácticas	17](#heading=h.3h4oprjvectu)

[Principios SOLID (aplicados de forma ligera)	17](#heading=h.xptxqq9wcs7)

[Buenas Prácticas Complementarias	18](#heading=h.cbbo9juw5rie)

[1.4 Ejemplo Simplificado de Flujo con MVP	18](#heading=h.hvpemiq0aul7)

[4.3 Lineamientos de UX/UI	18](#lineamientos-de-ux/ui)

[1\. Principios de Diseño	18](#heading=h.c4eicb9m0v9j)

[2\. Estilo Visual	19](#heading=h.p9wqr9xqn58t)

[3\. Sistema de Colores	19](#heading=h.oxumgae6umfm)

[4\. Tipografía	19](#heading=h.ksg795kvna9j)

[5\. Iconografía	19](#heading=h.t9f2wcc7adg1)

[6\. Componentes UI	20](#heading=h.7mtk5gojssj)

[7\. Responsividad	20](#heading=h.1x957csc2x5x)

[4.4 Modelo de datos	20](#diseño-fisico-de-base-de-datos-en-sql-server-+-restricciones-y-constraints)

[**4.5 Arquitectura de la Información (AI)	24**](#heading=h.sqgtlpfvab45)

[4.5.1 AI General del Sistema	25](#heading=h.ad4vmcjc6tiq)

[4.5.1.1 Estructura Global	25](#heading=h.ug80l5gxiovf)

[4.5.1.2 Etiquetado Global	25](#heading=h.wanirbs7jqqo)

[4.5.1.3 Navegación Global	25](#heading=h.wxm3fufq74yf)

[4.5.2 AI por Vista/Pantalla	26](#heading=h.grzhytiohnom)

[4.5.2.1 Login	26](#heading=h.j1b7o4xauld9)

[4.5.2.2 Dashboard (Inicio por rol)	26](#heading=h.yqh6iqvokax5)

[4.5.2.3 Gestión de Usuarios y Roles (Administrador)	26](#heading=h.l55ug8l55o4i)

[4.5.2.4 Pacientes y Obras Sociales	26](#heading=h.1wf8dd2jz666)

[4.5.2.5 Análisis y Resultados	26](#heading=h.zfy8ldoa7c2e)

[4.5.2.6 Métricas y Parámetros (Administrador/Clínico avanzado)	27](#heading=h.kl6mtwfyur8y)

[4.5.2.7 Turnos (Clínico/Asistente)	27](#heading=h.6q3r2849m6av)

[4.5.2.8 Informes (Clínico/Asistente)	27](#heading=h.9s0iufv9licn)

[4.5.2.9 Configuración del Sistema (Administrador)	27](#heading=h.m84slretiyaj)

[4.5.2.10 Soporte / Ayuda / Acerca de	27](#heading=h.7afujredifrm)

[4.5.3 Objetivos de la AI del SALC	28](#heading=h.16gd934eyj9s)

# **SALC: Sistema de Administración de Laboratorio Clínico** {#salc:-sistema-de-administración-de-laboratorio-clínico}

1. # **Introducción** {#introducción}

   1. ## **Propósito**

El propósito de este documento es definir los requisitos del **Sistema de Administración de Laboratorio Clínico (SALC)**, orientado a digitalizar y optimizar los procesos centrales del laboratorio:

* Registro y gestión de pacientes y sus obras sociales.  
* Administración de análisis clínicos y resultados asociados.  
* Carga estructurada de métricas con parámetros de referencia.  
* Generación de informes y notificación segura a los pacientes.  
* Control de accesos mediante roles diferenciados (Administrador, Clínico y Asistente).

El objetivo es disponer de una herramienta confiable y sencilla que garantice precisión en los datos, trazabilidad y eficiencia en la operación diaria del laboratorio.

2. ## **Alcance** {#alcance}

El **Sistema de Administración de Laboratorio Clínico (SALC)** permitirá gestionar pacientes, obras sociales, análisis clínicos y resultados de forma centralizada. Incluirá carga de métricas, generación de informes y notificación a pacientes, con control de accesos según roles (Administrador, Clínico y Asistente).

El sistema se enfocará en la operativa interna del laboratorio, sin contemplar módulos adicionales como gestión de turnos o agenda médica.

3. ## **Personal involucrado** {#personal-involucrado}

| Rol	 | Responsabilidad legal	 | Alcance funcional (solo acciones habilitadas)  |
| :---- | :---- | :---- |
| Administrador	 | Gestión técnica del sistema	 | ABM usuarios/roles, catálogos (análisis, métricas, obras sociales), backup, configuración. |
| Clínico / Médico | Único que valida y firma resultados (ley 18.283)	 | Crear análisis, cargar valores, validar (RF-18), imprimir, notificar paciente. |
| Asistente / Técnico | Recepción y trazabilidad de muestras | Registrar recepción (RF-17), imprimir informes ya validados, gestión logística bajo supervisión de un Clínico. |

**Relación jerárquica:**

* Cada **Asistente** tiene **un único supervisor** (asistente.dni\_supervisor → doctor.dni).  
* Solo el **Clínico** puede cambiar el estado de análisis a “Verificado” y firmar digitalmente (analisis.dni\_firma).  
* En el código del proyecto se nombra solo como clínico y asistente

  4. ## **Definiciones, acrónimos y abreviaturas** {#definiciones,-acrónimos-y-abreviaturas}

**SALC**: Sistema de Administración de Laboratorio Clínico.

**AI**: Arquitectura de la Información.

**UI**: Interfaz de Usuario.

**UX**: Experiencia de Usuario.

**CRUD**: Crear, Leer, Actualizar y Eliminar registros.

**Paciente**: Persona registrada en el sistema para la gestión de análisis clínicos.

**Clínico**: Usuario del laboratorio que gestiona pacientes, análisis y resultados.

**Administrador**: Usuario encargado de configuración y gestión técnica del sistema.

**Asistente**: Usuario de apoyo administrativo en informes y organización.

**Doctor**: Profesional médico externo registrado como responsable de un análisis, sin acceso al sistema.

5. ## **Referencias** {#referencias}

   

* IEEE 830-1998: Estándar para Especificación de Requisitos de Software.  
* Documentación oficial de Microsoft SQL Server 2022\.  
* Documentación oficial de .NET 7 y Windows Forms.  
* Guías de estilo de interfaz de usuario de Microsoft Windows.


  6. ## **Resumen** {#resumen}

El presente documento define los requisitos del Sistema de Administración de Laboratorio Clínico (SALC). Se establecen objetivos, alcance, perfiles de usuario, arquitectura de datos y lineamientos técnicos necesarios para guiar el desarrollo. El sistema se centra en la gestión de pacientes, usuarios y análisis clínicos, garantizando trazabilidad, seguridad y usabilidad.

2. # **Descripción General** {#descripción-general}

   1. ## **Perspectiva del producto**

El SALC es un sistema de software independiente, diseñado para la gestión de pacientes, usuarios y análisis clínicos dentro de un laboratorio. Centraliza el registro, seguimiento y consulta de resultados, garantizando integridad de datos y acceso seguro según perfiles definidos.

2. ## **Funcionalidades principales** {#funcionalidades-principales}

   

* Gestión de usuarios y roles.  
* Registro y administración de pacientes.  
* Definición de tipos de análisis y métricas.  
* Carga y actualización de resultados de análisis.  
* Control de estados de análisis y usuarios.  
* Generación y consulta de informes.  
* Notificación a pacientes sobre resultados.


  3. ## **Características de los usuarios** {#características-de-los-usuarios}

**Usuarios internos (acceso al sistema)**

El SALC contempla tres perfiles internos, con **responsabilidades excluyentes**:

1.  ADMINISTRADOR:   
   1. Responsabilidad legal: único con acceso total al motor y configuraciones críticas.

| Módulo / Pantalla	 | Funciones habilitadas	 | Restricciones / Reglas |
| :---- | :---- | :---- |
| Login | Ingresar con DNI \+ password (BCrypt)	 | Debe tener rol \= 1 en usuario.id\_rol |
| Dashboard | Ver todos los accesos rápidos	 | Ninguna  |
| Gestión de usuarios	 | Alta, baja lógica, edición, reset de clave, asignar rol, cambiar estado	 | No puede auto-darse de baja; valida DNI único y email único  |
| Gestión de catálogos	 | ABM de tipo\_analisis, metrica, obra\_social, estado, estado\_usuario | Valida CUIT único, nro\_matricula único, rango de valores de métrica |
| Configuración del sistema	 | Ruta de backup, días de vencimiento de clave, políticas de seguridad	 | Solo visible para este rol |
| Backup / Restore	 | Ejecutar y programar backups vía SQL Server Agent o PowerShell	 | Guarda log en archivo local (Serilog)  |
| Auditoría | Consulta de logs de errores y eventos de usuario	 | Solo lectura; no edita logs  |
| Informes	 | Imprime cualquier resultado ya validado	 | Sin límite de fecha ni paciente |

Diferenciador: único que puede crear o desactivar usuarios y modificar catálogos.

2. CLÍNICO:  
   1. Responsabilidad legal: único que puede validar y firmar resultados (ley 18.283).

| Módulo / Pantalla	 | Funciones habilitadas	 | Restricciones / Reglas |
| :---- | :---- | :---- |
| Login | Ingresa con DNI \+ password	 | Rol \= 2 en usuario.id\_rol |
| Dashboard | Acceso rápido a Crear análisis, Análisis pendientes, Resultados por validar	 | No ve opciones de administración  |
| Alta / edita paciente	 | CRUD completo	 | Ninguna |
| Crear análisis	 | Selecciona paciente, tipo, doctor solicitante, observaciones	 | Solo si el paciente está activo  |
| Cargar resultados	 | Ingresa valores numéricos por métrica; sistema valida rangos (valor\_min/max) | Solo cuando estado \= “Sin verificar”  |
| Validar resultado (RF-18)	 | Presiona “Validar” → sistema graba dni\_firma, fecha\_firma, cambia estado a “Verificado” | Obligatorio antes de imprimir o notificar  |
| Imprimir / exportar PDF	 | Genera informe con encabezado del laboratorio y tabla de valores	 | Solo después de validar (estado \= 2\)  |
| Notificar paciente (RF-16)	 | Envia PDF por email y/o WhatsApp/SMS con link temporal 48 h	 | Solo después de validar; datos de contacto desde paciente.mail/telefono  |
| Ver historial	 | Lista todos los análisis del paciente con estado y firma	 | Solo lectura  |

Diferenciador: botón “Validar” visible solo para este rol; nunca puede recepcionar muestras ni estar bajo supervisión.

3. ASISTENTE  
   1. Responsabilidad legal: cero sobre valores clínicos; trabaja bajo supervisión de un Clínico.

| Módulo / Pantalla	 | Funciones habilitadas	 | Restricciones / Reglas |
| :---- | :---- | :---- |
| Login | Ingresa con DNI \+ password	 | Rol \= 3 en usuario.id\_rol |
| Dashboard | Accesos rápidos: Recepcionar muestra, Imprimir informe, Listado de análisis	 | No ve botones “Crear”, “Cargar resultado”, “Validar”  |
| Alta / edita paciente	 | CRUD completo	 | Ninguna |
| Recepcionar muestra (RF-17)	 | Escanea o ingresa código, graba dni\_recepcion \+ timestamp	 | Solo si el paciente está activo; no puede validar  |
| Imprimir informe	 | Imprime PDF de análisis ya validado (estado \= “Verificado”)	 | Botón deshabilitado si estado ≠ 2  |
| Ver resultado	 | Solo lectura; no edita valores	 | Muestra etiqueta “Validado por: Dr. X – fecha/hora”  |
| Gestión de entrega	 | Marca como entregado (opcional campo fecha\_entrega)	 | Solo lectura de valores  |

Diferenciador: botón “Validar” inexistente; siempre figura su supervisor en la etiqueta “Supervisor: Dr. Juan Pérez”.

Relaciones y reglas transversales

* Supervisión: cada asistente.dni\_supervisor → doctor.dni (1-N).  
* Firma digital: analisis.dni\_firma \= doctor.dni obligatorio para estado “Verificado”.  
* Recepción: analisis.dni\_recepcion \= asistente.dni (logística).  
* Estado de análisis:  
  * 1 \= Sin verificar → puede cargar valores solo Clínico.  
  * 2 \= Verificado → solo Clínico puede imprimir/notificar.  
* Habilitación de botones: se controla por Enabled \= true/false según rol; nunca se ocultan opciones (coherencia visual).

**Entidades externas (sin acceso al sistema)**

* Paciente  
  * Recibe resultado vía email y/o WhatsApp/SMS cuando el Clínico presiona “Notificar”.  
  * El mensaje incluye: nombre del laboratorio, nombre del paciente, nº de análisis  
  * PDF adjunto (email) o link temporal de descarga de 48 h (WhatsApp/SMS).  
  * Los datos de contacto se toman directamente de la tabla paciente.mail y paciente.telefono (se asume que el teléfono es un celular con WhatsApp)  
* Doctor solicitante  
  * Solo se registra en analisis.doctor\_encargado para trazabilidad; sin credenciales de acceso.

  4. ## **Restricciones** {#restricciones}

     

* El sistema será una aplicación de escritorio basada en **Windows Forms**.  
* La base de datos debe implementarse en **SQL Server**.  
* Solo podrán acceder **Administrador, Clínico y Asistente** mediante autenticación con credenciales.  
* El sistema no contempla la gestión de turnos ni la interacción directa de pacientes o doctores.  
* Se requiere garantizar integridad referencial y seguridad de datos conforme a normas de confidencialidad clínica.


  5. ## **Suposiciones y dependencias** {#suposiciones-y-dependencias}

     

* Se asume que los usuarios poseen conocimientos básicos de informática y del flujo de trabajo en laboratorio clínico.  
* El sistema depende de la disponibilidad de **SQL Server** y del correcto funcionamiento del servidor de base de datos.  
* Se asume conectividad de red estable para el acceso multiusuario dentro de la institución.  
* La seguridad de acceso depende de la correcta gestión de credenciales por parte de los usuarios.

  6. ## **Evolución previsible del sistema** {#evolución-previsible-del-sistema}

El sistema podrá ampliarse en el futuro con módulos adicionales, como integración con sistemas hospitalarios externos, generación de reportes avanzados, soporte para firma digital de informes y notificaciones automáticas a pacientes por correo o mensajería.

3. ## **Requisitos Específicos** {#requisitos-específicos}

   1. ## **Requisitos comunes de los interfaces** {#requisitos-comunes-de-los-interfaces}

      1. ### **Interfaces de usuario** {#interfaces-de-usuario}

         

* UI-01 Interfaz gráfica en Windows Forms (.NET 4.7.2), diseñada con controles estándar sin librerías externas.  
* UI-02 Resolución mínima: 1280×720 px.  
* UI-03 Todos los formularios incluirán botones Aceptar/Cancelar y validación básica (campos vacíos, tipos de datos).  
* UI-04 El idioma será español (Argentina).  
* UI-05 No se incluirán accesos rápidos de teclado ni personalización visual en esta versión

  2. ### **Interfaces de hardware** {#interfaces-de-hardware}

     

* HW-01 PC con Windows 10 o 11 de 64 bits.  
* HW-02 Conexión a red local para acceder al servidor SQL Server.  
* HW-03 Impresora local o de red para imprimir informes en A4 (solo texto, sin gráficos avanzados).	


  3. ### **Interfaces de software** {#interfaces-de-software}

     

* SW-01 .NET 7.0 Runtime debe estar instalado en la máquina cliente.  
* SW-02 Entity Framework Core 7.0.11 se usará para acceder a SQL Server 2022\.  
* SW-03 Cadena de conexión se guardará en appsettings.json sin cifrado (por ahora).  
* SW-04 No se usarán librerías de terceros ni componentes externos en esta versión.


  4. ### **Interfaces de comunicación** {#interfaces-de-comunicación}

     

* COM-01 Comunicación directa con SQL Server 2022 por TCP/IP, puerto 1433\.  
* COM-02 No hay servicios web, APIs ni notificaciones externas.  
* COM-03 Si no hay conexión, el sistema mostrará un mensaje de error y cerrará la ventana de login


  2. ## **Requisitos Funcionales** {#requisitos-funcionales}

| Código	 | Nombre | Actor	 | Flujo básico / reglas de negocio  |
| :---- | :---- | :---- | :---- |
| RF-01	 | Iniciar sesión	 | Todos	 | Valida usuario.password (BCrypt) y estado; devuelve rol y DNI. |
| RF-02	 | Cerrar sesión	 | Todos	 | Invalida token interno y vuelve al login. |
| RF-03	 | Alta paciente	 | Clínico, Asistente | Inserta en paciente; valida único (tipo\_doc, dni). |
| RF-04	 | Modificar paciente | Clínico, Asistente | UPDATE con mismo CHECK que RF-03. |
| RF-05	 | Listar pacientes | Clínico, Asistente | Grid filtrable; doble clic abre ficha. |
| RF-06	 | Crear análisis	 | Clínico	 | INSERT en analisis (estado \= 1); encargado\_carga \= usuario.dni. |
| RF-07	 | Cargar resultados | Clínico	 | Edita analisis\_metrica mientras estado \= 1; valida valor\_min/max. |
| RF-17	 | Recepcionar muestra | Asistente	 | UPDATE analisis.dni\_recepcion \= asistente.dni \+ timestamp; habilita RF-06. |
| RF-18	 | Validar resultado | Clínico | UPDATE analisis (estado \= 2, dni\_firma \= doctor.dni, fecha\_firma \= GETDATE()); bloquea edición. |
| RF-16	 | Notificar paciente | Clínico	 | Solo si estado \= 2; envía email/WhatsApp con PDF adjunto \+ link 48 h. |
| RF-10	 | Gestionar usuarios | Administrador	 | ABM usuario, doctor, asistente; valida emails y matrículas únicas. |
| RF-11	 | Gestionar tipos análisis	 | Administrador	 | CRUD tipo\_analisis; baja lógica si no tiene análisis. |
| RF-12	 | Gestionar métricas | Administrador	 | CRUD metrica \+ asociación a tipos; valida valor\_min \<= valor\_max. |
| RF-13	 | Gestionar obras sociales	 | Administrador	 | CRUD obra\_social; CUIT único. |
| RF-14	 | Cambiar contraseña | Todos | UPDATE password (BCrypt) tras validar anterior. |

Reglas transversales:

* Botones habilitados por rol (Enabled \= true/false).  
* Supervisor siempre visible para Asistente: “Supervisor: Dr. Juan Pérez”.  
* Ningún externo (paciente/doctor solicitante) posee credenciales.

  3. ## **Requisitos No Funcionales** {#requisitos-no-funcionales}

| Código  | Requisito | Valor / Criterio |
| :---- | :---- | :---- |
| RNF-01	 | Tiempo de respuesta	 | Pantallas ≤ 2 s; impresión ≤ 5 s. |
| RNF-02	 | Disponibilidad	 | Laboratorio abierto: 08-20 hs, l-v (sin HA). |
| RNF-03	 | Seguridad	 | Login por DNI+clave; sin cifrado de columnas en v1.0. |
| RNF-04	 | Backup	 | Copia diaria full SQL Server; retención 7 días.  |
| RNF-05	 | Logs	 | Solo errores de aplicación; archivo local, rotación 30 días.  |
| RNF-06	 | Usuarios concurrentes	 | Máx 5; sin control de bloqueos. |
| RNF-07	 | Interfaz	 | Windows Forms, resolución 1280×720 mínimo.  |
| RNF-08	 | InstalacióN | Setup MSI incluye .NET 7 Runtime; sin conexión Internet requerida.  |

     

4. # **Apéndices** {#apéndices}

   1. ## **Stack tecnológico y versiones**

| Capa | Tecnología | Versión mínima | Propósito / Justificación |
| :---- | :---- | :---- | :---- |
| Lenguaje	 | C\# | 11 | Sintaxis moderna con nullable reference types. |
| Runtime	 | .NET | 4.7.2 | Soporte hasta mayo 2024; sin actualizaciones mayores durante el proyecto. |
| IDE	 | Visual Studio 2022 | 17.7	 | Compilador de C\# 11 y diseñador visual de WinForms. |
| Base de datos	 | SQL Server 2022 | Developer/Express | Motor local; compatibilidad 160\. |
| ORM	 | Entity Framework Core | 6 | Mapeo código-primero; sin cambios de esquema en producción. |
| Acceso a datos | Microsoft.Data.SqlClient	 | 5.1.0	 | Driver oficial, cifrado TLS 1.2 por defecto. |
| Hash de claves | BCrypt.Net-Next | 4.0.3	 | Algoritmo bcrypt (work-factor 12); reemplaza SHA-1.  |
| PDF | PdfSharpCore	 | 1.3.41	 | MIT, sin dependencia de System.Drawing; genera informes A4. |
| Email	 | System.Net.Mail | Nativo .NET 7	 | SMTP simple; credenciales en appsettings.json (sin secrets vault en v1.0). |
| WhatsApp/SMS | Twilio SDK	 | 5.81.0	 | Envío de link temporal; API-key en variable de entorno TWILIO\_KEY.  |
| UI | Windows Forms | Nativo .NET 7	 | No se agregan skins ni librerías de terceros. |
| Control de versiones | Git	 | 2.43	 | Flujo GitHub Flow (main \+ feature/PR). |
| CI	 | GitHub Actions | ubuntu-latest	 | Paso build \+ dotnet test (xUnit); artefacto: MSI. |
| Tests	 | xUnit \+ Moq		 | 2.5.0	 | Cobertura mínima 70 % (coverlet); Views excluidas. |
| Documentación código | DocFX	 | 2.76.2 | Genera sitio estático desde comentarios XML. |
| Sistema operativo | Windows 11 Pro x64 | 21H2	 | Runtime cliente; .NET 7 runtime incluido en MSI. |
| Validación | FluentValidation | 11.0	 | Reglas en capa de negocio; View solo pinta errores. |
| Logging | Serilog | 3.0	 | File (rolling 1 MB) \+ Debug; guarda excepciones y eventos. |
| Hardware mínimo | \- | 4 núcleos, 8 GB RAM, 500 GB HDD | Requisito para ejecutar SQL Server \+ WinForms sin swap. |

**Nota:** todas las dependencias se instalan vía NuGet con PackageReference; no se requiere GAC ni componentes COM.

2. ## **Arquitectura y patrones**

| Patrón / capa	 | Implementación concreta	 | Justificación |
| :---- | :---- | :---- |
| Capas físicas	 | Presentación → Negocio → Datos (todas dentro del EXE)	 | Suficiente para ≤ 5 usuarios concurrentes; sin servicios remotos.  |
| UI	 | Model-View-Presenter (Passive View)	 | WinForms no tiene MVVM; separa lógica de pantalla y permite tests unitarios.  |
| Repository	 | 1 interfaz por agregado (IPacienteRepository, IAnalisisRepository)	 | Aísla persistencia; implementación con EF Core.  |
| Inyección de dependencias	 | Constructor manual en Program.cs | new PacientePresenter(new PacienteService(new PacienteRepository(ctx))); sin contenedor. |
| Validación	 | FluentValidation 11.0 en Servicio | RuleFor(p \=\> p.Nombre).NotEmpty().Length(2,50); View solo pinta errores. |
| Transacciones | SaveChanges() por defecto; BeginTransactionAsync() si \>1 agregado	 | No hay UnitOfWork genérico.  |
| Logging | Serilog 3.0 → File (rolling 1 MB) \+ Debug	 | Log.Information("Análisis {Id} validado por {User}", id, dni); |
| Tests | xUnit \+ Moq; cobertura mínima 70 %	 | Presenters y servicios; Views excluidas. |
| Nomenclatura | PacienteFrm, PacientePresenter, PacienteService, PacienteRepository, PacienteDto | Evita “Form1”, “Class1”, etc. |

**Flujo típico**:

Form (View) → evento → Presenter → Service → Repository (EF Core) → SQL Server.

3. ## **Lineamientos de UX/UI** {#lineamientos-de-ux/ui}

| Tema | Regla fija	 | Implementación concreta |
| :---- | :---- | :---- |
| Dimensión de ventanas	 | Centradas en 1024×768 px; no se recuerda tamaño	 | StartPosition \= CenterScreen, FormBorderStyle \= FixedSingle  |
| Fondo y colores	 | Blanco puro (\#FFFFFF) en formularios y paneles	 | Sin personalización de BackColor; hereda SystemColors.Window |
| Tipografía | Microsoft Sans Serif 9 pt en todos los controles	 | Negrita solo en títulos de grupo |
| Colores de acción	 | \<ul\>\<li\>\<li\>\<li\>	 | Solo BackColor del botón; sin gradientes ni imágenes |
| Iconos | Solo 3: guardar, imprimir, salir (16×16 px monocromo) | button.Image \= Properties.Resources.save; TextImageRelation \= ImageBeforeText |
| Distribución | Etiqueta izquierda 120 px, campo derecha 250 px	 | TableLayoutPanel con separación 6 px |
| Grids | DataGridView sin edición, filas blancas, encabezado SystemColors.Control	 | Solo lectura; SelectionMode \= FullRowSelect |
| Orden de tabulación	 | Campos → Aceptar → Cancelar	 | TabIndex secuencial en diseñador |
| Mensajes | MessageBox estándar; título “SALC – \<módulo\>”	 | Sin mensajes técnicos al usuario |
| Teclas rápidas	 | Alt+G (Guardar), Alt+C (Cancelar), Esc (cerrar modal)	 | Declaradas con & en Text del botón |
| Scroll	 | Solo vertical si altura \> 700 px; sin redimensión automática	 | AutoScroll \= true, AutoSize \= false |

**Diferenciadores por rol**:

* Clínico: botón “Validar” visible y habilitado.  
* Asistente: etiqueta “Supervisor: Dr. Juan Pérez” visible siempre.  
* Administrador: módulo “Configuración” visible.

  4. ## **Arquitectura de la Información (AI)**

| Código | Vista / Rol	 | Información mostrada (orden de tabulación)	 | Botones de acción (inferior derecha)  |
| :---- | :---- | :---- | :---- |
| V-01	 | Login (todos)	 | DNI (solo números), Contraseña (mask) | Ingresar, Salir  |
| V-02	 | Dashboard (todos) | Cuadrilla 6 botones (180×80) con texto; barra estado: “Usuario – Rol” | Solo lanzadores  |
| V-03	 | Alta Paciente (Clínico, Asistente) | Grupo “Datos”: Tipo Doc, Nro Doc, Nombre, Apellido, Sexo, Fecha Nac, Obra Social (combo). Grupo “Contacto”: Teléfono, Mail.	 | Guardar, Cancelar  |
| V-04	 | Listado Pacientes (Clínico, Asistente) | Grid: Nro Doc │ Nombre │ Obra Social │ Teléfono. Filtro rápido: apellido (textbox inmediato). | Nuevo, Editar, Cínico/Asistente)  |
| V-05	 | Crear Análisis (solo Clínico)	 | 1\. Paciente (buscador ya seleccionado) 2\. Tipo Análisis (combo) 3\. Doctor (combo, nullable) 4\. Observaciones (texto libre) | Crear, Cancelar  |
| V-06	 | Cargar Resultados (Clínico)	 | Grid editable: Métrica │ Unidad │ Valor │ Referencia. Encabezado fijo: Paciente – Tipo Análisis – Fecha. | Guardar, Validar (solo Clínico), Cancelar  |
| V-07	 | Ver Resultado (Clínico, Asistente) | Solo lectura: mismo grid que V-06 \+ observaciones abajo. Etiqueta: “Validado por: Dr. X – fecha/hora”	 | Imprimir, Cerrar  |
| V-08	 | Generar Informe (Clínico, Asistente) | Paciente (readonly), Título (textbox), Contenido (RichText solo negrita/cursiva), Fecha auto.	 | Vista Previa, Exportar PDF, Cerrar  |
| V-09	 | Gestión Usuarios (Administrador) | Grid: DNI │ Nombre │ Rol │ Estado. | Alta, Editar, Reset Clave, Desactivar |
| V-10	 | Config Sistema (Administrador) | Grupo “Backup”: ruta carpeta (textbox \+ explorar). Grupo “Seguridad”: días vencimiento clave (numérico up-down). | Aplicar, Cerrar  |

**Reglas transversales**:

* Botones habilitados por rol (Enabled \= true/false); nunca se ocultan opciones.  
* Supervisor visible para Asistente: etiqueta “Supervisor: Dr. Juan Pérez”.  
* Navegación única: menú horizontal clásico; sin árboles ni sidebar

  5. ## **Diseño fisico de Base de Datos en SQL Server \+ Restricciones y constraints** {#diseño-fisico-de-base-de-datos-en-sql-server-+-restricciones-y-constraints}

| \----------------------------------------------------------- \-- 1\. CREACIÓN DE BASE DE DATOS \----------------------------------------------------------- CREATE DATABASE \[SALC\]; GO USE \[SALC\]; GO \----------------------------------------------------------- \-- 2\. TABLAS BÁSICAS \----------------------------------------------------------- \-- 2.1 ESTADO CREATE TABLE estado (     id\_estado   INT IDENTITY(1,1) CONSTRAINT PK\_estado PRIMARY KEY,     tipo\_estado VARCHAR(30) NOT NULL CONSTRAINT UQ\_tipo\_estado UNIQUE ); \-- 2.2 ROL CREATE TABLE rol (     id\_rol INT IDENTITY(1,1) CONSTRAINT PK\_rol PRIMARY KEY,     rol    VARCHAR(30) NOT NULL CONSTRAINT UQ\_rol UNIQUE ); \-- 2.3 ESTADO\_USUARIO CREATE TABLE estado\_usuario (     id\_estado INT IDENTITY(1,1) CONSTRAINT PK\_estado\_usuario PRIMARY KEY,     estado    VARCHAR(40) NOT NULL CONSTRAINT UQ\_estado\_usuario UNIQUE ); \-- 2.4 OBRA\_SOCIAL CREATE TABLE obra\_social (     id\_obra\_social INT IDENTITY(1,1) CONSTRAINT PK\_obra\_social PRIMARY KEY,     cuit           VARCHAR(20) NOT NULL CONSTRAINT UQ\_cuit UNIQUE,     nombre         VARCHAR(50) NOT NULL,     CONSTRAINT CHK\_obraSocial\_cuit CHECK (LEN(cuit) BETWEEN 10 AND 13\) ); \-- 2.5 TIPO\_ANALISIS CREATE TABLE tipo\_analisis (     id\_tipo\_analisis INT IDENTITY(1,1) CONSTRAINT PK\_tipo\_analisis PRIMARY KEY,     descripcion      VARCHAR(50) NOT NULL ); \-- 2.6 METRICA CREATE TABLE metrica (     id\_metrica   INT IDENTITY(1,1) CONSTRAINT PK\_metrica PRIMARY KEY,     nombre       VARCHAR(50) NOT NULL,     unidad       VARCHAR(10) NOT NULL,     valor\_maximo DECIMAL(10, 2\) NULL,     valor\_minimo DECIMAL(10, 2\) NULL,     CONSTRAINT CHK\_metrica\_valores CHECK (valor\_minimo \<= valor\_maximo) ); \----------------------------------------------------------- \-- 3\. TABLAS DE USUARIOS INTERNOS \----------------------------------------------------------- \-- 3.1 USUARIO CREATE TABLE usuario (     dni            INT CONSTRAINT PK\_usuario PRIMARY KEY,     nombre         VARCHAR(50) NOT NULL,     apellido       VARCHAR(50) NOT NULL,     id\_rol         INT NOT NULL,     telefono       VARCHAR(20) NULL,     email          VARCHAR(100) NULL,     password       VARCHAR(60) NOT NULL,     estado\_usuario INT NULL,     CONSTRAINT FK\_usuario\_rol FOREIGN KEY (id\_rol) REFERENCES rol(id\_rol),     CONSTRAINT FK\_usuario\_estadoUsuario FOREIGN KEY (estado\_usuario) REFERENCES estado\_usuario(id\_estado),     CONSTRAINT UQ\_usuario\_email UNIQUE (email),     CONSTRAINT CHK\_usuario\_dniPositivo CHECK (dni \> 0),     CONSTRAINT CHK\_usuario\_emailFormat CHECK (email LIKE '%@%.%' OR email IS NULL) ); \-- 3.2 DOCTOR (interno, con acceso al sistema) CREATE TABLE doctor (     dni           INT CONSTRAINT PK\_doctor PRIMARY KEY,     nro\_matricula INT NOT NULL CONSTRAINT UQ\_doctor\_matricula UNIQUE,     especialidad  VARCHAR(50) NOT NULL,     CONSTRAINT FK\_doctor\_usuario FOREIGN KEY (dni) REFERENCES usuario(dni),     CONSTRAINT CHK\_doctor\_matricula CHECK (nro\_matricula \> 0\) ); \-- 3.3 ASISTENTE CREATE TABLE asistente (     dni            INT CONSTRAINT PK\_asistente PRIMARY KEY,     nro\_legajo     INT NOT NULL CONSTRAINT UQ\_asistente\_legajo UNIQUE,     dni\_supervisor INT NOT NULL,      fecha\_ingreso  DATE NOT NULL DEFAULT GETDATE(),     CONSTRAINT FK\_asistente\_usuario FOREIGN KEY (dni) REFERENCES usuario(dni),     CONSTRAINT FK\_asistente\_supervisor FOREIGN KEY (dni\_supervisor) REFERENCES doctor(dni) ); \----------------------------------------------------------- \-- 4\. TABLAS DE PACIENTES Y DOCTORES EXTERNOS \----------------------------------------------------------- \-- 4.1 PACIENTE CREATE TABLE paciente (     dni              INT CONSTRAINT PK\_paciente PRIMARY KEY,     tipo\_doc         VARCHAR(10) NOT NULL,     nombre           VARCHAR(50) NOT NULL,     apellido         VARCHAR(50) NOT NULL,     sexo             VARCHAR(1) NOT NULL,     fecha\_nacimiento DATE NOT NULL,     direccion        VARCHAR(100) NULL,     localidad        VARCHAR(50) NULL,     provincia        VARCHAR(50) NULL,     telefono         VARCHAR(20) NULL,     email            VARCHAR(100) NULL,     id\_obra\_social   INT NULL,     CONSTRAINT FK\_paciente\_obraSocial FOREIGN KEY (id\_obra\_social) REFERENCES obra\_social(id\_obra\_social),     CONSTRAINT CHK\_paciente\_sexo CHECK (sexo IN ('M','F')),     CONSTRAINT CHK\_paciente\_tipoDoc CHECK (tipo\_doc IN ('DNI','LC','LE','PAS')),     CONSTRAINT CHK\_paciente\_fechaNac CHECK (fecha\_nacimiento \<= CAST(GETDATE() AS DATE)),     CONSTRAINT UQ\_paciente\_doc UNIQUE (tipo\_doc, dni) ); \-- 4.2 DOCTOR\_EXTERNO (nuevo) CREATE TABLE doctor\_externo (     dni               INT CONSTRAINT PK\_doctor\_externo PRIMARY KEY,     nombre            VARCHAR(50) NOT NULL,     apellido          VARCHAR(50) NOT NULL,     nro\_matricula     INT NOT NULL CONSTRAINT UQ\_doctorExt\_matricula UNIQUE,     especialidad      VARCHAR(50) NOT NULL,     telefono          VARCHAR(20) NULL,     email             VARCHAR(100) NULL,     CONSTRAINT CHK\_doctorExt\_matricula CHECK (nro\_matricula \> 0),     CONSTRAINT CHK\_doctorExt\_email CHECK (email LIKE '%@%.%' OR email IS NULL) ); \----------------------------------------------------------- \-- 5\. TABLAS DE ANÁLISIS Y RESULTADOS \----------------------------------------------------------- \-- 5.1 ANALISIS CREATE TABLE analisis (     id\_analisis             INT IDENTITY(1,1) CONSTRAINT PK\_analisis PRIMARY KEY,     id\_tipo\_analisis        INT NOT NULL,     id\_estado               INT NOT NULL,     dni\_paciente            INT NOT NULL,     observaciones           VARCHAR(MAX) NULL,     dni\_doctor\_externo      INT NOT NULL,      \-- Nuevo campo (reemplaza dni\_doctor\_solicitante)     dni\_recepcion           INT NOT NULL,      \-- asistente que recibe     dni\_firma               INT NULL,          \-- médico interno que valida     CONSTRAINT FK\_analisis\_tipoAnalisis FOREIGN KEY (id\_tipo\_analisis) REFERENCES tipo\_analisis(id\_tipo\_analisis),     CONSTRAINT FK\_analisis\_estado FOREIGN KEY (id\_estado) REFERENCES estado(id\_estado),     CONSTRAINT FK\_analisis\_paciente FOREIGN KEY (dni\_paciente) REFERENCES paciente(dni),     CONSTRAINT FK\_analisis\_recepcion FOREIGN KEY (dni\_recepcion) REFERENCES asistente(dni),     CONSTRAINT FK\_analisis\_firma FOREIGN KEY (dni\_firma) REFERENCES doctor(dni),     CONSTRAINT FK\_analisis\_doctorExterno FOREIGN KEY (dni\_doctor\_externo) REFERENCES doctor\_externo(dni) ); \-- 5.2 ANALISIS\_METRICA CREATE TABLE analisis\_metrica (     id\_analisis INT NOT NULL,     id\_metrica  INT NOT NULL,     valor       DECIMAL(10,2) NOT NULL,     CONSTRAINT PK\_analisis\_metrica PRIMARY KEY (id\_analisis, id\_metrica),     CONSTRAINT FK\_analisisMetricas\_analisis FOREIGN KEY (id\_analisis) REFERENCES analisis(id\_analisis),     CONSTRAINT FK\_analisisMetricas\_metrica FOREIGN KEY (id\_metrica) REFERENCES metrica(id\_metrica),     CONSTRAINT CHK\_analisisMetricas\_valor CHECK (valor \>= 0\) ); GO  | /\* \=========================================================    SALC – DATOS DE EJEMPLO – NUEVO MODELO v2    Incluye entidad: doctor\_externo    Orden: independientes → dependientes    \========================================================= \*/ USE \[SALC\]; GO \-- 1\.  ESTADOS \-------------------------------------------- INSERT INTO estado (tipo\_estado) VALUES ('Sin verificar'), ('Verificado'); GO \-- 2\.  ROLES \---------------------------------------------- INSERT INTO rol (rol) VALUES ('Administrador'), ('Clinico'), ('Asistente'); GO \-- 3\.  ESTADOS DE USUARIO \--------------------------------- INSERT INTO estado\_usuario (estado) VALUES ('Activo'), ('Inactivo'); GO \-- 4\.  OBRAS SOCIALES \------------------------------------- INSERT INTO obra\_social (cuit,nombre) VALUES ('30500000010','OSDE'), ('30650000020','Swiss Medical'), ('30990000040','Sin Obra Social'); GO \-- 5\.  TIPOS DE ANÁLISIS \---------------------------------- INSERT INTO tipo\_analisis (descripcion) VALUES ('Hemograma Completo'), ('Glucosa'), ('Perfil Lipídico'), ('Urianálisis'); GO \-- 6\.  MÉTRICAS \------------------------------------------- INSERT INTO metrica (nombre,unidad,valor\_minimo,valor\_maximo) VALUES ('Glucosa','mg/dL',70,110), ('Colesterol Total','mg/dL',0,200), ('HDL','mg/dL',40,60), ('LDL','mg/dL',0,100), ('Triglicéridos','mg/dL',0,150), ('Hemoglobina','g/dL',12,18), ('Leucocitos','10³/µL',4,11), ('Plaquetas','10³/µL',150,450); GO \-- 7\.  USUARIOS \-------------------- INSERT INTO usuario (dni,nombre,apellido,id\_rol,telefono,email,password,estado\_usuario) VALUES (30000001,'Juan','Pérez',1,'3415123456','jperez@lab.com','bA/00TI^4N',1), (30000002,'María','González',2,'3415213456','mgonzalez@lab.com',',Xa\\\\4457-UOk',1), (30000003,'Sofía','Gundisalvo',2,'31247245910','sofiagun@lab.com',')0RZ?8\~t7X7\_',1), (30000004,'Carlos','Ramírez',3,'4415323497','cramirez@lab.com','\\\\(46JH3Yy$P{',1), (30000005,'Erika','Miralles',3,'7822328490','erikamir@lab.com','JaKx3J?nv77H',1); GO \-- 8\.  DOCTORES INTERNOS \---------------------------------- INSERT INTO doctor (dni,nro\_matricula,especialidad) VALUES (30000002,25478,'Clínica Médica'),   \-- María González (30000003,36589, 'Clínica Médica');  \-- Sofía Gundisalvo GO \-- 9\.  ASISTENTES bajo supervisión del doctor 30000002 \---- INSERT INTO asistente (dni,nro\_legajo,dni\_supervisor,fecha\_ingreso) VALUES (30000004,44001,30000002,GETDATE()), (30000005,44002,30000003,GETDATE()); GO \-- 10\. PACIENTES \------------------------------------------- INSERT INTO paciente (dni,tipo\_doc,nombre,apellido,sexo,fecha\_nacimiento,direccion,localidad,provincia,telefono,email,id\_obra\_social) VALUES (40000005,'DNI','Ana','López','F','1985-06-15','San Martín 123','Rosario','Santa Fe','3415123456','analopez@gmail.com',1), (40000006,'DNI','Luis','Martínez','M','1990-11-22','Paraná 456','Rosario','Santa Fe','3415223456','luis.martinez@gmail.com',2), (40000007,'DNI','Sofía','Fernández','F','1978-03-08','Entre Ríos 789','Rosario','Santa Fe','3415323456','sofia.fernandez@gmail.com',3); GO \-- 11\. DOCTORES EXTERNOS (nuevo) \--------------------------- INSERT INTO doctor\_externo (dni,nombre,apellido,nro\_matricula,especialidad,telefono,email) VALUES (28000001,'Ricardo','Benítez',50214,'Clínico General','3415000001','rbenitez@hospitalcentral.com'), (28000002,'Elena','Suárez',51822,'Cardiología','3415000002','esuarez@hospitalcentral.com'), (28000003,'Hernán','Prieto',53045,'Pediatría','3415000003','hprieto@hospitalcentral.com'); GO \--  id\_doctor\_externo generados: 1, 2, 3 \-- 12\. ANÁLISIS (con recepción y firma) \-------------------- INSERT INTO analisis (id\_tipo\_analisis,id\_estado,dni\_paciente,observaciones,dni\_doctor\_externo,dni\_recepcion,dni\_firma) VALUES (1,2,40000005,'Control anual',28000001,30000004,30000002),   \-- Verificado → firma obligatoria (Dr. María González) (2,1,40000006,'Pre-operatorio',28000002,30000004,NULL),      \-- Sin verificar → sin firma (3,1,40000007,'Chequeo general',28000003,30000005,NULL);     \-- Sin verificar → sin firma GO \-- 13\. RESULTADOS \----------------------------------------- INSERT INTO analisis\_metrica (id\_analisis,id\_metrica,valor) VALUES (1,6,13.2), (1,7,5.8), (1,8,250), (2,1,95), (2,2,180), (2,3,45), (2,4,110), (2,5,125), (3,1,88); GO  |
| :---- | :---- |


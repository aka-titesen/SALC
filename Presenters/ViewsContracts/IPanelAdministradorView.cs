using System;

namespace SALC.Presenters.ViewsContracts
{
    /// <summary>
    /// Interfaz de contrato para la vista del panel del administrador.
    /// Define las propiedades y eventos para todas las funcionalidades administrativas:
    /// gestión de usuarios, catálogos, reportes y backups.
    /// </summary>
    public interface IPanelAdministradorView
    {
        #region Gestión de Usuarios

        /// <summary>
        /// Evento que se dispara cuando se solicita crear un nuevo usuario
        /// </summary>
        event EventHandler UsuariosNuevoClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita editar un usuario
        /// </summary>
        event EventHandler UsuariosEditarClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita eliminar un usuario
        /// </summary>
        event EventHandler UsuariosEliminarClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita ver detalles de un usuario
        /// </summary>
        event EventHandler UsuariosDetalleClick;

        /// <summary>
        /// Evento que se dispara cuando cambia el texto de búsqueda de usuarios
        /// </summary>
        event EventHandler<string> UsuariosBuscarTextoChanged;

        /// <summary>
        /// Evento que se dispara cuando cambia el filtro de estado de usuarios
        /// </summary>
        event EventHandler<string> UsuariosFiltroEstadoChanged;

        /// <summary>
        /// Carga la lista de usuarios en la vista
        /// </summary>
        void CargarUsuarios(System.Collections.IEnumerable usuarios);

        /// <summary>
        /// Obtiene el DNI del usuario seleccionado
        /// </summary>
        int? ObtenerUsuarioSeleccionadoDni();

        #endregion

        #region Catálogos - Obras Sociales

        /// <summary>
        /// Evento que se dispara cuando se solicita crear una nueva obra social
        /// </summary>
        event EventHandler ObrasSocialesNuevoClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita editar una obra social
        /// </summary>
        event EventHandler ObrasSocialesEditarClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita eliminar una obra social
        /// </summary>
        event EventHandler ObrasSocialesEliminarClick;

        /// <summary>
        /// Evento que se dispara cuando cambia el texto de búsqueda de obras sociales
        /// </summary>
        event EventHandler<string> ObrasSocialesBuscarTextoChanged;

        /// <summary>
        /// Evento que se dispara cuando cambia el filtro de estado de obras sociales
        /// </summary>
        event EventHandler<string> ObrasSocialesFiltroEstadoChanged;

        /// <summary>
        /// Carga la lista de obras sociales en la vista
        /// </summary>
        void CargarObrasSociales(System.Collections.IEnumerable obrasSociales);

        /// <summary>
        /// Obtiene el ID de la obra social seleccionada
        /// </summary>
        int? ObtenerObraSocialSeleccionadaId();

        #endregion

        #region Catálogos - Tipos de Análisis

        /// <summary>
        /// Evento que se dispara cuando se solicita crear un nuevo tipo de análisis
        /// </summary>
        event EventHandler TiposAnalisisNuevoClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita editar un tipo de análisis
        /// </summary>
        event EventHandler TiposAnalisisEditarClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita eliminar un tipo de análisis
        /// </summary>
        event EventHandler TiposAnalisisEliminarClick;

        /// <summary>
        /// Evento que se dispara cuando cambia el texto de búsqueda de tipos de análisis
        /// </summary>
        event EventHandler<string> TiposAnalisisBuscarTextoChanged;

        /// <summary>
        /// Evento que se dispara cuando cambia el filtro de estado de tipos de análisis
        /// </summary>
        event EventHandler<string> TiposAnalisisFiltroEstadoChanged;

        /// <summary>
        /// Carga la lista de tipos de análisis en la vista
        /// </summary>
        void CargarTiposAnalisis(System.Collections.IEnumerable tiposAnalisis);

        /// <summary>
        /// Obtiene el ID del tipo de análisis seleccionado
        /// </summary>
        int? ObtenerTipoAnalisisSeleccionadoId();

        #endregion

        #region Catálogos - Métricas

        /// <summary>
        /// Evento que se dispara cuando se solicita crear una nueva métrica
        /// </summary>
        event EventHandler MetricasNuevoClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita editar una métrica
        /// </summary>
        event EventHandler MetricasEditarClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita eliminar una métrica
        /// </summary>
        event EventHandler MetricasEliminarClick;

        /// <summary>
        /// Evento que se dispara cuando cambia el texto de búsqueda de métricas
        /// </summary>
        event EventHandler<string> MetricasBuscarTextoChanged;

        /// <summary>
        /// Evento que se dispara cuando cambia el filtro de estado de métricas
        /// </summary>
        event EventHandler<string> MetricasFiltroEstadoChanged;

        /// <summary>
        /// Carga la lista de métricas en la vista
        /// </summary>
        void CargarMetricas(System.Collections.IEnumerable metricas);

        /// <summary>
        /// Obtiene el ID de la métrica seleccionada
        /// </summary>
        int? ObtenerMetricaSeleccionadaId();

        #endregion

        #region Relaciones y Otras Funcionalidades

        /// <summary>
        /// Evento que se dispara cuando se solicita gestionar relaciones tipo análisis-métrica
        /// </summary>
        event EventHandler RelacionesTipoAnalisisMetricaGestionarClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita acceder a reportes
        /// </summary>
        event EventHandler ReportesClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita ejecutar un backup
        /// </summary>
        event EventHandler EjecutarBackupClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita probar la conexión a la base de datos
        /// </summary>
        event EventHandler ProbarConexionClick;

        #endregion

        /// <summary>
        /// Muestra un mensaje al usuario
        /// </summary>
        /// <param name="texto">Mensaje a mostrar</param>
        /// <param name="titulo">Título del mensaje</param>
        /// <param name="esError">Indica si es un mensaje de error</param>
        void MostrarMensaje(string texto, string titulo = "SALC", bool esError = false);
    }
}

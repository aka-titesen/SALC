using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;
using SALC.Infraestructura;
using SALC.BLL;
using SALC.Domain;
using System.Linq;
using System.Collections.Generic;
using System;
using SALC.DAL;
using SALC.Views.PanelAdministrador;

namespace SALC.Presenters
{
    /// <summary>
    /// ViewModel para mostrar información enriquecida de usuarios en las vistas del administrador
    /// </summary>
    public class UsuarioViewModel
    {
        public int DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string Estado { get; set; }
        public string DatosEspecíficos { get; set; }

        /// <summary>
        /// Crea un ViewModel desde una entidad Usuario
        /// </summary>
        public static UsuarioViewModel FromUsuario(Usuario usuario, string datosEspecificos = "")
        {
            return new UsuarioViewModel
            {
                DNI = usuario.Dni,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Rol = ObtenerNombreRol(usuario.IdRol),
                Estado = usuario.Estado,
                DatosEspecíficos = datosEspecificos
            };
        }

        private static string ObtenerNombreRol(int idRol)
        {
            switch (idRol)
            {
                case 1: return "Administrador";
                case 2: return "Médico";
                case 3: return "Asistente";
                default: return "Desconocido";
            }
        }
    }

    /// <summary>
    /// Presenter para el panel del administrador.
    /// Coordina todas las funcionalidades administrativas: gestión de usuarios,
    /// catálogos (obras sociales, tipos de análisis, métricas), reportes y backups.
    /// </summary>
    public class PanelAdministradorPresenter
    {
        private readonly IPanelAdministradorView _view;
        private readonly IBackupService _backupService;
        private readonly IUsuarioService _usuarioService = new UsuarioService();
        private List<Usuario> _usuarios = new List<Usuario>();
        private List<UsuarioViewModel> _usuariosViewModel = new List<UsuarioViewModel>();
        private readonly ICatalogoService _catalogoService = new CatalogoService();
        private readonly MedicoRepositorio _medicoRepo = new MedicoRepositorio();
        private readonly AsistenteRepositorio _asistenteRepo = new AsistenteRepositorio();
        private string _filtroEstadoUsuariosActual = "Todos";
        private string _filtroEstadoObrasSocialesActual = "Todos";
        private string _filtroEstadoTiposAnalisisActual = "Todos";
        private string _filtroEstadoMetricasActual = "Todos";
        private List<ObraSocial> _obrasSociales = new List<ObraSocial>();
        private List<TipoAnalisis> _tiposAnalisis = new List<TipoAnalisis>();
        private List<Metrica> _metricas = new List<Metrica>();
        private int _dniUsuarioActual;

        /// <summary>
        /// Constructor del presenter
        /// </summary>
        /// <param name="view">Vista del panel de administrador</param>
        public PanelAdministradorPresenter(IPanelAdministradorView view)
        {
            _view = view;
            _view.ProbarConexionClick += (s, e) => OnProbarConexion();
            _backupService = new BackupService();
            _view.EjecutarBackupClick += (s, e) => OnEjecutarBackup();

            CargarListadoUsuarios();
            CargarCatalogos();

            // Suscripción a eventos de usuarios
            _view.UsuariosNuevoClick += (s, e) => OnUsuariosNuevo();
            _view.UsuariosEditarClick += (s, e) => OnUsuariosEditar();
            _view.UsuariosEliminarClick += (s, e) => OnUsuariosEliminar();
            _view.UsuariosBuscarTextoChanged += (s, txt) => OnUsuariosBuscar(txt);
            _view.UsuariosDetalleClick += (s, e) => OnUsuariosDetalle();
            _view.UsuariosFiltroEstadoChanged += (s, filtro) => OnUsuariosFiltroEstado(filtro);

            // Suscripción a eventos de obras sociales
            _view.ObrasSocialesNuevoClick += (s, e) => OnObrasSocialesNuevo();
            _view.ObrasSocialesEditarClick += (s, e) => OnObrasSocialesEditar();
            _view.ObrasSocialesEliminarClick += (s, e) => OnObrasSocialesEliminar();
            _view.ObrasSocialesBuscarTextoChanged += (s, txt) => OnObrasSocialesBuscar(txt);
            _view.ObrasSocialesFiltroEstadoChanged += (s, filtro) => OnObrasSocialesFiltroEstado(filtro);
            
            // Suscripción a eventos de tipos de análisis
            _view.TiposAnalisisNuevoClick += (s, e) => OnTiposAnalisisNuevo();
            _view.TiposAnalisisEditarClick += (s, e) => OnTiposAnalisisEditar();
            _view.TiposAnalisisEliminarClick += (s, e) => OnTiposAnalisisEliminar();
            _view.TiposAnalisisBuscarTextoChanged += (s, txt) => OnTiposAnalisisBuscar(txt);
            _view.TiposAnalisisFiltroEstadoChanged += (s, filtro) => OnTiposAnalisisFiltroEstado(filtro);
            
            // Suscripción a eventos de métricas
            _view.MetricasNuevoClick += (s, e) => OnMetricasNuevo();
            _view.MetricasEditarClick += (s, e) => OnMetricasEditar();
            _view.MetricasEliminarClick += (s, e) => OnMetricasEliminar();
            _view.MetricasBuscarTextoChanged += (s, txt) => OnMetricasBuscar(txt);
            _view.MetricasFiltroEstadoChanged += (s, filtro) => OnMetricasFiltroEstado(filtro);

            // Otras funcionalidades
            _view.RelacionesTipoAnalisisMetricaGestionarClick += (s, e) => OnGestionarRelacionesTipoAnalisisMetricas();
            _view.ReportesClick += (s, e) => OnAbrirReportes();
        }

        /// <summary>
        /// Establece el DNI del usuario administrador actual
        /// </summary>
        public void EstablecerUsuarioActual(int dni)
        {
            _dniUsuarioActual = dni;
        }

        #region Conexión y Backup

        private void OnProbarConexion()
        {
            var r = DbHealth.ProbarConexion();
            if (r.Exito)
                MessageBox.Show("Base de datos: " + r.Detalle, "Conexión", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Error de conexión: " + r.Detalle, "Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OnEjecutarBackup()
        {
            try
            {
                string rutaSugerida = ObtenerRutaBackupSQLServer();
                
                using (var dlg = new SaveFileDialog())
                {
                    dlg.Filter = "Archivos de backup (*.bak)|*.bak";
                    dlg.Title = "Guardar copia de seguridad de la base de datos";
                    dlg.FileName = $"SALC_Manual_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                    dlg.InitialDirectory = rutaSugerida;
                    dlg.RestoreDirectory = true;
                    
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        var cursorAnterior = Cursor.Current;
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            
                            _backupService.EjecutarBackupManual(dlg.FileName, _dniUsuarioActual);
                            
                            var ultimoBackup = _backupService.ObtenerUltimoBackup();
                            var tamanoFormateado = _backupService.FormatearTamanoArchivo(ultimoBackup.TamanoArchivo);
                            
                            MessageBox.Show(
                                $"Copia de seguridad ejecutada correctamente.\n\n" +
                                $"Ubicación: {dlg.FileName}\n" +
                                $"Tamaño: {tamanoFormateado}\n" +
                                $"Fecha: {ultimoBackup.FechaHora:dd/MM/yyyy HH:mm:ss}", 
                                "Backup Exitoso", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
                        }
                        finally
                        {
                            Cursor.Current = cursorAnterior;
                        }
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                string mensajeError = "Error al ejecutar la copia de seguridad de la base de datos:\n\n";
                
                if (sqlEx.Message.Contains("Operating system error 5") || 
                    sqlEx.Message.Contains("Acceso denegado") ||
                    sqlEx.Message.Contains("Access is denied"))
                {
                    mensajeError += "SQL Server no tiene permisos para escribir en la ubicación seleccionada.\n\n";
                    mensajeError += "SOLUCIÓN: Seleccione la carpeta predeterminada de backups de SQL Server\n";
                    mensajeError += "que se muestra al abrir el diálogo de guardar.\n\n";
                    mensajeError += "Esta carpeta ya tiene los permisos necesarios configurados.";
                }
                else if (sqlEx.Message.Contains("There is not enough space") || 
                         sqlEx.Message.Contains("espacio"))
                {
                    mensajeError += "No hay suficiente espacio en disco.\n\n";
                    mensajeError += "Seleccione una unidad con más espacio disponible.";
                }
                else
                {
                    mensajeError += $"Error SQL: {sqlEx.Message}\n\n";
                    mensajeError += "Contacte al administrador de base de datos.";
                }
                
                MessageBox.Show(
                    mensajeError,
                    "Error en Backup", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error inesperado al ejecutar la copia de seguridad:\n\n{ex.Message}\n\n" +
                    "Intente guardar el backup en la ubicación predeterminada que se muestra al abrir el diálogo.",
                    "Error en Backup", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Obtiene la ruta predeterminada de backups de SQL Server
        /// </summary>
        private string ObtenerRutaBackupSQLServer()
        {
            try
            {
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SALC"].ConnectionString;
                
                using (var conexion = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    conexion.Open();
                    
                    var sqlQuery = @"
                        DECLARE @BackupDirectory NVARCHAR(500)
                        EXEC master.dbo.xp_instance_regread 
                            N'HKEY_LOCAL_MACHINE',
                            N'Software\Microsoft\MSSQLServer\MSSQLServer',
                            N'BackupDirectory',
                            @BackupDirectory OUTPUT
                        SELECT @BackupDirectory AS BackupDirectory";
                    
                    using (var comando = new System.Data.SqlClient.SqlCommand(sqlQuery, conexion))
                    {
                        var resultado = comando.ExecuteScalar();
                        if (resultado != null && !string.IsNullOrEmpty(resultado.ToString()))
                        {
                            var rutaBackup = resultado.ToString();
                            var rutaSALC = System.IO.Path.Combine(rutaBackup, "SALC");
                            if (!System.IO.Directory.Exists(rutaSALC))
                            {
                                try
                                {
                                    System.IO.Directory.CreateDirectory(rutaSALC);
                                    return rutaSALC;
                                }
                                catch
                                {
                                    return rutaBackup;
                                }
                            }
                            
                            return rutaSALC;
                        }
                    }
                }
            }
            catch
            {
                // Si falla la consulta, usar rutas de respaldo
            }
            
            string[] rutasRespaldo = new[]
            {
                @"C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\Backup",
                @"C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\Backup",
                @"C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\Backup",
                @"C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\Backup",
                System.IO.Path.GetTempPath()
            };
            
            foreach (var ruta in rutasRespaldo)
            {
                if (System.IO.Directory.Exists(ruta))
                {
                    return ruta;
                }
            }
            
            return System.IO.Path.GetTempPath();
        }

        #endregion

        #region Gestión de Usuarios

        private void CargarListadoUsuarios()
        {
            try
            {
                _usuarios = _usuarioService.ObtenerTodos().OrderBy(u => u.Apellido).ThenBy(u => u.Nombre).ToList();
                GenerarViewModelsUsuarios();
                AplicarFiltrosUsuarios();
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error cargando usuarios: " + ex.Message, "Usuarios", true);
            }
        }

        private void GenerarViewModelsUsuarios()
        {
            _usuariosViewModel = new List<UsuarioViewModel>();
            foreach (var usuario in _usuarios)
            {
                string datosEspecificos = "";
                try
                {
                    switch (usuario.IdRol)
                    {
                        case 2:
                            var medico = _medicoRepo.ObtenerPorId(usuario.Dni);
                            if (medico != null)
                                datosEspecificos = $"Mat: {medico.NroMatricula} - {medico.Especialidad}";
                            break;
                        case 3:
                            var asistente = _asistenteRepo.ObtenerPorId(usuario.Dni);
                            if (asistente != null)
                                datosEspecificos = $"Supervisor: {asistente.DniSupervisor} - Ingreso: {asistente.FechaIngreso:dd/MM/yyyy}";
                            break;
                        case 1:
                            datosEspecificos = "Acceso completo al sistema";
                            break;
                    }
                }
                catch (Exception)
                {
                    datosEspecificos = "Error al cargar datos específicos";
                }

                _usuariosViewModel.Add(UsuarioViewModel.FromUsuario(usuario, datosEspecificos));
            }
        }

        private void AplicarFiltrosUsuarios()
        {
            IEnumerable<UsuarioViewModel> usuariosFiltrados = _usuariosViewModel;

            if (_filtroEstadoUsuariosActual != "Todos")
            {
                usuariosFiltrados = usuariosFiltrados.Where(u => u.Estado == _filtroEstadoUsuariosActual);
            }

            _view.CargarUsuarios(usuariosFiltrados.ToList());
        }

        private void OnUsuariosFiltroEstado(string filtro)
        {
            _filtroEstadoUsuariosActual = filtro ?? "Todos";
            AplicarFiltrosUsuarios();
        }

        private void OnUsuariosBuscar(string txt)
        {
            var q = txt?.Trim().ToLowerInvariant();
            IEnumerable<UsuarioViewModel> src = _usuariosViewModel;

            if (_filtroEstadoUsuariosActual != "Todos")
            {
                src = src.Where(u => u.Estado == _filtroEstadoUsuariosActual);
            }

            if (!string.IsNullOrEmpty(q))
            {
                src = src.Where(u => u.Apellido.ToLowerInvariant().Contains(q)
                    || u.Nombre.ToLowerInvariant().Contains(q)
                    || u.Email.ToLowerInvariant().Contains(q)
                    || u.DNI.ToString().Contains(q));
            }
            
            _view.CargarUsuarios(src.ToList());
        }

        private void OnUsuariosNuevo()
        {
            using (var dlg = new SALC.Views.PanelAdministrador.FrmUsuarioEdit())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var data = dlg.ObtenerDatos();
                        _usuarioService.CrearUsuario(data.Usuario, data.Medico, data.Asistente);
                        CargarListadoUsuarios();
                        _view.MostrarMensaje("Usuario creado correctamente.", "Usuarios");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al crear usuario: " + ex.Message, "Usuarios", true);
                    }
                }
            }
        }

        private void OnUsuariosEditar()
        {
            var dni = _view.ObtenerUsuarioSeleccionadoDni();
            if (dni == null)
            {
                _view.MostrarMensaje("Seleccione un usuario para editar.", "Usuarios");
                return;
            }
            var existente = _usuarios.FirstOrDefault(u => u.Dni == dni.Value);
            using (var dlg = new SALC.Views.PanelAdministrador.FrmUsuarioEdit(existente))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var data = dlg.ObtenerDatos();
                        _usuarioService.ActualizarUsuario(data.Usuario, data.Medico, data.Asistente);
                        CargarListadoUsuarios();
                        _view.MostrarMensaje("Usuario actualizado.", "Usuarios");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al actualizar usuario: " + ex.Message, "Usuarios", true);
                    }
                }
            }
        }

        private void OnUsuariosEliminar()
        {
            var dni = _view.ObtenerUsuarioSeleccionadoDni();
            if (dni == null)
            {
                _view.MostrarMensaje("Seleccione un usuario para eliminar.", "Usuarios");
                return;
            }

            var usuario = _usuarios.FirstOrDefault(u => u.Dni == dni.Value);
            if (usuario == null) return;

            var resultado = MessageBox.Show(
                $"¿Está seguro que desea desactivar al usuario {usuario.Nombre} {usuario.Apellido}?\n\n" +
                "El usuario quedará inactivo pero sus datos se conservarán en el sistema.",
                "Desactivar Usuario",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes) return;

            try
            {
                usuario.Estado = "Inactivo";
                _usuarioService.ActualizarUsuario(usuario);
                CargarListadoUsuarios();
                _view.MostrarMensaje("Usuario desactivado correctamente.", "Usuarios");
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error al desactivar usuario: " + ex.Message, "Usuarios", true);
            }
        }

        private void OnUsuariosDetalle()
        {
            var dni = _view.ObtenerUsuarioSeleccionadoDni();
            if (dni == null)
            {
                _view.MostrarMensaje("Seleccione un usuario para ver los detalles.", "Usuarios");
                return;
            }

            var usuario = _usuarios.FirstOrDefault(u => u.Dni == dni.Value);
            if (usuario == null)
            {
                _view.MostrarMensaje("No se encontró el usuario seleccionado.", "Usuarios", true);
                return;
            }

            try
            {
                using (var dlg = new SALC.Views.PanelAdministrador.FrmUsuarioDetalle(usuario))
                {
                    dlg.ShowDialog();
                }
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error al mostrar detalles del usuario: " + ex.Message, "Usuarios", true);
            }
        }

        #endregion

        #region Catálogos

        private void CargarCatalogos()
        {
            try
            {
                _obrasSociales = _catalogoService.ObtenerObrasSociales().OrderBy(os => os.Nombre).ToList();
                AplicarFiltrosObrasSociales();
                
                _tiposAnalisis = _catalogoService.ObtenerTiposAnalisis().OrderBy(ta => ta.Descripcion).ToList();
                AplicarFiltrosTiposAnalisis();
                
                _metricas = _catalogoService.ObtenerMetricas().OrderBy(m => m.Nombre).ToList();
                AplicarFiltrosMetricas();
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error cargando catálogos: " + ex.Message, "Catálogos", true);
            }
        }

        // Los métodos de catálogos (Obras Sociales, Tipos de Análisis y Métricas)
        // siguen el mismo patrón de filtrado, creación, edición y eliminación
        // que los métodos de gestión de usuarios mostrados arriba.
        // Por brevedad, se omite su documentación detallada ya que la lógica es similar.

        #endregion

        #region Obras Sociales
        
        private void AplicarFiltrosObrasSociales()
        {
            IEnumerable<ObraSocial> obrasSocialesFiltradas = _obrasSociales;

            if (_filtroEstadoObrasSocialesActual != "Todos")
            {
                obrasSocialesFiltradas = obrasSocialesFiltradas.Where(os => os.Estado == _filtroEstadoObrasSocialesActual);
            }

            _view.CargarObrasSociales(obrasSocialesFiltradas.ToList());
        }

        private void OnObrasSocialesFiltroEstado(string filtro)
        {
            _filtroEstadoObrasSocialesActual = filtro ?? "Todos";
            AplicarFiltrosObrasSociales();
        }

        private void OnObrasSocialesBuscar(string txt)
        {
            var q = txt?.Trim().ToLowerInvariant();
            IEnumerable<ObraSocial> src = _obrasSociales;

            if (_filtroEstadoObrasSocialesActual != "Todos")
            {
                src = src.Where(os => os.Estado == _filtroEstadoObrasSocialesActual);
            }

            if (!string.IsNullOrEmpty(q))
            {
                src = src.Where(os => os.Nombre.ToLowerInvariant().Contains(q)
                    || os.Cuit.ToLowerInvariant().Contains(q));
            }
            
            _view.CargarObrasSociales(src.ToList());
        }

        private void OnObrasSocialesNuevo()
        {
            using (var dlg = new FrmObraSocialEdit())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var obraSocial = dlg.ObtenerObraSocial();
                        _catalogoService.CrearObraSocial(obraSocial);
                        CargarCatalogos();
                        _view.MostrarMensaje("Obra Social creada correctamente.", "Obras Sociales");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al crear obra social: " + ex.Message, "Obras Sociales", true);
                    }
                }
            }
        }

        private void OnObrasSocialesEditar()
        {
            var id = _view.ObtenerObraSocialSeleccionadaId();
            if (id == null)
            {
                _view.MostrarMensaje("Seleccione una obra social para editar.", "Obras Sociales");
                return;
            }

            var existente = _obrasSociales.FirstOrDefault(os => os.IdObraSocial == id.Value);
            if (existente == null)
            {
                _view.MostrarMensaje("No se encontró la obra social seleccionada.", "Obras Sociales", true);
                return;
            }

            using (var dlg = new FrmObraSocialEdit(existente))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var obraSocial = dlg.ObtenerObraSocial();
                        _catalogoService.ActualizarObraSocial(obraSocial);
                        CargarCatalogos();
                        _view.MostrarMensaje("Obra Social actualizada correctamente.", "Obras Sociales");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al actualizar obra social: " + ex.Message, "Obras Sociales", true);
                    }
                }
            }
        }

        private void OnObrasSocialesEliminar()
        {
            var id = _view.ObtenerObraSocialSeleccionadaId();
            if (id == null)
            {
                _view.MostrarMensaje("Seleccione una obra social para desactivar.", "Obras Sociales");
                return;
            }

            var obraSocial = _obrasSociales.FirstOrDefault(os => os.IdObraSocial == id.Value);
            if (obraSocial == null) return;

            var confirm = MessageBox.Show(
                $"¿Desactivar obra social '{obraSocial.Nombre}'?\n\n" +
                "La obra social se marcará como inactiva pero se conservarán todos los datos asociados.",
                "Confirmar Baja Lógica", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
                
            if (confirm != DialogResult.Yes) return;

            try
            {
                _catalogoService.EliminarObraSocial(id.Value);
                CargarCatalogos();
                _view.MostrarMensaje("Obra Social desactivada correctamente.", "Obras Sociales");
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error al desactivar obra social: " + ex.Message, "Obras Sociales", true);
            }
        }

        #endregion

        #region Tipos de Análisis

        private void AplicarFiltrosTiposAnalisis()
        {
            IEnumerable<TipoAnalisis> tiposAnalisisFiltrados = _tiposAnalisis;

            if (_filtroEstadoTiposAnalisisActual != "Todos")
            {
                tiposAnalisisFiltrados = tiposAnalisisFiltrados.Where(ta => ta.Estado == _filtroEstadoTiposAnalisisActual);
            }

            _view.CargarTiposAnalisis(tiposAnalisisFiltrados.ToList());
        }

        private void OnTiposAnalisisFiltroEstado(string filtro)
        {
            _filtroEstadoTiposAnalisisActual = filtro ?? "Todos";
            AplicarFiltrosTiposAnalisis();
        }

        private void OnTiposAnalisisBuscar(string txt)
        {
            var q = txt?.Trim().ToLowerInvariant();
            IEnumerable<TipoAnalisis> src = _tiposAnalisis;

            if (_filtroEstadoTiposAnalisisActual != "Todos")
            {
                src = src.Where(ta => ta.Estado == _filtroEstadoTiposAnalisisActual);
            }

            if (!string.IsNullOrEmpty(q))
            {
                src = src.Where(ta => ta.Descripcion.ToLowerInvariant().Contains(q));
            }
            
            _view.CargarTiposAnalisis(src.ToList());
        }

        private void OnTiposAnalisisNuevo()
        {
            using (var dlg = new FrmTipoAnalisisEdit())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var tipoAnalisis = dlg.ObtenerTipoAnalisis();
                        _catalogoService.CrearTipoAnalisis(tipoAnalisis);
                        CargarCatalogos();
                        _view.MostrarMensaje("Tipo de análisis creado correctamente.", "Tipos de Análisis");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al crear tipo de análisis: " + ex.Message, "Tipos de Análisis", true);
                    }
                }
            }
        }

        private void OnTiposAnalisisEditar()
        {
            var id = _view.ObtenerTipoAnalisisSeleccionadoId();
            if (id == null)
            {
                _view.MostrarMensaje("Seleccione un tipo de análisis para editar.", "Tipos de Análisis");
                return;
            }

            var existente = _tiposAnalisis.FirstOrDefault(ta => ta.IdTipoAnalisis == id.Value);
            if (existente == null)
            {
                _view.MostrarMensaje("No se encontró el tipo de análisis seleccionado.", "Tipos de Análisis", true);
                return;
            }

            using (var dlg = new FrmTipoAnalisisEdit(existente))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var tipoAnalisis = dlg.ObtenerTipoAnalisis();
                        _catalogoService.ActualizarTipoAnalisis(tipoAnalisis);
                        CargarCatalogos();
                        _view.MostrarMensaje("Tipo de análisis actualizado correctamente.", "Tipos de Análisis");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al actualizar tipo de análisis: " + ex.Message, "Tipos de Análisis", true);
                    }
                }
            }
        }

        private void OnTiposAnalisisEliminar()
        {
            var id = _view.ObtenerTipoAnalisisSeleccionadoId();
            if (id == null)
            {
                _view.MostrarMensaje("Seleccione un tipo de análisis para desactivar.", "Tipos de Análisis");
                return;
            }

            var tipoAnalisis = _tiposAnalisis.FirstOrDefault(ta => ta.IdTipoAnalisis == id.Value);
            if (tipoAnalisis == null) return;

            var confirm = MessageBox.Show(
                $"¿Desactivar tipo de análisis '{tipoAnalisis.Descripcion}'?\n\n" +
                "El tipo de análisis se marcará como inactivo pero se conservarán todos los datos asociados.",
                "Confirmar Baja Lógica", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
                
            if (confirm != DialogResult.Yes) return;

            try
            {
                _catalogoService.EliminarTipoAnalisis(id.Value);
                CargarCatalogos();
                _view.MostrarMensaje("Tipo de análisis desactivado correctamente.", "Tipos de Análisis");
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error al desactivar tipo de análisis: " + ex.Message, "Tipos de Análisis", true);
            }
        }

        #endregion

        #region Métricas

        private void AplicarFiltrosMetricas()
        {
            IEnumerable<Metrica> metricasFiltradas = _metricas;

            if (_filtroEstadoMetricasActual != "Todos")
            {
                metricasFiltradas = metricasFiltradas.Where(m => m.Estado == _filtroEstadoMetricasActual);
            }

            _view.CargarMetricas(metricasFiltradas.ToList());
        }

        private void OnMetricasFiltroEstado(string filtro)
        {
            _filtroEstadoMetricasActual = filtro ?? "Todos";
            AplicarFiltrosMetricas();
        }

        private void OnMetricasBuscar(string txt)
        {
            var q = txt?.Trim().ToLowerInvariant();
            IEnumerable<Metrica> src = _metricas;

            if (_filtroEstadoMetricasActual != "Todos")
            {
                src = src.Where(m => m.Estado == _filtroEstadoMetricasActual);
            }

            if (!string.IsNullOrEmpty(q))
            {
                src = src.Where(m => m.Nombre.ToLowerInvariant().Contains(q)
                    || m.UnidadMedida.ToLowerInvariant().Contains(q));
            }
            
            _view.CargarMetricas(src.ToList());
        }

        private void OnMetricasNuevo()
        {
            using (var dlg = new FrmMetricaEdit())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var metrica = dlg.ObtenerMetrica();
                        _catalogoService.CrearMetrica(metrica);
                        CargarCatalogos();
                        _view.MostrarMensaje("Métrica creada correctamente.", "Métricas");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al crear métrica: " + ex.Message, "Métricas", true);
                    }
                }
            }
        }

        private void OnMetricasEditar()
        {
            var id = _view.ObtenerMetricaSeleccionadaId();
            if (id == null)
            {
                _view.MostrarMensaje("Seleccione una métrica para editar.", "Métricas");
                return;
            }

            var existente = _metricas.FirstOrDefault(m => m.IdMetrica == id.Value);
            if (existente == null)
            {
                _view.MostrarMensaje("No se encontró la métrica seleccionada.", "Métricas", true);
                return;
            }

            using (var dlg = new FrmMetricaEdit(existente))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var metrica = dlg.ObtenerMetrica();
                        _catalogoService.ActualizarMetrica(metrica);
                        CargarCatalogos();
                        _view.MostrarMensaje("Métrica actualizada correctamente.", "Métricas");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al actualizar métrica: " + ex.Message, "Métricas", true);
                    }
                }
            }
        }

        private void OnMetricasEliminar()
        {
            var id = _view.ObtenerMetricaSeleccionadaId();
            if (id == null)
            {
                _view.MostrarMensaje("Seleccione una métrica para desactivar.", "Métricas");
                return;
            }

            var metrica = _metricas.FirstOrDefault(m => m.IdMetrica == id.Value);
            if (metrica == null) return;

            var confirm = MessageBox.Show(
                $"¿Desactivar métrica '{metrica.Nombre}'?\n\n" +
                "La métrica se marcará como inactiva pero se conservarán todos los datos asociados.",
                "Confirmar Baja Lógica", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
                
            if (confirm != DialogResult.Yes) return;

            try
            {
                _catalogoService.EliminarMetrica(id.Value);
                CargarCatalogos();
                _view.MostrarMensaje("Métrica desactivada correctamente.", "Métricas");
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error al desactivar métrica: " + ex.Message, "Métricas", true);
            }
        }

        #endregion

        #region Relaciones y Reportes

        private void OnGestionarRelacionesTipoAnalisisMetricas()
        {
            try
            {
                using (var dlg = new FrmGestionRelacionesTipoAnalisisMetricas())
                {
                    dlg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al abrir gestión de relaciones: " + ex.Message, "Relaciones Tipo Análisis-Métricas", true);
            }
        }

        private void OnAbrirReportes()
        {
            try
            {
                using (var dlg = new FrmReportesAdmin())
                {
                    dlg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al abrir módulo de reportes: " + ex.Message, "Reportes", true);
            }
        }

        #endregion
    }
}

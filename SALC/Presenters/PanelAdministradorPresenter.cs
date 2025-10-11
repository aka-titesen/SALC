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
    // ViewModel para mostrar información enriquecida de usuarios en la grilla
    public class UsuarioViewModel
    {
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string NombreRol { get; set; }
        public string Estado { get; set; }
        public string DatosEspecificos { get; set; }

        public static UsuarioViewModel FromUsuario(Usuario usuario, string datosEspecificos = "")
        {
            return new UsuarioViewModel
            {
                Dni = usuario.Dni,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                NombreRol = ObtenerNombreRol(usuario.IdRol),
                Estado = usuario.Estado,
                DatosEspecificos = datosEspecificos
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

    public class PanelAdministradorPresenter
    {
        private readonly IPanelAdministradorView _view;
        private readonly IBackupService _backupService;
        private readonly IPacienteService _pacienteService;
        private List<Paciente> _pacientes = new List<Paciente>();
        private readonly IUsuarioService _usuarioService = new UsuarioService();
        private List<Usuario> _usuarios = new List<Usuario>();
        private List<UsuarioViewModel> _usuariosViewModel = new List<UsuarioViewModel>();
        private readonly ICatalogoService _catalogoService = new CatalogoService();
        private readonly MedicoRepositorio _medicoRepo = new MedicoRepositorio();
        private readonly AsistenteRepositorio _asistenteRepo = new AsistenteRepositorio();
        private string _filtroEstadoActual = "Todos";

        public PanelAdministradorPresenter(IPanelAdministradorView view)
        {
            _view = view;
            _view.ProbarConexionClick += (s, e) => OnProbarConexion();
            _backupService = new BackupService();
            _view.EjecutarBackupClick += (s, e) => OnEjecutarBackup();
            _view.ProgramarBackupClick += (s, e) => OnProgramarBackup();

            // Pacientes
            _pacienteService = new PacienteService();
            _view.PacientesNuevoClick += (s, e) => OnPacientesNuevo();
            _view.PacientesEditarClick += (s, e) => OnPacientesEditar();
            _view.PacientesEliminarClick += (s, e) => OnPacientesEliminar();
            _view.PacientesBuscarTextoChanged += (s, txt) => OnPacientesBuscar(txt);

            CargarListadoPacientes();
            CargarListadoUsuarios();
            CargarCatalogos();

            // Usuarios
            _view.UsuariosNuevoClick += (s, e) => OnUsuariosNuevo();
            _view.UsuariosEditarClick += (s, e) => OnUsuariosEditar();
            _view.UsuariosEliminarClick += (s, e) => OnUsuariosEliminar();
            _view.UsuariosBuscarTextoChanged += (s, txt) => OnUsuariosBuscar(txt);
            _view.UsuariosDetalleClick += (s, e) => OnUsuariosDetalle();
            _view.UsuariosFiltroEstadoChanged += (s, filtro) => OnUsuariosFiltroEstado(filtro);

            // Catálogos
            _view.ObrasSocialesNuevoClick += (s, e) => OnObrasSocialesNuevo();
            _view.ObrasSocialesEditarClick += (s, e) => OnObrasSocialesEditar();
            _view.ObrasSocialesEliminarClick += (s, e) => OnObrasSocialesEliminar();
            _view.TiposAnalisisNuevoClick += (s, e) => OnTiposAnalisisNuevo();
            _view.TiposAnalisisEditarClick += (s, e) => OnTiposAnalisisEditar();
            _view.TiposAnalisisEliminarClick += (s, e) => OnTiposAnalisisEliminar();
            _view.MetricasNuevoClick += (s, e) => OnMetricasNuevo();
            _view.MetricasEditarClick += (s, e) => OnMetricasEditar();
            _view.MetricasEliminarClick += (s, e) => OnMetricasEliminar();
        }

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
            using (var sfd = new SaveFileDialog { Filter = "Archivo de backup (*.bak)|*.bak", FileName = $"SALC_{DateTime.Now:yyyyMMdd_HHmm}.bak" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _backupService.EjecutarBackup(sfd.FileName);
                        MessageBox.Show("Backup realizado correctamente.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Error al ejecutar backup: " + ex.Message, "Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnProgramarBackup()
        {
            MessageBox.Show("Programación de backups: pendiente de implementación.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

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
                        case 2: // Médico
                            var medico = _medicoRepo.ObtenerPorId(usuario.Dni);
                            if (medico != null)
                                datosEspecificos = $"Mat: {medico.NroMatricula} - {medico.Especialidad}";
                            break;
                        case 3: // Asistente
                            var asistente = _asistenteRepo.ObtenerPorId(usuario.Dni);
                            if (asistente != null)
                                datosEspecificos = $"Supervisor: {asistente.DniSupervisor} - Ingreso: {asistente.FechaIngreso:dd/MM/yyyy}";
                            break;
                        case 1: // Administrador
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

            // Filtro por estado
            if (_filtroEstadoActual != "Todos")
            {
                usuariosFiltrados = usuariosFiltrados.Where(u => u.Estado == _filtroEstadoActual);
            }

            _view.CargarUsuarios(usuariosFiltrados.ToList());
        }

        private void OnUsuariosFiltroEstado(string filtro)
        {
            _filtroEstadoActual = filtro ?? "Todos";
            AplicarFiltrosUsuarios();
        }

        private void CargarCatalogos()
        {
            try
            {
                _view.CargarObrasSociales(_catalogoService.ObtenerObrasSociales().ToList());
                _view.CargarTiposAnalisis(_catalogoService.ObtenerTiposAnalisis().ToList());
                _view.CargarMetricas(_catalogoService.ObtenerMetricas().ToList());
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error cargando catálogos: " + ex.Message, "Catálogos", true);
            }
        }

        // Catálogos — Obras Sociales
        private void OnObrasSocialesNuevo()
        {
            var nombre = PromptInput("Nombre de la Obra Social:");
            if (string.IsNullOrWhiteSpace(nombre)) return;
            var cuit = PromptInput("CUIT (10-13 caracteres):");
            if (string.IsNullOrWhiteSpace(cuit)) return;
            try { _catalogoService.CrearObraSocial(new ObraSocial { Nombre = nombre.Trim(), Cuit = cuit.Trim() }); CargarCatalogos(); _view.MostrarMensaje("Obra Social creada.", "Catálogos"); }
            catch (System.Exception ex) { _view.MostrarMensaje("Error: " + ex.Message, "Catálogos", true); }
        }

        private void OnObrasSocialesEditar()
        {
            var id = _view.ObtenerObraSocialSeleccionadaId(); if (id == null) { _view.MostrarMensaje("Seleccione una Obra Social."); return; }
            var nombre = PromptInput("Nuevo nombre:"); if (string.IsNullOrWhiteSpace(nombre)) return;
            var cuit = PromptInput("Nuevo CUIT:"); if (string.IsNullOrWhiteSpace(cuit)) return;
            try { _catalogoService.ActualizarObraSocial(new ObraSocial { IdObraSocial = id.Value, Nombre = nombre.Trim(), Cuit = cuit.Trim() }); CargarCatalogos(); _view.MostrarMensaje("Obra Social actualizada.", "Catálogos"); }
            catch (System.Exception ex) { _view.MostrarMensaje("Error: " + ex.Message, "Catálogos", true); }
        }

        private void OnObrasSocialesEliminar()
        {
            var id = _view.ObtenerObraSocialSeleccionadaId(); if (id == null) { _view.MostrarMensaje("Seleccione una Obra Social."); return; }
            if (MessageBox.Show("¿Eliminar Obra Social?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try { _catalogoService.EliminarObraSocial(id.Value); CargarCatalogos(); _view.MostrarMensaje("Obra Social eliminada.", "Catálogos"); }
            catch (System.Exception ex) { _view.MostrarMensaje("Error: " + ex.Message, "Catálogos", true); }
        }

        // Catálogos — Tipos de Análisis
        private void OnTiposAnalisisNuevo()
        {
            var desc = PromptInput("Descripción del tipo de análisis:"); if (string.IsNullOrWhiteSpace(desc)) return;
            try { _catalogoService.CrearTipoAnalisis(new TipoAnalisis { Descripcion = desc.Trim() }); CargarCatalogos(); _view.MostrarMensaje("Tipo de análisis creado.", "Catálogos"); }
            catch (System.Exception ex) { _view.MostrarMensaje("Error: " + ex.Message, "Catálogos", true); }
        }
        private void OnTiposAnalisisEditar()
        {
            var id = _view.ObtenerTipoAnalisisSeleccionadoId(); if (id == null) { _view.MostrarMensaje("Seleccione un tipo de análisis."); return; }
            var desc = PromptInput("Nueva descripción:"); if (string.IsNullOrWhiteSpace(desc)) return;
            try { _catalogoService.ActualizarTipoAnalisis(new TipoAnalisis { IdTipoAnalisis = id.Value, Descripcion = desc.Trim() }); CargarCatalogos(); _view.MostrarMensaje("Tipo de análisis actualizado.", "Catálogos"); }
            catch (System.Exception ex) { _view.MostrarMensaje("Error: " + ex.Message, "Catálogos", true); }
        }
        private void OnTiposAnalisisEliminar()
        {
            var id = _view.ObtenerTipoAnalisisSeleccionadoId(); if (id == null) { _view.MostrarMensaje("Seleccione un tipo de análisis."); return; }
            if (MessageBox.Show("¿Eliminar tipo de análisis?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try { _catalogoService.EliminarTipoAnalisis(id.Value); CargarCatalogos(); _view.MostrarMensaje("Tipo de análisis eliminado.", "Catálogos"); }
            catch (System.Exception ex) { _view.MostrarMensaje("Error: " + ex.Message, "Catálogos", true); }
        }

        // Catálogos — Métricas
        private void OnMetricasNuevo()
        {
            var nombre = PromptInput("Nombre de la métrica:"); if (string.IsNullOrWhiteSpace(nombre)) return;
            var unidad = PromptInput("Unidad de medida:"); if (string.IsNullOrWhiteSpace(unidad)) return;
            var minStr = PromptInput("Valor mínimo (opcional):"); decimal? min = string.IsNullOrWhiteSpace(minStr) ? (decimal?)null : decimal.Parse(minStr);
            var maxStr = PromptInput("Valor máximo (opcional):"); decimal? max = string.IsNullOrWhiteSpace(maxStr) ? (decimal?)null : decimal.Parse(maxStr);
            if (min.HasValue && max.HasValue && min > max) { _view.MostrarMensaje("Min no puede ser mayor que Max.", "Métricas", true); return; }
            try { _catalogoService.CrearMetrica(new Metrica { Nombre = nombre.Trim(), UnidadMedida = unidad.Trim(), ValorMinimo = min, ValorMaximo = max }); CargarCatalogos(); _view.MostrarMensaje("Métrica creada.", "Métricas"); }
            catch (System.Exception ex) { _view.MostrarMensaje("Error: " + ex.Message, "Métricas", true); }
        }
        private void OnMetricasEditar()
        {
            var id = _view.ObtenerMetricaSeleccionadaId(); if (id == null) { _view.MostrarMensaje("Seleccione una métrica."); return; }
            var nombre = PromptInput("Nuevo nombre:"); if (string.IsNullOrWhiteSpace(nombre)) return;
            var unidad = PromptInput("Nueva unidad:"); if (string.IsNullOrWhiteSpace(unidad)) return;
            var minStr = PromptInput("Nuevo mínimo (opcional):"); decimal? min = string.IsNullOrWhiteSpace(minStr) ? (decimal?)null : decimal.Parse(minStr);
            var maxStr = PromptInput("Nuevo máximo (opcional):"); decimal? max = string.IsNullOrWhiteSpace(maxStr) ? (decimal?)null : decimal.Parse(maxStr);
            if (min.HasValue && max.HasValue && min > max) { _view.MostrarMensaje("Min no puede ser mayor que Max.", "Métricas", true); return; }
            try { _catalogoService.ActualizarMetrica(new Metrica { IdMetrica = id.Value, Nombre = nombre.Trim(), UnidadMedida = unidad.Trim(), ValorMinimo = min, ValorMaximo = max }); CargarCatalogos(); _view.MostrarMensaje("Métrica actualizada.", "Métricas"); }
            catch (System.Exception ex) { _view.MostrarMensaje("Error: " + ex.Message, "Métricas", true); }
        }
        private void OnMetricasEliminar()
        {
            var id = _view.ObtenerMetricaSeleccionadaId(); if (id == null) { _view.MostrarMensaje("Seleccione una métrica."); return; }
            if (MessageBox.Show("¿Eliminar métrica?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try { _catalogoService.EliminarMetrica(id.Value); CargarCatalogos(); _view.MostrarMensaje("Métrica eliminada.", "Métricas"); }
            catch (System.Exception ex) { _view.MostrarMensaje("Error: " + ex.Message, "Métricas", true); }
        }

        private string PromptInput(string mensaje)
        {
            // Mini input prompt sin nuevas formas: usar InputBox improvisado
            string input = Microsoft.VisualBasic.Interaction.InputBox(mensaje, "SALC");
            return input;
        }

        private void OnUsuariosBuscar(string txt)
        {
            var q = txt?.Trim().ToLowerInvariant();
            IEnumerable<UsuarioViewModel> src = _usuariosViewModel;

            // Aplicar filtro de estado
            if (_filtroEstadoActual != "Todos")
            {
                src = src.Where(u => u.Estado == _filtroEstadoActual);
            }

            // Aplicar filtro de búsqueda
            if (!string.IsNullOrEmpty(q))
            {
                src = src.Where(u => u.Apellido.ToLowerInvariant().Contains(q)
                    || u.Nombre.ToLowerInvariant().Contains(q)
                    || u.Email.ToLowerInvariant().Contains(q)
                    || u.Dni.ToString().Contains(q));
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

            // Preguntar si quiere eliminar físicamente o desactivar
            var resultado = MessageBox.Show(
                $"¿Cómo desea proceder con el usuario {usuario.Nombre} {usuario.Apellido}?\n\n" +
                "• SÍ: Desactivar usuario (recomendado)\n" +
                "• NO: Eliminar físicamente\n" +
                "• CANCELAR: No hacer nada",
                "Eliminar Usuario",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Cancel) return;

            try
            {
                if (resultado == DialogResult.Yes)
                {
                    // Eliminación lógica (desactivar)
                    usuario.Estado = "Inactivo";
                    _usuarioService.ActualizarUsuario(usuario);
                    CargarListadoUsuarios();
                    _view.MostrarMensaje("Usuario desactivado correctamente.", "Usuarios");
                }
                else
                {
                    // Eliminación física
                    _usuarioService.EliminarUsuario(dni.Value);
                    CargarListadoUsuarios();
                    _view.MostrarMensaje("Usuario eliminado físicamente.", "Usuarios");
                }
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error al procesar usuario: " + ex.Message, "Usuarios", true);
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

        private void CargarListadoPacientes()
        {
            try
            {
                _pacientes = _pacienteService.ObtenerTodos().OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToList();
                _view.CargarPacientes(_pacientes);
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error cargando pacientes: " + ex.Message, "Pacientes", true);
            }
        }

        private void OnPacientesBuscar(string txt)
        {
            var q = txt?.Trim().ToLowerInvariant();
            IEnumerable<Paciente> src = _pacientes;
            if (!string.IsNullOrEmpty(q))
            {
                src = _pacientes.Where(p => p.Apellido.ToLowerInvariant().Contains(q)
                    || p.Nombre.ToLowerInvariant().Contains(q)
                    || p.Dni.ToString().Contains(q));
            }
            _view.CargarPacientes(src.ToList());
        }

        private void OnPacientesNuevo()
        {
            using (var dlg = new SALC.Views.PanelAdministrador.FrmPacienteEdit())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var p = dlg.ObtenerPaciente();
                        _pacienteService.CrearPaciente(p);
                        CargarListadoPacientes();
                        _view.MostrarMensaje("Paciente creado correctamente.", "Pacientes");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al crear paciente: " + ex.Message, "Pacientes", true);
                    }
                }
            }
        }

        private void OnPacientesEditar()
        {
            var dni = _view.ObtenerPacienteSeleccionadoDni();
            if (dni == null)
            {
                _view.MostrarMensaje("Seleccione un paciente para editar.", "Pacientes");
                return;
            }
            var existente = _pacientes.FirstOrDefault(p => p.Dni == dni.Value);
            if (existente == null) return;
            using (var dlg = new SALC.Views.PanelAdministrador.FrmPacienteEdit(existente))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var p = dlg.ObtenerPaciente();
                        _pacienteService.ActualizarPaciente(p);
                        CargarListadoPacientes();
                        _view.MostrarMensaje("Paciente actualizado.", "Pacientes");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al actualizar paciente: " + ex.Message, "Pacientes", true);
                    }
                }
            }
        }

        private void OnPacientesEliminar()
        {
            var dni = _view.ObtenerPacienteSeleccionadoDni();
            if (dni == null)
            {
                _view.MostrarMensaje("Seleccione un paciente para eliminar.", "Pacientes");
                return;
            }
            var confirm = MessageBox.Show($"¿Eliminar paciente DNI {dni}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
            try
            {
                _pacienteService.EliminarPaciente(dni.Value);
                CargarListadoPacientes();
                _view.MostrarMensaje("Paciente eliminado.", "Pacientes");
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error al eliminar paciente: " + ex.Message, "Pacientes", true);
            }
        }
    }
}

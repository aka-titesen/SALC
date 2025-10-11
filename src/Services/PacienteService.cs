using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Models;
using SALC.DataAccess.Repositories;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gesti�n de pacientes seg�n ERS v2.7
    /// Implementa RF-03: ABM de Pacientes
    /// </summary>
    public class PacienteService
    {
        private readonly PacienteRepository _pacienteRepository;
        private readonly ObraSocialRepository _obraSocialRepository;

        public PacienteService()
        {
            _pacienteRepository = new PacienteRepository();
            _obraSocialRepository = new ObraSocialRepository();
        }

        #region Operaciones de Consulta

        /// <summary>
        /// Obtiene todos los pacientes con filtros opcionales
        /// </summary>
        public List<Paciente> ObtenerPacientes(string filtroNombre = "", int? filtroObraSocial = null)
        {
            try
            {
                return _pacienteRepository.ObtenerTodos(filtroNombre, filtroObraSocial);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener pacientes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un paciente espec�fico por DNI
        /// </summary>
        public Paciente ObtenerPaciente(int dni)
        {
            try
            {
                return _pacienteRepository.ObtenerPorDni(dni);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener paciente con DNI {dni}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las obras sociales disponibles
        /// </summary>
        public List<ObraSocial> ObtenerObrasSociales()
        {
            try
            {
                return _obraSocialRepository.ObtenerTodas();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener obras sociales: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Creaci�n

        /// <summary>
        /// Crea un nuevo paciente
        /// </summary>
        public bool CrearPaciente(Paciente paciente)
        {
            try
            {
                // Validaciones de negocio
                ValidarPacienteParaCreacion(paciente);

                // Crear en base de datos
                return _pacienteRepository.Crear(paciente);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al crear paciente: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Actualizaci�n

        /// <summary>
        /// Actualiza un paciente existente
        /// </summary>
        public bool ActualizarPaciente(Paciente paciente)
        {
            try
            {
                // Validaciones de negocio
                ValidarPacienteParaActualizacion(paciente);

                // Actualizar en base de datos
                return _pacienteRepository.Actualizar(paciente);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar paciente: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Eliminaci�n

        /// <summary>
        /// Elimina un paciente (solo si no tiene an�lisis asociados)
        /// </summary>
        public bool EliminarPaciente(int dni)
        {
            try
            {
                // Verificar que el paciente existe
                var paciente = _pacienteRepository.ObtenerPorDni(dni);
                if (paciente == null)
                    throw new ArgumentException("Paciente no encontrado");

                // Verificar que no tenga an�lisis asociados
                if (_pacienteRepository.TieneAnalisisAsociados(dni))
                    throw new InvalidOperationException("No se puede eliminar un paciente que tiene an�lisis asociados");

                return _pacienteRepository.Eliminar(dni);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar paciente: {ex.Message}", ex);
            }
        }

        #endregion

        #region M�todos de Validaci�n

        /// <summary>
        /// Valida los datos de un paciente para creaci�n
        /// </summary>
        private void ValidarPacienteParaCreacion(Paciente paciente)
        {
            if (paciente == null)
                throw new ArgumentNullException(nameof(paciente));

            if (!paciente.EsValido())
                throw new ArgumentException("Los datos del paciente son inv�lidos");

            // Verificar que no existe otro paciente con el mismo DNI
            if (_pacienteRepository.ExistePorDni(paciente.Dni))
                throw new ArgumentException($"Ya existe un paciente con DNI {paciente.Dni}");

            // Verificar que no existe otro paciente con el mismo email (si se proporciona)
            if (!string.IsNullOrWhiteSpace(paciente.Email) && 
                _pacienteRepository.ExisteEmailEnOtroPaciente(paciente.Email, 0))
                throw new ArgumentException($"Ya existe un paciente con email {paciente.Email}");

            // Validar obra social si se especifica
            if (paciente.IdObraSocial.HasValue)
            {
                var obraSocial = _obraSocialRepository.ObtenerPorId(paciente.IdObraSocial.Value);
                if (obraSocial == null)
                    throw new ArgumentException("La obra social especificada no existe");
            }

            // Validar edad m�nima (debe ser mayor a 0 a�os)
            if (paciente.Edad < 0)
                throw new ArgumentException("La fecha de nacimiento no puede ser futura");

            // Validar edad m�xima razonable (150 a�os)
            if (paciente.Edad > 150)
                throw new ArgumentException("La fecha de nacimiento no es v�lida (edad mayor a 150 a�os)");
        }

        /// <summary>
        /// Valida los datos de un paciente para actualizaci�n
        /// </summary>
        private void ValidarPacienteParaActualizacion(Paciente paciente)
        {
            if (paciente == null)
                throw new ArgumentNullException(nameof(paciente));

            if (!paciente.EsValido())
                throw new ArgumentException("Los datos del paciente son inv�lidos");

            // Verificar que el paciente existe
            if (!_pacienteRepository.ExistePorDni(paciente.Dni))
                throw new ArgumentException($"No existe un paciente con DNI {paciente.Dni}");

            // Verificar que no existe otro paciente con el mismo email
            if (!string.IsNullOrWhiteSpace(paciente.Email) && 
                _pacienteRepository.ExisteEmailEnOtroPaciente(paciente.Email, paciente.Dni))
                throw new ArgumentException($"Ya existe otro paciente con email {paciente.Email}");

            // Validar obra social si se especifica
            if (paciente.IdObraSocial.HasValue)
            {
                var obraSocial = _obraSocialRepository.ObtenerPorId(paciente.IdObraSocial.Value);
                if (obraSocial == null)
                    throw new ArgumentException("La obra social especificada no existe");
            }

            // Validaciones de edad (similar a creaci�n)
            if (paciente.Edad < 0)
                throw new ArgumentException("La fecha de nacimiento no puede ser futura");

            if (paciente.Edad > 150)
                throw new ArgumentException("La fecha de nacimiento no es v�lida (edad mayor a 150 a�os)");
        }

        #endregion

        #region M�todos de B�squeda y Filtrado

        /// <summary>
        /// Busca pacientes por criterios m�ltiples
        /// </summary>
        public List<Paciente> BuscarPacientes(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
                return ObtenerPacientes();

            return ObtenerPacientes(criterio);
        }

        /// <summary>
        /// Obtiene pacientes por rango de edad
        /// </summary>
        public List<Paciente> ObtenerPacientesPorEdad(int edadMinima, int edadMaxima)
        {
            var todosPacientes = ObtenerPacientes();
            return todosPacientes.Where(p => p.Edad >= edadMinima && p.Edad <= edadMaxima).ToList();
        }

        /// <summary>
        /// Obtiene pacientes por sexo
        /// </summary>
        public List<Paciente> ObtenerPacientesPorSexo(string sexo)
        {
            var todosPacientes = ObtenerPacientes();
            return todosPacientes.Where(p => string.Equals(p.Sexo, sexo, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        #endregion
    }
}
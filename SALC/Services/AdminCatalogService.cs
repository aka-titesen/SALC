// Services/AdminCatalogService.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using SALC.Models;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gestión de catálogos del sistema (solo Administrador)
    /// Implementa las operaciones CRUD para tipo_analisis, metrica, obra_social, estado, estado_usuario, rol
    /// </summary>
    public class AdminCatalogService
    {
        private readonly string _connectionString;

        public AdminCatalogService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada.");
        }

        #region Obras Sociales (RF-13)
        
        /// <summary>
        /// Obtiene todas las obras sociales
        /// </summary>
        public List<Models.ObraSocial> GetObrasSociales()
        {
            var obras = new List<Models.ObraSocial>();
            const string query = "SELECT id_obra_social, cuit, nombre FROM obra_social ORDER BY nombre";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        obras.Add(new Models.ObraSocial
                        {
                            IdObraSocial = reader.GetInt32(0),
                            Cuit = reader.GetString(1),
                            Nombre = reader.GetString(2)
                        });
                    }
                }
            }
            return obras;
        }

        /// <summary>
        /// Crea una nueva obra social. Valida CUIT único.
        /// </summary>
        public bool CreateObraSocial(Models.ObraSocial obra)
        {
            if (obra == null || string.IsNullOrWhiteSpace(obra.Cuit) || string.IsNullOrWhiteSpace(obra.Nombre))
                throw new ArgumentException("CUIT y Nombre son obligatorios");

            // Validar longitud CUIT (10-13 caracteres según constraint)
            if (obra.Cuit.Length < 10 || obra.Cuit.Length > 13)
                throw new ArgumentException("CUIT debe tener entre 10 y 13 caracteres");

            const string query = "INSERT INTO obra_social (cuit, nombre) VALUES (@Cuit, @Nombre)";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Cuit", obra.Cuit);
                        command.Parameters.AddWithValue("@Nombre", obra.Nombre);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation
            {
                throw new InvalidOperationException($"Ya existe una obra social con CUIT: {obra.Cuit}");
            }
        }

        /// <summary>
        /// Actualiza una obra social existente
        /// </summary>
        public bool UpdateObraSocial(Models.ObraSocial obra)
        {
            if (obra == null || obra.IdObraSocial <= 0)
                throw new ArgumentException("ID de obra social es requerido");

            const string query = "UPDATE obra_social SET cuit = @Cuit, nombre = @Nombre WHERE id_obra_social = @Id";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", obra.IdObraSocial);
                        command.Parameters.AddWithValue("@Cuit", obra.Cuit);
                        command.Parameters.AddWithValue("@Nombre", obra.Nombre);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                throw new InvalidOperationException($"Ya existe una obra social con CUIT: {obra.Cuit}");
            }
        }

        /// <summary>
        /// Elimina una obra social (solo si no tiene pacientes asociados)
        /// </summary>
        public bool DeleteObraSocial(int idObraSocial)
        {
            const string checkQuery = "SELECT COUNT(*) FROM paciente WHERE id_obra_social = @Id";
            const string deleteQuery = "DELETE FROM obra_social WHERE id_obra_social = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // Verificar si tiene pacientes asociados
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Id", idObraSocial);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count > 0)
                        throw new InvalidOperationException($"No se puede eliminar la obra social. Tiene {count} paciente(s) asociado(s)");
                }

                // Eliminar
                using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", idObraSocial);
                    return deleteCommand.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Tipos de Análisis (RF-11)

        /// <summary>
        /// Obtiene todos los tipos de análisis
        /// </summary>
        public List<Models.TipoAnalisis> GetTiposAnalisis()
        {
            var tipos = new List<Models.TipoAnalisis>();
            const string query = "SELECT id_tipo_analisis, descripcion FROM tipo_analisis ORDER BY descripcion";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tipos.Add(new Models.TipoAnalisis
                        {
                            IdTipo = reader.GetInt32(0),
                            Descripcion = reader.GetString(1)
                        });
                    }
                }
            }
            return tipos;
        }

        /// <summary>
        /// Crea un nuevo tipo de análisis
        /// </summary>
        public bool CreateTipoAnalisis(Models.TipoAnalisis tipo)
        {
            if (tipo == null || string.IsNullOrWhiteSpace(tipo.Descripcion))
                throw new ArgumentException("Descripción es obligatoria");

            const string query = "INSERT INTO tipo_analisis (descripcion) VALUES (@Descripcion)";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", tipo.Descripcion);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Actualiza un tipo de análisis existente
        /// </summary>
        public bool UpdateTipoAnalisis(Models.TipoAnalisis tipo)
        {
            if (tipo == null || tipo.IdTipo <= 0)
                throw new ArgumentException("ID de tipo análisis es requerido");

            const string query = "UPDATE tipo_analisis SET descripcion = @Descripcion WHERE id_tipo_analisis = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", tipo.IdTipo);
                    command.Parameters.AddWithValue("@Descripcion", tipo.Descripcion);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Baja lógica: elimina solo si no tiene análisis asociados
        /// </summary>
        public bool DeleteTipoAnalisis(int idTipo)
        {
            const string checkQuery = "SELECT COUNT(*) FROM analisis WHERE id_tipo_analisis = @Id";
            const string deleteQuery = "DELETE FROM tipo_analisis WHERE id_tipo_analisis = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // Verificar si tiene análisis asociados
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Id", idTipo);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count > 0)
                        throw new InvalidOperationException($"No se puede eliminar el tipo de análisis. Tiene {count} análisis asociado(s)");
                }

                // Eliminar
                using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", idTipo);
                    return deleteCommand.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Métricas (RF-12)

        /// <summary>
        /// Obtiene todas las métricas
        /// </summary>
        public List<Models.Metrica> GetMetricas()
        {
            var metricas = new List<Models.Metrica>();
            const string query = "SELECT id_metrica, nombre, unidad, valor_minimo, valor_maximo FROM metrica ORDER BY nombre";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        metricas.Add(new Models.Metrica
                        {
                            IdMet = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Unidad = reader.GetString(2),
                            ValorMinimo = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                            ValorMaximo = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4)
                        });
                    }
                }
            }
            return metricas;
        }

        /// <summary>
        /// Crea una nueva métrica. Valida valor_min <= valor_max
        /// </summary>
        public bool CreateMetrica(Models.Metrica metrica)
        {
            if (metrica == null || string.IsNullOrWhiteSpace(metrica.Nombre) || string.IsNullOrWhiteSpace(metrica.Unidad))
                throw new ArgumentException("Nombre y Unidad son obligatorios");

            if (metrica.ValorMinimo > metrica.ValorMaximo)
                throw new ArgumentException("El valor mínimo no puede ser mayor al valor máximo");

            const string query = "INSERT INTO metrica (nombre, unidad, valor_minimo, valor_maximo) VALUES (@Nombre, @Unidad, @ValorMin, @ValorMax)";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", metrica.Nombre);
                    command.Parameters.AddWithValue("@Unidad", metrica.Unidad);
                    command.Parameters.AddWithValue("@ValorMin", metrica.ValorMinimo);
                    command.Parameters.AddWithValue("@ValorMax", metrica.ValorMaximo);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Actualiza una métrica existente
        /// </summary>
        public bool UpdateMetrica(Models.Metrica metrica)
        {
            if (metrica == null || metrica.IdMet <= 0)
                throw new ArgumentException("ID de métrica es requerido");

            if (metrica.ValorMinimo > metrica.ValorMaximo)
                throw new ArgumentException("El valor mínimo no puede ser mayor al valor máximo");

            const string query = "UPDATE metrica SET nombre = @Nombre, unidad = @Unidad, valor_minimo = @ValorMin, valor_maximo = @ValorMax WHERE id_metrica = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", metrica.IdMet);
                    command.Parameters.AddWithValue("@Nombre", metrica.Nombre);
                    command.Parameters.AddWithValue("@Unidad", metrica.Unidad);
                    command.Parameters.AddWithValue("@ValorMin", metrica.ValorMinimo);
                    command.Parameters.AddWithValue("@ValorMax", metrica.ValorMaximo);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Elimina una métrica (solo si no tiene análisis asociados)
        /// </summary>
        public bool DeleteMetrica(int idMetrica)
        {
            const string checkQuery = "SELECT COUNT(*) FROM analisis_metrica WHERE id_metrica = @Id";
            const string deleteQuery = "DELETE FROM metrica WHERE id_metrica = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // Verificar si tiene análisis asociados
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Id", idMetrica);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count > 0)
                        throw new InvalidOperationException($"No se puede eliminar la métrica. Tiene {count} resultado(s) de análisis asociado(s)");
                }

                // Eliminar
                using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", idMetrica);
                    return deleteCommand.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Estados y Roles

        /// <summary>
        /// Obtiene todos los estados (para análisis)
        /// </summary>
        public List<Models.Estado> GetEstados()
        {
            var estados = new List<Models.Estado>();
            const string query = "SELECT id_estado, tipo_estado FROM estado ORDER BY tipo_estado";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        estados.Add(new Models.Estado
                        {
                            IdEstado = reader.GetInt32(0),
                            TipoEstado = reader.GetString(1)
                        });
                    }
                }
            }
            return estados;
        }

        /// <summary>
        /// Obtiene todos los estados de usuario
        /// </summary>
        public List<Models.EstadoUsuario> GetEstadosUsuario()
        {
            var estados = new List<Models.EstadoUsuario>();
            const string query = "SELECT id_estado, estado FROM estado_usuario ORDER BY estado";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        estados.Add(new Models.EstadoUsuario
                        {
                            IdEstado = reader.GetInt32(0),
                            Estado = reader.GetString(1)
                        });
                    }
                }
            }
            return estados;
        }

        /// <summary>
        /// Obtiene todos los roles
        /// </summary>
        public List<Models.Rol> GetRoles()
        {
            var roles = new List<Models.Rol>();
            const string query = "SELECT id_rol, rol FROM rol ORDER BY rol";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Models.Rol
                        {
                            IdRol = reader.GetInt32(0),
                            NombreRol = reader.GetString(1)
                        });
                    }
                }
            }
            return roles;
        }

        #endregion
    }
}
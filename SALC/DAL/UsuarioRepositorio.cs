using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos de usuarios.
    /// Gestiona las operaciones CRUD sobre la tabla de usuarios y sus tablas relacionadas (médicos, asistentes).
    /// </summary>
    public class UsuarioRepositorio : IRepositorioBase<Usuario>
    {
        /// <summary>
        /// Crea un nuevo usuario en la base de datos
        /// </summary>
        /// <param name="entidad">Usuario a crear</param>
        public void Crear(Usuario entidad)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("INSERT INTO usuarios (dni, nombre, apellido, email, password_hash, id_rol, estado) VALUES (@dni, @nombre, @apellido, @email, @pass, @rol, @estado)", cn))
            {
                cmd.Parameters.AddWithValue("@dni", entidad.Dni);
                cmd.Parameters.AddWithValue("@nombre", entidad.Nombre);
                cmd.Parameters.AddWithValue("@apellido", entidad.Apellido);
                cmd.Parameters.AddWithValue("@email", entidad.Email);
                cmd.Parameters.AddWithValue("@pass", entidad.PasswordHash);
                cmd.Parameters.AddWithValue("@rol", entidad.IdRol);
                cmd.Parameters.AddWithValue("@estado", entidad.Estado ?? "Activo");
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente
        /// </summary>
        /// <param name="entidad">Usuario con los datos actualizados</param>
        public void Actualizar(Usuario entidad)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE usuarios SET nombre=@nombre, apellido=@apellido, email=@email, password_hash=@pass, id_rol=@rol, estado=@estado WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", entidad.Dni);
                cmd.Parameters.AddWithValue("@nombre", entidad.Nombre);
                cmd.Parameters.AddWithValue("@apellido", entidad.Apellido);
                cmd.Parameters.AddWithValue("@email", entidad.Email);
                cmd.Parameters.AddWithValue("@pass", entidad.PasswordHash);
                cmd.Parameters.AddWithValue("@rol", entidad.IdRol);
                cmd.Parameters.AddWithValue("@estado", entidad.Estado ?? "Activo");
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina un usuario mediante baja lógica, cambiando su estado a Inactivo
        /// </summary>
        /// <param name="id">DNI del usuario a eliminar</param>
        public void Eliminar(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE usuarios SET estado='Inactivo' WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene un usuario por su DNI
        /// </summary>
        /// <param name="id">DNI del usuario</param>
        /// <returns>El usuario encontrado o null si no existe</returns>
        public Usuario ObtenerPorId(object id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT dni, nombre, apellido, email, password_hash, id_rol, estado FROM usuarios WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return MapUsuario(rd);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema
        /// </summary>
        /// <returns>Colección de todos los usuarios</returns>
        public IEnumerable<Usuario> ObtenerTodos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT dni, nombre, apellido, email, password_hash, id_rol, estado FROM usuarios", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return MapUsuario(rd);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios con estado Activo
        /// </summary>
        /// <returns>Colección de usuarios activos</returns>
        public IEnumerable<Usuario> ObtenerActivos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT dni, nombre, apellido, email, password_hash, id_rol, estado FROM usuarios WHERE estado = 'Activo'", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return MapUsuario(rd);
                }
            }
        }

        /// <summary>
        /// Obtiene un usuario activo por su email
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <returns>El usuario encontrado o null si no existe o está inactivo</returns>
        public Usuario ObtenerPorEmail(string email)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT dni, nombre, apellido, email, password_hash, id_rol, estado FROM usuarios WHERE email=@em AND estado='Activo'", cn))
            {
                cmd.Parameters.AddWithValue("@em", email);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        return MapUsuario(rd);
                }
            }
            return null;
        }

        /// <summary>
        /// Mapea un registro de la base de datos a un objeto Usuario
        /// </summary>
        /// <param name="rd">Registro leído de la base de datos</param>
        /// <returns>Instancia de Usuario con los datos del registro</returns>
        private Usuario MapUsuario(IDataRecord rd)
        {
            return new Usuario
            {
                Dni = rd.GetInt32(0),
                Nombre = rd.GetString(1),
                Apellido = rd.GetString(2),
                Email = rd.GetString(3),
                PasswordHash = rd.GetString(4),
                IdRol = rd.GetInt32(5),
                Estado = rd.GetString(6)
            };
        }

        /// <summary>
        /// Crea un usuario médico en una transacción, insertando en las tablas usuarios y medicos
        /// </summary>
        /// <param name="usuario">Datos del usuario</param>
        /// <param name="medico">Datos específicos del médico</param>
        public void CrearUsuarioMedico(Usuario usuario, Medico medico)
        {
            using (var cn = DbConexion.CrearConexion())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand("INSERT INTO usuarios (dni, nombre, apellido, email, password_hash, id_rol, estado) VALUES (@dni, @nombre, @apellido, @email, @pass, @rol, @estado)", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@dni", usuario.Dni);
                            cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                            cmd.Parameters.AddWithValue("@apellido", usuario.Apellido);
                            cmd.Parameters.AddWithValue("@email", usuario.Email);
                            cmd.Parameters.AddWithValue("@pass", usuario.PasswordHash);
                            cmd.Parameters.AddWithValue("@rol", usuario.IdRol);
                            cmd.Parameters.AddWithValue("@estado", usuario.Estado ?? "Activo");
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand("INSERT INTO medicos (dni, nro_matricula, especialidad) VALUES (@dni, @mat, @esp)", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@dni", medico.Dni);
                            cmd.Parameters.AddWithValue("@mat", medico.NroMatricula);
                            cmd.Parameters.AddWithValue("@esp", medico.Especialidad);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Crea un usuario asistente en una transacción, insertando en las tablas usuarios y asistentes
        /// </summary>
        /// <param name="usuario">Datos del usuario</param>
        /// <param name="asistente">Datos específicos del asistente</param>
        public void CrearUsuarioAsistente(Usuario usuario, Asistente asistente)
        {
            using (var cn = DbConexion.CrearConexion())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand("INSERT INTO usuarios (dni, nombre, apellido, email, password_hash, id_rol, estado) VALUES (@dni, @nombre, @apellido, @email, @pass, @rol, @estado)", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@dni", usuario.Dni);
                            cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                            cmd.Parameters.AddWithValue("@apellido", usuario.Apellido);
                            cmd.Parameters.AddWithValue("@email", usuario.Email);
                            cmd.Parameters.AddWithValue("@pass", usuario.PasswordHash);
                            cmd.Parameters.AddWithValue("@rol", usuario.IdRol);
                            cmd.Parameters.AddWithValue("@estado", usuario.Estado ?? "Activo");
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand("INSERT INTO asistentes (dni, dni_supervisor, fecha_ingreso) VALUES (@dni, @sup, @ing)", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@dni", asistente.Dni);
                            cmd.Parameters.AddWithValue("@sup", asistente.DniSupervisor);
                            cmd.Parameters.AddWithValue("@ing", asistente.FechaIngreso);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Elimina un usuario médico mediante baja lógica
        /// </summary>
        /// <param name="dni">DNI del médico a eliminar</param>
        public void EliminarUsuarioMedico(int dni)
        {
            Eliminar(dni);
        }

        /// <summary>
        /// Elimina un usuario asistente mediante baja lógica
        /// </summary>
        /// <param name="dni">DNI del asistente a eliminar</param>
        public void EliminarUsuarioAsistente(int dni)
        {
            Eliminar(dni);
        }

        /// <summary>
        /// Obtiene los médicos activos que han firmado al menos un análisis
        /// </summary>
        /// <returns>Colección de médicos con análisis firmados</returns>
        public IEnumerable<Usuario> ObtenerMedicosConAnalisisFirmados()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(
                @"SELECT DISTINCT u.dni, u.nombre, u.apellido, u.email, u.password_hash, u.id_rol, u.estado 
                  FROM usuarios u 
                  INNER JOIN analisis a ON u.dni = a.dni_firma 
                  WHERE u.id_rol = 2 AND u.estado = 'Activo' AND a.dni_firma IS NOT NULL
                  ORDER BY u.apellido, u.nombre", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return MapUsuario(rd);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los médicos activos del sistema
        /// </summary>
        /// <returns>Colección de médicos activos ordenados por apellido y nombre</returns>
        public IEnumerable<Usuario> ObtenerMedicosActivos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT dni, nombre, apellido, email, password_hash, id_rol, estado FROM usuarios WHERE id_rol = 2 AND estado = 'Activo' ORDER BY apellido, nombre", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return MapUsuario(rd);
                }
            }
        }
    }
}

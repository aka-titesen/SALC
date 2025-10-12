using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class UsuarioRepositorio : IRepositorioBase<Usuario>
    {
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

        public void Eliminar(object id)
        {
            // Baja lógica - cambiar estado a "Inactivo"
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE usuarios SET estado='Inactivo' WHERE dni=@dni", cn))
            {
                cmd.Parameters.AddWithValue("@dni", (int)id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

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

        // Método para obtener solo usuarios activos
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

        public Usuario ObtenerPorEmail(string email)
        {
            // Solo buscar entre usuarios activos para el login
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

        // Operaciones transaccionales combinando usuarios + medicos/asistentes
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

        // Métodos de eliminación que usan baja lógica
        public void EliminarUsuarioMedico(int dni)
        {
            // Usar baja lógica
            Eliminar(dni);
        }

        public void EliminarUsuarioAsistente(int dni)
        {
            // Usar baja lógica
            Eliminar(dni);
        }
    }
}

using System;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class UsuarioTransaccionesRepositorio
    {
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
                            cmd.Parameters.AddWithValue("@estado", usuario.Estado);
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
                            cmd.Parameters.AddWithValue("@estado", usuario.Estado);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand("INSERT INTO asistentes (dni, legajo) VALUES (@dni, @leg)", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@dni", asistente.Dni);
                            cmd.Parameters.AddWithValue("@leg", asistente.Legajo);
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

        public void EliminarUsuarioMedico(int dni)
        {
            using (var cn = DbConexion.CrearConexion())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand("DELETE FROM medicos WHERE dni=@dni", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@dni", dni);
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SqlCommand("DELETE FROM usuarios WHERE dni=@dni", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@dni", dni);
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

        public void EliminarUsuarioAsistente(int dni)
        {
            using (var cn = DbConexion.CrearConexion())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand("DELETE FROM asistentes WHERE dni=@dni", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@dni", dni);
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SqlCommand("DELETE FROM usuarios WHERE dni=@dni", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@dni", dni);
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
    }
}

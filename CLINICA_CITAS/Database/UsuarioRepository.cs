using BCrypt.Net;
using CLINICA_CITAS.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace CLINICA_CITAS.Database
{
    public class UsuarioRepository
    {
        /// <summary>
        /// Intenta autenticar a un usuario con su correo y contraseña.
        /// Retorna el objeto Usuario si es correcto, null si las credenciales no coinciden.
        /// </summary>
        public Usuario? ValidarCredenciales(string correo, string contrasena)
        {
        using (var connection = new SqlConnection(DbConfig.ConnectionString))
        {
        connection.Open();
                // Contraseña en texto plano
                string contrasenaPlano = "pas123";

                // Generar el hash con BCrypt
                string hash = BCrypt.Net.BCrypt.HashPassword(contrasenaPlano);
              Console.WriteLine(hash);

                string query = @"SELECT * FROM Usuario WHERE Correo = @Correo";

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Correo", correo);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string hashGuardado = reader["ContrasenaHash"].ToString() ?? string.Empty;

                    // Verificar la contraseña ingresada contra el hash
                    if (BCrypt.Net.BCrypt.Verify(contrasena, hashGuardado))
                    {
                        return MapearUsuario(reader);
}
                }
            }
        }
    }
    return null;
}

        //public Usuario? ValidarCredenciales(string correo, string contrasena)//creando lafuncion ValidarCredenciales
        //{
        //    using (var connection = new SqlConnection(DbConfig.ConnectionString))
        //    {
        //        connection.Open();
                
        //        // Nota: Seleccionamos los campos necesarios de forma explícita.
        //        // Usamos ContrasenaHash directamente para la comparación en texto plano, de acuerdo a los datos de prueba ('123').
        //        string query = @"SELECT ID_Usuario, Nombre, Apellidos, Correo, Rol, Estado, Logueado, ContrasenaHash, UltimoAcceso, FechaRegistro
        //                         FROM Usuario 
        //                         WHERE Correo = @Correo AND ContrasenaHash = @Contrasena";

        //        using (var command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Correo", correo);
        //            command.Parameters.AddWithValue("@Contrasena", contrasena);

        //            using (var reader = command.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    return MapearUsuario(reader);
        //                }
        //            }
        //        }

        //    }
        //    return null;
        //}

        /// <summary>
        /// Actualiza el estado de logueo y la fecha de último acceso del usuario en la base de datos.
        /// </summary>
        public void RegistrarInicioSesionExitoso(int idUsuario)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"UPDATE Usuario 
                                 SET UltimoAcceso = GETDATE(), Logueado = 1 
                                 WHERE ID_Usuario = @ID_Usuario";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Usuario", idUsuario);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Usuario> ListarTodos()
        {
            var lista = new List<Usuario>();
 using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Usuario ORDER BY Apellidos, Nombre";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearUsuario(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public List<Usuario> BuscarPorFiltro(string filtro)
        {
            var lista = new List<Usuario>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT * FROM Usuario 
                                 WHERE Nombre LIKE @Filtro 
                                    OR Apellidos LIKE @Filtro 
                                    OR Correo LIKE @Filtro 
                                 ORDER BY Apellidos, Nombre";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Filtro", $"%{filtro}%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearUsuario(reader));
                        }
                    }
                }
            }
            return lista;
        }
        public void Insertar(Usuario u)
        //public void Insertar(Usuario u, string contrasenaPlano)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                // Hashear la contraseña en texto plano antes de guardarla
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(u.ContrasenaHash);

                string query = @"INSERT INTO Usuario 
                         (Nombre, Apellidos, Correo,Telefono, Rol,Apodo, Estado, ContrasenaHash, FechaRegistro, Logueado) 
                         VALUES (@Nombre, @Apellidos, @Correo,@Telefono, @Rol, @Apodo, @Estado, @ContrasenaHash, GETDATE(), 0)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", u.Nombre);
                    command.Parameters.AddWithValue("@Apellidos", u.Apellidos);
                    command.Parameters.AddWithValue("@Correo", u.Correo);
                    command.Parameters.AddWithValue("@Telefono","945876254");
                    command.Parameters.AddWithValue("@Rol", u.Rol);
                    command.Parameters.AddWithValue("@Apodo", "SuperApodo");
                    command.Parameters.AddWithValue("@Estado", u.Estado);
                    command.Parameters.AddWithValue("@ContrasenaHash", hashedPassword);

                    command.ExecuteNonQuery();
                }
            }
        }


        //public void Insertar(Usuario u)
        //{
        //    using (var connection = new SqlConnection(DbConfig.ConnectionString))
        //    {
        //        connection.Open();


        //        string query = @"INSERT INTO Usuario (Nombre, Apellidos, Correo, Rol, Estado, ContrasenaHash, FechaRegistro, Logueado) 
        //                         VALUES (@Nombre, @Apellidos, @Correo, @Rol, @Estado, @ContrasenaHash, GETDATE(), 0)";
        //        using (var command = new SqlCommand(query, connection))
        //        {
        //            AddParameters(command, u);
        //            command.ExecuteNonQuery();

        //        }
        //    }
        //}
        public void Actualizar(Usuario u, string? nuevaContrasenaPlano = null)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();

                // Si se pasa una nueva contraseña en texto plano, la hasheamos
                string hashedPassword = string.IsNullOrEmpty(nuevaContrasenaPlano)
                    ? u.ContrasenaHash // Mantener el hash existente si no se cambió la contraseña
                    : BCrypt.Net.BCrypt.HashPassword(nuevaContrasenaPlano);

                string query = @"UPDATE Usuario 
                         SET Nombre = @Nombre, 
                             Apellidos = @Apellidos, 
                             Correo = @Correo, 
                             Rol = @Rol, 
                             Estado = @Estado, 
                             ContrasenaHash = @ContrasenaHash
                         WHERE ID_Usuario = @ID_Usuario";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Usuario", u.ID_Usuario);
                    command.Parameters.AddWithValue("@Nombre", u.Nombre);
                    command.Parameters.AddWithValue("@Apellidos", u.Apellidos);
                    command.Parameters.AddWithValue("@Correo", u.Correo);
                    command.Parameters.AddWithValue("@Rol", u.Rol);
                    command.Parameters.AddWithValue("@Estado", u.Estado);
                    command.Parameters.AddWithValue("@ContrasenaHash", hashedPassword);

                    command.ExecuteNonQuery();
                }
            }
        }


        //public void Actualizar(Usuario u)
        //{
        //    using (var connection = new SqlConnection(DbConfig.ConnectionString))
        //    {
        //        connection.Open();


        //        string query = @"UPDATE Usuario 
        //                         SET Nombre = @Nombre, 
        //                             Apellidos = @Apellidos, 
        //                             Correo = @Correo, 
        //                             Rol = @Rol, 
        //                             Estado = @Estado, 
        //                             ContrasenaHash = @ContrasenaHash
        //                         WHERE ID_Usuario = @ID_Usuario";
        //        using (var command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@ID_Usuario", u.ID_Usuario);
        //            AddParameters(command, u);
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        public void Eliminar(int id)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Usuario SET Estado = 'Inactivo' WHERE ID_Usuario = @ID_Usuario";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Usuario", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddParameters(SqlCommand command, Usuario u)
        {
            command.Parameters.AddWithValue("@Nombre", u.Nombre);
            command.Parameters.AddWithValue("@Apellidos", u.Apellidos);
            command.Parameters.AddWithValue("@Correo", u.Correo);
            command.Parameters.AddWithValue("@Rol", u.Rol);
            command.Parameters.AddWithValue("@Estado", u.Estado);
            command.Parameters.AddWithValue("@ContrasenaHash", u.ContrasenaHash);
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                ID_Usuario = Convert.ToInt32(reader["ID_Usuario"]),
                Nombre = reader["Nombre"].ToString() ?? string.Empty,
                Apellidos = reader["Apellidos"].ToString() ?? string.Empty,
                Correo = reader["Correo"].ToString() ?? string.Empty,
                Rol = reader["Rol"].ToString() ?? string.Empty,
                Estado = reader["Estado"].ToString() ?? string.Empty,
                Logueado = Convert.ToBoolean(reader["Logueado"]),
                ContrasenaHash = reader["ContrasenaHash"].ToString() ?? string.Empty,
                UltimoAcceso = reader["UltimoAcceso"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["UltimoAcceso"]),
                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
            };
        }
    }
}
using System;
using Microsoft.Data.SqlClient;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Database
{
    public class UsuarioRepository
    {
        /// <summary>
        /// Intenta autenticar a un usuario con su correo y contraseña.
        /// Retorna el objeto Usuario si es correcto, null si las credenciales no coinciden.
        /// </summary>
        public Usuario? ValidarCredenciales(string correo, string contrasena)//creando lafuncion ValidarCredenciales
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                
                // Nota: Seleccionamos los campos necesarios de forma explícita.
                // Usamos ContrasenaHash directamente para la comparación en texto plano, de acuerdo a los datos de prueba ('123').
                string query = @"SELECT ID_Usuario, Nombre, Apellidos, Correo, Rol, Estado, Logueado 
                                 FROM Usuario 
                                 WHERE Correo = @Correo AND ContrasenaHash = @Contrasena";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Correo", correo);
                    command.Parameters.AddWithValue("@Contrasena", contrasena);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                ID_Usuario = Convert.ToInt32(reader["ID_Usuario"]),
                                Nombre = reader["Nombre"].ToString() ?? string.Empty,
                                Apellidos = reader["Apellidos"].ToString() ?? string.Empty,
                                Correo = reader["Correo"].ToString() ?? string.Empty,
                                Rol = reader["Rol"].ToString() ?? string.Empty,
                                Estado = reader["Estado"].ToString() ?? string.Empty,
                                Logueado = Convert.ToBoolean(reader["Logueado"])
                            };
                        }
                    }
                }
            }
            return null;
        }

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
    }
}

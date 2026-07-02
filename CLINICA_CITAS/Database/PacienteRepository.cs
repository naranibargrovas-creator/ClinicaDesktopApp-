using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Database
{
    public class PacienteRepository
    {
        public List<Paciente> ListarTodos()
        {
            var lista = new List<Paciente>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Paciente ORDER BY Apellidos, Nombre";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearPaciente(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public List<Paciente> BuscarPorFiltro(string filtro)
        {
            var lista = new List<Paciente>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT * FROM Paciente 
                                 WHERE Nombre LIKE @Filtro 
                                    OR Apellidos LIKE @Filtro 
                                    OR NumeroDoc LIKE @Filtro 
                                 ORDER BY Apellidos, Nombre";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Filtro", $"%{filtro}%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearPaciente(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public void Insertar(Paciente p)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Paciente (Nombre, Apellidos, FechaNacimiento, Telefono, Correo, Sexo, Direccion, TipoDoc, NumeroDoc, FechaRegistro, UsuarioRegistrador, FechaModificacion, UsuarioModificador, Estado) 
                                 VALUES (@Nombre, @Apellidos, @FechaNacimiento, @Telefono, @Correo, @Sexo, @Direccion, @TipoDoc, @NumeroDoc, GETDATE(), @UsuarioRegistrador, GETDATE(), @UsuarioModificador, @Estado)";
                using (var command = new SqlCommand(query, connection))
                {
                    AddParameters(command, p);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Paciente p)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"UPDATE Paciente 
                                 SET Nombre = @Nombre, 
                                     Apellidos = @Apellidos, 
                                     FechaNacimiento = @FechaNacimiento, 
                                     Telefono = @Telefono, 
                                     Correo = @Correo, 
                                     Sexo = @Sexo, 
                                     Direccion = @Direccion, 
                                     TipoDoc = @TipoDoc, 
                                     NumeroDoc = @NumeroDoc, 
                                     FechaModificacion = GETDATE(), 
                                     UsuarioModificador = @UsuarioModificador, 
                                     Estado = @Estado 
                                 WHERE ID_Paciente = @ID_Paciente";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Paciente", p.ID_Paciente);
                    AddParameters(command, p);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Paciente SET Estado = 0 WHERE ID_Paciente = @ID_Paciente";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Paciente", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddParameters(SqlCommand command, Paciente p)
        {
            command.Parameters.AddWithValue("@Nombre", p.Nombre);
            command.Parameters.AddWithValue("@Apellidos", p.Apellidos);
            command.Parameters.AddWithValue("@FechaNacimiento", p.FechaNacimiento.HasValue ? (object)p.FechaNacimiento.Value : DBNull.Value);
            command.Parameters.AddWithValue("@Telefono", p.Telefono);
            command.Parameters.AddWithValue("@Correo", p.Correo);
            command.Parameters.AddWithValue("@Sexo", p.Sexo);
            command.Parameters.AddWithValue("@Direccion", p.Direccion);
            command.Parameters.AddWithValue("@TipoDoc", p.TipoDoc);
            command.Parameters.AddWithValue("@NumeroDoc", p.NumeroDoc);
            command.Parameters.AddWithValue("@UsuarioRegistrador", p.UsuarioRegistrador);
            command.Parameters.AddWithValue("@UsuarioModificador", p.UsuarioModificador);
            command.Parameters.AddWithValue("@Estado", p.Estado ? 1 : 0);
        }

        private Paciente MapearPaciente(SqlDataReader reader)
        {
            return new Paciente
            {
                ID_Paciente = Convert.ToInt32(reader["ID_Paciente"]),
                Nombre = reader["Nombre"].ToString() ?? string.Empty,
                Apellidos = reader["Apellidos"].ToString() ?? string.Empty,
                FechaNacimiento = reader["FechaNacimiento"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["FechaNacimiento"]),
                Telefono = reader["Telefono"].ToString() ?? string.Empty,
                Correo = reader["Correo"].ToString() ?? string.Empty,
                Sexo = reader["Sexo"].ToString() ?? string.Empty,
                Direccion = reader["Direccion"].ToString() ?? string.Empty,
                TipoDoc = reader["TipoDoc"].ToString() ?? string.Empty,
                NumeroDoc = reader["NumeroDoc"].ToString() ?? string.Empty,
                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                UsuarioRegistrador = reader["UsuarioRegistrador"].ToString() ?? string.Empty,
                FechaModificacion = Convert.ToDateTime(reader["FechaModificacion"]),
                UsuarioModificador = reader["UsuarioModificador"].ToString() ?? string.Empty,
                Estado = Convert.ToBoolean(reader["Estado"])
            };
        }
    }
}

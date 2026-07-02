using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Database
{
    public class MedicoRepository
    {
        public List<Medico> ListarTodos()
        {
            var lista = new List<Medico>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Medico ORDER BY Apellidos, Nombre";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearMedico(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public List<Medico> BuscarPorFiltro(string filtro)
        {
            var lista = new List<Medico>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT * FROM Medico 
                                 WHERE Nombre LIKE @Filtro 
                                    OR Apellidos LIKE @Filtro 
                                    OR Colegiatura LIKE @Filtro 
                                    OR NumeroDoc LIKE @Filtro 
                                 ORDER BY Apellidos, Nombre";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Filtro", $"%{filtro}%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearMedico(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public void Insertar(Medico m)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Medico (Nombre, Apellidos, Correo, Direccion, FechaNacimiento, Colegiatura, DisponibilidadHorario, Apodo, ContrasenaHash, TipoDoc, NumeroDoc) 
                                 VALUES (@Nombre, @Apellidos, @Correo, @Direccion, @FechaNacimiento, @Colegiatura, @DisponibilidadHorario, @Apodo, @ContrasenaHash, @TipoDoc, @NumeroDoc)";
                using (var command = new SqlCommand(query, connection))
                {
                    AddParameters(command, m);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Medico m)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"UPDATE Medico 
                                 SET Nombre = @Nombre, 
                                     Apellidos = @Apellidos, 
                                     Correo = @Correo, 
                                     Direccion = @Direccion, 
                                     FechaNacimiento = @FechaNacimiento, 
                                     Colegiatura = @Colegiatura, 
                                     DisponibilidadHorario = @DisponibilidadHorario, 
                                     Apodo = @Apodo, 
                                     ContrasenaHash = @ContrasenaHash, 
                                     TipoDoc = @TipoDoc, 
                                     NumeroDoc = @NumeroDoc 
                                 WHERE ID_Medico = @ID_Medico";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Medico", m.ID_Medico);
                    AddParameters(command, m);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM Medico WHERE ID_Medico = @ID_Medico";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Medico", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddParameters(SqlCommand command, Medico m)
        {
            command.Parameters.AddWithValue("@Nombre", m.Nombre);
            command.Parameters.AddWithValue("@Apellidos", m.Apellidos);
            command.Parameters.AddWithValue("@Correo", m.Correo);
            command.Parameters.AddWithValue("@Direccion", m.Direccion);
            command.Parameters.AddWithValue("@FechaNacimiento", m.FechaNacimiento.HasValue ? (object)m.FechaNacimiento.Value : DBNull.Value);
            command.Parameters.AddWithValue("@Colegiatura", m.Colegiatura);
            command.Parameters.AddWithValue("@DisponibilidadHorario", m.DisponibilidadHorario);
            command.Parameters.AddWithValue("@Apodo", m.Apodo);
            command.Parameters.AddWithValue("@ContrasenaHash", string.IsNullOrEmpty(m.ContrasenaHash) ? "123" : m.ContrasenaHash); // Contraseña por defecto si vacía
            command.Parameters.AddWithValue("@TipoDoc", m.TipoDoc);
            command.Parameters.AddWithValue("@NumeroDoc", m.NumeroDoc);
        }

        private Medico MapearMedico(SqlDataReader reader)
        {
            return new Medico
            {
                ID_Medico = Convert.ToInt32(reader["ID_Medico"]),
                Nombre = reader["Nombre"].ToString() ?? string.Empty,
                Apellidos = reader["Apellidos"].ToString() ?? string.Empty,
                Correo = reader["Correo"].ToString() ?? string.Empty,
                Direccion = reader["Direccion"].ToString() ?? string.Empty,
                FechaNacimiento = reader["FechaNacimiento"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["FechaNacimiento"]),
                Colegiatura = reader["Colegiatura"].ToString() ?? string.Empty,
                DisponibilidadHorario = reader["DisponibilidadHorario"].ToString() ?? string.Empty,
                Apodo = reader["Apodo"].ToString() ?? string.Empty,
                ContrasenaHash = reader["ContrasenaHash"].ToString() ?? string.Empty,
                TipoDoc = reader["TipoDoc"].ToString() ?? string.Empty,
                NumeroDoc = reader["NumeroDoc"].ToString() ?? string.Empty
            };
        }
    }
}

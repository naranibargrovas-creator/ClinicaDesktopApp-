using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Database
{
    public class HistorialClinicoRepository
    {
        public List<HistorialClinico> ListarTodos()
        {
            var lista = new List<HistorialClinico>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT hc.*, 
                                        p.Nombre + ' ' + p.Apellidos AS PacienteNombreCompleto,
                                        m.Nombre + ' ' + m.Apellidos AS MedicoNombreCompleto,
                                        c.Motivo AS CitaMotivo
                                 FROM HistorialClinico hc
                                 LEFT JOIN Paciente p ON hc.ID_Paciente = p.ID_Paciente
                                 LEFT JOIN Cita c ON hc.ID_Cita = c.ID_Cita
                                 LEFT JOIN Medico m ON c.ID_Medico = m.ID_Medico
                                 ORDER BY hc.FechaCreacionHistorial DESC";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearHistorialClinico(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public List<HistorialClinico> BuscarPorFiltro(string filtro)
        {
            var lista = new List<HistorialClinico>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT hc.*, 
                                        p.Nombre + ' ' + p.Apellidos AS PacienteNombreCompleto,
                                        m.Nombre + ' ' + m.Apellidos AS MedicoNombreCompleto,
                                        c.Motivo AS CitaMotivo
                                 FROM HistorialClinico hc
                                 LEFT JOIN Paciente p ON hc.ID_Paciente = p.ID_Paciente
                                 LEFT JOIN Cita c ON hc.ID_Cita = c.ID_Cita
                                 LEFT JOIN Medico m ON c.ID_Medico = m.ID_Medico
                                 WHERE p.Nombre LIKE @Filtro 
                                    OR p.Apellidos LIKE @Filtro 
                                    OR p.NumeroDoc LIKE @Filtro
                                    OR hc.Observaciones LIKE @Filtro
                                 ORDER BY hc.FechaCreacionHistorial DESC";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Filtro", $"%{filtro}%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearHistorialClinico(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public List<HistorialClinico> ListarPorPaciente(int idPaciente)
        {
            var lista = new List<HistorialClinico>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT hc.*, 
                                        p.Nombre + ' ' + p.Apellidos AS PacienteNombreCompleto,
                                        m.Nombre + ' ' + m.Apellidos AS MedicoNombreCompleto,
                                        c.Motivo AS CitaMotivo
                                 FROM HistorialClinico hc
                                 LEFT JOIN Paciente p ON hc.ID_Paciente = p.ID_Paciente
                                 LEFT JOIN Cita c ON hc.ID_Cita = c.ID_Cita
                                 LEFT JOIN Medico m ON c.ID_Medico = m.ID_Medico
                                 WHERE hc.ID_Paciente = @ID_Paciente
                                 ORDER BY hc.FechaCreacionHistorial DESC";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Paciente", idPaciente);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearHistorialClinico(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public HistorialClinico ObtenerPorId(int id)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT hc.*, 
                                        p.Nombre + ' ' + p.Apellidos AS PacienteNombreCompleto,
                                        m.Nombre + ' ' + m.Apellidos AS MedicoNombreCompleto,
                                        c.Motivo AS CitaMotivo
                                 FROM HistorialClinico hc
                                 LEFT JOIN Paciente p ON hc.ID_Paciente = p.ID_Paciente
                                 LEFT JOIN Cita c ON hc.ID_Cita = c.ID_Cita
                                 LEFT JOIN Medico m ON c.ID_Medico = m.ID_Medico
                                 WHERE hc.ID_HistorialClinico = @ID_HistorialClinico";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_HistorialClinico", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearHistorialClinico(reader);
                        }
                    }
                }
            }
            return null;
        }

        public void Insertar(HistorialClinico hc)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"INSERT INTO HistorialClinico (Observaciones, FechaCreacionHistorial, Estado, Peso, Altura, PresionArterial, TipoSangre, Antecedentes, UsuarioRegistrador, FechaModificacion, UsuarioModificador, ID_Cita, ID_Paciente) 
                                 VALUES (@Observaciones, GETDATE(), @Estado, @Peso, @Altura, @PresionArterial, @TipoSangre, @Antecedentes, @UsuarioRegistrador, GETDATE(), @UsuarioModificador, @ID_Cita, @ID_Paciente)";
                using (var command = new SqlCommand(query, connection))
                {
                    AddParameters(command, hc);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(HistorialClinico hc)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"UPDATE HistorialClinico 
                                 SET Observaciones = @Observaciones,
                                     Estado = @Estado,
                                     Peso = @Peso,
                                     Altura = @Altura,
                                     PresionArterial = @PresionArterial,
                                     TipoSangre = @TipoSangre,
                                     Antecedentes = @Antecedentes,
                                     FechaModificacion = GETDATE(),
                                     UsuarioModificador = @UsuarioModificador,
                                     ID_Cita = @ID_Cita,
                                     ID_Paciente = @ID_Paciente
                                 WHERE ID_HistorialClinico = @ID_HistorialClinico";
                using (var command = new SqlCommand(query, connection))
                {
                    AddParameters(command, hc);
                    command.Parameters.AddWithValue("@ID_HistorialClinico", hc.ID_HistorialClinico);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM HistorialClinico WHERE ID_HistorialClinico = @ID_HistorialClinico";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_HistorialClinico", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddParameters(SqlCommand command, HistorialClinico hc)
        {
            command.Parameters.AddWithValue("@Observaciones", hc.Observaciones);
            command.Parameters.AddWithValue("@Estado", hc.Estado);
            command.Parameters.AddWithValue("@Peso", hc.Peso.HasValue ? (object)hc.Peso.Value : DBNull.Value);
            command.Parameters.AddWithValue("@Altura", hc.Altura.HasValue ? (object)hc.Altura.Value : DBNull.Value);
            command.Parameters.AddWithValue("@PresionArterial", hc.PresionArterial);
            command.Parameters.AddWithValue("@TipoSangre", hc.TipoSangre);
            command.Parameters.AddWithValue("@Antecedentes", hc.Antecedentes);
            command.Parameters.AddWithValue("@UsuarioRegistrador", hc.UsuarioRegistrador);
            command.Parameters.AddWithValue("@UsuarioModificador", hc.UsuarioModificador);
            command.Parameters.AddWithValue("@ID_Cita", hc.ID_Cita);
            command.Parameters.AddWithValue("@ID_Paciente", hc.ID_Paciente);
        }

        private HistorialClinico MapearHistorialClinico(SqlDataReader reader)
        {
            return new HistorialClinico
            {
                ID_HistorialClinico = reader.GetInt32(reader.GetOrdinal("ID_HistorialClinico")),
                Observaciones = reader.GetString(reader.GetOrdinal("Observaciones")),
                FechaCreacionHistorial = reader.GetDateTime(reader.GetOrdinal("FechaCreacionHistorial")),
                Estado = reader.GetString(reader.GetOrdinal("Estado")),
                Peso = reader.IsDBNull(reader.GetOrdinal("Peso")) ? null : reader.GetDecimal(reader.GetOrdinal("Peso")),
                Altura = reader.IsDBNull(reader.GetOrdinal("Altura")) ? null : reader.GetDecimal(reader.GetOrdinal("Altura")),
                PresionArterial = reader.GetString(reader.GetOrdinal("PresionArterial")),
                TipoSangre = reader.GetString(reader.GetOrdinal("TipoSangre")),
                Antecedentes = reader.GetString(reader.GetOrdinal("Antecedentes")),
                UsuarioRegistrador = reader.GetString(reader.GetOrdinal("UsuarioRegistrador")),
                FechaModificacion = reader.GetDateTime(reader.GetOrdinal("FechaModificacion")),
                UsuarioModificador = reader.GetString(reader.GetOrdinal("UsuarioModificador")),
                ID_Cita = reader.GetInt32(reader.GetOrdinal("ID_Cita")),
                ID_Paciente = reader.GetInt32(reader.GetOrdinal("ID_Paciente")),
                PacienteNombreCompleto = reader.IsDBNull(reader.GetOrdinal("PacienteNombreCompleto")) ? null : reader.GetString(reader.GetOrdinal("PacienteNombreCompleto")),
                MedicoNombreCompleto = reader.IsDBNull(reader.GetOrdinal("MedicoNombreCompleto")) ? null : reader.GetString(reader.GetOrdinal("MedicoNombreCompleto")),
                CitaMotivo = reader.IsDBNull(reader.GetOrdinal("CitaMotivo")) ? null : reader.GetString(reader.GetOrdinal("CitaMotivo"))
            };
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Database
{
    public class CitaRepository
    {
        public List<Cita> ListarTodas()
        {
            var lista = new List<Cita>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT c.*, 
                                        p.Nombre AS PacienteNombre, p.Apellidos AS PacienteApellidos,
                                        m.Nombre AS MedicoNombre, m.Apellidos AS MedicoApellidos,
                                        (SELECT STRING_AGG(e.NombreEspecialidad, ', ') 
                                         FROM MedicoEspecialidad me 
                                         INNER JOIN Especialidad e ON me.ID_Especialidad = e.ID_Especialidad 
                                         WHERE me.ID_Medico = m.ID_Medico) AS MedicoEspecialidades,
                                        u.Nombre AS UsuarioNombre, u.Apellidos AS UsuarioApellidos,
                                        esp.NombreEspecialidad AS EspecialidadNombre
                                 FROM Cita c
                                 INNER JOIN Paciente p ON c.ID_Paciente = p.ID_Paciente
                                 INNER JOIN Medico m ON c.ID_Medico = m.ID_Medico
                                 INNER JOIN Usuario u ON c.Id_Usuario = u.ID_Usuario
                                 LEFT JOIN Especialidad esp ON c.ID_Especialidad = esp.ID_Especialidad
                                 ORDER BY c.HoraInicio DESC";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearCita(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public List<Cita> BuscarPorFiltro(string filtro)
        {
            var lista = new List<Cita>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT c.*, 
                                        p.Nombre AS PacienteNombre, p.Apellidos AS PacienteApellidos,
                                        m.Nombre AS MedicoNombre, m.Apellidos AS MedicoApellidos,
                                        (SELECT STRING_AGG(e.NombreEspecialidad, ', ') 
                                         FROM MedicoEspecialidad me 
                                         INNER JOIN Especialidad e ON me.ID_Especialidad = e.ID_Especialidad 
                                         WHERE me.ID_Medico = m.ID_Medico) AS MedicoEspecialidades,
                                        u.Nombre AS UsuarioNombre, u.Apellidos AS UsuarioApellidos,
                                        esp.NombreEspecialidad AS EspecialidadNombre
                                 FROM Cita c
                                 INNER JOIN Paciente p ON c.ID_Paciente = p.ID_Paciente
                                 INNER JOIN Medico m ON c.ID_Medico = m.ID_Medico
                                 INNER JOIN Usuario u ON c.Id_Usuario = u.ID_Usuario
                                 LEFT JOIN Especialidad esp ON c.ID_Especialidad = esp.ID_Especialidad
                                 WHERE p.Nombre LIKE @Filtro 
                                    OR p.Apellidos LIKE @Filtro 
                                    OR m.Nombre LIKE @Filtro 
                                    OR m.Apellidos LIKE @Filtro
                                    OR c.Estado LIKE @Filtro
                                    OR esp.NombreEspecialidad LIKE @Filtro
                                 ORDER BY c.HoraInicio DESC";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Filtro", $"%{filtro}%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearCita(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public void CrearCita(Cita c)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Cita (HoraInicio, HoraFin, Motivo, Estado, ID_Paciente, ID_Medico, Id_Usuario, ID_Especialidad) 
                                 VALUES (@HoraInicio, @HoraFin, @Motivo, @Estado, @ID_Paciente, @ID_Medico, @Id_Usuario, @ID_Especialidad)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@HoraInicio", c.HoraInicio);
                    command.Parameters.AddWithValue("@HoraFin", c.HoraFin.HasValue ? (object)c.HoraFin.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Motivo", c.Motivo);
                    command.Parameters.AddWithValue("@Estado", c.Estado);
                    command.Parameters.AddWithValue("@ID_Paciente", c.ID_Paciente);
                    command.Parameters.AddWithValue("@ID_Medico", c.ID_Medico);
                    command.Parameters.AddWithValue("@Id_Usuario", c.Id_Usuario);
                    command.Parameters.AddWithValue("@ID_Especialidad", c.ID_Especialidad.HasValue ? (object)c.ID_Especialidad.Value : DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarEstado(int idCita, string estado)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Cita SET Estado = @Estado WHERE ID_Cita = @ID_Cita";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Estado", estado);
                    command.Parameters.AddWithValue("@ID_Cita", idCita);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarEspecialidadesCitasExistentes()
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"UPDATE Cita 
                                 SET ID_Especialidad = (
                                     SELECT TOP 1 me.ID_Especialidad 
                                     FROM MedicoEspecialidad me 
                                     WHERE me.ID_Medico = Cita.ID_Medico
                                 )
                                 WHERE ID_Especialidad IS NULL";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public Dictionary<string, int> ObtenerEstadisticasDashboard()
        {
            var stats = new Dictionary<string, int>
            {
                { "Pacientes", 0 },
                { "Medicos", 0 },
                { "CitasPendientes", 0 },
                { "CitasHoy", 0 }
            };

            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                
                // Total Pacientes Activos
                string qPacientes = "SELECT COUNT(*) FROM Paciente WHERE Estado = 1";
                using (var cmd = new SqlCommand(qPacientes, connection))
                {
                    stats["Pacientes"] = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Total Médicos
                string qMedicos = "SELECT COUNT(*) FROM Medico";
                using (var cmd = new SqlCommand(qMedicos, connection))
                {
                    stats["Medicos"] = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Citas Pendientes
                string qCitasPend = "SELECT COUNT(*) FROM Cita WHERE Estado = 'Pendiente' OR Estado = 'Confirmada'";
                using (var cmd = new SqlCommand(qCitasPend, connection))
                {
                    stats["CitasPendientes"] = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Citas de Hoy
                string qCitasHoy = "SELECT COUNT(*) FROM Cita WHERE CAST(HoraInicio AS DATE) = CAST(GETDATE() AS DATE)";
                using (var cmd = new SqlCommand(qCitasHoy, connection))
                {
                    stats["CitasHoy"] = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return stats;
        }

        private Cita MapearCita(SqlDataReader reader)
        {
            string especialidades = reader["MedicoEspecialidades"] == DBNull.Value ? string.Empty : reader["MedicoEspecialidades"].ToString() ?? string.Empty;
            return new Cita
            {
                ID_Cita = Convert.ToInt32(reader["ID_Cita"]),
                HoraInicio = Convert.ToDateTime(reader["HoraInicio"]),
                HoraFin = reader["HoraFin"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["HoraFin"]),
                Motivo = reader["Motivo"].ToString() ?? string.Empty,
                Estado = reader["Estado"].ToString() ?? string.Empty,
                ID_Paciente = Convert.ToInt32(reader["ID_Paciente"]),
                ID_Medico = Convert.ToInt32(reader["ID_Medico"]),
                Id_Usuario = Convert.ToInt32(reader["Id_Usuario"]),
                ID_Especialidad = reader["ID_Especialidad"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ID_Especialidad"]),
                PacienteNombreCompleto = $"{reader["PacienteNombre"]} {reader["PacienteApellidos"]}",
                MedicoNombreCompleto = string.IsNullOrEmpty(especialidades)
                    ? $"{reader["MedicoNombre"]} {reader["MedicoApellidos"]}"
                    : $"{reader["MedicoNombre"]} {reader["MedicoApellidos"]} ({especialidades})",
                UsuarioNombreCompleto = $"{reader["UsuarioNombre"]} {reader["UsuarioApellidos"]}",
                EspecialidadNombre = reader["EspecialidadNombre"] == DBNull.Value ? string.Empty : reader["EspecialidadNombre"].ToString() ?? string.Empty
            };
        }
    }
}

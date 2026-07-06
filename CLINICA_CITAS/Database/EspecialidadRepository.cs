using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Database
{
    public class EspecialidadRepository
    {
        public List<Especialidad> ListarActivos()
        {
            var lista = new List<Especialidad>();
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Especialidad WHERE Estado = 1 ORDER BY NombreEspecialidad";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Especialidad
                            {
                                ID_Especialidad = Convert.ToInt32(reader["ID_Especialidad"]),
                                NombreEspecialidad = reader["NombreEspecialidad"].ToString() ?? string.Empty,
                                Descripcion = reader["Descripcion"].ToString() ?? string.Empty,
                                Estado = Convert.ToBoolean(reader["Estado"])
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}

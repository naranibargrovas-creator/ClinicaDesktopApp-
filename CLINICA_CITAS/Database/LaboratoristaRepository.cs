using System;
using Microsoft.Data.SqlClient;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Database
{
    public class LaboratoristaRepository
    {
        public Laboratorista? ValidarCredenciales(string apodo, string contrasena)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT ID_Lab, Nombre, Apellidos, Correo, Direccion, 
                                        FechaNacimiento, Apodo, ContrasenaHash, TipoDoc, NumeroDoc
                                 FROM Laboratorista 
                                 WHERE Apodo = @Apodo AND ContrasenaHash = @Contrasena";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Apodo", apodo);
                    command.Parameters.AddWithValue("@Contrasena", contrasena);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearLaboratorista(reader);
                        }
                    }
                }
            }
            return null;
        }

        private Laboratorista MapearLaboratorista(SqlDataReader reader)
        {
            return new Laboratorista
            {
                ID_Lab = Convert.ToInt32(reader["ID_Lab"]),
                Nombre = reader["Nombre"].ToString() ?? string.Empty,
                Apellidos = reader["Apellidos"].ToString() ?? string.Empty,
                Correo = reader["Correo"].ToString() ?? string.Empty,
                Direccion = reader["Direccion"].ToString() ?? string.Empty,
                FechaNacimiento = Convert.ToDateTime(reader["FechaNacimiento"]),
                Apodo = reader["Apodo"].ToString() ?? string.Empty,
                ContrasenaHash = reader["ContrasenaHash"].ToString() ?? string.Empty,
                TipoDoc = reader["TipoDoc"].ToString() ?? string.Empty,
                NumeroDoc = reader["NumeroDoc"].ToString() ?? string.Empty
            };
        }
    }
}

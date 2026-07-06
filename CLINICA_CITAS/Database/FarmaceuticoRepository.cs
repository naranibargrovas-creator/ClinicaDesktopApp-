using System;
using Microsoft.Data.SqlClient;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Database
{
    public class FarmaceuticoRepository
    {
        public Farmaceutico? ValidarCredenciales(string apodo, string contrasena)
        {
            using (var connection = new SqlConnection(DbConfig.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT ID_Farmaceutico, Nombre, Apellidos, Correo, Direccion, 
                                        ContrasenaHash, Apodo, FechaNacimiento, Telefono, TipoDoc, NumeroDoc
                                 FROM Farmaceutico 
                                 WHERE Apodo = @Apodo AND ContrasenaHash = @Contrasena";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Apodo", apodo);
                    command.Parameters.AddWithValue("@Contrasena", contrasena);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearFarmaceutico(reader);
                        }
                    }
                }
            }
            return null;
        }

        private Farmaceutico MapearFarmaceutico(SqlDataReader reader)
        {
            return new Farmaceutico
            {
                ID_Farmaceutico = Convert.ToInt32(reader["ID_Farmaceutico"]),
                Nombre = reader["Nombre"].ToString() ?? string.Empty,
                Apellidos = reader["Apellidos"].ToString() ?? string.Empty,
                Correo = reader["Correo"].ToString() ?? string.Empty,
                Direccion = reader["Direccion"].ToString() ?? string.Empty,
                ContrasenaHash = reader["ContrasenaHash"].ToString() ?? string.Empty,
                Apodo = reader["Apodo"].ToString() ?? string.Empty,
                FechaNacimiento = Convert.ToDateTime(reader["FechaNacimiento"]),
                Telefono = reader["Telefono"].ToString() ?? string.Empty,
                TipoDoc = reader["TipoDoc"].ToString() ?? string.Empty,
                NumeroDoc = reader["NumeroDoc"].ToString() ?? string.Empty
            };
        }
    }
}

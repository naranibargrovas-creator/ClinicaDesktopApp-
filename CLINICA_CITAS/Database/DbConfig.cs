namespace CLINICA_CITAS.Database
{
    public static class DbConfig
    {
        /// <summary>
        /// Cadena de conexión para la base de datos CLINICA_CITAS.
        /// Modifique este valor si su servidor de base de datos tiene un nombre o credenciales diferentes.
        /// </summary>
        public static string ConnectionString { get; set; } =
            @"Server=LAPTOP-2BE5D2EQ;Database=GESTION_CLINICA;Integrated Security=True;TrustServerCertificate=True;";
    }
}

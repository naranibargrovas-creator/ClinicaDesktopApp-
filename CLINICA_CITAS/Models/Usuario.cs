namespace CLINICA_CITAS.Models
{
    public class Usuario
    {
        public int ID_Usuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool Logueado { get; set; }
    }
}

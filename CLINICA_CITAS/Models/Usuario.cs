using System;

namespace CLINICA_CITAS.Models
{
    public class Usuario
    {
        public int ID_Usuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool Logueado { get; set; }
        public string Apodo { get; set; } = string.Empty;
        public string ContrasenaHash { get; set; } = string.Empty;
        public DateTime? UltimoAcceso { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Propiedad de ayuda para enlace de datos
        public string NombreCompleto => $"{Apellidos}, {Nombre}";
        public string EstadoTexto => Estado;
    }
}

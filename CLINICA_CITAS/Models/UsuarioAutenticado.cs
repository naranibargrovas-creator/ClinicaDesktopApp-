using System;

namespace CLINICA_CITAS.Models
{
    public class UsuarioAutenticado
    {
        public string TipoUsuario { get; set; } = string.Empty; // Usuario, Medico, Farmaceutico, Laboratorista
        public int ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public object? UsuarioOriginal { get; set; } // El objeto original (Usuario, Medico, etc.)

        // Propiedad de ayuda para enlace de datos
        public string NombreCompleto => $"{Apellidos}, {Nombre}";
    }
}

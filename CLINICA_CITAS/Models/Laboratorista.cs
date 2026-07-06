using System;

namespace CLINICA_CITAS.Models
{
    public class Laboratorista
    {
        public int ID_Lab { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Apodo { get; set; } = string.Empty;
        public string ContrasenaHash { get; set; } = string.Empty;
        public string TipoDoc { get; set; } = string.Empty;
        public string NumeroDoc { get; set; } = string.Empty;

        // Propiedad de ayuda para enlace de datos
        public string NombreCompleto => $"{Apellidos}, {Nombre}";
    }
}

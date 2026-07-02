using System;

namespace CLINICA_CITAS.Models
{
    public class Paciente
    {
        public int ID_Paciente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty; // CHAR(1)
        public string Direccion { get; set; } = string.Empty;
        public string TipoDoc { get; set; } = string.Empty;
        public string NumeroDoc { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string UsuarioRegistrador { get; set; } = string.Empty;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public string UsuarioModificador { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

        // Propiedades de ayuda para enlace de datos
        public string NombreCompleto => $"{Apellidos}, {Nombre}";
        public string EstadoTexto => Estado ? "Activo" : "Alta";
    }
}

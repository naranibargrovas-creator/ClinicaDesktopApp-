using System;

namespace CLINICA_CITAS.Models
{
    public class Medico
    {
        public int ID_Medico { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string Colegiatura { get; set; } = string.Empty;
        public string DisponibilidadHorario { get; set; } = string.Empty;
        public string Apodo { get; set; } = string.Empty;
        public string ContrasenaHash { get; set; } = string.Empty;
        public string TipoDoc { get; set; } = string.Empty;
        public string NumeroDoc { get; set; } = string.Empty;

        // Propiedad de ayuda para enlace de datos
        public string NombreCompleto => $"{Apellidos}, {Nombre}";

        public string Especialidades { get; set; } = string.Empty;

        public string NombreConEspecialidad => string.IsNullOrEmpty(Especialidades)
            ? $"{Nombre} {Apellidos}"
            : $"{Nombre} {Apellidos} ({Especialidades})";
    }
}

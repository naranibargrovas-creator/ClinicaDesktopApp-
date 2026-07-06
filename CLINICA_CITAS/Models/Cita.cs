using System;

namespace CLINICA_CITAS.Models
{
    public class Cita
    {
        public int ID_Cita { get; set; }
        public DateTime HoraInicio { get; set; } = DateTime.Now;
        public DateTime? HoraFin { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Estado { get; set; } = "Pendiente";
        public int ID_Paciente { get; set; }
        public int ID_Medico { get; set; }
        public int Id_Usuario { get; set; }
        public int? ID_Especialidad { get; set; }

        // Mapeo auxiliar para representarlo en DataGrid
        public string PacienteNombreCompleto { get; set; } = string.Empty;
        public string MedicoNombreCompleto { get; set; } = string.Empty;
        public string UsuarioNombreCompleto { get; set; } = string.Empty;
        public string EspecialidadNombre { get; set; } = string.Empty;
    }
}

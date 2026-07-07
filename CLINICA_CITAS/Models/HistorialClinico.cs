using System;

namespace CLINICA_CITAS.Models
{
    public class HistorialClinico
    {
        public int ID_HistorialClinico { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacionHistorial { get; set; }
        public string Estado { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }
        public string PresionArterial { get; set; }
        public string TipoSangre { get; set; }
        public string Antecedentes { get; set; }
        public string UsuarioRegistrador { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string UsuarioModificador { get; set; }
        public int ID_Cita { get; set; }
        public int ID_Paciente { get; set; }

        // Propiedades auxiliares para mostrar nombres relacionados
        public string PacienteNombreCompleto { get; set; }
        public string MedicoNombreCompleto { get; set; }
        public string CitaMotivo { get; set; }
    }
}

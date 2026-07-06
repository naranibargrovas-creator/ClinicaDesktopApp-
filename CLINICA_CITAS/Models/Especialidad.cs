using System;

namespace CLINICA_CITAS.Models
{
    public class Especialidad
    {
        public int ID_Especialidad { get; set; }
        public string NombreEspecialidad { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}

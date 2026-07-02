using System.Windows.Controls;
using CLINICA_CITAS.Database;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Views
{
    public partial class DashboardControl : UserControl
    {
        private readonly CitaRepository _citaRepository;
        private readonly Usuario _usuario;

        public DashboardControl()
        {
            InitializeComponent();
            _citaRepository = new CitaRepository();
            _usuario = new Usuario { Nombre = "Usuario" };
        }

        public DashboardControl(Usuario usuario, CitaRepository citaRepository)
        {
            InitializeComponent();
            _usuario = usuario;
            _citaRepository = citaRepository;
            CargarDatos();
        }

        private void CargarDatos()
        {
            TxtWelcomeMsg.Text = $"Hola {_usuario.Nombre}, bienvenido de vuelta al sistema de gestión de citas.";
            ActualizarEstadisticas();
        }

        public void ActualizarEstadisticas()
        {
            try
            {
                var stats = _citaRepository.ObtenerEstadisticasDashboard();
                TxtStatPacientes.Text = stats["Pacientes"].ToString();
                TxtStatMedicos.Text = stats["Medicos"].ToString();
                TxtStatCitasHoy.Text = stats["CitasHoy"].ToString();
                TxtStatCitasPendientes.Text = stats["CitasPendientes"].ToString();
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al cargar estadísticas: {ex.Message}", "Error de BD", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}

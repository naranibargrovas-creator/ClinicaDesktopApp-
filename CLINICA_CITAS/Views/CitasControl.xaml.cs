using System;
using System.Windows;
using System.Windows.Controls;
using CLINICA_CITAS.Database;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Views
{
    public partial class CitasControl : UserControl
    {
        private readonly CitaRepository _citaRepository;
        private readonly PacienteRepository _pacienteRepository;
        private readonly MedicoRepository _medicoRepository;
        private readonly EspecialidadRepository _especialidadRepository;
        private readonly UsuarioAutenticado _usuario;

        public CitasControl()
        {
            InitializeComponent();
            _citaRepository = new CitaRepository();
            _pacienteRepository = new PacienteRepository();
            _medicoRepository = new MedicoRepository();
            _especialidadRepository = new EspecialidadRepository();
            _usuario = new UsuarioAutenticado();
        }

        public CitasControl(UsuarioAutenticado usuario, CitaRepository citaRepository, PacienteRepository pacienteRepository, MedicoRepository medicoRepository)
        {
            InitializeComponent();
            _usuario = usuario;
            _citaRepository = citaRepository;
            _pacienteRepository = pacienteRepository;
            _medicoRepository = medicoRepository;
            _especialidadRepository = new EspecialidadRepository();
            _citaRepository.ActualizarEspecialidadesCitasExistentes();
            CargarListadoCitas();
        }

        public void CargarListadoCitas()
        {
            try
            {
                DgCitas.ItemsSource = _citaRepository.ListarTodas();
                GridCitaList.Visibility = Visibility.Visible;
                GridCitaForm.Visibility = Visibility.Collapsed;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al listar citas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBuscarCita_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filtro = TxtBuscarCita.Text.Trim();
                if (string.IsNullOrEmpty(filtro))
                {
                    DgCitas.ItemsSource = _citaRepository.ListarTodas();
                }
                else
                {
                    DgCitas.ItemsSource = _citaRepository.BuscarPorFiltro(filtro);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al buscar citas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cargar listas en combo
                CbCitaPaciente.ItemsSource = _pacienteRepository.ListarTodos();
                CbCitaEspecialidad.ItemsSource = _especialidadRepository.ListarActivos();
                CbCitaMedico.ItemsSource = null;

                // Limpiar campos
                CbCitaPaciente.SelectedIndex = -1;
                CbCitaEspecialidad.SelectedIndex = -1;
                CbCitaMedico.SelectedIndex = -1;
                DpCitaFecha.SelectedDate = DateTime.Today;
                TxtCitaHora.Text = DateTime.Now.ToString("HH:mm");
                TxtCitaMotivo.Text = string.Empty;

                GridCitaList.Visibility = Visibility.Collapsed;
                GridCitaForm.Visibility = Visibility.Visible;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al inicializar formulario de citas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CbCitaEspecialidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CbCitaEspecialidad.SelectedValue is int idEspecialidad)
                {
                    CbCitaMedico.ItemsSource = _medicoRepository.ListarPorEspecialidad(idEspecialidad);
                }
                else
                {
                    CbCitaMedico.ItemsSource = null;
                }
                CbCitaMedico.SelectedIndex = -1;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al filtrar médicos por especialidad: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelarCitaForm_Click(object sender, RoutedEventArgs e)
        {
            CargarListadoCitas();
        }

        private void BtnGuardarCita_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones
            if (CbCitaPaciente.SelectedValue == null ||
                CbCitaEspecialidad.SelectedValue == null ||
                CbCitaMedico.SelectedValue == null ||
                DpCitaFecha.SelectedDate == null ||
                string.IsNullOrWhiteSpace(TxtCitaHora.Text) ||
                string.IsNullOrWhiteSpace(TxtCitaMotivo.Text))
            {
                MessageBox.Show("Por favor complete todos los campos obligatorios (*).", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(TxtCitaHora.Text.Trim(), out TimeSpan hora))
            {
                MessageBox.Show("Por favor ingrese una hora válida en formato de 24 horas (Ej: 09:30 o 14:15).", "Hora inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var cita = new Cita();
                cita.ID_Paciente = Convert.ToInt32(CbCitaPaciente.SelectedValue);
                cita.ID_Medico = Convert.ToInt32(CbCitaMedico.SelectedValue);
                cita.ID_Especialidad = Convert.ToInt32(CbCitaEspecialidad.SelectedValue);
                cita.HoraInicio = DpCitaFecha.SelectedDate.Value.Date.Add(hora);
                cita.HoraFin = cita.HoraInicio.AddMinutes(30); // Cita dura 30 mins por defecto
                cita.Motivo = TxtCitaMotivo.Text.Trim();
                cita.Estado = "Pendiente";
                cita.Id_Usuario = _usuario.ID;

                _citaRepository.CrearCita(cita);

                MessageBox.Show("Cita programada correctamente.", "Cita registrada", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarListadoCitas();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al programar la cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnConfirmarCita_Click(object sender, RoutedEventArgs e)
        {
            ActualizarEstadoCitaSeleccionada("Confirmada");
        }

        private void BtnAtenderCita_Click(object sender, RoutedEventArgs e)
        {
            ActualizarEstadoCitaSeleccionada("Atendida");
        }

        private void BtnCancelarCita_Click(object sender, RoutedEventArgs e)
        {
            ActualizarEstadoCitaSeleccionada("Cancelada");
        }

        private void ActualizarEstadoCitaSeleccionada(string nuevoEstado)
        {
            if (DgCitas.SelectedItem is Cita c)
            {
                try
                {
                    _citaRepository.ActualizarEstado(c.ID_Cita, nuevoEstado);
                    MessageBox.Show($"La cita ha sido cambiada a: {nuevoEstado}", "Cita actualizada", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarListadoCitas();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Error al actualizar estado de cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una cita de la lista.", "Seleccionar cita", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

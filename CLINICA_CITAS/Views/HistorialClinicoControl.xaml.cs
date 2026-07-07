using System;
using System.Windows;
using System.Windows.Controls;
using CLINICA_CITAS.Database;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Views
{
    public partial class HistorialClinicoControl : UserControl
    {
        private readonly HistorialClinicoRepository _historialRepository;
        private readonly PacienteRepository _pacienteRepository;
        private readonly CitaRepository _citaRepository;
        private readonly UsuarioAutenticado _usuario;
        private HistorialClinico _historialSeleccionado;

        public HistorialClinicoControl()
        {
            InitializeComponent();
            _historialRepository = new HistorialClinicoRepository();
            _pacienteRepository = new PacienteRepository();
            _citaRepository = new CitaRepository();
            _usuario = new UsuarioAutenticado();
        }

        public HistorialClinicoControl(UsuarioAutenticado usuario, HistorialClinicoRepository historialRepository, PacienteRepository pacienteRepository, CitaRepository citaRepository)
        {
            InitializeComponent();
            _usuario = usuario;
            _historialRepository = historialRepository;
            _pacienteRepository = pacienteRepository;
            _citaRepository = citaRepository;
            CargarListadoHistorial();
        }

        public void CargarListadoHistorial()
        {
            try
            {
                DgHistorial.ItemsSource = _historialRepository.ListarTodos();
                GridHistorialList.Visibility = Visibility.Visible;
                GridHistorialForm.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al listar historial clínico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBuscarHistorial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filtro = TxtBuscarHistorial.Text.Trim();
                if (string.IsNullOrEmpty(filtro))
                {
                    DgHistorial.ItemsSource = _historialRepository.ListarTodos();
                }
                else
                {
                    DgHistorial.ItemsSource = _historialRepository.BuscarPorFiltro(filtro);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar historial clínico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNuevoHistorial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cargar listas en combo
                CbHistorialPaciente.ItemsSource = _pacienteRepository.ListarTodos();
                CbHistorialCita.ItemsSource = null;

                // Limpiar campos
                _historialSeleccionado = null;
                CbHistorialPaciente.SelectedIndex = -1;
                CbHistorialCita.SelectedIndex = -1;
                CbHistorialEstado.SelectedIndex = 0;
                CbHistorialTipoSangre.SelectedIndex = 0;
                TxtHistorialPeso.Text = string.Empty;
                TxtHistorialAltura.Text = string.Empty;
                TxtHistorialPresion.Text = string.Empty;
                TxtHistorialAntecedentes.Text = string.Empty;
                TxtHistorialObservaciones.Text = string.Empty;

                LblTituloFormHistorial.Text = "Nuevo Historial Clínico";

                GridHistorialList.Visibility = Visibility.Collapsed;
                GridHistorialForm.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar formulario de historial clínico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CbHistorialPaciente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CbHistorialPaciente.SelectedItem is Paciente pacienteSeleccionado)
                {
                    // Cargar citas del paciente seleccionado
                    var citas = _citaRepository.ListarPorPaciente(pacienteSeleccionado.ID_Paciente);
                    CbHistorialCita.ItemsSource = citas;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas del paciente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditarHistorial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgHistorial.SelectedItem is HistorialClinico historial)
                {
                    _historialSeleccionado = _historialRepository.ObtenerPorId(historial.ID_HistorialClinico);

                    if (_historialSeleccionado != null)
                    {
                        // Cargar listas
                        CbHistorialPaciente.ItemsSource = _pacienteRepository.ListarTodos();
                        CbHistorialCita.ItemsSource = _citaRepository.ListarTodas();

                        // Cargar datos
                        CbHistorialPaciente.SelectedValue = _historialSeleccionado.ID_Paciente;
                        CbHistorialCita.SelectedValue = _historialSeleccionado.ID_Cita;
                        CbHistorialEstado.Text = _historialSeleccionado.Estado;
                        CbHistorialTipoSangre.Text = _historialSeleccionado.TipoSangre;
                        TxtHistorialPeso.Text = _historialSeleccionado.Peso?.ToString() ?? string.Empty;
                        TxtHistorialAltura.Text = _historialSeleccionado.Altura?.ToString() ?? string.Empty;
                        TxtHistorialPresion.Text = _historialSeleccionado.PresionArterial;
                        TxtHistorialAntecedentes.Text = _historialSeleccionado.Antecedentes;
                        TxtHistorialObservaciones.Text = _historialSeleccionado.Observaciones;

                        LblTituloFormHistorial.Text = "Editar Historial Clínico";

                        GridHistorialList.Visibility = Visibility.Collapsed;
                        GridHistorialForm.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un historial clínico para editar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar historial clínico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEliminarHistorial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgHistorial.SelectedItem is HistorialClinico historial)
                {
                    var result = MessageBox.Show($"¿Está seguro de eliminar el historial clínico del paciente {historial.PacienteNombreCompleto}?", 
                        "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _historialRepository.Eliminar(historial.ID_HistorialClinico);
                        CargarListadoHistorial();
                        MessageBox.Show("Historial clínico eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un historial clínico para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar historial clínico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGuardarHistorial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones
                if (CbHistorialPaciente.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione un paciente.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CbHistorialCita.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione una cita.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtHistorialPresion.Text))
                {
                    MessageBox.Show("Ingrese la presión arterial.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtHistorialAntecedentes.Text))
                {
                    MessageBox.Show("Ingrese los antecedentes.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtHistorialObservaciones.Text))
                {
                    MessageBox.Show("Ingrese las observaciones.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var paciente = (Paciente)CbHistorialPaciente.SelectedItem;
                var cita = (Cita)CbHistorialCita.SelectedItem;

                var historial = new HistorialClinico
                {
                    ID_Paciente = paciente.ID_Paciente,
                    ID_Cita = cita.ID_Cita,
                    Estado = CbHistorialEstado.Text,
                    TipoSangre = CbHistorialTipoSangre.Text,
                    PresionArterial = TxtHistorialPresion.Text.Trim(),
                    Antecedentes = TxtHistorialAntecedentes.Text.Trim(),
                    Observaciones = TxtHistorialObservaciones.Text.Trim(),
                    UsuarioRegistrador = _usuario.NombreCompleto ?? "Sistema",
                    UsuarioModificador = _usuario.NombreCompleto ?? "Sistema"
                };

                // Parsear peso y altura si tienen valor
                if (decimal.TryParse(TxtHistorialPeso.Text.Trim(), out decimal peso))
                {
                    historial.Peso = peso;
                }

                if (decimal.TryParse(TxtHistorialAltura.Text.Trim(), out decimal altura))
                {
                    historial.Altura = altura;
                }

                if (_historialSeleccionado != null)
                {
                    // Actualizar
                    historial.ID_HistorialClinico = _historialSeleccionado.ID_HistorialClinico;
                    _historialRepository.Actualizar(historial);
                    MessageBox.Show("Historial clínico actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Insertar
                    _historialRepository.Insertar(historial);
                    MessageBox.Show("Historial clínico creado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CargarListadoHistorial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar historial clínico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelarHistorialForm_Click(object sender, RoutedEventArgs e)
        {
            GridHistorialList.Visibility = Visibility.Visible;
            GridHistorialForm.Visibility = Visibility.Collapsed;
            _historialSeleccionado = null;
        }
    }
}

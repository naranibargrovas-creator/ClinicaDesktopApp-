using System.Windows;
using System.Windows.Controls;
using CLINICA_CITAS.Database;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Views
{
    public partial class PacientesControl : UserControl
    {
        private readonly PacienteRepository _pacienteRepository;
        private readonly Usuario _usuario;
        private Paciente? _selectedPaciente;

        public PacientesControl()
        {
            InitializeComponent();
            _pacienteRepository = new PacienteRepository();
            _usuario = new Usuario();
        }

        public PacientesControl(Usuario usuario, PacienteRepository pacienteRepository)
        {
            InitializeComponent();
            _usuario = usuario;
            _pacienteRepository = pacienteRepository;
            CargarListadoPacientes();
        }

        public void CargarListadoPacientes()
        {
            try
            {
                DgPacientes.ItemsSource = _pacienteRepository.ListarTodos();
                GridPacienteList.Visibility = Visibility.Visible;
                GridPacienteForm.Visibility = Visibility.Collapsed;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al listar pacientes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBuscarPaciente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filtro = TxtBuscarPaciente.Text.Trim();
                if (string.IsNullOrEmpty(filtro))
                {
                    DgPacientes.ItemsSource = _pacienteRepository.ListarTodos();
                }
                else
                {
                    DgPacientes.ItemsSource = _pacienteRepository.BuscarPorFiltro(filtro);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al buscar pacientes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNuevoPaciente_Click(object sender, RoutedEventArgs e)
        {
            _selectedPaciente = null;
            LblTituloFormPaciente.Text = "Registrar Paciente";

            // Limpiar campos
            TxtPacNombre.Text = string.Empty;
            TxtPacApellidos.Text = string.Empty;
            CbPacTipoDoc.SelectedIndex = 0;
            TxtPacNumeroDoc.Text = string.Empty;
            CbPacSexo.SelectedIndex = 0;
            DpPacFechaNacimiento.SelectedDate = null;
            TxtPacTelefono.Text = string.Empty;
            TxtPacCorreo.Text = string.Empty;
            TxtPacDireccion.Text = string.Empty;
            ChkPacEstado.IsChecked = true;

            GridPacienteList.Visibility = Visibility.Collapsed;
            GridPacienteForm.Visibility = Visibility.Visible;
        }

        private void BtnEditarPaciente_Click(object sender, RoutedEventArgs e)
        {
            if (DgPacientes.SelectedItem is Paciente p)
            {
                _selectedPaciente = p;
                LblTituloFormPaciente.Text = "Editar Paciente";

                TxtPacNombre.Text = p.Nombre;
                TxtPacApellidos.Text = p.Apellidos;

                // TipoDoc Combo Selection
                SelectComboItemByContent(CbPacTipoDoc, p.TipoDoc);
                TxtPacNumeroDoc.Text = p.NumeroDoc;

                // Sexo Selection
                if (p.Sexo == "M") CbPacSexo.SelectedIndex = 0;
                else CbPacSexo.SelectedIndex = 1;

                DpPacFechaNacimiento.SelectedDate = p.FechaNacimiento;
                TxtPacTelefono.Text = p.Telefono;
                TxtPacCorreo.Text = p.Correo;
                TxtPacDireccion.Text = p.Direccion;
                ChkPacEstado.IsChecked = p.Estado;

                GridPacienteList.Visibility = Visibility.Collapsed;
                GridPacienteForm.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Por favor seleccione un paciente de la lista para editar.", "Seleccionar paciente", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancelarPacForm_Click(object sender, RoutedEventArgs e)
        {
            CargarListadoPacientes();
        }

        private void BtnGuardarPaciente_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(TxtPacNombre.Text) ||
                string.IsNullOrWhiteSpace(TxtPacApellidos.Text) ||
                string.IsNullOrWhiteSpace(TxtPacNumeroDoc.Text) ||
                string.IsNullOrWhiteSpace(TxtPacTelefono.Text) ||
                string.IsNullOrWhiteSpace(TxtPacDireccion.Text))
            {
                MessageBox.Show("Por favor complete todos los campos marcados con (*).", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var pac = _selectedPaciente ?? new Paciente();
                pac.Nombre = TxtPacNombre.Text.Trim();
                pac.Apellidos = TxtPacApellidos.Text.Trim();
                pac.TipoDoc = (CbPacTipoDoc.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "DNI";
                pac.NumeroDoc = TxtPacNumeroDoc.Text.Trim();
                pac.Sexo = (CbPacSexo.SelectedIndex == 0) ? "M" : "F";
                pac.FechaNacimiento = DpPacFechaNacimiento.SelectedDate;
                pac.Telefono = TxtPacTelefono.Text.Trim();
                pac.Correo = TxtPacCorreo.Text.Trim();
                pac.Direccion = TxtPacDireccion.Text.Trim();
                pac.Estado = ChkPacEstado.IsChecked ?? true;
                pac.UsuarioRegistrador = _usuario.Nombre + " " + _usuario.Apellidos;
                pac.UsuarioModificador = _usuario.Nombre + " " + _usuario.Apellidos;

                if (_selectedPaciente == null)
                {
                    _pacienteRepository.Insertar(pac);
                    MessageBox.Show("Paciente registrado correctamente.", "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _pacienteRepository.Actualizar(pac);
                    MessageBox.Show("Paciente actualizado correctamente.", "Actualización exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CargarListadoPacientes();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al guardar paciente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDarAltaPaciente_Click(object sender, RoutedEventArgs e)
        {
            if (DgPacientes.SelectedItem is Paciente p)
            {
                var result = MessageBox.Show($"¿Está seguro de que desea dar de alta (inactivar) al paciente {p.Nombre} {p.Apellidos}?", "Confirmar Alta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _pacienteRepository.Eliminar(p.ID_Paciente);
                        MessageBox.Show("Estado del paciente actualizado.", "Operación exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarListadoPacientes();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar estado del paciente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione un paciente de la lista.", "Seleccionar paciente", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SelectComboItemByContent(ComboBox cb, string content)
        {
            for (int i = 0; i < cb.Items.Count; i++)
            {
                if (cb.Items[i] is ComboBoxItem cbi && string.Equals(cbi.Content.ToString(), content, System.StringComparison.OrdinalIgnoreCase))
                {
                    cb.SelectedIndex = i;
                    return;
                }
            }
        }
    }
}

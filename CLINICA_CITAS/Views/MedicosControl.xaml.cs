using System.Windows;
using System.Windows.Controls;
using CLINICA_CITAS.Database;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Views
{
    public partial class MedicosControl : UserControl
    {
        private readonly MedicoRepository _medicoRepository;
        private Medico? _selectedMedico;

        public MedicosControl()
        {
            InitializeComponent();
            _medicoRepository = new MedicoRepository();
        }

        public MedicosControl(MedicoRepository medicoRepository)
        {
            InitializeComponent();
            _medicoRepository = medicoRepository;
            CargarListadoMedicos();
        }

        public void CargarListadoMedicos()
        {
            try
            {
                DgMedicos.ItemsSource = _medicoRepository.ListarTodos();
                GridMedicoList.Visibility = Visibility.Visible;
                GridMedicoForm.Visibility = Visibility.Collapsed;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al listar médicos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBuscarMedico_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filtro = TxtBuscarMedico.Text.Trim();
                if (string.IsNullOrEmpty(filtro))
                {
                    DgMedicos.ItemsSource = _medicoRepository.ListarTodos();
                }
                else
                {
                    DgMedicos.ItemsSource = _medicoRepository.BuscarPorFiltro(filtro);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al buscar médicos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNuevoMedico_Click(object sender, RoutedEventArgs e)
        {
            _selectedMedico = null;
            LblTituloFormMedico.Text = "Registrar Médico";

            TxtMedNombre.Text = string.Empty;
            TxtMedApellidos.Text = string.Empty;
            CbMedTipoDoc.SelectedIndex = 0;
            TxtMedNumeroDoc.Text = string.Empty;
            TxtMedColegiatura.Text = string.Empty;
            CbMedHorario.SelectedIndex = 0;
            TxtMedApodo.Text = string.Empty;
            TxtMedContrasena.Text = string.Empty;
            TxtMedCorreo.Text = string.Empty;
            TxtMedDireccion.Text = string.Empty;
            DpMedFechaNacimiento.SelectedDate = null;

            GridMedicoList.Visibility = Visibility.Collapsed;
            GridMedicoForm.Visibility = Visibility.Visible;
        }

        private void BtnEditarMedico_Click(object sender, RoutedEventArgs e)
        {
            if (DgMedicos.SelectedItem is Medico m)
            {
                _selectedMedico = m;
                LblTituloFormMedico.Text = "Editar Médico";

                TxtMedNombre.Text = m.Nombre;
                TxtMedApellidos.Text = m.Apellidos;
                SelectComboItemByContent(CbMedTipoDoc, m.TipoDoc);
                TxtMedNumeroDoc.Text = m.NumeroDoc;
                TxtMedColegiatura.Text = m.Colegiatura;

                // Select Horario from combo content
                string baseHorario = m.DisponibilidadHorario.Split(' ')[0]; // E.g. Mañana
                SelectComboItemByPartialContent(CbMedHorario, baseHorario);

                TxtMedApodo.Text = m.Apodo;
                TxtMedContrasena.Text = m.ContrasenaHash;
                TxtMedCorreo.Text = m.Correo;
                TxtMedDireccion.Text = m.Direccion;
                DpMedFechaNacimiento.SelectedDate = m.FechaNacimiento;

                GridMedicoList.Visibility = Visibility.Collapsed;
                GridMedicoForm.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Por favor seleccione un médico de la lista para editar.", "Seleccionar médico", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancelarMedForm_Click(object sender, RoutedEventArgs e)
        {
            CargarListadoMedicos();
        }

        private void BtnGuardarMedico_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtMedNombre.Text) ||
                string.IsNullOrWhiteSpace(TxtMedApellidos.Text) ||
                string.IsNullOrWhiteSpace(TxtMedNumeroDoc.Text) ||
                string.IsNullOrWhiteSpace(TxtMedColegiatura.Text) ||
                string.IsNullOrWhiteSpace(TxtMedApodo.Text) ||
                string.IsNullOrWhiteSpace(TxtMedCorreo.Text) ||
                string.IsNullOrWhiteSpace(TxtMedDireccion.Text))
            {
                MessageBox.Show("Por favor complete todos los campos marcados con (*).", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var med = _selectedMedico ?? new Medico();
                med.Nombre = TxtMedNombre.Text.Trim();
                med.Apellidos = TxtMedApellidos.Text.Trim();
                med.TipoDoc = (CbMedTipoDoc.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "DNI";
                med.NumeroDoc = TxtMedNumeroDoc.Text.Trim();
                med.Colegiatura = TxtMedColegiatura.Text.Trim();
                med.DisponibilidadHorario = (CbMedHorario.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Mañana (08:00 - 13:00)";
                med.Apodo = TxtMedApodo.Text.Trim();
                med.ContrasenaHash = TxtMedContrasena.Text.Trim();
                med.Correo = TxtMedCorreo.Text.Trim();
                med.Direccion = TxtMedDireccion.Text.Trim();
                med.FechaNacimiento = DpMedFechaNacimiento.SelectedDate;

                if (_selectedMedico == null)
                {
                    _medicoRepository.Insertar(med);
                    MessageBox.Show("Médico registrado correctamente.", "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _medicoRepository.Actualizar(med);
                    MessageBox.Show("Médico actualizado correctamente.", "Actualización exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CargarListadoMedicos();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al guardar médico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEliminarMedico_Click(object sender, RoutedEventArgs e)
        {
            if (DgMedicos.SelectedItem is Medico m)
            {
                var result = MessageBox.Show($"¿Está seguro de que desea eliminar al médico {m.Nombre} {m.Apellidos}?\nEsta acción no se puede deshacer.", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _medicoRepository.Eliminar(m.ID_Medico);
                        MessageBox.Show("Médico eliminado correctamente.", "Eliminación exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarListadoMedicos();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar médico: {ex.Message}\nEs posible que tenga citas asociadas.", "Error de integridad", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione un médico de la lista.", "Seleccionar médico", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void SelectComboItemByPartialContent(ComboBox cb, string partialContent)
        {
            for (int i = 0; i < cb.Items.Count; i++)
            {
                if (cb.Items[i] is ComboBoxItem cbi && cbi.Content.ToString()!.Contains(partialContent, System.StringComparison.OrdinalIgnoreCase))
                {
                    cb.SelectedIndex = i;
                    return;
                }
            }
        }
    }
}

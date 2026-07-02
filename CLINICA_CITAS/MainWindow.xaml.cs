using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CLINICA_CITAS.Database;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Usuario _usuario;
        
        // Repositorios
        private readonly PacienteRepository _pacienteRepository;
        private readonly MedicoRepository _medicoRepository;
        private readonly CitaRepository _citaRepository;
        private readonly UsuarioRepository _usuarioRepository;

        // Variables de estado
        private Paciente? _selectedPaciente;
        private Medico? _selectedMedico;
        private Usuario? _selectedUsuario;

        // Constructor para diseño de WPF y fallback
        public MainWindow()
        {
            InitializeComponent();
            _usuario = new Usuario { ID_Usuario = 1, Nombre = "Usuario", Apellidos = "Demo", Rol = "Administrador" };
            
            _pacienteRepository = new PacienteRepository();
            _medicoRepository = new MedicoRepository();
            _citaRepository = new CitaRepository();
            _usuarioRepository = new UsuarioRepository();
            
            InicializarComponentesPersonalizados();
        }

        // Constructor principal que recibe al usuario autenticado
        public MainWindow(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            
            _pacienteRepository = new PacienteRepository();
            _medicoRepository = new MedicoRepository();
            _citaRepository = new CitaRepository();
            _usuarioRepository = new UsuarioRepository();
            
            InicializarComponentesPersonalizados();
        }

        private void InicializarComponentesPersonalizados()
        {
            CargarDatosUsuario();
            ActualizarEstadisticasDashboard();
        }

        private void CargarDatosUsuario()
        {
            if (_usuario != null)
            {
                TxtUserRol.Text = _usuario.Rol;
                TxtWelcomeMsg.Text = $"Hola {_usuario.Nombre}, bienvenido de vuelta. Has iniciado sesión correctamente en el sistema de gestión de citas.";
            }
        }

        private void ActualizarEstadisticasDashboard()
        {
            try
            {
                var stats = _citaRepository.ObtenerEstadisticasDashboard();
                TxtStatPacientes.Text = stats["Pacientes"].ToString();
                TxtStatMedicos.Text = stats["Medicos"].ToString();
                TxtStatCitasHoy.Text = stats["CitasHoy"].ToString();
                TxtStatCitasPendientes.Text = stats["CitasPendientes"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar estadísticas: {ex.Message}", "Error de BD", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Navegación entre Paneles

        private void SetActivePanel(Grid activePanel, Border activeMenuBorder, TextBlock activeMenuText)
        {
            // Ocultar todos los paneles principales
            PanelDashboard.Visibility = Visibility.Collapsed;
            PanelPacientes.Visibility = Visibility.Collapsed;
            PanelMedicos.Visibility = Visibility.Collapsed;
            PanelCitas.Visibility = Visibility.Collapsed;
            PanelUsuarios.Visibility = Visibility.Collapsed;

            // Mostrar el panel seleccionado
            activePanel.Visibility = Visibility.Visible;

            // Restablecer estilos de la barra lateral (Bordes)
            BorderMenuInicio.Background = Brushes.Transparent;
            BorderMenuPacientes.Background = Brushes.Transparent;
            BorderMenuMedicos.Background = Brushes.Transparent;
            BorderMenuCitas.Background = Brushes.Transparent;
            BorderMenuUsuarios.Background = Brushes.Transparent;

            // Restablecer estilos de la barra lateral (Textos)
            var normalBrush = new SolidColorBrush(Color.FromRgb(71, 85, 105));
            TxtMenuInicio.Foreground = normalBrush;
            TxtMenuInicio.FontWeight = FontWeights.Medium;
            TxtMenuPacientes.Foreground = normalBrush;
            TxtMenuPacientes.FontWeight = FontWeights.Medium;
            TxtMenuMedicos.Foreground = normalBrush;
            TxtMenuMedicos.FontWeight = FontWeights.Medium;
            TxtMenuCitas.Foreground = normalBrush;
            TxtMenuCitas.FontWeight = FontWeights.Medium;
            TxtMenuUsuarios.Foreground = normalBrush;
            TxtMenuUsuarios.FontWeight = FontWeights.Medium;

            // Aplicar estilo al menú activo
            activeMenuBorder.Background = new SolidColorBrush(Color.FromRgb(241, 245, 249));
            activeMenuText.Foreground = new SolidColorBrush(Color.FromRgb(11, 114, 231));
            activeMenuText.FontWeight = FontWeights.SemiBold;
        }

        private void MenuUsuarios_Click(object sender, MouseButtonEventArgs e)
        {
            SetActivePanel(PanelUsuarios, BorderMenuUsuarios, TxtMenuUsuarios);
            CargarListadoUsuarios();
        }

        #region Módulo de Usuarios

        private void CargarListadoUsuarios()
        {
            try
            {
                DgUsuarios.ItemsSource = _usuarioRepository.ListarTodos();
                GridUsuarioList.Visibility = Visibility.Visible;
                GridUsuarioForm.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al listar usuarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBuscarUsuario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filtro = TxtBuscarUsuario.Text.Trim();
                if (string.IsNullOrEmpty(filtro))
                {
                    DgUsuarios.ItemsSource = _usuarioRepository.ListarTodos();
                }
                else
                {
                    DgUsuarios.ItemsSource = _usuarioRepository.BuscarPorFiltro(filtro);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar usuarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            _selectedUsuario = null;
            LblTituloFormUsuario.Text = "Registrar Usuario";
            
            // Limpiar campos
            TxtUsuNombre.Text = string.Empty;
            TxtUsuApellidos.Text = string.Empty;
            TxtUsuCorreo.Text = string.Empty;
            CbUsuRol.SelectedIndex = 0;
            TxtUsuContrasena.Text = string.Empty;
            CbUsuEstado.SelectedIndex = 0;

            GridUsuarioList.Visibility = Visibility.Collapsed;
            GridUsuarioForm.Visibility = Visibility.Visible;
        }

        private void BtnEditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (DgUsuarios.SelectedItem is Usuario u)
            {
                _selectedUsuario = u;
                LblTituloFormUsuario.Text = "Editar Usuario";

                TxtUsuNombre.Text = u.Nombre;
                TxtUsuApellidos.Text = u.Apellidos;
                TxtUsuCorreo.Text = u.Correo;
                SelectComboItemByContent(CbUsuRol, u.Rol);
                TxtUsuContrasena.Text = u.ContrasenaHash;
                SelectComboItemByContent(CbUsuEstado, u.Estado);

                GridUsuarioList.Visibility = Visibility.Collapsed;
                GridUsuarioForm.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Por favor seleccione un usuario de la lista para editar.", "Seleccionar usuario", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancelarUsuForm_Click(object sender, RoutedEventArgs e)
        {
            CargarListadoUsuarios();
        }

        private void BtnGuardarUsuario_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(TxtUsuNombre.Text) || 
                string.IsNullOrWhiteSpace(TxtUsuApellidos.Text) || 
                string.IsNullOrWhiteSpace(TxtUsuCorreo.Text) ||
                string.IsNullOrWhiteSpace(TxtUsuContrasena.Text))
            {
                MessageBox.Show("Por favor complete todos los campos marcados con (*).", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var usu = _selectedUsuario ?? new Usuario();
                usu.Nombre = TxtUsuNombre.Text.Trim();
                usu.Apellidos = TxtUsuApellidos.Text.Trim();
                usu.Correo = TxtUsuCorreo.Text.Trim();
                usu.Rol = (CbUsuRol.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Administrador";
                usu.ContrasenaHash = TxtUsuContrasena.Text.Trim();
                usu.Estado = (CbUsuEstado.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Activo";

                if (_selectedUsuario == null)
                {
                    _usuarioRepository.Insertar(usu);
                    MessageBox.Show("Usuario registrado correctamente.", "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _usuarioRepository.Actualizar(usu);
                    MessageBox.Show("Usuario actualizado correctamente.", "Actualización exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CargarListadoUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDesactivarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (DgUsuarios.SelectedItem is Usuario u)
            {
                var result = MessageBox.Show($"¿Está seguro de que desea desactivar al usuario {u.Nombre} {u.Apellidos}?", "Confirmar desactivación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _usuarioRepository.Eliminar(u.ID_Usuario);
                        MessageBox.Show("Usuario desactivado correctamente.", "Operación exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarListadoUsuarios();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al desactivar usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione un usuario de la lista.", "Seleccionar usuario", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        private void MenuInicio_Click(object sender, MouseButtonEventArgs e)
        {
            SetActivePanel(PanelDashboard, BorderMenuInicio, TxtMenuInicio);
            ActualizarEstadisticasDashboard();
        }

        private void MenuPacientes_Click(object sender, MouseButtonEventArgs e)
        {
            SetActivePanel(PanelPacientes, BorderMenuPacientes, TxtMenuPacientes);
            CargarListadoPacientes();
        }

        private void MenuMedicos_Click(object sender, MouseButtonEventArgs e)
        {
            SetActivePanel(PanelMedicos, BorderMenuMedicos, TxtMenuMedicos);
            CargarListadoMedicos();
        }

        private void MenuCitas_Click(object sender, MouseButtonEventArgs e)
        {
            SetActivePanel(PanelCitas, BorderMenuCitas, TxtMenuCitas);
            CargarListadoCitas();
        }

        #endregion

        #region Módulo de Pacientes

        private void CargarListadoPacientes()
        {
            try
            {
                DgPacientes.ItemsSource = _pacienteRepository.ListarTodos();
                GridPacienteList.Visibility = Visibility.Visible;
                GridPacienteForm.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
                    catch (Exception ex)
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

        #endregion

        #region Módulo de Médicos

        private void CargarListadoMedicos()
        {
            try
            {
                DgMedicos.ItemsSource = _medicoRepository.ListarTodos();
                GridMedicoList.Visibility = Visibility.Visible;
                GridMedicoForm.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
                    catch (Exception ex)
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

        #endregion

        #region Módulo de Citas

        private void CargarListadoCitas()
        {
            try
            {
                DgCitas.ItemsSource = _citaRepository.ListarTodas();
                GridCitaList.Visibility = Visibility.Visible;
                GridCitaForm.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
                CbCitaMedico.ItemsSource = _medicoRepository.ListarTodos();
                
                // Limpiar campos
                CbCitaPaciente.SelectedIndex = -1;
                CbCitaMedico.SelectedIndex = -1;
                DpCitaFecha.SelectedDate = DateTime.Today;
                TxtCitaHora.Text = DateTime.Now.ToString("HH:mm");
                TxtCitaMotivo.Text = string.Empty;

                GridCitaList.Visibility = Visibility.Collapsed;
                GridCitaForm.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar formulario de citas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                cita.HoraInicio = DpCitaFecha.SelectedDate.Value.Date.Add(hora);
                cita.HoraFin = cita.HoraInicio.AddMinutes(30); // Cita dura 30 mins por defecto
                cita.Motivo = TxtCitaMotivo.Text.Trim();
                cita.Estado = "Pendiente";
                cita.Id_Usuario = _usuario.ID_Usuario;

                _citaRepository.CrearCita(cita);

                MessageBox.Show("Cita programada correctamente.", "Cita registrada", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarListadoCitas();
            }
            catch (Exception ex)
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
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar estado de cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una cita de la lista.", "Seleccionar cita", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Helpers y Window Control

        private void SelectComboItemByContent(ComboBox cb, string content)
        {
            for (int i = 0; i < cb.Items.Count; i++)
            {
                if (cb.Items[i] is ComboBoxItem cbi && string.Equals(cbi.Content.ToString(), content, StringComparison.OrdinalIgnoreCase))
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
                if (cb.Items[i] is ComboBoxItem cbi && cbi.Content.ToString()!.Contains(partialContent, StringComparison.OrdinalIgnoreCase))
                {
                    cb.SelectedIndex = i;
                    return;
                }
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        #endregion
    }
}
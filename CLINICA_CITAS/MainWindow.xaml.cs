using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CLINICA_CITAS.Database;
using CLINICA_CITAS.Models;
using CLINICA_CITAS.Views;

namespace CLINICA_CITAS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UsuarioAutenticado _usuario;
        
        // Repositorios
        private readonly PacienteRepository _pacienteRepository;
        private readonly MedicoRepository _medicoRepository;
        private readonly CitaRepository _citaRepository;
        private readonly UsuarioRepository _usuarioRepository;

        // Controles de vista
        private DashboardControl? _dashboardControl;
        private PacientesControl? _pacientesControl;
        private MedicosControl? _medicosControl;
        private CitasControl? _citasControl;
        private UsuariosControl? _usuariosControl;

        // Constructor para diseño de WPF y fallback
        public MainWindow()
        {
            InitializeComponent();
            _usuario = new UsuarioAutenticado { ID = 1, Nombre = "Usuario", Apellidos = "Demo", Rol = "Administrador" };
            
            _pacienteRepository = new PacienteRepository();
            _medicoRepository = new MedicoRepository();
            _citaRepository = new CitaRepository();
            _usuarioRepository = new UsuarioRepository();
            
            InicializarComponentesPersonalizados();
        }

        // Constructor principal que recibe al usuario autenticado
        public MainWindow(UsuarioAutenticado usuario)
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
            MostrarDashboard();
        }

        private void CargarDatosUsuario()
        {
            if (_usuario != null)
            {
                TxtUserRol.Text = _usuario.Rol;
            }
        }

        #region Navegación entre Paneles

        private void SetActiveMenuStyle(Border activeMenuBorder, TextBlock activeMenuText)
        {
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

        private void MenuInicio_Click(object sender, MouseButtonEventArgs e)
        {
            MostrarDashboard();
        }

        private void MenuPacientes_Click(object sender, MouseButtonEventArgs e)
        {
            MostrarPacientes();
        }

        private void MenuMedicos_Click(object sender, MouseButtonEventArgs e)
        {
            MostrarMedicos();
        }

        private void MenuCitas_Click(object sender, MouseButtonEventArgs e)
        {
            MostrarCitas();
        }

        private void MenuUsuarios_Click(object sender, MouseButtonEventArgs e)
        {
            MostrarUsuarios();
        }

        #endregion

        #region Métodos para mostrar vistas

        private void MostrarDashboard()
        {
            SetActiveMenuStyle(BorderMenuInicio, TxtMenuInicio);

            if (_dashboardControl == null)
            {
                _dashboardControl = new DashboardControl(_usuario, _citaRepository);
            }
            else
            {
                _dashboardControl.ActualizarEstadisticas();
            }

            MainContent.Content = _dashboardControl;
        }

        private void MostrarPacientes()
        {
            SetActiveMenuStyle(BorderMenuPacientes, TxtMenuPacientes);

            if (_pacientesControl == null)
            {
                _pacientesControl = new PacientesControl(_usuario, _pacienteRepository);
            }
            else
            {
                _pacientesControl.CargarListadoPacientes();
            }

            MainContent.Content = _pacientesControl;
        }

        private void MostrarMedicos()
        {
            SetActiveMenuStyle(BorderMenuMedicos, TxtMenuMedicos);

            if (_medicosControl == null)
            {
                _medicosControl = new MedicosControl(_medicoRepository);
            }
            else
            {
                _medicosControl.CargarListadoMedicos();
            }

            MainContent.Content = _medicosControl;
        }

        private void MostrarCitas()
        {
            SetActiveMenuStyle(BorderMenuCitas, TxtMenuCitas);

            if (_citasControl == null)
            {
                _citasControl = new CitasControl(_usuario, _citaRepository, _pacienteRepository, _medicoRepository);
            }
            else
            {
                _citasControl.CargarListadoCitas();
            }

            MainContent.Content = _citasControl;
        }

        private void MostrarUsuarios()
        {
            SetActiveMenuStyle(BorderMenuUsuarios, TxtMenuUsuarios);

            if (_usuariosControl == null)
            {
                _usuariosControl = new UsuariosControl(_usuarioRepository);
            }
            else
            {
                _usuariosControl.CargarListadoUsuarios();
            }

            MainContent.Content = _usuariosControl;
        }

        #endregion

        #region Helpers y Window Control

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

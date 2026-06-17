using System;
using System.Windows;
using System.Windows.Input;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Usuario _usuario;

        // Constructor sin parámetros para compatibilidad con el Diseñador de WPF y fallback
        public MainWindow()
        {
            InitializeComponent();
            _usuario = new Usuario { Nombre = "Usuario", Apellidos = "Demo", Rol = "Administrador" };
            CargarDatosUsuario();
        }

        // Constructor principal que recibe al usuario autenticado
        public MainWindow(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            if (_usuario != null)
            {
                TxtUserName.Text = $"{_usuario.Nombre} {_usuario.Apellidos}";
                TxtUserRol.Text = _usuario.Rol;
                TxtInitial.Text = string.IsNullOrEmpty(_usuario.Nombre) ? "U" : _usuario.Nombre[0].ToString().ToUpper();
                TxtWelcomeMsg.Text = $"Hola {_usuario.Nombre}, bienvenido de vuelta. Has iniciado sesión correctamente en el sistema de gestión de citas.";
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
            // Cerramos la sesión del usuario volviendo a la pantalla de Login
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
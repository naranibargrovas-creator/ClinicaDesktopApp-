using System;
using System.Windows;
using System.Windows.Input;
using CLINICA_CITAS.Database;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS
{
    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
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
            Close();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string correo = TxtCorreo.Text.Trim();
            string contrasena = TxtContrasena.Password;

            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Por favor, ingresa tu correo y contraseña.", "Campos vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                UsuarioRepository repo = new UsuarioRepository();
                Usuario? usuario = repo.ValidarCredenciales(correo, contrasena);

                if (usuario == null)
                {
                    MessageBox.Show("Correo o contraseña incorrectos.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!string.Equals(usuario.Estado, "Activo", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"La cuenta del usuario {usuario.Nombre} {usuario.Apellidos} está inactiva (Estado: {usuario.Estado}).\nPor favor, contacte con el Administrador.", "Cuenta Inactiva", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Registrar inicio de sesión exitoso (actualizar Logueado = 1 y UltimoAcceso)
                repo.RegistrarInicioSesionExitoso(usuario.ID_Usuario);

                // Abrir el dashboard
                MainWindow dashboard = new MainWindow(usuario);
                dashboard.Show();

                // Cerrar la pantalla de Login
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos:\n{ex.Message}\n\nVerifique la cadena de conexión en DbConfig.cs y asegúrese de que el servidor SQL Express o LocalDB esté en ejecución.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

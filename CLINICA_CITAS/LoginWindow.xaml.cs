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
            string usuarioInput = TxtCorreo.Text.Trim();
            string contrasena = TxtContrasena.Password;

            if (string.IsNullOrWhiteSpace(usuarioInput) || string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Por favor, ingresa tu usuario/apodo y contraseña.", "Campos vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                UsuarioAutenticado? usuarioAutenticado = ValidarCredencialesMultiTabla(usuarioInput, contrasena);

                if (usuarioAutenticado == null)
                {
                    MessageBox.Show("Usuario, apodo o contraseña incorrectos.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Abrir el dashboard
                MainWindow dashboard = new MainWindow(usuarioAutenticado);
                dashboard.Show();

                // Cerrar la pantalla de Login
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos:\n{ex.Message}\n\nVerifique la cadena de conexión en DbConfig.cs y asegúrese de que el servidor SQL Express o LocalDB esté en ejecución.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private UsuarioAutenticado? ValidarCredencialesMultiTabla(string usuarioInput, string contrasena)
        {
            // Intentar validar como Usuario (por correo)
            var usuarioRepo = new UsuarioRepository();
            var usuario = usuarioRepo.ValidarCredenciales(usuarioInput, contrasena);
            if (usuario != null)
            {
                if (!string.Equals(usuario.Estado, "Activo", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"La cuenta del usuario {usuario.Nombre} {usuario.Apellidos} está inactiva (Estado: {usuario.Estado}).\nPor favor, contacte con el Administrador.", "Cuenta Inactiva", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                usuarioRepo.RegistrarInicioSesionExitoso(usuario.ID_Usuario);
                return new UsuarioAutenticado
                {
                    TipoUsuario = "Usuario",
                    ID = usuario.ID_Usuario,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Rol = usuario.Rol,
                    UsuarioOriginal = usuario
                };
            }

            // Intentar validar como Medico (por apodo)
            var medicoRepo = new MedicoRepository();
            var medico = medicoRepo.ValidarCredenciales(usuarioInput, contrasena);
            if (medico != null)
            {
                return new UsuarioAutenticado
                {
                    TipoUsuario = "Medico",
                    ID = medico.ID_Medico,
                    Nombre = medico.Nombre,
                    Apellidos = medico.Apellidos,
                    Rol = "Medico",
                    UsuarioOriginal = medico
                };
            }

            // Intentar validar como Farmaceutico (por apodo)
            var farmaceuticoRepo = new FarmaceuticoRepository();
            var farmaceutico = farmaceuticoRepo.ValidarCredenciales(usuarioInput, contrasena);
            if (farmaceutico != null)
            {
                return new UsuarioAutenticado
                {
                    TipoUsuario = "Farmaceutico",
                    ID = farmaceutico.ID_Farmaceutico,
                    Nombre = farmaceutico.Nombre,
                    Apellidos = farmaceutico.Apellidos,
                    Rol = "Farmaceutico",
                    UsuarioOriginal = farmaceutico
                };
            }

            // Intentar validar como Laboratorista (por apodo)
            var laboratoristaRepo = new LaboratoristaRepository();
            var laboratorista = laboratoristaRepo.ValidarCredenciales(usuarioInput, contrasena);
            if (laboratorista != null)
            {
                return new UsuarioAutenticado
                {
                    TipoUsuario = "Laboratorista",
                    ID = laboratorista.ID_Lab,
                    Nombre = laboratorista.Nombre,
                    Apellidos = laboratorista.Apellidos,
                    Rol = "Laboratorista",
                    UsuarioOriginal = laboratorista
                };
            }

            return null;
        }
    }
}

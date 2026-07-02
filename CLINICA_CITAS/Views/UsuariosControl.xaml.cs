using System.Windows;
using System.Windows.Controls;
using CLINICA_CITAS.Database;
using CLINICA_CITAS.Models;

namespace CLINICA_CITAS.Views
{
    public partial class UsuariosControl : UserControl
    {
        private readonly UsuarioRepository _usuarioRepository;
        private Usuario? _selectedUsuario;

        public UsuariosControl()
        {
            InitializeComponent();
            _usuarioRepository = new UsuarioRepository();
        }

        public UsuariosControl(UsuarioRepository usuarioRepository)
        {
            InitializeComponent();
            _usuarioRepository = usuarioRepository;
            CargarListadoUsuarios();
        }

        public void CargarListadoUsuarios()
        {
            try
            {
                DgUsuarios.ItemsSource = _usuarioRepository.ListarTodos();
                GridUsuarioList.Visibility = Visibility.Visible;
                GridUsuarioForm.Visibility = Visibility.Collapsed;
            }
            catch (System.Exception ex)
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
            catch (System.Exception ex)
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
            catch (System.Exception ex)
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
                    catch (System.Exception ex)
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

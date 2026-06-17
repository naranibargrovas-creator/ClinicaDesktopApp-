using System.Windows;
using System.Windows.Controls;

namespace CLINICA_CITAS.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty PasswordLengthProperty =
            DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxHelper), new PropertyMetadata(0));

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MonitorProperty =
            DependencyProperty.RegisterAttached("Monitor", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, OnMonitorChanged));

        public static int GetPasswordLength(DependencyObject obj) => (int)obj.GetValue(PasswordLengthProperty);
        public static void SetPasswordLength(DependencyObject obj, int value) => obj.SetValue(PasswordLengthProperty, value);

        public static string GetWatermark(DependencyObject obj) => (string)obj.GetValue(WatermarkProperty);
        public static void SetWatermark(DependencyObject obj, string value) => obj.SetValue(WatermarkProperty, value);

        public static bool GetMonitor(DependencyObject obj) => (bool)obj.GetValue(MonitorProperty);
        public static void SetMonitor(DependencyObject obj, bool value) => obj.SetValue(MonitorProperty, value);

        private static void OnMonitorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                    SetPasswordLength(passwordBox, passwordBox.Password.Length);
                }
                else
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetPasswordLength(passwordBox, passwordBox.Password.Length);
            }
        }
    }
}

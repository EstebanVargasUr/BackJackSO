using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cliente.Conexion;

namespace Cliente.Pages
{
    /// <summary>
    /// Lógica de interacción para Verificacion.xaml
    /// </summary>
    public partial class Verificacion : Page
    {
        Comunicacion cm = new Comunicacion();
        public Verificacion()
        {
            
            InitializeComponent();
        }

        private void lbIp_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void lbPuerto_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            Object[] enviar = {lbusuario.Text, lbContrasena.Text};
            cm.SendRequest("login",enviar);

            if(cm.ReceiveResponse().operacion == "actualizar")
            {
                VariablesStaticas.nombreUsuario = lbusuario.Text;
                NavigationService.Navigate(new MesaJuego());
            }
            else
            {
                MessageBox.Show("Error, usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            //NavigationService.Navigate(new MesaJuego());
            
        }

        private void btnRegistrarse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Registro());
        }
    }
}

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

namespace Cliente.Pages
{
    /// <summary>
    /// Lógica de interacción para Verificacion.xaml
    /// </summary>
    public partial class Verificacion : Page
    {
        Inicio ini = new Inicio();
        Cliente cl = new Cliente();
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
            string env = "Usuario: " + lbusuario.Text + " Contraseña " + lbContrasena.Text;
          cl.SendRequest(env);
        }

        private void btnRegistrarse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}

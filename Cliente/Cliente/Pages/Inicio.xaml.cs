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
using System.Net.Sockets;
using System.Net;


using Cliente.Pages;

namespace Cliente.Pages
{
    /// <summary>
    /// Lógica de interacción para Inicio.xaml
    /// </summary>
    public partial class Inicio : Page
    {

        private const int PORT = 100;

       Cliente cl = new Cliente();

        public Inicio()
        {
          
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (cl.ConnectToServer(lbIp.Text, 100))
            {
                NavigationService.Navigate(new Verificacion());
                //cl.SendRequest("");
            }
            else
            {
                MessageBox.Show("Error en la conexion con el servidor", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lbIp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void lbPuerto_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

      
        
    }

}

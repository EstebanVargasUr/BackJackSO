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
using Cliente.Conexion;

using Cliente.Pages;


namespace Cliente.Pages
{
    /// <summary>
    /// Lógica de interacción para Inicio.xaml
    /// </summary>
    public partial class Inicio : Page
    {

        Comunicacion cm = new Comunicacion();

        public Inicio()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          
            if (cm.ConnectToServer(lbIp.Text))
            {

                NavigationService.Navigate(new Verificacion());
            }
            else
            {
                MessageBox.Show("Error en la conexion con el servidor", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            
            //NavigationService.Navigate(new Verificacion());
        }

        private void lbIp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void lbPuerto_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

      
        
    }

}

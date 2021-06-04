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
    /// Lógica de interacción para Registro.xaml
    /// </summary>
    public partial class Registro : Page
    {
        Comunicacion cm = new Comunicacion();
        public Registro()
        {
            InitializeComponent();
        }

        private void lbIp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void lbPuerto_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Verificacion());
        }

        private void registrar(object sender, RoutedEventArgs e)
        {
            
            Object[] enviar = { lbusuarioR.Text, lbContrasenaR.Text};
            cm.enviarPeticion("registrarse", enviar);


            if (cm.resivirRespuesta().operacion == "registrado")
            {
                MessageBox.Show("Usuario registrado con exito", "Usuario Registrado", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                MessageBox.Show("Hubo un error al registrar el usuario, ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

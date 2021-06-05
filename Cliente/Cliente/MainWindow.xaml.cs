using Cliente.Conexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliente
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Comunicacion cm = new Comunicacion();
        public MainWindow()
        {
            this.Closed += new EventHandler(MainWindow_Closed);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (VariablesStaticas.nombreUsuario != null || VariablesStaticas.nombreUsuario != "")
            {
                object[] sed = { VariablesStaticas.nombreUsuario };
                cm.enviarPeticion("salir", sed);
            }
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //connect.Visibility = Visibility.Hidden;
            
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}

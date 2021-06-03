using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
    /// Lógica de interacción para MesaJuego.xaml
    /// </summary>
    public partial class MesaJuego : Page
    {
        public static int dineroActual = 0;
        int apuesta = 0;
        bool init = false;
        Comunicacion cm = new Comunicacion();

        public MesaJuego()
        {
         
            InitializeComponent();
            lbApuesta.Text = apuesta.ToString() + "$";
            txtDinero.Content = dineroActual.ToString() + "$";

            /* object[] sed = { "pipo" };
             cl.SendRequest("pedir carta", sed);*/
            txtDinero.Content = VariablesStaticas.transferencia.jugadores[1].saldo.ToString();
            Thread th1 = new Thread(new ThreadStart(recibirDatos));
            th1.Start();


            /*
            Image im = new Image();
            im.Width = 100;
            im.Height = 110;
            im.Source = new BitmapImage(new Uri("pack://application:,,,/Cliente;component/resources/Cartas/101.bmp", UriKind.RelativeOrAbsolute));
            Canvas.SetLeft(im, 0);
            Canvas.SetTop(im, 0);
            //cvCartas.Children.Add(im);

            Image im2 = new Image();
            im2.Width = 100;
            im2.Height = 110;
            im2.Source = new BitmapImage(new Uri("pack://application:,,,/Cliente;component/resources/Cartas/102.bmp", UriKind.RelativeOrAbsolute));
            Canvas.SetLeft(im2, 30);
            Canvas.SetTop(im2, 0);
            //cvCartas.Children.Add(im2);
            */
        }

       
        private void recibirDatos()
        {

            while (true)
            {
                if (cm.ReceiveResponse() != null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        //SETEAR TODO LO GRAFICO AQUI
                        txtDinero.Content = VariablesStaticas.transferencia.jugadores[1].saldo.ToString();
                        
                    }));
                }
            }
        }

        private void btnApostar_Click(object sender, RoutedEventArgs e)
        {
            if(apuesta > 0)
            {
                frameApuesta.Visibility = Visibility.Hidden;
                btnApostar.Visibility = Visibility.Hidden;
                init = true;
            }
            else
            {
                MessageBox.Show("Se necesita apostar mas dinero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void btn25_Click(object sender, RoutedEventArgs e)
        {
            if (!init)
            {
                dineroActual -= 25;
                apuesta += 25;
                lbApuesta.Text = apuesta.ToString() + "$";
                txtDinero.Content = dineroActual.ToString() + "$";
            }
            
        }

        private void btn50_Click_1(object sender, RoutedEventArgs e)
        {
            if (!init)
            {
                dineroActual -= 50;
                apuesta += 50;
                lbApuesta.Text = apuesta.ToString() + "$";
                txtDinero.Content = dineroActual.ToString() + "$";
            }
                
        }

        private void btn100_Click_1(object sender, RoutedEventArgs e)
        {
            if (!init)
            {
                dineroActual -= 100;
                apuesta += 100;
                lbApuesta.Text = apuesta.ToString() + "$";
                txtDinero.Content = dineroActual.ToString() + "$";
            }
                
        }

        private void btn200_Click_1(object sender, RoutedEventArgs e)
        {
            if (!init)
            {
                dineroActual -= 200;
                apuesta += 200;
                lbApuesta.Text = apuesta.ToString() + "$";
                txtDinero.Content = dineroActual.ToString() + "$";
            }    
        }

        private void btnPedir_Click(object sender, RoutedEventArgs e)
        {
            //SOLO REALIZAR ACCION EN TURNO DEL JUGADOR
            if(VariablesStaticas.transferencia.juego.turnoJugador == VariablesStaticas.nombreUsuario)
            {
                object[] sed = { VariablesStaticas.nombreUsuario };
                cm.SendRequest("pedir carta", sed);
            }
            else
            {
                MessageBox.Show("No puede realizar esta acción porque no es su turno", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            
            /*
            Image im = new Image();
            im.Width = 100;
            im.Height = 110;
            im.Source = new BitmapImage(new Uri("pack://application:,,,/Cliente;component/resources/Cartas/"+ cl.ReceiveResponse().juego.cartas[0].valor+ cl.ReceiveResponse().juego.cartas[0].tipo+".bmp", UriKind.RelativeOrAbsolute));
            Canvas.SetLeft(im, 0);
            Canvas.SetTop(im, 0);
            cvCartas.Children.Add(im);
            */
        }

        private void btnPlantarse_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

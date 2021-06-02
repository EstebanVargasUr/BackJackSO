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
        Cliente cl = new Cliente();
        Jugadores prin = new Jugadores();
        int us = 0;
        private List<Jugadores> cartas = new List<Jugadores>();
        public MesaJuego()
        {
            prin.usuario = "pipo";
            InitializeComponent();
            lbApuesta.Text = apuesta.ToString() + "$";
            txtDinero.Content = dineroActual.ToString() + "$";

            object[] sed = { "pipo" };
            cl.SendRequest("pedir carta", sed);

            Thread th1 = new Thread(new ThreadStart(recibirDatos));
            th1.Start();

            txtDinero.Content = dineroActual.ToString() + "$";
            // dineroActual = cl.tr1.jugadores[1].saldo;


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
                if (cl.ReceiveResponse() != null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        txtDinero.Content = cl.ReceiveResponse().juego.cartas.Count.ToString();
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



            object[] sed= { "pipo"};
            cl.SendRequest("pedir carta", sed);


            txtDinero.Content = dineroActual.ToString() + "$";
            //int tamanio = cl.ReceiveResponse().jugadores[0].cartas.Count;
            //tamanio--;

            //int valor = cl.ReceiveResponse()
            //lbAux.Content = valor.ToString();
            //rtas.Count;


            //cartas.Add(cli.ReceiveResponse().jugadores[0]);
            //int tipo = cartas[0].cartas[0].tipo;
            //int tipo = cli.ReceiveResponse().jugadores[0].cartas[0].tipo;
            //lbAux.Content = tipo.ToString();
            //us++;
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


        public static void Actualizar()
        {
            //cl.ReceiveResponse();

        }

        private void btnPlantarse_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controlador
{
    class Jugadores
    {
        public string usuario { get; set; }
        public int saldo { get; set; }
        public int apuesta { get; set; }
        public List<Cartas> cartas { get; set; }

        public Jugadores()
        {

        }

        public Jugadores(string _usuario)
        {
            usuario = _usuario;
            saldo = 1000;
            apuesta = 0;
        }
    }
    class Juego
    {
        public List<Cartas> cartas { get; set; }
        public int turnoJugador { get; set; }

        
        public Juego()
        {
            cartas = new List<Cartas>();
            turnoJugador = 1;
        }

        public void CrearCartas()
        {
            for ( int i = 1; i < 5; i++ )
            {
                for (int j = 1; j < 14; j++)
                {
                    if ( j > 10 )
                    {
                        Cartas c1 = new Cartas(j, i, 10);
                        cartas.Add(c1);
                    }
                    else
                    {
                        Cartas c1 = new Cartas(j, i, j);
                        cartas.Add(c1);
                    }                  
                }
            }

            cartas = Shuffle(cartas);

            /*foreach (Cartas x in cartas)
            {
                Console.WriteLine(x.valor.ToString() + " | " + x.tipo.ToString() + " | " + x.caracter.ToString());
            }
            Console.WriteLine(cartas.Count);*/
        }

        public static List<T> Shuffle<T>(List<T> input)
        {
            List<T> arr = input;
            List<T> arrDes = new List<T>();

            Random randNum = new Random();
            while (arr.Count > 0)
            {
                int val = randNum.Next(0, arr.Count - 1);
                arrDes.Add(arr[val]);
                arr.RemoveAt(val);
            }

            return arrDes;
        }

        public string VerificarGanadores(Jugadores jugador , Jugadores crupier)
        {
            string Ganador = "";
            int puntuacionCrupier = 0;
            int puntuacionJugador = 0;

            foreach ( Cartas x in crupier.cartas)
            {
                puntuacionCrupier += x.valor;
            }
            foreach (Cartas x in jugador.cartas)
            {
                puntuacionJugador += x.valor;
            }

            if (puntuacionCrupier > puntuacionJugador)
            {
                Ganador = "Crupier";
            }
            else if (puntuacionCrupier < puntuacionJugador)
            {
                Ganador = "Jugador";
            }
            else if (puntuacionCrupier == puntuacionJugador)
            {
                Ganador = "Empate";
            }

            if (puntuacionCrupier > 21)
            {
                Ganador = "Jugador";
            }
            if (puntuacionJugador > 21)
            {
                Ganador = "Crupier";
            }

            return Ganador;
        }
    }
    class Cartas
    {
        public int caracter { get; set; } // 1 = A ... K = 13
        public int tipo { get; set; } // 1 = Corazones , 2 = Picas , 3 = Diamantes , 4 = Trebol
        public int valor { get; set; } 

        public Cartas(int _caracter, int _tipo, int _valor)
        {
            caracter = _caracter;
            tipo = _tipo;
            valor = _valor;
        }
    }

    class Transferencia
    {
        public string operacion;

        public Object[] datos;

        public List<Jugadores> jugadores;

        public Juego juego;

        public Transferencia(string a, Object[] b, List<Jugadores>  c, Juego d)
        {
            operacion = a;
            datos = b;
            jugadores = c;
            juego = d;
        }

    }
}

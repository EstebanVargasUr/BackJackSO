using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Funciones;
using System.Threading;

namespace Controlador
{
    
    class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 8192;
        private const int PORT = 100;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        private static Juego juego = new Juego();
        private static List<Jugadores> ListJugadores = new List<Jugadores>();
        private static List<Jugadores> ListJugadoresEspera = new List<Jugadores>();

        static void Main()
        {
            Console.Title = "Server";

            SetupServer();
            Console.ReadLine(); // When we press enter close everything
            CloseAllSockets();
        }

        private static void pasarTurno(Transferencia deserialized)
        {
            for (int i = 0; i < ListJugadores.Count; i++)
            {
                if (deserialized.datos[0].ToString() == ListJugadores[i].usuario)
                {
                    if (i < ListJugadores.Count-1)
                    {
                        juego.turnoJugador = ListJugadores[i + 1].usuario;
                        updatateAllSockets();
                    }
                    else
                    {
                        juego.turnoJugador = ListJugadores[0].usuario;

                        finRonda();

                        updatateAllSockets();

                        Thread th1 = new Thread(new ThreadStart(reiniciarPartida));
                        th1.Start();
                    }
                    break;
                }
            }
            
        }
        private static void reiniciarPartida()
        {
            Thread.Sleep(6000);

            juego = new Juego();
            juego.CrearCartas();

            foreach (Jugadores x in ListJugadores)
            {
                x.cartas.Clear();
                x.apuesta = 0;
            }
            if (ListJugadores.Count > 1)
            {
                juego.turnoJugador = ListJugadores[1].usuario;
            }
            ListJugadores[0].cartas.Add(juego.cartas[0]);
            juego.cartas.RemoveAt(0);
            ListJugadores[0].cartas.Add(juego.cartas[0]);
            juego.cartas.RemoveAt(0);
            updatateAllSockets();
        }

        private static void finRonda()
        {
            if (ListJugadores.Count > 1)
            {
                int puntuacionCrupier = 0;

                while (puntuacionCrupier <= 16)
                {
                    puntuacionCrupier = 0;

                    foreach (Cartas x in ListJugadores[0].cartas)
                    {
                        puntuacionCrupier += x.valor;
                    }

                    if (puntuacionCrupier <= 16)
                    {
                        ListJugadores[0].cartas.Add(juego.cartas[0]);
                        juego.cartas.RemoveAt(0);
                    }
                }

                for (int i = 1; i < ListJugadores.Count; i++)
                {
                    string ganador = juego.VerificarGanadores(ListJugadores[i], ListJugadores[0]);

                    if (ganador == "Jugador")
                    {
                        ListJugadores[i].saldo += ListJugadores[i].apuesta * 2;
                    }
                    else if(ganador == "Empate")
                    {
                        ListJugadores[i].saldo += ListJugadores[i].apuesta;
                    }
                }
            }

        }
        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server running...");

            juego.CrearCartas();

            //Se agrega el crupier a la lista de jugadores
            Jugadores jug = new Jugadores("crupier");
            ListJugadores.Add(jug);
        }

        /// <summary>
        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients).
        /// </summary>
        private static void CloseAllSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            serverSocket.Close();
        }

        private static void updatateAllSockets()
        {
            Transferencia tr = new Transferencia("actualizar", null, ListJugadores, juego);
            byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tr));

            foreach (Socket socket in clientSockets)
            {
                socket.Send(data);
            }
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            Thread thplayers = new Thread(new ParameterizedThreadStart(loadThreadPlayer));
            thplayers.Start(socket);

        }

        private static void loadThreadPlayer(object obj)
        {
            Socket socket = (Socket)obj;
            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected, waiting for request...");
            serverSocket.BeginAccept(AcceptCallback, null);
        }
        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                clientSockets.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Transferencia deserialized = JsonConvert.DeserializeObject<Transferencia>(text);


            if (deserialized.operacion == "pedir carta")
            {
                bool band = false;
                foreach (Jugadores x in ListJugadores)
                {
                    if (deserialized.datos[0].ToString() == x.usuario)
                    {
                        x.cartas.Add(juego.cartas[0]);
                        Console.WriteLine(juego.cartas[0].caracter.ToString() + juego.cartas[0].tipo.ToString());
                        juego.cartas.RemoveAt(0);

                        int puntuacionJugador = 0;

                        foreach (Cartas y in x.cartas)
                        {
                            puntuacionJugador += y.valor;
                        }

                        if (puntuacionJugador >= 21)
                        {
                            pasarTurno(deserialized);
                            band = true;
                        }
                     
                        break;
                    }
                }

                if (!band)
                {
                    updatateAllSockets();
                }
                
            }

            else if (deserialized.operacion == "pasar turno")
            {
                pasarTurno(deserialized);
            }

            else if (deserialized.operacion == "apuesta")
            {
                foreach (Jugadores x in ListJugadores)
                {
                    if (deserialized.datos[0].ToString() == x.usuario)
                    {
                        x.apuesta += int.Parse(deserialized.datos[1].ToString());
                        x.saldo -= int.Parse(deserialized.datos[1].ToString());
                        x.cartas.Add(juego.cartas[0]);
                        juego.cartas.RemoveAt(0);
                        x.cartas.Add(juego.cartas[0]);
                        juego.cartas.RemoveAt(0);
                    }
                }

                updatateAllSockets();
            }

            else if (deserialized.operacion == "registrarse")
            {
                Funciones.UsuariosFunciones.register(deserialized.datos[0].ToString(), deserialized.datos[1].ToString());

                if (Funciones.UsuariosFunciones.auth(deserialized.datos[0].ToString(), deserialized.datos[1].ToString()))
                {
                    Transferencia tr = new Transferencia("registrado", null, null, null);
                    byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tr));
                    current.Send(data);
                }
                else
                {
                    Transferencia tr = new Transferencia("no registrado", null, null, null);
                    byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tr));
                    current.Send(data);
                }
            }

            else if (deserialized.operacion == "login")
            {
                Console.WriteLine(deserialized.datos[0].ToString());
                if (Funciones.UsuariosFunciones.auth(deserialized.datos[0].ToString(), deserialized.datos[1].ToString()))
                {
                    if (ListJugadores.Count == 1)
                    {
                        ListJugadores[0].cartas.Add(juego.cartas[0]);
                        juego.cartas.RemoveAt(0);
                        ListJugadores[0].cartas.Add(juego.cartas[0]);
                        juego.cartas.RemoveAt(0);

                        juego.turnoJugador = deserialized.datos[0].ToString();

                        //INICIA RONDA
                    }

                    if (ListJugadores.Count < 8)
                    {
                        bool espera = false;
                        Jugadores jugadorEspera = new Jugadores();

                        foreach (Jugadores x in ListJugadoresEspera)
                        {
                            if (deserialized.datos[0].ToString() == x.usuario)
                            {
                                espera = true;
                                jugadorEspera = x;
                                break;
                            }
                        }

                        if (espera == false)
                        {
                            Jugadores jug = new Jugadores(deserialized.datos[0].ToString());
                            ListJugadores.Add(jug);
                            updatateAllSockets();
                        }
                        else if (espera == true && ListJugadoresEspera[0].usuario == jugadorEspera.usuario)
                        {
                            ListJugadores.Add(jugadorEspera);
                            ListJugadoresEspera.RemoveAt(0);
                            Console.WriteLine("EN cola en turno");
                            updatateAllSockets();
                        }
                        else
                        {
                            Transferencia tr = new Transferencia("en cola", null, null, null);
                            byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tr));
                            current.Send(data);
                            Console.WriteLine("EN cola sin turno");
                        }
                    }
                    else
                    {
                        bool jugadorEspera = false;
                        foreach (Jugadores x in ListJugadoresEspera)
                        {
                            if (deserialized.datos[0].ToString() == x.usuario)
                            {
                                jugadorEspera = true;
                                break;
                            }
                        }

                        if (jugadorEspera == false)
                        {
                            Jugadores jug = new Jugadores(deserialized.datos[0].ToString());
                            ListJugadoresEspera.Add(jug);
                        }
                        
                        Transferencia tr = new Transferencia("en cola", null, null, null);
                        byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tr));
                        current.Send(data);
                    }
                }
                else
                {
                    Transferencia tr = new Transferencia("rechazado",null, null, null);
                    byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tr));
                    current.Send(data);
                    Console.WriteLine("Fue rechazado");
                }
                
            }

            else if (deserialized.operacion == "salir") // Client wants to exit gracefully
            {
                // Always Shutdown before closing
                bool encontrado = false;

                foreach (Jugadores x in ListJugadores)
                {
                    if (deserialized.datos[0].ToString() == x.usuario)
                    {
                        if(juego.turnoJugador == x.usuario)
                        {
                            for (int i = 0; i < ListJugadores.Count; i++)
                            {
                                if (deserialized.datos[0].ToString() == ListJugadores[i].usuario)
                                {
                                    if (i < ListJugadores.Count - 1)
                                    {
                                        juego.turnoJugador = ListJugadores[i + 1].usuario;
                                    }
                                    else
                                    {
                                        juego.turnoJugador = ListJugadores[0].usuario;

                                        finRonda();

                                        Thread th1 = new Thread(new ThreadStart(reiniciarPartida));
                                        th1.Start();
                                    }
                                    break;
                                }
                            }
                        }
                        
                        ListJugadores.Remove(x);
                        encontrado = true;
                        break;
                    }
                }

                if(encontrado == false)
                {
                    foreach (Jugadores x in ListJugadoresEspera)
                    {
                        if (deserialized.datos[0].ToString() == x.usuario)
                        {
                            ListJugadoresEspera.Remove(x);
                            break;
                        }
                    }
                }
                
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clientSockets.Remove(current);
                Console.WriteLine("Client disconnected");

                updatateAllSockets();

                return;
            }
            else
            {
                Console.WriteLine("Text is an invalid request");
                Transferencia tr = new Transferencia("Operacion invalida", null, null, null);
                byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tr));
                current.Send(data);
                
                Console.WriteLine("Warning Sent");
            }

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
    }
}

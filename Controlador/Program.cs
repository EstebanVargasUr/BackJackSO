using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Funciones;

namespace Controlador
{
    
    class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 100;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        private static Juego juego = new Juego();
        private static List<Jugadores> ListJugadores = new List<Jugadores>();

        static void Main()
        {
            Console.Title = "Server";

            SetupServer();
            Console.ReadLine(); // When we press enter close everything
            CloseAllSockets();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server running...");
            juego.CrearCartas();
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

            /*if(clientSockets.Count < 7)
            {
                clientSockets.Add(socket);
            }
            */

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


            if (deserialized.operacion == "pedir cartas inicial")
            {
                /*EN PROCESO*/
                foreach (Jugadores x in ListJugadores)
                {
                    if(deserialized.datos[0].ToString() == x.usuario)
                    {
                        x.cartas.Add(juego.cartas[0]);
                        juego.cartas.RemoveAt(0);
                        x.cartas.Add(juego.cartas[0]);
                        juego.cartas.RemoveAt(0);
                    }
                }
                
            }

            else if (deserialized.operacion == "pedir carta")
            {
                /*EN PROCESO*/
                foreach (Jugadores x in ListJugadores)
                {
                    if (deserialized.datos[0].ToString() == x.usuario)
                    {
                        x.cartas.Add(juego.cartas[0]);
                        juego.cartas.RemoveAt(0);
                    }
                }

                updatateAllSockets();
            }

            else if (deserialized.operacion == "pasar turno")
            {
                /*EN PROCESO*/
                for (int i = 0; i < ListJugadores.Count; i++)
                {
                    if (deserialized.datos[0].ToString() == ListJugadores[i].usuario)
                    {
                        if (i < ListJugadores.Count)
                        {
                            juego.turnoJugador = ListJugadores[i + 1].usuario;
                        }
                        else
                        {
                            juego.turnoJugador = ListJugadores[0].usuario;
                        }
                        break;
                    }
                }
                updatateAllSockets();
            }

            else if (deserialized.operacion == "apuesta")
            {
                /*EN PROCESO*/
                foreach (Jugadores x in ListJugadores)
                {
                    if (deserialized.datos[0].ToString() == x.usuario)
                    {
                        x.apuesta = (int)deserialized.datos[1];
                    }
                }
                Transferencia tr = new Transferencia("actualizar", null, ListJugadores, juego);
                byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tr));
                current.Send(data);
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
                /*EN PROCESO*/
                Console.WriteLine(deserialized.datos[0].ToString()+" "+deserialized.datos[1].ToString());
                if(Funciones.UsuariosFunciones.auth(deserialized.datos[0].ToString(), deserialized.datos[1].ToString()))
                {
                    Jugadores jug = new Jugadores(deserialized.datos[0].ToString());
                    ListJugadores.Add(jug);

                    updatateAllSockets();
                }
                else
                {
                    Transferencia tr = new Transferencia("rechazado",null, null, null);
                    byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(tr));
                    current.Send(data);
                }
            }

            else if (deserialized.operacion == "salir") // Client wants to exit gracefully
            {
                /*EN PROCESO*/
                // Always Shutdown before closing
                foreach (Jugadores x in ListJugadores)
                {
                    if (deserialized.datos[0].ToString() == x.usuario)
                    {
                        ListJugadores.Remove(x);
                    }
                }

                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clientSockets.Remove(current);
                Console.WriteLine("Client disconnected");
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

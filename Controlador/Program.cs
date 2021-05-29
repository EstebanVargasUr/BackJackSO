﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Controlador
{
    
    class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 100;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        private static Jugadores jugadores = new Jugadores();
        private static Juego juego = new Juego();
        private static List<Jugadores> ListJugadores = new List<Jugadores>();

        static void Main()
        {
            Console.Title = "Server";
            FuncionesDll.auth("esteban", "Salchicha7");
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
            byte[] data = Encoding.ASCII.GetBytes("Actualizado");
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

            Jugadores jug = new Jugadores();
            ListJugadores.Add(jug);

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


            Console.WriteLine("Received Text: " + deserialized.datos[0]);

            if (text.ToLower() == "get time") // Client requested time
            {
                Console.WriteLine("Text is a get time request");
                byte[] data = Encoding.ASCII.GetBytes(DateTime.Now.ToLongTimeString());
                current.Send(data);
                Console.WriteLine("Time sent to client");

            }

            else if (deserialized.datos[0].ToString() == "pedir cartas inicial")
            {
                jugadores.cartas.Add(juego.cartas[0]);
                juego.cartas.RemoveAt(0);
                jugadores.cartas.Add(juego.cartas[0]);
                juego.cartas.RemoveAt(0);
            }

            else if (deserialized.datos[0].ToString() == "pedir carta")
            {
                jugadores.cartas.Add(juego.cartas[0]);
                juego.cartas.RemoveAt(0);
            }

            else if (deserialized.datos[0].ToString() == "quedarse")
            {
                /*EN PROCESO*/
            }

            else if (deserialized.datos[0].ToString() == "apuesta")
            {
                /*EN PROCESO*/
            }

            else if (deserialized.datos[0].ToString() == "registrarse")
            {
                /*EN PROCESO*/
            }

            else if (deserialized.datos[0].ToString() == "login")
            {
                /*EN PROCESO*/
            }

            else if (text.ToLower() == "exit") // Client wants to exit gracefully
            {
                // Always Shutdown before closing
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clientSockets.Remove(current);
                Console.WriteLine("Client disconnected");
                return;
            }
            else
            {
                Console.WriteLine("Text is an invalid request");
                byte[] data = Encoding.ASCII.GetBytes("Invalid request");
                current.Send(data);
                updatateAllSockets();
                Console.WriteLine("Warning Sent");
            }

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
    }
}

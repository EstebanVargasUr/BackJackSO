﻿using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.ComponentModel;
using System.Windows;
using Newtonsoft.Json;
using System.Threading;
using System.IO;

namespace Cliente.Conexion
{

    class Comunicacion
    {
        public static readonly Socket ClientSocket = new Socket
           (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 100;

        public bool ConnectToServer(string ip)
        {
            int attempts = 0;
            bool pb = false;

                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    ClientSocket.Connect(ip, PORT);
                    pb = true;

                }
                catch (SocketException)
                {
                pb = false;
                }

            return pb;
        }

        /// <summary>
        /// Close socket and exit program.
        /// </summary>
        public static void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }

        public void SendRequest(string accion,object[] datos)
        {
            //Console.Write("Send a request: ");
            //string request = Console.ReadLine();
            //string request = Console.ReadLine();
            Transferencia a = new Transferencia(accion, datos,null,null);
            string serialized = JsonConvert.SerializeObject(a);
            SendString(serialized);
        }

        /// <summary>
        /// Sends a string to the server with ASCII encoding.
        /// </summary>
        public static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public Transferencia ReceiveResponse()
        {

            try
            {
                var buffer = new byte[8192];
                int received = ClientSocket.Receive(buffer, SocketFlags.None);
                if (received == 0)
                {
                    return null;
                }
                var data = new byte[received];
                Array.Copy(buffer, data, received);
                string text = Encoding.ASCII.GetString(data);
                VariablesStaticas.transferencia = JsonConvert.DeserializeObject<Transferencia>(text);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"The file was not found: '{e}'");
            }

            return VariablesStaticas.transferencia;
        }

   
    }
}

using System;
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
        public static readonly Socket clienteSocket = new Socket
           (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 100;

        public bool conectarServer(string ip)
        {

            int intentos = 0;
            bool pb = false;

                try
                {
                    intentos++;
                    Console.WriteLine("Conexion establecida" + intentos);
                    clienteSocket.Connect(ip, PORT);
                    pb = true;

                }
                catch (SocketException)
                {
                pb = false;
                }

            return pb;
        }


        public static void salir()
        {
            enviarString("exit"); 
            clienteSocket.Shutdown(SocketShutdown.Both);
            clienteSocket.Close();
            Environment.Exit(0);
        }

        public void enviarPeticion(string accion,object[] datos)
        {
            Transferencia a = new Transferencia(accion, datos,null,null);
            string serialized = JsonConvert.SerializeObject(a);
            enviarString(serialized);
        }

        public static void enviarString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            clienteSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public Transferencia resivirRespuesta()
        {

            try
            {
                var buffer = new byte[8192];
                int received = clienteSocket.Receive(buffer, SocketFlags.None);
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
                Console.WriteLine($"El archivo no se encontro: '{e}'");
            }

            return VariablesStaticas.transferencia;
        }

   
    }
}

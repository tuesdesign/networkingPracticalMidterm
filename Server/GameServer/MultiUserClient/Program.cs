using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace MultiUserClient
{
    class Program
    {
        private static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] buffer = new byte[1024];
        private static byte[] SendBuffer = new byte[1024];

        static void Main(string[] args)
        {
            client.Connect(IPAddress.Parse("127.0.0.1"),8888);
            Console.WriteLine("Conencted to Server");

            client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(RecieveCallback), client);
            Send();

        }

        private static void Send()
        {
            int c = 0;
            while (true)
            {
                c++;
                SendBuffer = Encoding.ASCII.GetBytes(c.ToString());
                client.Send(SendBuffer);
                Thread.Sleep(1000);
            }
        }

        private static void RecieveCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            int rec = socket.EndReceive(result);
            byte[] data = new byte[rec];
            Array.Copy(buffer, data, rec);
            string msg = Encoding.ASCII.GetString(data);
            Console.WriteLine("Recieved: " + msg);
            socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(RecieveCallback), buffer);
        }
    }
}

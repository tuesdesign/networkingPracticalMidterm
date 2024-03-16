using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    

    class Program
    {
        static int TCP_Port = 8888;
        static int UDP_Port = 8889;
        static IPAddress ip = IPAddress.Parse("127.0.0.1");

        static void Main(string[] args)
        {
            StartTCP();
            StartUDP();
        }

        public static void StartTCP()
        {
            byte[] buffer = new byte[1024];
        }
        public static void StartUDP()
        {
            byte[] buffer = new byte[1024];
            
            UdpClient udpClient = new UdpClient(UDP_Port);
            while (true)
            {
                // Receive data
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                byte[] receivedBytes = result.Buffer;

                // Send data back
                await udpClient.SendAsync(receivedBytes, receivedBytes.Length, result.RemoteEndPoint);
            }
        }
    }
}

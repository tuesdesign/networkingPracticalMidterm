using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/*
 *  recv updates 
 *  create message with all updates
 *  server send mesg to all connected clients
 * 
 *  keep track of all clients
 *  update every 50ms
 * 
 *  When a client connects
 *      - add client to list
 *      - maybe thread process recv packets
 *  
 *  Server sends updates to all clients in list
 *  iterate and send update to one client at a time
 * 
 *  maybe mutex for shared buffer
 */


namespace MultiUser
{
    class Program
    {
        private static byte[] buffer = new byte[1024];
        private static byte[] sendBuffer = new byte[1024];
        private static Socket server;
        private static string sendMsg = "";

        // Client List
        private static List<Socket> clientSockets = new List<Socket>();


        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888)); // because tcp
            server.Listen(10);

            server.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Thread sendThread = new Thread(new ThreadStart(SendLoop));
            sendThread.Start();

            Console.ReadLine();
        }

        private static void AcceptCallback(IAsyncResult result)
        {
            Socket socket = server.EndAccept(result);
            Console.WriteLine("Client Connected!");

            clientSockets.Add(socket);

            socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(RecieveCallback), socket);

            server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void RecieveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int rec = socket.EndReceive(ar);
            byte[] data = new byte[rec];
            Array.Copy(buffer, data, rec);
            string msg = Encoding.ASCII.GetString(data);
            Console.WriteLine("Recv'd: " + msg);

            // build the message to be sent to all clients
            sendMsg += " " + msg; // replace

            socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(RecieveCallback), socket);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        private static void SendLoop()
        {
            while (true)
            {
                sendBuffer = Encoding.ASCII.GetBytes(sendMsg);
                //send to all clients

                foreach (var socket in clientSockets)
                {
                    Console.WriteLine("Sending to: " + socket.RemoteEndPoint.ToString());
                    socket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SendCallback), socket);
                }

                sendMsg = "";
                Thread.Sleep(1000);
            }
        }

    }
}
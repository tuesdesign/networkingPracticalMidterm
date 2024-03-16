using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NetMidtermServer
{

    class TCPServer
    {
        private TcpListener _listener;
        
        // replace this with an array later
        private TcpClient client1;
        private TcpClient client2;

        public TCPServer(IPAddress ip,  int port)
        {
            _listener = new TcpListener(ip, port);
        }

        public void StartTCP()
        {
            _listener.Start();
            Console.WriteLine("Server Started!");

            // this sucks :/ fix it later

            client1 = _listener.AcceptTcpClient();
            Console.WriteLine("First Client Connected!");

            client2 = _listener.AcceptTcpClient();
            Console.WriteLine("Second Client Connected!");


            SendMsg(client1, "---CONNECTION OK BEGIN UDP---");
            SendMsg(client2, "---CONNECTION OK BEGIN UDP---");

            // let's do threads instead of async for sanity and readability
            Thread threadClient1 = new Thread(() => ClientHandling(client1));
            threadClient1.Start();

            Thread threadClient2 = new Thread(() => ClientHandling(client2));
            threadClient2.Start();
        }

        private void ClientHandling(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytes;

            try
            {
                while ((bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string msg = Encoding.ASCII.GetString(buffer, 0, bytes);
                    Console.WriteLine("Recv'd: " + msg + " from " + client.Client.RemoteEndPoint.ToString());
                    // this is cursed, I should do a list :/
                    if(client == client1)
                    {
                        SendMsg(client2, msg);
                    } else if (client == client2) 
                    {
                        SendMsg(client1, msg);
                    }

                    if (msg == "shutdown")
                    {
                        Console.WriteLine("Users Disconnected. Shutting Down Server.");
                        Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally { client.Close(); }
        }

        private void SendMsg(TcpClient client, string msg)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            stream.Write(buffer, 0, buffer.Length);
            Console.WriteLine($"Relayed: {msg} to {client.Client.RemoteEndPoint.ToString()}");
        }

        public void Shutdown()
        {
            Console.WriteLine("Closing Server!");
            
            _listener.Stop();
            
            client1.Close();
            client2.Close();

            Console.WriteLine("Server Stopped Successfully!");
        }
    }

    class UDPServer
    {
        private UdpClient _client;
        private IPEndPoint client1_EP;
        private IPEndPoint client2_EP;

        public UDPServer(int port)
        {
            _client = new UdpClient(port);
        }

        public void StartUDP() 
        {
            Console.WriteLine("Starting UDP!");
            client1_EP = null;
            client2_EP = null;
            while(client1_EP == null || client2_EP == null)
            {
                IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _client.Receive(ref clientEP);
                
                // somehow even more scuffed than the TCP one
                if(client1_EP == null)
                {
                    client1_EP = clientEP;
                    Console.WriteLine("Client 1 Connected Over UDP");
                }
                else if (client2_EP == null && !clientEP.Equals(client1_EP)) 
                {
                    client2_EP = clientEP;
                    Console.WriteLine("Client 2 Connected Over UDP");
                }

            }

            Thread thread1 = new Thread(() => ClientHandling(client1_EP, client2_EP));
            thread1.Start();
            Thread thread2 = new Thread(() => ClientHandling(client2_EP, client1_EP));
            thread2.Start();
        }

        private void ClientHandling(IPEndPoint sender,  IPEndPoint receiver)
        {
            while(true) // I'm sure this here infinite loop won't cause any problems!
            {
                byte[] data = _client.Receive(ref sender);
                Console.WriteLine($"Recv'd data from {sender}: {Encoding.ASCII.GetString(data)}");

                if(sender.Equals(client1_EP) )
                {
                    _client.Send(data, data.Length, client2_EP);
                }
                else if (sender.Equals(client2_EP))
                {
                    _client.Send(data, data.Length, client1_EP);
                }
                Console.WriteLine($"Data relayed to {receiver}: {Encoding.ASCII.GetString(data)}");
            }
        }

        public void Shutdown()
        {
            _client.Close();
            Console.WriteLine("Shut Down UDP Server!");
        }

    }


    class Program
    {
        static int TCP_Port = 8888;
        static int UDP_Port = 8889;
        static IPAddress ip = IPAddress.Parse("127.0.0.1");

        

        static void Main(string[] args)
        {
            TCPServer tcp = StartTCP();
            UDPServer udp = StartUDP();
            Console.ReadLine();
            tcp.Shutdown();
            udp.Shutdown();
        }

        public static TCPServer StartTCP()
        {
            TCPServer tcp = new TCPServer(ip, TCP_Port);
            tcp.StartTCP();
            return tcp;
            
        }
        public static UDPServer StartUDP()
        {
            UDPServer udp = new UDPServer(UDP_Port);
            udp.StartUDP();
            return udp;
        }
    }
}

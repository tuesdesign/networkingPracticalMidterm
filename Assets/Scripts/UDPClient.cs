using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using TMPro;
using PimDeWitte.UnityMainThreadDispatcher;


public class UDPClient : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public int serverPort = 8889;

    public NetworkingManager networkingManager;

    private UdpClient udp;
    private IPEndPoint remoteEP;

    public bool connect = false;
    public string msg = "";
    public bool send = false;

    public void Connect()
    {
        try
        {
            udp = new UdpClient();
            remoteEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            Debug.Log("Connected to server");
            udp.BeginReceive(ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
    }

    public void SendData(byte[] data)
    {
        try
        {
            udp.Send(data, data.Length, remoteEP);
            Debug.Log("Sent data: " + data.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udp.EndReceive(ar, ref sender);
            Debug.Log("Received data: " + data.ToString());
            
            try
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => networkingManager.HandleData(data) );
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }


            udp.BeginReceive(ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
    }

    private void Shutdown()
    {
        SendData(Encoding.ASCII.GetBytes("shutdown"));
        if(udp != null)
        {
            udp.Close();
        }
    }

    private void Start()
    {
        //Connect();
    }

    private void Update()
    {
        if(connect){
            Connect();
            Debug.Log("Connecting to server");
            connect = false;
        }
        if(send)
        {
            SendData(Encoding.ASCII.GetBytes(msg));
            send = false;
        }
    }
}

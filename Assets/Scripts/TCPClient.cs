using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using PimDeWitte.UnityMainThreadDispatcher;

public class TCPClient : MonoBehaviour
{
    // Connect to the server
    // Send establishing connection message
    // Begin a loop of async callbacks that will send and recieve data

    public NetworkingManager NetworkManager;

    public string serverIP = "127.0.0.1";
    public int serverPort = 8888;
    public bool TcpReady = false;

    public string msg;
    public bool send = false;

    private TcpClient client;
    private NetworkStream stream;

    // Start is called before the first frame update
    void Start()
    {
        //establishConnection();
    }

    // Update is called once per frame
    void Update()
    {
        if(send)
        {
            SendData(Encoding.ASCII.GetBytes(msg));
            send = false;
        }
    }

    public void establishConnection()
    {
        // Connect to the server
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            Debug.Log("Connected to server!");
            RecieveData();
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }

        //SendData(Encoding.ASCII.GetBytes("connect"));

    }

    public void SendData(byte[] data)
    {
        try
        {
            stream.Write(data, 0, data.Length);
            Debug.Log("Sent data: " + Encoding.ASCII.GetString(data));
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
    }

    public void Shutdown()
    {
        SendData(Encoding.ASCII.GetBytes("shutdown"));
        if(client != null)
        {
            client.Close();
        }
        Application.Quit();
    }

    private void RecieveData()
    {
        byte[] buffer = new byte[1024];
        stream.BeginRead(buffer, 0, buffer.Length, OnDataRecieved, buffer);
    }
    private void OnDataRecieved(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);
            if(bytesRead > 0)
            {
                byte[] buffer = (byte[])ar.AsyncState;
                string recievedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                if (recievedData == "---CONNECTION OK BEGIN UDP---")
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.Log ( TcpReady = true ));
                } else 
                {
                    UnityMainThreadDispatcher.Instance().Enqueue( () => Debug.Log("Recieved data: " + recievedData) );
                    UnityMainThreadDispatcher.Instance().Enqueue( () => NetworkManager.displayMessage("Other User: "+recievedData) );
                }
                if(recievedData == "shutdown")
                {
                    UnityMainThreadDispatcher.Instance().Enqueue( () => Shutdown() );
                }
                


                // reset buffer
                //Array.Clear(buffer, 0, buffer.Length);

                RecieveData();
            } else
            {
                Debug.Log("Other User or Server ended Session");            
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
    }
}
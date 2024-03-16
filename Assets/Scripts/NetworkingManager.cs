using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;
[System.Serializable]
public class GameData
{
    // This class is used to store the data that will be sent over the network using UDP (ie: everything except chat messages)
    
    //--------------------------------------------------------------------------------Game Data
        [SerializeField] public Vector3 pos; // Stored position, rotation, and scale Data

        [SerializeField] public Color color; // Stored RGBA color data

        [SerializeField] public string name; // Stored name data
    //--------------------------------------------------------------------------------/Game Data

    public byte[] ConvertToByteArray() // serializes a GameData to a byte array
    {
        string json = JsonUtility.ToJson(this);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }
    public void ConvertFromByteArray(byte[] data) // deserializes a GameData to a byte array
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        JsonUtility.FromJsonOverwrite(json, this);
    }
}


public class NetworkingManager : MonoBehaviour
{
    public GameData ThisUser;
    public PlayerOther OtherUser;

    public TCPClient tcpClient;
    public UDPClient udpClient;

    public TMP_Text chatBox;
    public TMP_InputField IPInputField;
    public TMP_InputField chatInputField;

    private bool beginSend = false;
    public int updateRate = 120;

    public byte[] GameDataToBytes(GameData data)
    {
        byte[] buffer = data.ConvertToByteArray();
        Debug.Log($"Converted: encoded {buffer.Length} bytes");
        return buffer;
    }

    public GameData BytesToGameData(byte[] bytes)
    {
        GameData buffer = new GameData();
        buffer.ConvertFromByteArray(bytes);
        Debug.Log($"Converted: decoded - {buffer.name}");
        return buffer;
    }

    public void setIP(string IP)
    {
        
        Debug.Log($"Setting IP to {IP}");
        tcpClient.serverIP = IP;
        udpClient.serverIP = IP;
    }

    public void BeginGame()
    {
        Debug.Log("Starting Networking");
        StartCoroutine(StartNetworking());
    }

    public void PlayerSentMessage(string message)
    {
        // Send message to server
        tcpClient.msg = message;
        tcpClient.send = true;

        if(message == "shutdown")
        {
            tcpClient.Shutdown();
        }

        displayMessage($"<color=blue>{ThisUser.name}</color>: {message}");
    }
    public void displayMessage(string message)
    {
        Debug.Log("Displaying message: " + message);
        chatBox.text += message + "\n";
        if(chatBox.text.Split('\n').Length > 20)
        {
            chatBox.text = chatBox.text.Substring(chatBox.text.IndexOf('\n') + 1);
        }
    }

    public byte[] GetSendData()
    {
        return GameDataToBytes(ThisUser);
    }

    public void updateOtherUser(byte[] data)
    {
        GameData OtherUserGameData = BytesToGameData(data);
        OtherUser.updatePlayerData(OtherUserGameData.pos, OtherUserGameData.color, OtherUserGameData.name);
    }

    public IEnumerator StartNetworking()
    {
        Debug.Log("Starting Networking");
        tcpClient.establishConnection();
        yield return new WaitUntil(() => tcpClient.TcpReady == true);
        Debug.Log("Starting UDP");
        udpClient.Connect();
        udpClient.send = true;
        beginSend = true;
    }

    void Awake()
    {
        //StartCoroutine(StartNetworking());
    }

    public void HandleData(byte[] data)
    {
        try
        {
            updateOtherUser(data);
        }
        catch
        {
            Debug.Log("Error: Recieved UDP packet is not a GameData binary");
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if(beginSend)
        {
            if (Time.frameCount % updateRate == 0)
            {
                udpClient.SendData(GetSendData());
            }
        }        
    }
}

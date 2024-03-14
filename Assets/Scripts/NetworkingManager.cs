using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // This class is used to store the data that will be sent over the network using UDP (ie: everything except chat messages)
    
    //--------------------------------------------------------------------------------Game Data
        [SerializeField] public Transform trans; // Stored position, rotation, and scale Data

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

    void Awake()
    {

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

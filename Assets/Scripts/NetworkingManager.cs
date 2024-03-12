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

    [SerializeField] private GameData test;

    public void tx_gamedata(GameData data)
    {
        byte[] bytes = data.ConvertToByteArray();
        Debug.Log($"Sending {bytes.Length} bytes");
        
        GameData outstest = new GameData();
        outstest.ConvertFromByteArray(bytes);
        Debug.Log($"Received {outstest.name}");
    }

    void Awake()
    {
        if (test == null)
        {
            test = new GameData();
            test.name = "Test Hello This is a test!!";
            // Initialize the properties of test here if necessary
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test send");
        tx_gamedata(test);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

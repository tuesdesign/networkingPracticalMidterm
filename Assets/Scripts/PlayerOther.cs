using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOther : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updatePlayerData(Vector3 pos, Color color, string name)
    {
        Debug.Log("Updating Other Player Data");
        // Update the player's position
        gameObject.transform.position = pos;
        // Update the player's color
        gameObject.GetComponent<Renderer>().material.color = color;
        // Update the player's name
        gameObject.name = name;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

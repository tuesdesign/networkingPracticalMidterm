using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelf : MonoBehaviour
{
    [SerializeField] public GameData playerSelfData;
    public NetworkingManager networkingManager;

    private void Awake()
    {
        if (playerSelfData.color == null)
        {
            playerSelfData.color = Color.white;
        }
        if (playerSelfData.pos == null)
        {
            playerSelfData.pos = gameObject.transform.position;
        }
        if (playerSelfData.name == null)
        {
            playerSelfData.name = gameObject.name;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerSelfData.pos = gameObject.transform.position;
        playerSelfData.color = gameObject.GetComponent<Renderer>().material.color;
        playerSelfData.name = gameObject.name;
        networkingManager.ThisUser = playerSelfData;
    }
}

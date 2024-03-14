using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelf : MonoBehaviour
{
    [SerializeField] public GameData playerSelfData;

    private void Awake()
    {
        if (playerSelfData.color == null)
        {
            playerSelfData.color = Color.white;
        }
        if (playerSelfData.trans == null)
        {
            playerSelfData.trans = gameObject.transform;
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
        
    }
}

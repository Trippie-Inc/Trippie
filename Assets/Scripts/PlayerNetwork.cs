using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetwork : NetworkBehaviour
{
    public GameObject Camera;
    public override void OnStartLocalPlayer()
    {
        GetComponent<Player>().enabled = true;
        Camera.GetComponent<Look>().enabled = true;
        Camera.GetComponent<Camera>().enabled = true;
        Camera.tag = "LookAtCamera";
    }
}

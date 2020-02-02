using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    GameObject Camera;
    void Start()
    {
        Camera = GameObject.FindGameObjectWithTag("LookAtCamera");
    }

    void Update()
    {
        if (Camera != null)
        {
            transform.LookAt(Camera.transform);
        }
    }
}

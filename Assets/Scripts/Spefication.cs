using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spefication : MonoBehaviour
{
    public bool VisibleState;
    public bool TrippieState;
    public bool TrueMaterialFalsePosition;
    public Material Color1;
    public Material Color2;
    public GameObject Looker;
    public float OriginalY;
    public float ChangeY;

    private void Start()
    {
        OriginalY = transform.position.y;
    }
}

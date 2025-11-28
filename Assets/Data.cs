using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    [HideInInspector]
    public string Code;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void CodeUpdate(string code)
    {
        Code = code;
    }
}

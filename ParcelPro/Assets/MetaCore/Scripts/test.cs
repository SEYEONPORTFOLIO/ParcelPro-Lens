using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    http http;
    // Start is called before the first frame update
    void Start()
    {
        http = new http();
        http.Request();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

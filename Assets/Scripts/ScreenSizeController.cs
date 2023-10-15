using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //int screenWidth = Screen.width;
        //int screenHeight = Screen.height;
        //Screen.SetResolution(screenWidth, screenHeight, false);
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

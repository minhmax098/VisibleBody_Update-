using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationManager : MonoBehaviour
{
    public ScreenOrientation screenOrientation;

    void Awake() 
    {
        Screen.orientation = screenOrientation;
    }
}

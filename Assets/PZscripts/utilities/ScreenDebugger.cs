using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenDebugger : MonoBehaviour
{ 
    public string debugger1;
    public string debugger2;
    public string debugger3;
    public string debugger4;
    public string debugger5;

    public void OnGUI()
    {
        GUI.Box(new Rect(20, 480, 150, 250),
            "debugger1: " + debugger1 +
            "\ndebugger2: " + debugger2 +
            "\ndebugger3: " + debugger3 +
            "\ndebugger4: " + debugger4 +
            "\ndebugger5: " + debugger5
            );
    }
}

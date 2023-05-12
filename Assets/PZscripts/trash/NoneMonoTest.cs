using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NoneMonoTest : MonoBehaviour
{
    public bool run = false;
    public float debugger1;

    private DataHandlerTest DHT = new DataHandlerTest();

    private void Update()
    {
        if (run == true)
        {
            run = false;
            testing();
        }
    }

    private void testing()
    {
        debugger1 = DHT.dothis();
    }
}

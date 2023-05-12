using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectWall : MonoBehaviour
{
    public ScalableWall loggedWall;
    public string type;
    public float wallY;

    private void OnTriggerEnter(Collider other)
    {
        ScalableWall sw = other.gameObject.GetComponent<ScalableWall>();
        if (sw)
        loggedWall = other.gameObject.GetComponent<ScalableWall>();
        type = loggedWall.wallType;
        wallY = other.gameObject.transform.position.y;
    }
    private void OnTriggerExit(Collider other)
    {
        ScalableWall sw = other.gameObject.GetComponent<ScalableWall>();
        if (sw == loggedWall)
        {
            loggedWall = null;
            type = null;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    public ObstacleTrigger loggedWall;
    public string type;
    public Transform wallRoot;

    private void OnTriggerEnter(Collider other)
    {
        ObstacleTrigger ot = other.gameObject.GetComponent<ObstacleTrigger>();
        if (ot)
        {
            loggedWall = ot;
            type = loggedWall.type;
            wallRoot = other.gameObject.transform;
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        ObstacleTrigger ot = other.gameObject.GetComponent<ObstacleTrigger>();
        if (ot == loggedWall)
        {
            loggedWall = null;
            type = null;
        }
    }
}

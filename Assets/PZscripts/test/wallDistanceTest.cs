using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class wallDistanceTest : MonoBehaviour
{
    public Transform wall;
    public Transform player;
    public float result;
    public bool RUN = false;

    private void Update()
    {
        if (RUN == true)
        {
            RUN = false;
            result = PlayerWallDistance(wall, player);
        }
    }
    private float PlayerWallDistance(Transform wall, Transform player)
    {
        Vector3 flatwallPos = new Vector3(wall.position.x, 0f, wall.position.z);
        Vector3 flatplayerPos = new Vector3(player.position.x, 0f, player.position.z);
        float angleA = Vector3.Angle(wall.right, flatplayerPos - flatwallPos) / 57.2958f; //in radian
        float sineA = Mathf.Sin(angleA);
        float Hypotenuse = (flatwallPos - flatplayerPos).magnitude;
        return sineA * Hypotenuse;
    }
}
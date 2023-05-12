using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EulerRotateTowardsTest : MonoBehaviour
{
    public Transform current;
    public Transform target;
    public float maxRadian;
    public float maxMagnitude;
    public bool stepRun = false;

    private Vector3 stepvec;

    private void Update()
    {
        if (stepRun == true)
        {
            stepRun = false;
            DoThis();
        }
    }
    private void DoThis()
    {
        stepvec = Vector3.RotateTowards(current.eulerAngles, target.eulerAngles, maxRadian, maxMagnitude);
        current.eulerAngles = stepvec;
    }
}

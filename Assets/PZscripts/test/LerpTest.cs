using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LerpTest : MonoBehaviour
{
    public Transform target;
    public Vector3 VecCache;
    public bool isLerping;
    public float ticker = 0f;

    public bool RUN = false;

    private void Update()
    {
        if (RUN == true)
        {
            PositionLerp();
        }
    }
    private void PositionLerp()
    {
        float lerpTime = 2f;
        if (isLerping == false)//set lerping flag and cache vector, one time operate
        {
            isLerping = true;
            VecCache = gameObject.transform.position;
        }

        if (ticker < lerpTime)
        {
            ticker += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(VecCache, target.position, (ticker / lerpTime));
        }
        else
        {
            RUN = false;
            isLerping = false;
            ticker = 0f;
            VecCache = new Vector3(0f, 0f, 0f);
        }    
    }

}

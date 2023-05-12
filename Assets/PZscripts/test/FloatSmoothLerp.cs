using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatSmoothLerp : MonoBehaviour
{
    public Transform testSubject;
    public Transform testTarget;

    public bool RUN = false;

    private FloatLerpFastInEaseOut UtilLerp = new FloatLerpFastInEaseOut();

    private void Update()
    {
        if (RUN == true)
        {
            float y = UtilLerp.FloatSmoothLerp(testSubject.position.y, testTarget.position.y, Time.deltaTime * 2f, 1f);
            testSubject.position = new Vector3(testSubject.position.x, y, testSubject.position.z);
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScrollTest : MonoBehaviour
{
    public Transform testSubject;

    private void Update()
    {
        GetScroll();
    }

    private void GetScroll()
    {
        float val = Input.GetAxis("Mouse ScrollWheel");
        Vector3 cacheVec = testSubject.transform.position;
        testSubject.transform.position = new Vector3(cacheVec.x, cacheVec.y + val, cacheVec.z);
    }
}

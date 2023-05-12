using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SpinCounterClockwiseTest : MonoBehaviour
{
    public bool RUN = false;
    public bool log = false;
    public Vector3 startingZ;

    private void Update()
    {
        if (RUN == true)
        {
            rotateCC(gameObject.transform);
        }
    }

    private void rotateCC(Transform self)
    {
        if (log == false)
        {
            startingZ = gameObject.transform.forward;
            log = true;
        }
        var targetRot = Quaternion.LookRotation(self.right * -1f, new Vector3(0f, 1f, 0f));
        self.rotation = Quaternion.RotateTowards(self.rotation, targetRot, 1f);
        if (Vector3.Angle(self.forward, startingZ) > 178)
        {
            RUN = false;
            log = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FloatSmoothlerpTest : MonoBehaviour
{
    public Transform testSubject;
    public Transform targetSubject;
    public float targetMag;
    public bool isLerping = false;
    public bool RUN = false;

    public AnimationCurve LerpCurve = new AnimationCurve();


    private void OnEnable()
    {
        LerpCurve.AddKey(0f, 1f);
        LerpCurve.AddKey(0.4f, 2f);
        LerpCurve.AddKey(1f, 0.05f);

        var key0 = LerpCurve.keys[0];
        key0.inTangent = 0f;
        key0.outTangent = 3f;
        var key1 = LerpCurve.keys[1];
        key1.inTangent = 0f;
        key1.outTangent = 0f;
        var key2 = LerpCurve.keys[2];
        key2.inTangent = 0f;
        key2.outTangent = 0f;

        LerpCurve.MoveKey(0, key0);
        LerpCurve.MoveKey(1, key1);
        LerpCurve.MoveKey(2, key2);
    }


    private void Update()
    {
        if (RUN == true)
        {
            if (isLerping == false)
            {
                targetMag = Mathf.Abs(testSubject.position.x - targetSubject.position.x);
                isLerping = true;
            }
            
            float x = testSubject.position.x;
            x = FloatSmoothLerp(x, targetSubject.position.x, Time.deltaTime * 2f, targetMag);
            testSubject.position = new Vector3 (x, testSubject.position.y, testSubject.position.z);

            if (testSubject.position.x == targetSubject.position.x)
            {
                RUN = false;
                isLerping = false;
            }
        }
    }

    private float FloatSmoothLerp(float self, float to, float step, float magnitude)
    {
        float output = self;
        float normProgress = 1 - (Mathf.Abs(to - self) / magnitude);
        if (normProgress < 0) // if magnitude doesnt match, push self to within magnitude range
        {
            if (output > to)
            {
                output = to + magnitude;
            }
            else
            {
                output = to - magnitude;
            }
        }

        if (output < to)
        {
            output += step * LerpCurve.Evaluate(normProgress);
            if (output > to)
            {
                output = to;
            }
        }
        else
        {
            output -= step * LerpCurve.Evaluate(normProgress);
            if (output < to)
            {
                output = to;
            }
        }

        return output;
    }
        
}

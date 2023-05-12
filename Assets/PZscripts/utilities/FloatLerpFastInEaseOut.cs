using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatLerpFastInEaseOut
{
    public AnimationCurve LerpCurve = new AnimationCurve();
    private bool init = false;

    private void Init()
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

    public float FloatSmoothLerp(float self, float to, float step, float magnitude)
    {
        if (init == false)
        {
            Init();
            init = true;
        }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this on "Root" with where the controller is
/// </summary>
public class AnimationControlModule : MonoBehaviour
{
    public PlayerControlModule PCM;

    private Animator animCont;
    private float strafWgt;
    private float walkWgt;
    private float movementSpeed;

    private void Awake()
    {
        animCont = GetComponent<Animator>();
    }

    private void Update()
    {
        SetBlendWeight();
        SetState();
    }

    private void SetState()
    {
        animCont.SetInteger("walkState", PCM.m_walkState);
        animCont.SetInteger("strafState", PCM.m_strafState);
        animCont.SetFloat("walkWgt", walkWgt, 0.2f, Time.deltaTime);
        animCont.SetFloat("strafWgt", strafWgt, 0.2f, Time.deltaTime);
    }

    private void SetBlendWeight()
    {
        if (PCM)
        {
            if (PCM.m_walkState == 1) //walking front
            {
                if (PCM.m_strafState == 0)
                {
                    walkWgt = 1f;
                    strafWgt = 0f;
                }
                else if (PCM.m_strafState == -1)
                {
                    walkWgt = 0.5f;
                    strafWgt = -0.5f;
                }
                else if (PCM.m_strafState == 1)
                {
                    walkWgt = 0.5f;
                    strafWgt = 0.5f;
                }
            }

            if (PCM.m_walkState == 0) // not walking front
            {
                if (PCM.m_strafState == 0)
                {
                    //do nothing when all state is cleared
                }
                else if (PCM.m_strafState == -1)
                {
                    walkWgt = 0f;
                    strafWgt = -1f;
                }
                else if (PCM.m_strafState == 1)
                {
                    walkWgt = 0f;
                    strafWgt = 1f;
                }
                
            }


        }
    }

    /// <summary>
    /// Lerp a float in a slightly smoother manner
    /// </summary>
    /// <param name="self"></param>
    /// <param name="to"></param>
    /// <param name="step"></param>
    /// <param name="magnitude"></param>
    /// <returns></returns>
    private float FloatSmoothLerp(float self, float to, float step, float magnitude)
    {
        float output = self;
        float normProgress = Mathf.Abs(to - self) / magnitude;
        if (normProgress > 1f)
        {
            magnitude = Mathf.Abs(to - self);
        }
        float smoothStep;
        if (normProgress < 0.5f)
        {
            smoothStep = step * normProgress * 4f + step * 0.1f;
        }
        else
        {
            smoothStep = step * (1f - normProgress) * 4f + step * 0.1f;
        }

        if (self < to)
        {
            output += smoothStep * Time.deltaTime * 60f;
            if (output > to)
            {
                output = to;
            }
        }
        else
        {
            output -= smoothStep * Time.deltaTime * 60f;
            if (output < to)
            {
                output = to;
            }
        }
        return output;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BipedCalculator
{
    private float BkFactor;
    private float WkFactor;
    private float SptFactor;
    private float StfFactor;

    /// <summary>
    /// handles XYZ movement
    /// </summary>
    /// <param name="TMF"></param>
    /// <param name="walk"></param>
    /// <param name="straf"></param>
    /// <param name="stand"></param>
    /// <param name="forward"></param>
    /// <returns></returns>
    public Vector3 BipedMovementHandler(TPSMotorFunction TMF, CharacterController CC, int walk, int straf, int stand, int forward, ScreenDebugger sdb)
    {
        Transform currentTrans = TMF.GetTPS();
        //use target factors to lerp float into correspondent value
        //each walkState has its own combination
        float targetBkFactor = 0f;
        float targetWkFactor = 0f;
        float targetSptFactor = 0f;
        float targetStfFactor = 0f;
        float speedBuffering = 3f;
        Vector3 finalVec;
        float speedNormalizer = 1f;

        //Z movement
        Vector3 frontVec = currentTrans.forward;
        if (walk == -2)
        {
            targetWkFactor = 0f;
            targetSptFactor = TMF.sprintFactor;
            targetBkFactor = TMF.backFactor;
            frontVec *= TMF.baseMovementSpeed * (BkFactor + (SptFactor * -1f));
        }
        else if (walk == -1)
        {
            targetWkFactor = 0f;
            targetSptFactor = 0f;
            targetBkFactor = TMF.backFactor;
            frontVec *= TMF.baseMovementSpeed * BkFactor * -1f;
        }
        else if (walk == 0)
        {
            targetWkFactor = 0f;
            targetBkFactor = 0f;
            frontVec *= TMF.baseMovementSpeed * ((WkFactor + (BkFactor * -1f)) + SptFactor);
            sdb.debugger3 = frontVec.ToString();
            float deg4 = ((WkFactor + (BkFactor * -1f)) + SptFactor);
            sdb.debugger4 = deg4.ToString();
        }
        else if (walk == 1)
        {
            targetWkFactor = TMF.walkFactor;
            targetSptFactor = 0f;
            targetBkFactor = 0f;
            frontVec *= TMF.baseMovementSpeed * WkFactor;
        }
        else if (walk == 2)
        {
            targetWkFactor = TMF.walkFactor;
            targetSptFactor = TMF.sprintFactor;
            targetBkFactor = 0f;
            frontVec *= TMF.baseMovementSpeed * (WkFactor + SptFactor);
        }

        //X movement
        Vector3 rightVec = currentTrans.right;
        if (straf == -2)
        {
            targetStfFactor = TMF.strafFactor * -1f;
            targetSptFactor = TMF.sprintFactor;
            rightVec *= TMF.baseMovementSpeed * (StfFactor + SptFactor);
        }
        else if (straf == -1)
        {
            targetStfFactor = TMF.strafFactor * -1f;
            targetSptFactor = 0f;
            rightVec *= TMF.baseMovementSpeed * StfFactor;
        }
        else if (straf == 0)
        {
            targetStfFactor = 0f;
            rightVec *= TMF.baseMovementSpeed * StfFactor;
        }
        else if (straf == 1)
        {
            targetStfFactor = TMF.strafFactor;
            targetSptFactor = 0f;
            rightVec *= TMF.baseMovementSpeed * StfFactor;
        }
        else if (straf == 2)
        {
            targetStfFactor = TMF.strafFactor;
            targetSptFactor = TMF.sprintFactor;
            rightVec *= TMF.baseMovementSpeed * (StfFactor + SptFactor);
        }

        //lerping the speed factors linearly
        BkFactor = FloatTowards(BkFactor, targetBkFactor, Time.deltaTime * speedBuffering);
        WkFactor = FloatTowards(WkFactor, targetWkFactor, Time.deltaTime * speedBuffering);
        SptFactor = FloatTowards(SptFactor, targetSptFactor, Time.deltaTime * speedBuffering);
        StfFactor = FloatTowards(StfFactor, targetStfFactor, Time.deltaTime * speedBuffering);
        sdb.debugger1 = WkFactor.ToString();
        sdb.debugger2 = SptFactor.ToString();

        //normalize speed
        if (straf == 0 || walk == 0)
        {
            speedNormalizer = 1f;
        }
        else
        {
            speedNormalizer = 0.7f;
        }

        //final output
        finalVec = (frontVec + rightVec) * speedNormalizer;
        Debug.DrawLine(currentTrans.position, currentTrans.position + finalVec);
        return finalVec;
    }

    /// <summary>
    /// Jump Related
    /// </summary>
    /// <param name="TMF"></param>
    /// <param name="CC"></param>
    /// <param name="walk"></param>
    /// <param name="straf"></param>
    /// <param name="stand"></param>
    /// <returns></returns>
    public Vector3 BipedJumpHandler(TPSMotorFunction TMF, CharacterController CC, int walk, int straf, int stand)
    {
        Transform currentTrans = TMF.GetTPS();
        Vector3 upVec = new Vector3(0f, 0f, 0f);

        //jump
        if (stand == 0 || stand == -1)
        {
            upVec *= 0f;
        }
        if (CC.isGrounded) //touching ground immediately set jump flag off
        {
            //TMF.isJumping = false; //what if we don't need grounded to determine jump is completed?
        }

        if (stand == 1 && TMF.ObstInProgress == false && CC.isGrounded == true && TMF.ObstDetected == false)//main jump criteria: you press space, you are not in an obstacle course, you are on the ground
        {
            TMF.isJumping = true;
        }

        if (TMF.isJumping == true && TMF.ObstInProgress == false)//jump loop criteria: jump flag is on, not in an obstacle course
        {

            if (TMF.jumpTick < 1.5f)// count to x second then stop, it gets reset after grounded
            {
                TMF.jumpTick += Time.deltaTime;
            }
            else
            {
                TMF.jumpTick = 0f;
                TMF.isJumping = false;
            }
            //primary jump sequence
            if (TMF.jumpTick < 0.3f)//pre jump in second, happens in any state
            {
                upVec *= 0f;// no jump in pre jump
            }
            else//after prejump
            {
                if (walk == 2)//sprinting forward with or without strafing
                {
                    if (TMF.jumpTick < (0.8f + 0.3f))
                    {
                        upVec = currentTrans.up * TMF.jumpFactor * (1.2f - TMF.jumpTick) * 0.05f;
                        upVec *= 0.9f;
                    }
                    else
                    {
                        upVec *= 0f;
                    }

                }
                else if (walk == 0 && straf == 0)//stationary
                {
                    if (TMF.jumpTick < (0.5f + 0.3f))
                    {
                        upVec = currentTrans.up * TMF.jumpFactor * (1.2f - TMF.jumpTick) * 0.05f;
                        upVec *= 1.1f;
                    }
                    else
                    {
                        upVec *= 0f;
                    }

                }
                else if (walk < 0)//backing both state with or without strafing
                {
                    upVec *= 0f;
                }
                else//walking, pure strafing both state
                {
                    if (TMF.jumpTick < (0.65f + 0.3f))
                    {
                        upVec = currentTrans.up * TMF.jumpFactor * (1.2f - TMF.jumpTick) * 0.05f;
                    }
                    else
                    {
                        upVec *= 0f;
                    }
                }

            }
        }
        else//pretty much means jump interupted
        {
            TMF.isJumping = false;
            TMF.jumpTick = 0f;
        }

        return upVec;
    }

    /// <summary>
    /// collider adjustment for crouching
    /// </summary>
    /// <param name="TMF"></param>
    /// <param name="CC"></param>
    /// <param name="stand"></param>
    public void CrouchHandler(TPSMotorFunction TMF, CharacterController CC, int stand)
    {
        if (stand == 0)
        {
            TMF.crouchTick += Time.deltaTime * 2f;
            TMF.crouchTick = Mathf.Clamp(TMF.crouchTick, 0f, 1f);
        }
        else if (stand == -1)
        {
            TMF.crouchTick -= Time.deltaTime * 2f;
            TMF.crouchTick = Mathf.Clamp(TMF.crouchTick, 0f, 1f);
        }
        CC.height = TMF.characterHeight / 2f + (TMF.characterHeight / 2f * TMF.crouchTick);
        CC.center = new Vector3(0f, CC.height / 2f, 0f);
    }

    /// <summary>
    /// Aim character
    /// </summary>
    /// <param name="TMF"></param>
    /// <param name="lookstate"></param>
    /// <returns></returns>
    public Quaternion Forwardhanlder(TPSMotorFunction TMF, int lookstate)
    {
        Quaternion currentVec = TMF.transform.rotation;
        Quaternion finalVec;
        Quaternion targetVec;
        float centeringSpeed = Time.deltaTime * 300f * TMF.spinFactor;
        float cameraY = Camera.main.transform.eulerAngles.y;

        if (lookstate == 0)
        {
            targetVec = Quaternion.Euler(0, cameraY, 0);
            finalVec = Quaternion.RotateTowards(currentVec, targetVec, centeringSpeed);

        }
        else
        {
            finalVec = TMF.GetTPS().rotation;
        }


        return finalVec;
    }
    /// <summary>
    /// Move a float towards target value
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    private float FloatTowards(float from, float to, float step)
    {
        float output = from;
        if (from < to)
        {
            output += step;
            if (output > to)
            {
                output = to;
            }
        }
        else
        {
            output -= step;
            if (output < to)
            {
                output = to;
            }
        }
        return output;

    }
}

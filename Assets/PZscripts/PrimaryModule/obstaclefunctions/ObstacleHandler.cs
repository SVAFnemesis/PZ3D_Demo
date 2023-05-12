using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHandler
{
    public void ScalingSequence(Transform TPS, Transform Wallroot, CharacterController CC, int standState, int walkState)
    {
        TPSMotorFunction TMF = TPS.GetComponent<TPSMotorFunction>();

        if ((Wallroot.position.y - TPS.position.y) > 2.2f)//wall 2.2m and above
        {
            if (walkState == 2)//sprinting forward check
            {
                ScalingSnapOn(TPS, Wallroot);
            }
            else//if failed sprinting check, reset all flag and back to normal state
            {
                TMF.ObstInProgress = false;
                TMF.AllMovementDisabled = false;
            }
        }
        else//wall not 2.2m
        {
            ScalingSnapOn(TPS, Wallroot);//do it anyway
        }

        if (TMF.ObstIsSnapped == true && standState == 1)//trigger for flopping
        {
            TMF.ObstIsFlopping = true;
        }
        if (TMF.ObstIsFlopping == true)
        {
            ScalingOver(TPS, Wallroot);
        }
        if (TMF.ObstIsSnapped == true && standState == -1)
        {
            ScalingRelease(TPS);
        }
    }

    /// <summary>
    /// This sub sequence will snap player to wall ledge, flags: isSnapping, isSnapped, SnappingTick
    /// </summary>
    /// <param name="TPS"></param>
    /// <param name="wallroot"></param>
    private void ScalingSnapOn(Transform TPS, Transform wallroot)
    {
        TPSMotorFunction TMF = TPS.GetComponent<TPSMotorFunction>();
        Quaternion targetRot;
        float height = TMF.characterHeight;

        if (TMF.ObstIsSnapped == false && TMF.ObstIsFlopping == false)//beginning snapping prep, one time operation
        {
            if (ObstacleDegreeLimiter(wallroot, TPS) == true)//angle check, will not proceed without passing
            {
                TMF.ObstIsSnapping = true;//set snapping flag to start the loop
                TMF.ObstVectorCache1 = TPS.position;//cache player position
                TMF.ObstSide = DetermineWallSide(wallroot, TPS);//cache side
                TMF.ObstVectorCache2 = (new Vector3(TPS.position.x, wallroot.position.y - height, TPS.position.z)) + wallroot.forward * (0.2f - PlayerWallDistance(wallroot, TPS)) * TMF.ObstSide;//to get to target position: 1. determine how far player is to the wall; 2. push player to the wall on wall.z; 3. push player away from wall by 0.2; 4. raise player up to proper height;
                //**vectorcache2 is the target pos**//
            }
        }

        if (TMF.ObstIsSnapping == true) //snapping loop begin, loop stops when snapping flag turned off
        {
            targetRot = Quaternion.LookRotation(wallroot.forward * TMF.ObstSide * -1f, wallroot.up);
            

            float snaptime1 = 0.3f;//rotate towards wall time
            float snaptime2 = 0.5f;//snap on ledge time
            if (TMF.ObstScalingTick < snaptime1)//phase1, rotate to wall
            {
                TMF.ObstScalingTick += Time.deltaTime;
                TPS.rotation = Quaternion.Lerp(TPS.rotation, targetRot, (TMF.ObstScalingTick / snaptime1 / 2f));
            }
            else if (TMF.ObstScalingTick >= snaptime1 && TMF.ObstScalingTick < (snaptime1 + snaptime2))//phase2, snap to target pos
            {
                TMF.ObstScalingTick += Time.deltaTime;
                TPS.position = Vector3.Lerp(TMF.ObstVectorCache1, TMF.ObstVectorCache2, ((TMF.ObstScalingTick - snaptime1) / snaptime2));
            }
            else//complete
            {
                TMF.ObstIsSnapping = false;
                TMF.ObstIsSnapped = true;
                TMF.ObstScalingTick = 0f;
            }
        }
        if (TMF.ObstIsSnapped == true)
        {
            TPS.position = new Vector3(TPS.position.x, wallroot.position.y - height, TPS.position.z);
            TPS.rotation = Quaternion.LookRotation(wallroot.forward * -1f * TMF.ObstSide, wallroot.up);
            TMF.ObstScalingTick = 0f;
        }
    }

    private void ScalingOver(Transform TPS, Transform wallroot)
    {
        TPSMotorFunction TMF = TPS.GetComponent<TPSMotorFunction>();
        float height = TMF.characterHeight;
           
        if (TMF.ObstIsSnapped == true) //one time operation using the flag "isSnapped"
        {
            TMF.ObstIsSnapped = false;//temporarily disarm snap flag
            TMF.ObstVectorCache1 = TPS.position;//cache current pos
            TMF.ObstSide = DetermineWallSide(wallroot, TPS);
            TMF.ObstVectorCache2 = (new Vector3(TPS.position.x, wallroot.position.y - height, TPS.position.z)) + wallroot.forward * (0.2f + PlayerWallDistance(wallroot, TPS)) * TMF.ObstSide * -1f;
        }

        var CCRot = Quaternion.LookRotation(TPS.right * -1f, new Vector3(0f, 1f, 0f));//CC rotation by step
        var targetRot = Quaternion.LookRotation(wallroot.forward * TMF.ObstSide, new Vector3(0f, 1f, 0f));
        //var targetPos = (new Vector3(TPS.position.x, wallroot.position.y - height, TPS.position.z)) + wallroot.forward * (0.2f + PlayerWallDistance(wallroot, TPS)) * TMF.ObstSide * -1f;
        float turnspeed = 0.73f;
        float flopTime = 3f;

        if (TMF.ObstIsFlopping == true)
        {
            if (TMF.ObstScalingTick < flopTime)
            {
                TMF.ObstScalingTick += Time.deltaTime;
                TPS.position = Vector3.Lerp(TMF.ObstVectorCache1, TMF.ObstVectorCache2, (TMF.ObstScalingTick / flopTime));
                TPS.rotation = Quaternion.Lerp(TPS.rotation, CCRot, turnspeed * Time.deltaTime);
            }
            else
            {
                TPS.rotation = targetRot;
                TMF.ObstScalingTick = 0f;
                TMF.ObstIsSnapped = true;
                TMF.ObstIsFlopping = false;
                TMF.ObstSide = DetermineWallSide(wallroot, TPS);//update wallside after flipping side
                //go back to snapped
            }
        }
    }

    private void ScalingRelease(Transform TPS)
    {
        TPSMotorFunction TMF = TPS.GetComponent<TPSMotorFunction>();
        if (TMF.ObstIsFlopping == false && TMF.ObstIsSnapped == true)
        {
            TMF.ObstIsSnapped = false;
            TMF.ObstInProgress = false;
            TMF.ObstIsSnapping = false;
            TMF.AllMovementDisabled = false;
            TMF.ObstScalingTick = 0;

        }
    }


    public void VaultSequence(Transform TPS, Transform wallroot)
    {
        VaultOver(TPS, wallroot);
    }

    private void VaultOver(Transform TPS, Transform wallroot)
    {
        TPSMotorFunction TMF = TPS.GetComponent<TPSMotorFunction>();
        if (ObstacleDegreeLimiter(wallroot, TPS) && TMF.ObstIsVaulting == false)//if pass angle test and hasn't vault yet, do the prepration
        {
            TMF.ObstIsVaulting = true;
            float distance = DetermineVaultDistance(wallroot, TPS);
            TMF.ObstVaultStart = TPS.position;
            TMF.ObstVaultDestiny = TPS.position + TPS.forward * distance * 2.1f + new Vector3(0f, wallroot.position.y, 0f);//my position + 2 forward distance and a bit more + go up to the wall Y
        }
        if (TMF.ObstIsVaulting == true)
        {
            float vaulttime = 0.6f;
            float waittime = 0.3f;
            if (TMF.ObstVaultingTick  < vaulttime)
            {
                TMF.ObstVaultingTick += Time.deltaTime;
                TPS.position = Vector3.Lerp(TMF.ObstVaultStart, TMF.ObstVaultDestiny, TMF.ObstVaultingTick * 1f / vaulttime);// lerp from start to destiny by tick
            }
            else if(TMF.ObstVaultingTick < vaulttime + waittime)
            {
                TMF.ObstVaultingTick += Time.deltaTime;
            }
            else
            {
                TMF.ObstVaultingTick = 0f;
                TMF.ObstIsVaulting = false;
                TMF.AllMovementDisabled = false;
                TMF.ObstInProgress = false;
            }
        }
        
    }


    /// <summary>
    /// Positive 1 -> you are on the Z side
    /// </summary>
    /// <param name="wall"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private int DetermineWallSide(Transform wall, Transform player)
    {
        var front = (player.position - wall.position + wall.forward).magnitude;
        var back = (player.position - wall.position - wall.forward).magnitude;
        if (front > back)
        {
            return 1;
        }
        else
        {
            return -1;
        }

    }
    /// <summary>
    /// float distance straight in front of player and to the wall
    /// </summary>
    /// <param name="wall"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private float DetermineVaultDistance(Transform wall, Transform player)
    {
        float sideC = (new Vector3(wall.position.x, 0f, wall.position.z) - new Vector3(player.position.x, 0f, player.position.z)).magnitude;
        float angleW = Vector3.Angle(wall.right * -1f, player.position - wall.position) / 57.2958f; // in radian
        float angleC = Vector3.Angle(player.forward * -1f, wall.right) / 57.2958f;// in radian
        float SineW = Mathf.Sin(angleW);
        float sineC = Mathf.Sin(angleC);

        return sideC * SineW / sineC;
    }

    private bool ObstacleDegreeLimiter(Transform wall, Transform player)
    {
        int side = DetermineWallSide(wall, player);
        float ForwardDist = (player.forward - wall.forward).magnitude;
        {
            if (side == 1)
            {
                if (1.85f < ForwardDist && ForwardDist < 2f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (0f < ForwardDist && ForwardDist < 0.765f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    /// <summary>
    /// Cloeset distance bewteen player and wall
    /// </summary>
    /// <param name="wall"></param>
    /// <param name="player"></param>
    /// <returns></returns>
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor;

public class PowerTrainCalculator : MonoBehaviour
{
    //obtainable vairable
    public VehicleTransmissionCurveAT m_ATModule;
    public VehicleTransmissionCurveMT m_MTModule;

    public float log1;//debug purposes
    public float log2;//debug purposes
    public float log3;//debug purposes


    //returnable value

    //private


    private void Awake()
    {

    }

    /// <summary>
    /// feed throttle state and gear state, this function determines AT MT on its own.
    /// zero throttle will go right through without generating torque.
    /// return the final torque that needs to be cache back to VMF
    /// </summary>
    /// <param name="throttle"></param>
    /// <param name="gear"></param>
    public float TorqueCalculator(VehicleMotorFunction VMF, int throttle, int gear, float velocity)
    {
        float finalTorque = 0f;
        float tuneRatio = 3.8f;
        int currentGear = VMF.m_ATCurrentGear; //ATcurrent needs to be cached by VMF!!!
        float wheelspeed = VMF.SamplizingWheelSpeed();// hope this will have a stablized wheel speed
        float gaslevel = 0f;
        if (throttle == 0)//change gaslevel according to throttle state, using zero to bypass torque gen
        {
            gaslevel = 0f;
        }
        else if (throttle == 1)
        {
            gaslevel = 1f;
        }
        else if (throttle == 2)
        {
            gaslevel = VMF.m_floorFactor;
            if (velocity < 2.5f && velocity > -0.01f)
            {
                gaslevel = (3.5f + (-1f * velocity)) * VMF.m_floorFactor;//while less than 5mph, floor factor increase drastically
            }
        }

        if (VMF.isAT == true) // when automatic transmission 
        {
            
            float shiftBuffer = 0.08f;
            float normalizedWSpd = wheelspeed / VMF.m_maxWheelSpeed;//wheelspeed is not a stable number, need to samplized to get a proper value
            float firstShift = m_ATModule.firstSecondThreshold;
            float secondShift = m_ATModule.secondThirdThreshold;
            float thirdShift = m_ATModule.thirdFourthThreshold;
            VMF.m_parkbrake = 0f;//reseting park brake every frame;
            if (-1f < normalizedWSpd && normalizedWSpd < firstShift - shiftBuffer)
            {
                currentGear = 0;
                VMF.m_ATCurrentGear = 0;
            }
            else if (firstShift < normalizedWSpd && normalizedWSpd < secondShift - shiftBuffer)
            {
                currentGear = 1;
                VMF.m_ATCurrentGear = 1;
            }
            else if (secondShift < normalizedWSpd && normalizedWSpd < thirdShift - shiftBuffer)
            {
                currentGear = 2;
                VMF.m_ATCurrentGear = 2;
            }
            else if (thirdShift < normalizedWSpd)
            {
                currentGear = 3;
                VMF.m_ATCurrentGear = 3;
            }

            if (gear == 1)
            {
                finalTorque = (m_ATModule.ATgear[currentGear].Evaluate(wheelspeed / VMF.m_maxWheelSpeed) - 0.5f) * tuneRatio * VMF.m_horsePower * gaslevel * VMF.m_engineDamageFactor;
            }
            else if (gear == -1)
            {
                finalTorque = (m_ATModule.ATgear[0].Evaluate(-wheelspeed / VMF.m_maxWheelSpeed) - 0.5f) * tuneRatio * VMF.m_horsePower * gaslevel * VMF.m_engineDamageFactor * -1f;
            }
            else if (gear == -2)//park
            {
                VMF.m_parkbrake = VMF.m_brakePower * VMF.m_deadBrake;
            }
        }
        else // when at manual transmission, generate torque from gear to gear
        {
            if (gear > 0)
            {
                finalTorque = (m_MTModule.MTgear[gear - 1].Evaluate(wheelspeed / VMF.m_maxWheelSpeed) - 0.5f) * tuneRatio * VMF.m_horsePower * gaslevel * VMF.m_engineDamageFactor;
            }
            else if (gear == 0)
            {
                finalTorque = 0;
            }
            else if (gear == -1)
            {
                finalTorque = (m_MTModule.MTgear[0].Evaluate(-wheelspeed / VMF.m_maxWheelSpeed) - 0.5f) * tuneRatio * VMF.m_horsePower * gaslevel * VMF.m_engineDamageFactor * -1f;
            }
            log1 = gear;

        }
        return finalTorque;
    }
    /// <summary>
    /// feed brake state, zero will go thru, brake apply to front wheel only.
    /// return the final brake that needs to be cache back to VMF
    /// </summary>
    /// <param name="brake"></param>
    public float BrakeCalculator(VehicleMotorFunction VMF, int brake)
    {
        float finalBrake = 0;
        float wheelspeed = (VMF.wheel[0].rpm + VMF.wheel[1].rpm) / 2f; // brake apply to front wheel only
        float brakeLevel = 0f;//use stompbrake = 0 to bypass brake
        if (brake == 1)
        {
            brakeLevel = 1f;
        }
        else if (brake == 2)
        {
            brakeLevel = VMF.m_deadBrake;
        }
        finalBrake = VMF.m_curveBrake.Evaluate(wheelspeed / VMF.m_maxWheelSpeed) * VMF.m_brakePower * brakeLevel * VMF.m_brakeDamageFactor;
        return finalBrake;

    }
    /// <summary>
    /// all steer state go through, zero state will do swerve recenter.
    /// velocity is the velocity of rigid body!!!
    /// return the final steer that needs to te cached to steer
    /// </summary>
    /// <param name="steer"></param>
    public float SteerCalculator(VehicleMotorFunction VMF, int steerstate, float velocity)
    {
        float finalAngle = 0f;
        float maxVelocity = VMF.m_maxWheelSpeed * 0.104112f * VMF.wheel[0].radius; // converting maxAWS to maxVel, by default the maxVel is 27.1(m/s)
        float currentSteer = VMF.m_steer;
        float limitedsteerangle = VMF.m_maxSteerAngle - VMF.m_maxSteerAngle * (velocity / maxVelocity) * 0.9f;//0.1 of steer angle at top speed, exceeding will cause problem
        float steerlevel = VMF.m_swerveFactor;//when no steer, steer recenter at serve speed
        if (steerstate == 0)
        {
            if (VMF.m_steer > 0.2f)
            {
                steerlevel = VMF.m_swerveFactor * -1f;
            }
            else if (VMF.m_steer < -0.2f)
            {
                steerlevel = VMF.m_swerveFactor;
            }
            else if(VMF.m_steer > -0.2f && VMF.m_steer < 0.2f)
            {
                finalAngle = 0f;
                steerlevel = 0f;
            }
        }
        else if (steerstate == 1)
        {
            if (VMF.m_steer > 0)
            {
                steerlevel = VMF.m_steerFactor;
            }
            else
            {
                steerlevel = VMF.m_swerveFactor;
            }
        }
        else if (steerstate == 2)
        {
            steerlevel = VMF.m_swerveFactor;
        }
        else if (steerstate == -1)
        {
            if (VMF.m_steer < 0)
            {
                steerlevel = VMF.m_steerFactor * -1f;
            }
            else
            {
                steerlevel = VMF.m_swerveFactor * -1f;
            }
            
        }
        else if (steerstate == -2)
        {
            steerlevel = VMF.m_swerveFactor * -1f;
        }
        currentSteer += Time.deltaTime * steerlevel * 8f; //increment into currentSteer which is obtained from VMF
        limitedsteerangle = VMF.m_maxSteerAngle - VMF.m_maxSteerAngle * (velocity / maxVelocity) * 1.6f;
        limitedsteerangle = Mathf.Clamp(limitedsteerangle, 5f, VMF.m_maxSteerAngle);
        finalAngle = Mathf.Clamp(currentSteer, limitedsteerangle * -1f, limitedsteerangle);
        return finalAngle;

    }
    /// <summary>
    /// on and off, simple as that, use half speed stomp brake force as the brake force. Does not have return value.
    /// </summary>
    /// <param name="handbrake"></param>
    public float HandbrakeCalculator(VehicleMotorFunction VMF, int handbrake)
    {
        float finalHandbrake = VMF.m_curveBrake.Evaluate(0.5f) * VMF.m_brakePower * VMF.m_deadBrake * handbrake;
        return finalHandbrake;
    }
    /// <summary>
    /// Euler local angle
    /// </summary>
    /// <param name="VMF"></param>
    /// <param name="gear"></param>
    /// <param name="stick"></param>
    /// <returns></returns>
    public Vector3 SetStickAxis(VehicleMotorFunction VMF, int gear)
    {
        Vector3 outVec = new Vector3(0f, 0f, 0f);
        if (VMF.isAT == true)
        {
            Vector3 vec = new Vector3(0f, 0f, 0f);
            vec.z = gear * -15f;
            outVec = vec;
        }
        else
        {
            Vector3 vec = new Vector3(0f, 0f, 0f);
            if (gear == -1)
            {
                vec.z = 10f;
                vec.x = -15f;
                outVec = vec;
            }
            else if (gear == 0)
            {
                vec.z = 0f;
                vec.x = 0f;
                outVec = vec;
            }
            else if (gear == 1)
            {
                vec.z = 0f;
                vec.x = 15f;
                outVec = vec;
            }
            else if (gear == 2)
            {
                vec.z = 0f;
                vec.x = -15f;
                outVec = vec;
            }
            else if (gear == 3)
            {
                vec.z = -10f;
                vec.x = 15f;
                outVec = vec;
            }
            else if (gear == 4)
            {
                vec.z = -10f;
                vec.x = -15f;
                outVec = vec;
            }
            else if (gear == 5)
            {
                vec.z = -20f;
                vec.x = 15f;
                outVec = vec;
            }
        }
            
        return outVec;
    }
    public Vector3 SetHandbrake(VehicleMotorFunction VMF, int handbrake)
    {
        if (handbrake == 1)
        {
            return new Vector3(-30f, 0f, 0f);
        }
        else
        {
            return new Vector3(0f, 0f, 0f);
        }
    }
}

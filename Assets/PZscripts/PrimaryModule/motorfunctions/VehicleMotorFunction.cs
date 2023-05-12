using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor;

[RequireComponent(typeof(PowerTrainCalculator))]
public class VehicleMotorFunction : MonoBehaviour
{
    [Header("self grab, need no assignment", order = 0)]
    [Space(0, order = 1)]
    public Transform m_Vehicle;
    [Header("put wheels in, 0 and 1 are for front wheels", order = 0)]
    [Space(0, order = 1)]
    public WheelCollider[] wheel;
    public Transform[] wheelMesh;
    public Transform steeringAxis;
    public Transform stickAxis;
    public Transform handbrakeAxis;
    [Header("Tick both for AWD", order = 0)]
    [Space(0, order = 1)]
    public bool isFWD = true;
    public bool isRWD = false;
    public bool isAT = true;
    [Header("AT will use 012 gear and then final gear", order = 0)]
    [Space(0, order = 1)]
    /// <summary>
    /// CAUTION! gear[0] -> 1st gear!!!
    /// </summary>
    public AnimationCurve m_curveBrake;
    [Header("Top speed this car can reach", order = 0)]
    [Space(0, order = 1)]
    public float m_maxWheelSpeed = 1f;// for normalizing speed on curve
    [Header("How Fast it reaches top speed", order = 0)]
    [Space(0, order = 1)]
    public float m_horsePower = 1f;// for amplifying "torque"
    [Header("high performance car may have higher floorfactor", order = 0)]
    [Space(0, order = 1)]
    public float m_floorFactor = 2.2f;
    [Header("Brake efficiency", order = 0)]
    [Space(0, order = 1)]
    public float m_brakePower = 1f;//for amplyfying "brake"
    [Header("adjust this till stomp brake causes skip", order = 0)]
    [Space(0, order = 1)]
    public float m_deadBrake = 5f;//adjust this until skipping happens
    [Header("as the name suggested", order = 0)]
    [Space(0, order = 1)]
    public float m_maxSteerAngle = 35f;
    [Header("how fast when normal steering", order = 0)]
    [Space(0, order = 1)]
    public float m_steerFactor = 1f;
    [Header("how fast when swerve steering", order = 0)]
    [Space(0, order = 1)]
    public float m_swerveFactor = 3f;
    [Header("1 for full health, 0 for totalled", order = 0)]
    [Space(0, order = 1)]
    [Range(0f, 1f)]
    public float m_engineDamageFactor = 1f;
    [Range(0f, 1f)]
    public float m_brakeDamageFactor = 1f;
    //[Header("Specify capacity for max MCU that can sit in", order = 0)]
    //[Space(0, order = 1)]
    //public int seatCapacity = 1;
    public Transform[] seat;
    public Transform[] exit;
    [Header("0,1,2 - mid, left, right behind", order = 0)]
    [Space(0, order = 1)]
    public Transform[] viewpoint;

    [Header("The following aren't settings", order = 0)]
    [Space(0, order = 1)]
    public float m_torque;
    public float m_brake;
    public float m_steer;
    public float m_handbrake;
    public float m_parkbrake;
    public int m_ATCurrentGear;//AT current gear is cache dependent!!!
    public float m_stablizedWheelSpeed;
    public bool[] isSeated;//a current record of seating status, filled by iSlave, emptied by MCU
    /// <summary>
    /// remember sitting members of MCU, filled by iSlave, emptied by MCU
    /// </summary>
    public MotionControlModule[] SittingMCU;

    private Rigidbody mainRigid;
    private PowerTrainCalculator m_PTcal;
    private float[] wheelSpeedCache;

    //------------------------------------------------------------------------//


    private void Awake()
    {
        SittingMCU = new MotionControlModule[seat.Length];
        isSeated = new bool[seat.Length];
        m_PTcal = GetComponent<PowerTrainCalculator>();
        mainRigid = GetComponent<Rigidbody>();
        wheelSpeedCache = new float[5];
        m_Vehicle = gameObject.transform;
        wheel[2].brakeTorque = 100f;
        wheel[3].brakeTorque = 100f;//freshly loaded car will have brake on
    }

    private void Update()
    {
        
    }

    private void ClearMouse()
    {
        CursorLockMode m_cursor;
        m_cursor = CursorLockMode.Locked;
        Cursor.lockState = m_cursor;
        Cursor.visible = false;
    }



    /// <summary>
    /// give me your MCU and I'll tell you which seat you're sitting on
    /// </summary>
    /// <param name="MCU"></param>
    public int QuerryMySeat(MotionControlModule MCU)
    {
        int mySeat = System.Array.IndexOf(SittingMCU, MCU);
        return mySeat;
    }

    #region MOTOR FUNCTION
    /// <summary>
    /// only MCU at seat0 can call this!
    /// </summary>
    public void RunMotorFunction(PlayerControlModule PCM)
    {
        //calculate final torque, at and mt has different input
        if (isAT == true)
        {
            m_torque = m_PTcal.TorqueCalculator(this, PCM.m_throttleState, PCM.m_ATgearState, mainRigid.velocity.magnitude);
        }
        else
        {
            m_torque = m_PTcal.TorqueCalculator(this, PCM.m_throttleState, PCM.m_gearState, mainRigid.velocity.magnitude);
        }
        //calculate brake
        m_brake = m_PTcal.BrakeCalculator(this, PCM.m_brakeState);
        //calculate steer
        m_steer = m_PTcal.SteerCalculator(this, PCM.m_steerState, mainRigid.velocity.magnitude);
        //calculate handbrake
        m_handbrake = m_PTcal.HandbrakeCalculator(this, PCM.m_handbrakeState);

        Powertrain(m_torque, m_brake, m_handbrake, m_steer, m_parkbrake);//have torque, brake, steer, handbrake ready (at final form) before running powertain
        if (isAT == true)//AT MT stick rotation
        {
            stickAxis.localEulerAngles = m_PTcal.SetStickAxis(this, PCM.m_ATgearState);
        }
        else
        {
            if (isAT == true)
            {
                stickAxis.localEulerAngles = m_PTcal.SetStickAxis(this, PCM.m_ATgearState);
            }
            else
            {
                stickAxis.localEulerAngles = m_PTcal.SetStickAxis(this, PCM.m_gearState);
            }  
        }

        handbrakeAxis.localEulerAngles = m_PTcal.SetHandbrake(this, PCM.m_handbrakeState);
        
    }
    /// <summary>
    /// Test function mirrors the main function, used for test driving
    /// </summary>
    /// <param name="VTT"></param>
    public void TestRunMotorFunction(VehicleTestTool VTT)
    {
        //calculate final torque, at and mt has different input
        if (isAT == true)//while AT
        {
            m_torque = m_PTcal.TorqueCalculator(this, VTT.m_throttleState, VTT.m_ATgearState, mainRigid.velocity.magnitude);
        }
        else// while MT
        {
            m_torque = m_PTcal.TorqueCalculator(this, VTT.m_throttleState, VTT.m_gearState, mainRigid.velocity.magnitude);
        }
        //calculate brake
        m_brake = m_PTcal.BrakeCalculator(this, VTT.m_brakeState);
        //calculate steer
        m_steer = m_PTcal.SteerCalculator(this, VTT.m_steerState, mainRigid.velocity.magnitude);
        //calculate handbrake
        m_handbrake = m_PTcal.HandbrakeCalculator(this, VTT.m_handbrakeState);

        Powertrain(m_torque, m_brake, m_handbrake, m_steer, m_parkbrake);//have torque, brake, steer, handbrake ready (at final form) before running powertain
    }

    /// <summary>
    /// final output the wheelcollider system is taking
    /// </summary>
    private void Powertrain(float torque, float brake, float handbrake, float steer, float parkbrake)
    {
        //FWD, RWD, AWD
        int FW = 0;
        int RW = 0;
        if (isFWD == true)
        {
            FW = 1;
        }
        if (isRWD == true)
        {
            RW = 1;
        }
        if (isAT == false)
        {
            parkbrake = 0f;
        }
        //gas
        wheel[0].motorTorque = torque * FW;
        wheel[1].motorTorque = torque * FW;
        wheel[2].motorTorque = torque * RW;
        wheel[3].motorTorque = torque * RW;
        //brake
        wheel[0].brakeTorque = brake + parkbrake;
        wheel[1].brakeTorque = brake + parkbrake;
        //steer
        wheel[0].steerAngle = steer;
        wheel[1].steerAngle = steer;
        //wheel mesh
        Vector3 cwpos;
        Quaternion cwquat;
        wheel[0].GetWorldPose(out cwpos, out cwquat);
        wheelMesh[0].position = cwpos;
        wheelMesh[0].rotation = cwquat;
        wheel[1].GetWorldPose(out cwpos, out cwquat);
        wheelMesh[1].position = cwpos;
        wheelMesh[1].rotation = cwquat;
        wheel[2].GetWorldPose(out cwpos, out cwquat);
        wheelMesh[2].position = cwpos;
        wheelMesh[2].rotation = cwquat;
        wheel[3].GetWorldPose(out cwpos, out cwquat);
        wheelMesh[3].position = cwpos;
        wheelMesh[3].rotation = cwquat;
        //steering mesh
        Vector3 steerX = steeringAxis.localEulerAngles;
        steerX.z = steer * -12f;
        steeringAxis.localEulerAngles = steerX;
        //handbrake
        wheel[2].brakeTorque = handbrake;
        wheel[3].brakeTorque = handbrake;

    }


    #endregion

    

    public float SamplizingWheelSpeed()
    {
        float roughWheelSpeed = 0f;
        if (isFWD == true)
        {
            roughWheelSpeed = (wheel[0].rpm + wheel[1].rpm) / 2f;
        }
        else if (isRWD == true)
        {
            roughWheelSpeed = (wheel[2].rpm + wheel[3].rpm) / 2f;
        }
            wheelSpeedCache[4] = wheelSpeedCache[3];
        wheelSpeedCache[3] = wheelSpeedCache[2];
        wheelSpeedCache[2] = wheelSpeedCache[1];
        wheelSpeedCache[1] = wheelSpeedCache[0];
        wheelSpeedCache[0] = roughWheelSpeed;

        m_stablizedWheelSpeed = (wheelSpeedCache[0] + wheelSpeedCache[1] + wheelSpeedCache[2] + wheelSpeedCache[3] + wheelSpeedCache[4]) * 0.2f;
        return m_stablizedWheelSpeed;
    }
}

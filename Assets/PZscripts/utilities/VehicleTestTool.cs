using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleTestTool : MonoBehaviour
{
    #region PUBLIC VARIABLE
    /// <summary>
    /// 0 on foot, 1 in motorized vehicle, 2 in static seat
    /// </summary>
    public int m_mountState = 1;
    /// <summary>
    /// -2 fast backing, -1 backing, 0 not moving, 1 walking, 2 running
    /// </summary>
    public int m_walkState = 0;
    /// <summary>
    /// -2 skip straf left, -1 strafing left, 0 not moving, 1 strafing right, 2 skip straf right
    /// </summary>
    public int m_strafState = 0;
    /// <summary>
    /// -2 prone, -1 crouch, 0 standing, 1 jump(toggle only)
    /// </summary>
    public int m_standState = 0;
    /// <summary>
    /// 1 locked, 0 follow cam
    /// </summary>
    public int m_forwardState = 0;

    /// <summary>
    /// 0 no throttle, 1 throttle, 2 flooring throttle
    /// </summary>
    public int m_throttleState = 0;
    /// <summary>
    /// 0 no brake, 1 brake, 2 stomp brake
    /// </summary>
    public int m_brakeState = 0;
    /// <summary>
    /// -2 hard left, -1 left, 0 mid, 1 right, 2 hard right
    /// </summary>
    public int m_steerState = 0;
    /// <summary>
    /// 0 no handbrake, 1 handbrake on
    /// </summary>
    public int m_handbrakeState = 0;
    /// <summary>
    /// -1 reverse, 0 neutral, 1 drive\1st gear, 2 2nd gear, 3 3rd gear, 4 4th gear, 5 5th gear
    /// </summary>
    public int m_gearState = 0;
    /// <summary>
    /// -2 park, -1 reverse, 0 neutral, 1 drive
    /// </summary>
    public int m_ATgearState = 0;
    /// <summary>
    /// 0 no honk, 1 honk
    /// </summary>
    public int m_honkState = 0;

    public float m_Xaxis;

    public float M_Yaxis;

    /// <summary>
    /// 1 is ready to interact
    /// </summary>
    public int m_interactState = 0;
    /// <summary>
    /// 0 no menu, 1 open pie menu, currently it is being used to exit vehicle
    /// </summary>
    public int m_menuState = 0;

    [Header("TPSNode is unique and needs", order = 0)]
    [Space(-10, order = 1)]
    [Header("to be assigned", order = 2)]
    [Space(0, order = 3)]
    /// <summary>
    /// Right now we just manually assign TPS Node
    /// </summary>
    public Transform TPSNode;

    [Header("VehicleNode is null until", order = 0)]
    [Space(-10, order = 1)]
    [Header("a vehicle is entered", order = 2)]
    [Space(0, order = 3)]
    /// <summary>
    /// Vehicle Node should be assigned upon entering a vehicle.
    /// It basically means Hey this is your vehicle
    /// </summary>
    public Transform VehicleNode;
    #endregion

    #region PRIVATE VARIABLE
    private float tabGap = 0.15f;
    private float lastTapW = 0;
    private bool isDoubleTapW = false;
    private float lastTapS = 0;
    private bool isDoubleTapS = false;
    private float lastTapA = 0;
    private bool isDoubleTapA = false;
    private float lastTapD = 0;
    private bool isDoubleTapD = false;

    private VehicleMotorFunction testVehicle;
    private TPSMotorFunction testTPS;

    #endregion
    private void Awake()
    {
        //testVehicle = VehicleNode.GetComponent<VehicleMotorFunction>();
        testTPS = TPSNode.GetComponent<TPSMotorFunction>();
    }
    private void Update()
    {
        InitAllHoldDownState();
        DoKeyScan(m_mountState);
        //testVehicle.TestRunMotorFunction(this);
        //testTPS.RunMotorFunction(this);
    }

    private void DoKeyScan(int state)
    {
        if (state == 0)
        {
            keyScanTPS();
        }
        if (state == 1)
        {
            keyScanAutomobile();
        }
        keyScanMouse();
    }

    private void InitAllHoldDownState()
    {
        m_walkState = 0;
        m_strafState = 0;
        m_forwardState = 0;
        m_throttleState = 0;
        m_brakeState = 0;
        m_steerState = 0;
        m_honkState = 0;
    }
    private void keyScanTPS()
    {
        //Basically shift make each state one value higher
        int shiftState = 0;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            shiftState = 1;
        }
        //WASD
        if (Input.GetKey(KeyCode.W))
        {
            m_walkState = 1 + shiftState;
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_walkState = -1 - shiftState;
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_strafState = -1 - shiftState;
        }
        if (Input.GetKey(KeyCode.D))
        {
            m_strafState = 1 + shiftState;
        }
        //couch prone jump, beware of toggle!
        if (m_standState == 1)
        {
            m_standState = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_standState += 1;
            m_standState = Mathf.Clamp(m_standState, -2, 1);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_standState = -2;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            m_standState = -1;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            m_standState = 0;
        }
        //interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_interactState = 1;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_menuState = 1;
        }
        //forward state
        if (Input.GetKey(KeyCode.Mouse1))
        {
            m_forwardState = 1;
        }
    }

    private void keyScanAutomobile()
    {
        //motor state
        //W
        if (Input.GetKeyDown(KeyCode.W))
        {
            if ((Time.time - lastTapW) < tabGap)
            {
                isDoubleTapW = true;
            }
            lastTapW = Time.time;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (isDoubleTapW == true)
            {
                m_throttleState = 2;
            }
            else
            {
                m_throttleState = 1;
            }

        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            isDoubleTapW = false;
        }
        //S
        if (Input.GetKeyDown(KeyCode.S))
        {
            if ((Time.time - lastTapS) < tabGap)
            {
                isDoubleTapS = true;
            }
            lastTapS = Time.time;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (isDoubleTapS == true)
            {
                m_brakeState = 2;
            }
            else
            {
                m_brakeState = 1;
            }

        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            isDoubleTapS = false;
        }
        //A
        if (Input.GetKeyDown(KeyCode.A))
        {
            if ((Time.time - lastTapA) < tabGap)
            {
                isDoubleTapA = true;
            }
            lastTapA = Time.time;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (isDoubleTapA == true)
            {
                m_steerState = -2;
            }
            else
            {
                m_steerState = -1;
            }

        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            isDoubleTapA = false;
        }
        //D
        if (Input.GetKeyDown(KeyCode.D))
        {
            if ((Time.time - lastTapD) < tabGap)
            {
                isDoubleTapD = true;
            }
            lastTapD = Time.time;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (isDoubleTapD == true)
            {
                m_steerState = 2;
            }
            else
            {
                m_steerState = 1;
            }

        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            isDoubleTapA = false;
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))// In case both key are pressed
        {
            m_steerState = 0;
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) // as long as both are released
        {
            m_steerState = 0;
            isDoubleTapA = false;
            isDoubleTapD = false;
        }
        //hand brake
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (m_handbrakeState == 1)
            {
                m_handbrakeState = 0;
            }
            else if (m_handbrakeState == 0)
            {
                m_handbrakeState = 1;
            }
        }
        //gear state
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_gearState += 1;
            m_gearState = Mathf.Clamp(m_gearState, -1, 5);
            m_ATgearState += 1;
            m_ATgearState = Mathf.Clamp(m_ATgearState, -2, 1);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            m_gearState -= 1;
            m_gearState = Mathf.Clamp(m_gearState, -1, 5);
            m_ATgearState -= 1;
            m_ATgearState = Mathf.Clamp(m_ATgearState, -2, 1);
        }
        //honk
        if (Input.GetKey(KeyCode.H))
        {
            m_honkState = 1;
        }


        //interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_interactState = 1;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_menuState = 1;
        }
    }

    private void keyScanMouse()
    {
        m_Xaxis = Input.GetAxis("Mouse X");
        M_Yaxis = Input.GetAxis("Mouse Y");
    }

}

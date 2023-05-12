using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class interactSlave : MonoBehaviour
{
    public string itemType;
    [Header("door")]
    public Transform m_door;
    public Transform m_doorOpen;
    [Header("passengerseat")]
    public Transform m_vehicleNode;
    public int m_seatNumber = 1;
    [Header("Public Use Utilities")]
    public int itemState = 0;
    public float ticker = 0;

    public bool testRun = false;

    private MotionControlModule m_playerMCU;
    private Vector3 doorRot;
    private Vector3 doorPos;

    private void Update()
    {
        if (testRun == true)
        {
            testRun = false;
            DoInteract();
        }
    }
    /// <summary>
    /// Each time iMaster calls iSlave, it sends him the MCU regardless if it needs it or not
    /// </summary>
    /// <param name="mcu"></param>
    public void ReceivePlayerID(MotionControlModule mcu)
    {
        m_playerMCU = mcu;
    }
    /// <summary>
    /// Available interact type: none, door, passengerseat
    /// this is where all the weird shits happen during an interaction, so fill this baby up
    /// </summary>
    /// <param name="interacttype"></param>
    public void DoInteract()
    {
        string interacttype = itemType;
        if (interacttype == "none")
        {
            Debug.Log("No interactable item...");
        }
        if (interacttype == "door")
        {
            if (m_door)
            {
                ToggleDoor();
            }
        }
        if (interacttype == "passengerseat")
        {
            if (m_vehicleNode)
            {
                ToPassengerSeat(m_seatNumber);
            }
        }

    }

    /// <summary>
    /// side can only be 1 or -1, it determinds which side it opens
    /// </summary>
    /// <param name="side"></param>
    private void ToggleDoor()
    {
        if (itemState == 0)
        {
            itemState = 1;
            doorPos = m_door.localPosition;
            doorRot = m_door.localEulerAngles;
            m_door.localEulerAngles = m_doorOpen.localEulerAngles;
            m_door.localPosition = m_doorOpen.localPosition;
        }
        else
        {
            itemState = 0;
            m_door.localEulerAngles = doorRot;
            m_door.localPosition = doorPos;
        }
    }
    private void ToPassengerSeat(int seatNum)
    {
        VehicleMotorFunction vmf = m_vehicleNode.GetComponent<VehicleMotorFunction>();//obtain the vehicle's VMF
        PlayerControlModule pcm = m_playerMCU.T_PCM.GetComponent<PlayerControlModule>();//obtain the player's PCM through MCU
        MotionControlModule mcu = m_playerMCU;
        // ----
        // Remember to keep future boarding criteria in the VMF!!!
        // ----
        if (vmf.isSeated[seatNum] == false)//check if seat is empty first
        {
            vmf.isSeated[seatNum] = true; //tells vehicle the seat is seated
            vmf.SittingMCU[seatNum] = mcu;//tells vehicle which MCU is sitting in here
            pcm.VehicleNode = m_vehicleNode; //gives player control its vehicle node
            mcu.ObtainVehicle();//tell motion control to obtain the vehicle from player control
            pcm.m_mountState = 1; //tells player control mount state is mounted
            pcm.TPSNode.gameObject.SetActive(false);//turn off TPS node
        }
        
    }
}

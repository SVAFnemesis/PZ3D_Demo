using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Module for Player In Game Scene, Player Stays With This Module Through Different Nodes
/// </summary>
public class MotionControlModule : MonoBehaviour
{
    /// <summary>
    /// Player Control Module, the one that sits still, game object goes here
    /// This slot determines which player controls this Motion Control Unit
    /// </summary>
    public Transform T_PCM;
    /// <summary>
    /// for player this is unique, assign it by manual
    /// </summary>
    public interactMaster m_InteractMaster;


    /// <summary>
    /// The current latched-on node, use this to send command thru.
    /// It will always try to find out which node is being registered in Player Control Module.
    /// </summary>
    /// 
    private Transform m_currentNode;
    private PlayerControlModule PCM;//PCM
    private TPSMotorFunction TMF;//TMF
    private VehicleMotorFunction VMF;//VMF

    private void Awake()
    {

    }

    private void Start()//dependent loadup
    {
        PCM = T_PCM.GetComponent<PlayerControlModule>();
        TMF = PCM.TPSNode.GetComponent<TPSMotorFunction>();
    }

    private void Update()
    {
        #region MOTORFUNCTION LISTENER
        if (PCM.m_mountState == 0)//Is this MCU on foot?
        {
            //get TPSMotorfunction
            m_currentNode = PCM.TPSNode;
            TPSNodeSeating();//snap to TPS node
            TPSMotorFunctionMaster();//call TMF to perform all callers
        }
        else if (VMF && PCM.m_mountState == 1)//Is there an obtained VMF and this MCU is sitting in a car?
        {
            m_currentNode = PCM.VehicleNode;
            VehicleNodeSeating();//node snapping by querrying my seat number from VMF
            if (VMF.SittingMCU[0] == this)//check if this MCU are the driver (seat0)
            {
                VehicleMotorFunctionMaster();//then call for the main vehicle motor function, this is the one that controls vehicle movement
            }
            VehicleAuxiliaryFunctionMaster();//regardlessly you still have control to none driving function such as stereo
        }
        else if (PCM.m_mountState == 2)//Is this MCU sitting in a chair\bathtub\bed?
        {
            //sitting master goes here
        }
        #endregion
        #region INTERACTION LISTENER
        //listen to interact state and trigger an interact, also reset interact state
        if (PCM.m_interactState == 1)
        {
            PCM.m_interactState = 0;
            InteractMaster();
        }
        #endregion
        #region MENU LISTENER
        if (PCM.m_menuState == 1)
        {
            PCM.m_menuState = 0;
            ExitVehicle();
        }
        else
        {
            ClearMouse();
        }
        #endregion
    }


    #region TPS CALLING SECTION
    private void TPSNodeSeating()
    {
        gameObject.transform.position = m_currentNode.transform.position;
        gameObject.transform.rotation = m_currentNode.transform.rotation;
    }
    private void TPSMotorFunctionMaster()
    {
        TMF.RunMotorFunction(PCM);
        TMF.ObstacleInitiator(PCM);

    }
    #endregion

    #region VEHICLE CALLING SECTION
    public void ObtainVehicle()
    {
        VMF = PCM.VehicleNode.GetComponent<VehicleMotorFunction>();
    }
    private void VehicleNodeSeating()
    {
        VehicleMotorFunction currentVMF = m_currentNode.GetComponent<VehicleMotorFunction>();
        var seat = currentVMF.QuerryMySeat(this);
        gameObject.transform.position = currentVMF.seat[seat].position;
        gameObject.transform.rotation = currentVMF.seat[seat].rotation;
    }
    
    private void VehicleMotorFunctionMaster()
    {
        VehicleMotorFunction currentVMF = m_currentNode.GetComponent<VehicleMotorFunction>();
        currentVMF.RunMotorFunction(PCM);
        //communicate with VMF to drive the car
    }
    private void VehicleAuxiliaryFunctionMaster()
    {
        //communicate with VMF to mess with the car
    }
    /// <summary>
    /// Leave Vehicle, gets called without prior criteria, inside of it determins whether or not exiting is possible
    /// </summary>
    public void ExitVehicle()
    {
        //skipping criteria for safe exit
        // ----
        // Remember to keep future exiting criteria in the VMF!!!
        // ----
        if (PCM.m_mountState == 1)
        {
            var seat = VMF.QuerryMySeat(this);//get my seat number
            PCM.TPSNode.gameObject.SetActive(true);//reactivate TMF
            TMF.transform.position = VMF.exit[seat].position;//get my TMF to the exit point
            TMF.transform.rotation = VMF.exit[seat].rotation;//get my TMF to the exit point
            PCM.m_mountState = 0;//change my mountState back to on foot
            m_currentNode = PCM.TPSNode;//change the node
            VMF.isSeated[seat] = false;//mark the seat as empty again
            VMF.SittingMCU[seat] = null;//mark the sitting MCU as null
            VMF = null;//dumping the VMF node in case of accidental snapping
        }

    }
    #endregion

    #region INTERACTION CALLING SECTION
    private void InteractMaster()
    {
        m_InteractMaster.Interact();
    }
    #endregion

    #region MENU CALLING SECTION
    private void ClearMouse()
    {
        CursorLockMode m_cursor = CursorLockMode.Locked;
        Cursor.lockState = m_cursor;
        Cursor.visible = false;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(collisionTriggerDetector))]
public class interactMaster : MonoBehaviour
{
    [Header("assign Motion Control Unit", order = 0)]
    [Space(0, order = 1)]
    public MotionControlModule m_playerIdentification;
    public bool m_pressEee = false;

    private collisionTriggerDetector triggerDetector;
    private interactSlave interactSlave;
    private GameObject m_Detector;

    private void Awake()
    {
        m_Detector = gameObject;
        triggerDetector = m_Detector.GetComponent<collisionTriggerDetector>();
    }
    private void Update()
    {
        if (m_pressEee == true)
        {
            m_pressEee = false;
            Interact();
        }

        AlignWithCam();
    }
    /// <summary>
    /// it sends an interact signal the interact slave
    /// do the actual thing in interact slave script
    /// prioritize first bumped obj
    /// </summary>
    public void Interact()
    {
        if (triggerDetector.regObject[0])
        {
            interactSlave = triggerDetector.regObject[0].GetComponent<interactSlave>();
            interactSlave.ReceivePlayerID(m_playerIdentification);//MCU needs to be issued first, because interaction must likely will need it
            interactSlave.DoInteract();
        }
        else
        {
            Debug.Log("No interactable item...");
        }
        
    }
    public void InteractSpecified(int specified)
    {
        if (triggerDetector.regObject[specified])
        {
            interactSlave = triggerDetector.regObject[specified].GetComponent<interactSlave>();
            interactSlave.ReceivePlayerID(m_playerIdentification);  
            interactSlave.DoInteract();
        }
        else
        {
            Debug.Log("No interactable item...");
        }

    }

    private void AlignWithCam()
    {
        m_Detector.transform.rotation = Camera.main.transform.rotation;
    }
}

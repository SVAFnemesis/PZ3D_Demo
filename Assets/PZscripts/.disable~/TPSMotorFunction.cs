using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
/// <summary>
/// It only serves as a foot vehicle. It doesn't handle character and interactions
/// </summary>
public class TPSMotorFunction : MonoBehaviour
{
    #region PUBLIC
    [Header("The problem for this is that This TPSnode", order = 0)]
    [Space(-10, order = 1)]
    [Header("becomes player specific", order = 2)]
    [Space(0, order = 3)]
    public Transform m_PlayerControl;
    public Transform m_frontMarker;
    public Transform m_backMarker;
    public Transform m_leftMarker;
    public Transform m_rightMarker;
    
    #endregion
    #region CONFIG
    public float m_movementSpeedWalk = 1f;
    public float m_movementSpeedStraf = 1f;
    public float m_movementSpeedBack = 1f;
    #endregion

    private PlayerControlModule PlayerControlModule;
    private Transform m_nodeSelf;

    private void OnEnable()
    {
        GrabComponent();
        ClearMouse();
    }


    #region TRIVIA STUFFS
    private void GrabComponent()
    {
        m_nodeSelf = gameObject.transform;
        PlayerControlModule = m_PlayerControl.GetComponent<PlayerControlModule>();
    }
    private void ClearMouse()
    {
        CursorLockMode m_cursor;
        m_cursor = CursorLockMode.Locked;
        Cursor.lockState = m_cursor;
        Cursor.visible = false;
    }
    #endregion

    #region MOTORFUNCTIONS
    /// <summary>
    /// TMF is player unique, it does not need to specify PCM
    /// </summary>
    public void RunMotorFunction()
    {
        //at least it looks organized...
        int walkState = PlayerControlModule.m_walkState;
        int strafState = PlayerControlModule.m_strafState;
        int forwardState = PlayerControlModule.m_forwardState;
        //normalizing
        float normalizer = 1f;
        if (walkState != 0 && strafState !=0)
        {
            normalizer = 0.7f;
        }
        //look
        if (forwardState == 0)
        {
            float x = m_nodeSelf.eulerAngles.x;
            float z = m_nodeSelf.eulerAngles.z;
            float y = Camera.main.transform.eulerAngles.y;
            Quaternion targetVec = Quaternion.Euler(x, y, z);
            m_nodeSelf.rotation = Quaternion.RotateTowards(m_nodeSelf.rotation, targetVec, Time.deltaTime * 220f);
        }
        //normalized

        //walk
        if (walkState == -2)
        {
            m_nodeSelf.position = Vector3.MoveTowards(m_nodeSelf.position, m_frontMarker.position, Time.deltaTime * m_movementSpeedBack * normalizer * -2f);
        }
        else if (walkState == -1)
        {
            m_nodeSelf.position = Vector3.MoveTowards(m_nodeSelf.position, m_frontMarker.position, Time.deltaTime * m_movementSpeedBack * normalizer * -1f);
        }
        else if (walkState == 1)
        {
            m_nodeSelf.position = Vector3.MoveTowards(m_nodeSelf.position, m_frontMarker.position, Time.deltaTime * m_movementSpeedWalk * normalizer * 1f);
        }
        else if (walkState == 2)
        {
            m_nodeSelf.position = Vector3.MoveTowards(m_nodeSelf.position, m_frontMarker.position, Time.deltaTime * m_movementSpeedWalk * normalizer * 2.5f);
        }
        //straf
        if (strafState == -2)
        {
            m_nodeSelf.position = Vector3.MoveTowards(m_nodeSelf.position, m_leftMarker.position, Time.deltaTime * m_movementSpeedStraf * normalizer * 2f);
        }
        else if (strafState == -1)
        {
            m_nodeSelf.position = Vector3.MoveTowards(m_nodeSelf.position, m_leftMarker.position, Time.deltaTime * m_movementSpeedStraf * normalizer * 1f);
        }
        else if (strafState == 1)
        {
            m_nodeSelf.position = Vector3.MoveTowards(m_nodeSelf.position, m_rightMarker.position, Time.deltaTime * m_movementSpeedStraf * normalizer * 1f);
        }
        else if (strafState == 2)
        {
            m_nodeSelf.position = Vector3.MoveTowards(m_nodeSelf.position, m_rightMarker.position, Time.deltaTime * m_movementSpeedStraf * normalizer * 2f);
        }
    }
    #endregion

}

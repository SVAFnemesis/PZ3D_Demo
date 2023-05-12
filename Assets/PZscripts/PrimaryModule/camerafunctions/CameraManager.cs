using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    #region PUBLIC VAR
    public PlayerControlModule PCM;
    public MotionControlModule MCM;
    public float zoomSpeed;
    #endregion

    #region PRIVATE VAR
    private CinemachineFreeLook TpsCamIn;
    private CinemachineFreeLook TpsCamOut;
    private CinemachineFreeLook TpsCamBlend;
    private CinemachineVirtualCamera VehicleCam;
    private Transform TpsCamAux;
    private Transform VehicleCamAux;
    private RotationConstraint TpsCamAuxRotCont;
    private float zoomFacTarget;
    private float zoomFac;
    private Transform FollowOut;
    private Transform PivotOut;
    private Transform PivotOutTop;
    private Transform FollowIn;
    private Transform PivotIn;
    private Transform PivotInTop;
    private Transform FollowBlend;
    private Transform PivotBlend;
    private Transform PivotBlendTop;

    //---UTILITY FLAGS---//
    private int lastMountState;
    #endregion

    /// <summary>
    /// None Dependent Loadup
    /// </summary>
    private void Awake()
    {
        zoomFacTarget = 1f;
    }
    /// <summary>
    /// Dependent Loadup
    /// </summary>
    private void Start()
    {
        lastMountState = PCM.m_mountState;
        
        foreach (Transform child in this.transform)
        {
            string name = child.name;
            if (name == "CM FreeLookTPS in")
            {
                TpsCamIn = child.GetComponent<CinemachineFreeLook>();
            }
            if (name == "CM FreeLookTPS out")
            {
                TpsCamOut = child.GetComponent<CinemachineFreeLook>();
            }
            if (name == "CM FreeLookTPS blend")
            {
                TpsCamBlend = child.GetComponent<CinemachineFreeLook>();
            }
            if (name == "CM vcamVehicle")
            {
                VehicleCam = child.GetComponent<CinemachineVirtualCamera>();
                //Debug.Log("Cinemachine Initiation Complete!");
            }
        }

        if (MCM) //log both cam aux
        {
            Transform T_MCU = MCM.transform;
            foreach (Transform child in T_MCU)
            {
                string name = child.name;
                if (name == "TPSCamAux")
                {
                    TpsCamAux = child;
                    //Debug.Log("TpsCamAux Initiation Complete!");
                }
                if (name == "VehicleCamAux")
                {
                    VehicleCamAux = child;
                }
            }
        }
        else
        {
            Debug.LogError("CameraManager: MCU Component not found!!!");
        }

        if (TpsCamAux)
        {
            foreach (Transform child in TpsCamAux)
            {
                string name = child.name;
                if (name == "FollowOut")
                {
                    FollowOut = child;
                }
                if (name == "PivotOut")
                {
                    PivotOut = child;
                }
                if (name == "PivotOutTop")
                {
                    PivotOutTop = child;
                }
                if (name == "FollowIn")
                {
                    FollowIn = child;
                }
                if (name == "PivotIn")
                {
                    PivotIn = child;
                }
                if (name == "PivotInTop")
                {
                    PivotInTop = child;
                }
                if (name == "FollowBlend")
                {
                    FollowBlend = child;
                }
                if (name == "PivotBlend")
                {
                    PivotBlend = child;
                }
                if (name == "PivotBlendTop")
                {
                    PivotBlendTop = child;
                }
            }
        }//log all tps cam aux object
        else
        {
            Debug.LogError("CameraManager: TPSCamAux Transform not found!!!");
        }

        TpsCamAuxRotCont = TpsCamAux.GetComponent<RotationConstraint>();
        ChangeCamera();
    }
    //--- END OF VOID START ---//


    private void Update()
    {
        if (lastMountState != PCM.m_mountState)//if mountState changed from PCM, sync and then do ChangeCamera
        {
            ChangeCamera();
            lastMountState = PCM.m_mountState;
        }
        if (PCM.m_mountState == 0)
        {
            TPSCameraZoom();
        }    
    }

    private void ChangeCamera()
    {
        if (PCM.m_mountState == 0) //mountState = 0, TPS mode
        {
            TpsCamBlend.Priority = 10;
            TpsCamAuxRotCont.constraintActive = true;
        }
        else
        {
            TpsCamAuxRotCont.constraintActive = false;
            TpsCamBlend.Priority = 0;
        }
    }
    private void TPSCameraZoom()
    {
        if (PCM)
        {
            zoomFacTarget -= PCM.m_midScroll * 0.5f;
            zoomFacTarget = Mathf.Clamp(zoomFacTarget, 0f, 1f);
            zoomFac = Mathf.Lerp(zoomFac, zoomFacTarget, Time.deltaTime * zoomSpeed);
        }
        if (TpsCamBlend)
        {
            for (int i = 0; i < 3; i++)
            {
                TpsCamBlend.m_Orbits[i].m_Height = Mathf.Lerp(TpsCamIn.m_Orbits[i].m_Height, TpsCamOut.m_Orbits[i].m_Height, zoomFac);
                TpsCamBlend.m_Orbits[i].m_Radius = Mathf.Lerp(TpsCamIn.m_Orbits[i].m_Radius, TpsCamOut.m_Orbits[i].m_Radius, zoomFac);
            }
        }
        if (TpsCamAux)
        {
            FollowBlend.position = Vector3.Lerp(FollowIn.position, FollowOut.position, zoomFac);
            PivotBlend.position = Vector3.Lerp(PivotIn.position, PivotOut.position, zoomFac);
            PivotBlendTop.position = Vector3.Lerp(PivotInTop.position, PivotOutTop.position, zoomFac);
        }
    }
}

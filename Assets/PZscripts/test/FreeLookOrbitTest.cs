using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteAlways]
public class FreeLookOrbitTest : MonoBehaviour
{
    public CinemachineFreeLook cine;
    public float h;
    public float r;
    public int testIndex;

    public bool RUN = false;

    private void Update()
    {
        if (RUN == true)
        {
            RUN = false;
            GetOrbit();
        }
    }

    private void GetOrbit()
    {
        h = cine.m_Orbits[testIndex].m_Height;
        r = cine.m_Orbits[testIndex].m_Radius;
    }
}

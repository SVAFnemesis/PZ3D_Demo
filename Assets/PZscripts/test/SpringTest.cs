using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringTest : MonoBehaviour
{
    public Transform obj1;
    public Transform obj2;
    public float springForce = 1f;
    public float decayFactor = 0.01f;
    public bool RUN = false;

    private Vector3 m_speed;

    private void Update()
    {
        Operator();
    }

    private void Operator()
    {
        if (RUN == true)
        {
            SpringLerp();
        }
    }

    private void SpringLerp()
    {
        Vector3 distInVector = obj1.position - obj2.position;

        //Operator off switch
        if (m_speed.magnitude < 0.0002f && distInVector.magnitude < 0.0002f)
        {
            RUN = false;
            HardReset();
            return;
        }  

        //Spring Lerp Section
        Vector3 speedChange = (obj1.position - obj2.position) * springForce * Time.deltaTime;
        Vector3 speedDecay = decayFactor * m_speed * -1f;
        m_speed = m_speed + speedChange + speedDecay;

        //ASSIGN
        obj2.position = obj2.position + m_speed;
    }

    private void HardReset()
    {
        m_speed = new Vector3(0f, 0f, 0f);
    }
}

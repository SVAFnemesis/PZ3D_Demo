using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionTriggerDetector : MonoBehaviour
{ 
    public Transform m_theVoid;
    public int m_MaxDetection = 10;
    public Transform[] objectObj;
    public Transform[] regObject;

    public string[] ignoreList;

    private Transform Detector;

    private void Awake()
    {
        Detector = gameObject.transform;
        objectObj = new Transform[m_MaxDetection];
        regObject = new Transform[m_MaxDetection];
        for (int i = 0; i < m_MaxDetection; i++)
        {
            objectObj[i] = m_theVoid;
        }

        //IgnoreList();
    }

    private void Update()
    {
        Prioritize();
    }

    /// <summary>
    /// Trigger Detection Section, auto fired upon collision, obj is registered at first 'untouchable' slot
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {

        Registry(other.transform);
    }
    /// <summary>
    /// Trigger Purging Section, auto fired upon exiting, then obj is purged from array
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {

        Unregistry(other.transform);
    }

    private void Registry(Transform trans)
    {
        interactSlave iSlave = trans.GetComponent<interactSlave>();
        if (iSlave)
        {
            for (int i = 0; i < m_MaxDetection; i++)
            {
                if (objectObj[i] == m_theVoid)
                {
                    objectObj[i] = trans;
                    break;
                }
            }
        } 
    }

    private void Unregistry(Transform trans)
    {
        for (int i = 0; i < m_MaxDetection; i++)
        {
            if (objectObj[i] == trans)
            {
                objectObj[i] = m_theVoid;
                break;
            }
        }
    }

    private void Prioritize()
    {
        var dist = new float[m_MaxDetection];
        for (int i = 0; i < m_MaxDetection; i++)
        {
            dist[i] = (Detector.position - objectObj[i].position).magnitude;
        }

        List< DetectorList > dlist = new List<DetectorList>(m_MaxDetection);
        for (int i = 0; i < m_MaxDetection; i++)
        {
            dlist.Add(new DetectorList(dist[i], objectObj[i]));
        }

        dlist.Sort((a, b) =>
        {
            if (a.distance != b.distance)
            {
                return a.distance < b.distance ? -1 : 1;
            }
            return 0;
        });

        for (int i = 0; i < m_MaxDetection; i++)
        {
            regObject[i] = dlist[i].obj;
        }
    }

    private void IgnoreList()
    {

    }
}

public class DetectorList
{
    public DetectorList(float distance, Transform obj)
    {
        this.distance = distance;
        this.obj = obj;
    }
    public float distance;
    public Transform obj;
}

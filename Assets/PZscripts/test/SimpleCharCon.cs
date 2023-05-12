using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharCon : MonoBehaviour
{
    public CharacterController charCon;
    public DetectWall wallCollie;
    public float ticker;
    public bool readyToClimb = false;
    public Vector3 snapPos;
    public bool isSnapped;
    public bool isClimbing;


    private void Awake()
    {
        charCon = GetComponent<CharacterController>();
    }
    private void Update()
    {
        KeyScan();
        ClimbSequence();
    }

    private void KeyScan()
    {
        if (Input.GetKey(KeyCode.W))
        {
            charCon.SimpleMove(gameObject.transform.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            charCon.SimpleMove(gameObject.transform.forward*-1f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Quaternion targetQuat = Quaternion.LookRotation(gameObject.transform.right * -1f, gameObject.transform.up);
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetQuat, Time.deltaTime * 90f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Quaternion targetQuat = Quaternion.LookRotation(gameObject.transform.right * 1f, gameObject.transform.up);
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetQuat, Time.deltaTime * 90f);
        }
        if (Input.GetKeyDown(KeyCode.Space) && wallCollie.loggedWall)
        {
            readyToClimb = true;
        }

    }

    private void ClimbSequence()
    {
        if (readyToClimb == false && isSnapped == true)
        {
            wallScalingSnap();
        }
        if (readyToClimb == true && isSnapped == false)
        {
            charCon.enabled = false;
            wallScalingSnap();
            isSnapped = true;
            readyToClimb = false;
        }
        if (readyToClimb == true && isSnapped == true)
        {
            wallScalingClimb();
            isSnapped = false;
            isClimbing = true;
        }
        if (readyToClimb == false && isClimbing == true)
        {
            wallScalingClimb();
        }


    }


    private void wallScalingSnap()
    {

        float snapY = wallCollie.wallY;
        Transform currentTrans = gameObject.transform;
        Vector3 currentPos = currentTrans.position;
        snapPos = new Vector3(currentPos.x, snapY, currentPos.z);
        gameObject.transform.position = snapPos;
        isSnapped = true;

    }
    private void wallScalingClimb()
    {
        float climbToY = wallCollie.wallY + 1f;
        float climbStep = Time.deltaTime * 1f;
        Vector3 currentPos = gameObject.transform.position;
        if (currentPos.y < climbToY)
        {
            gameObject.transform.position = new Vector3(currentPos.x, currentPos.y + climbStep, currentPos.z);
            isClimbing = true;
            isSnapped = false;
        }
        else
        {
            isSnapped = false;
            isClimbing = false;
            charCon.enabled = true;
        } 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPhysicsTest : MonoBehaviour
{
    public WheelCollider[] wheel;
    public Transform[] wheelMesh;
    public Rigidbody chassis;
    [Range(-100, 1000)]
    public float torque;
    [Range(0, 1800)]
    public float brake;
    [Range(-15, 15)]
    public float angle;

    private string debugger1;
    private string debugger2;
    private string debugger3;
    private string debugger4;
    private string debugger5;
    private void Update()
    {
        keyscan();
        drivetrain();
    }

    private void keyscan()
    {
        if (Input.GetKey(KeyCode.W))
        {
            torque = 400;
        }
        else
        {
            torque = 0;
        }
        if (Input.GetKey(KeyCode.A))
        {
            angle = -15;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            angle = 15;
        }
        else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            angle = 0;
        }
        if (Input.GetKey(KeyCode.S))
        {
            brake = 1000;
        }
        else
        {
            brake = 0;
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 CWpos;
            Quaternion CWquat;
            wheel[i].GetWorldPose(out CWpos, out CWquat );
            wheelMesh[i].position = CWpos;
            wheelMesh[i].rotation = CWquat;
        }
    }


    private void drivetrain()
    {
        wheel[0].motorTorque = torque;
        wheel[1].motorTorque = torque;
        wheel[0].brakeTorque = brake;
        wheel[1].brakeTorque = brake;
        wheel[0].steerAngle = angle;
        wheel[1].steerAngle = angle;
        if (brake > 0)
        {
            wheel[0].motorTorque =0f;
            wheel[1].motorTorque = 0f;
        }

        float wheelspeed;
        wheelspeed = (wheel[0].rpm + wheel[1].rpm + wheel[2].rpm + wheel[3].rpm) / 4f;
        debugger1 = chassis.velocity.magnitude.ToString("0000.0000");
        debugger4 = (chassis.velocity.magnitude * 2.236f).ToString("0000.0000");
        debugger2 = torque.ToString("0000.0000");
        debugger3 = wheelspeed.ToString("0000.0000");
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 350, 250, 300),
            "Rigid Body Velocity:" + debugger1 +
            "\nMPH:" + debugger4 +
            "\n final toruqe:" + debugger2 +
            "\n average wheel speed:" + debugger3);
    }
}

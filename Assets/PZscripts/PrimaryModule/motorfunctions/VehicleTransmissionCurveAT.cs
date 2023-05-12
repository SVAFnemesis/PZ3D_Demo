using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
/// <summary>
/// ATgear[], firstSecondThreshold, secondThirdThreshold, thirdFourthThreshold
/// </summary>
public class VehicleTransmissionCurveAT : MonoBehaviour
{
    public AnimationCurve[] ATgear;

    public float firstSecondThreshold;
    public float secondThirdThreshold;
    public float thirdFourthThreshold;
}

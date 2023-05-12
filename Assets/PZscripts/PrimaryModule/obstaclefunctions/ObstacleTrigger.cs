using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTrigger : MonoBehaviour
{
    /// <summary>
    /// "scale" "vault"
    /// </summary>
    public enum Type
    {
        step,
        vault,
        scale
    };
    public Type m_Type;
    public string type;

    private void Awake()
    {
        SetType();
    }

    private void SetType()
    {
        if (m_Type == Type.scale)
        {
            type = "scale";
        }
        if (m_Type == Type.vault)
        {
            type = "vault";
        }
        if (m_Type == Type.step)
        {
            type = "step";
        }
    }
}

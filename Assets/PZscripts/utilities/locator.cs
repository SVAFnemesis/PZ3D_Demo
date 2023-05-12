using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class locator : MonoBehaviour
{
    [Range(0.02f, 5f)]
    public float size = 1f;
    [Range(0, 7)]
    public int setColor = 0;

    private Color[] color;
    private void Update()
    {
        color = new Color[8];
        color[0] = Color.green;
        color[1] = Color.red;
        color[2] = Color.blue;
        color[3] = Color.yellow;
        color[4] = Color.cyan;
        color[5] = Color.white;
        color[6] = Color.black;
        color[7] = Color.magenta;

        DrawCross();
    }

    private void DrawCross()
    {
        Transform self = gameObject.transform;
        Debug.DrawLine(self.right * -1 * size + self.position, self.right * size + self.position, color[setColor]);
        Debug.DrawLine(self.up * -1 * size + self.position, self.up * size + self.position, color[setColor]);
        Debug.DrawLine(self.forward * -1 * size + self.position, self.forward * size + self.position, color[setColor]);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
    public LineRenderer circle;
    public float radius;

    // Start is called before the first frame update
    void Start()
    {
        // DrawShape(256, radius);
    }

    // Update is called once per frame
    void Update()
    {
        circle.startWidth = PlayerCommands.inst.MainCamera.transform.position.y / 500f;
        circle.endWidth = PlayerCommands.inst.MainCamera.transform.position.y / 500f;
    }

    public void DrawShape(int prec, float radius)
    {
        circle = GetComponent<LineRenderer>();
        this.radius = radius;

        float red = 1 - radius / 100;
        red = red * red;
        float green = 1 - 2 * (0.5f - Mathf.Abs(radius / 100));
        float blue = radius / 100;

        circle.startColor = new Color(red, green, blue, 1.0f);
        circle.endColor  =  new Color(red, green, blue, 1.0f);

        circle.positionCount = prec;

        for (int i = 0; i < prec; i++)
        {
            float currPoint = (float)i / prec;
            float angle = currPoint * 6.2830f;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            circle.SetPosition(i, new Vector3(x, 0, y));
        }
    }
}

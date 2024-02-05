using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    private Transform parent;
    private float GalacticTime;
    public float revolutionDistance;
    public float revolutionSpeed;
    public float revolutionOffset;
    public float rotationSpeed;
    public float rotationOffset;
    public float axialTilt;
    public bool tidal;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        GalacticTime = GameTimeTest.instance.time;
        SetRevolution();
        SetRotation();
    }

    void SetRevolution()
    {
        float x = parent.position.x + revolutionDistance * Mathf.Cos(GalacticTime * revolutionSpeed / 58.12f + revolutionOffset);
        float y = parent.position.z + revolutionDistance * Mathf.Sin(GalacticTime * revolutionSpeed / 58.12f + revolutionOffset);
        this.transform.position = new Vector3(x, parent.position.y, y);
    }

    void SetRotation()
    {
        // Quaternion rot = new Quaternion(0f, 0f, 0f, 0f);
        // rot.eulerAngles = new Vector3(axialTilt, GalacticTime * rotationSpeed * 36, 0f); // 10* slower, for visual appeal
        // transform.rotation = rot;
        if (tidal)
        {
            transform.LookAt(parent);
        }
        else
        {
            transform.localEulerAngles = new Vector3(axialTilt, GalacticTime * rotationSpeed * 36 + rotationOffset, 0f); // 10* slower, for visual appeal
        }
    }
}

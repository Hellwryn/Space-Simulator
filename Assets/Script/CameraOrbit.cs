using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    private Transform parent;
    private float GalacticTime;
    public float revolutionDistance;
    public float revolutionSpeed;
    public float revolutionOffset;

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
    }

    void SetRevolution()
    {
        float x = parent.position.x + revolutionDistance * Mathf.Cos(GalacticTime * revolutionSpeed / 58.12f + revolutionOffset);
        float y = parent.position.z + revolutionDistance * Mathf.Sin(GalacticTime * revolutionSpeed / 58.12f + revolutionOffset);
        this.transform.position = new Vector3(x, parent.position.y, y);
    }
}

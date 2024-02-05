using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeTest : MonoBehaviour
{
    public static GameTimeTest instance;
    public float time;
    public bool pause;

    [Range(0.0f, 5.0f)]
    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause)
        {
            time += Time.deltaTime * speed;
        }
    }
}

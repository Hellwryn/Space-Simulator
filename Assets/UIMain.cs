using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    public UIPlanetaryView planetaryView;

    // Start is called before the first frame update
    void Start()
    {
        UIPlanetaryView.instance = planetaryView;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

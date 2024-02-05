using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlanetaryView : MonoBehaviour
{
    public static UIPlanetaryView instance;

    public UIPlanetarySurface surface;


    void Start()
    {
        
    }

    public void SelectPlanet(PlanetGeneration pg)
    {
        surface.SelectPlanet(pg);
    }
}

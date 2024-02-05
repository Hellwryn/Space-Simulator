using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetGeneration : MonoBehaviour, IPointerDownHandler
{
    // public Shader planet;
    // public Shader planetHeight;

    public Material m_Material;

    public bool Generate;

    public float planetSeed;
    public Vector2 seedOffset;
    public float width;
    public float borderChaos; // Range : 0.05-0.95
    public float northPole; // Range : 1-2
    public float southPole; // Range : 1-2
    public float OceanLimit; // Range : 0-1
    public Color OceanColor;
    public float CoastSize; // Range : 0-0.2
    public Color CoastalWaterColor;
    public float BeachSize; // Range : 0-0.1
    public Color BeachColor;
    public Color FirstBiomeColor;
    public Color SecondBiomeColor;
    public float BiomeSize; // Range : 1-20
    public float BiomeWeight; // Range : 0.1-10
    public float BiomesLimit; // Range : 0-1
    public Color MountainColor;
    public float NightLevel; // Range : 0-10
    public bool UI;

    void Start()
    {
        Renderer r_Renderer = GetComponent<Renderer>();
        m_Material = new Material(r_Renderer.sharedMaterial);
        r_Renderer.sharedMaterial = m_Material;
        Actualize();
    }

    void Update()
    {
        if (Generate)
        {
            Actualize();
            Generate = false;
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Debug.Log("Test");
        UIPlanetaryView.instance.gameObject.SetActive(true);
        UIPlanetaryView.instance.SelectPlanet(this);
    }  

    public void Actualize()
    {
        //TEST VALUES
        /*
        seedOffset = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
        width = 1;
        borderChaos = 0.2f;
        northPole = 1.2f;
        southPole = 1.2f;
        OceanLimit = 0.4f;
        OceanColor = new Color(0.289f, 0.475f, 0.801f, 1.0f);
        CoastSize = 0.05f;
        CoastalWaterColor = new Color(0.207f, 0.786f, 0.896f, 1.0f);
        BeachSize = 0.02f;
        BeachColor = new Color(0.85f, 0.82f, 0.52f, 1.0f);
        FirstBiomeColor = new Color(0.39f, 0.75f, 0.29f, 1.0f);
        SecondBiomeColor = new Color(0.77f, 0.44f, 0.21f, 1.0f);
        BiomeSize = 4f;
        BiomeWeight = 0.5f;
        BiomesLimit = 0.65f;
        MountainColor = new Color(0.95f, 0.95f, 0.95f, 1.0f);
        NightLevel = 0;
        */
        //TEST VALUES

        SetShaderValues();
    }

    private void SetShaderValues()
    {
        m_Material.SetVector("SeedOffset", seedOffset);
        m_Material.SetFloat("Width", width);
        m_Material.SetFloat("BorderChaos", borderChaos);
        m_Material.SetFloat("NorthPole", northPole);
        m_Material.SetFloat("SouthPole", southPole);
        m_Material.SetFloat("OceanLimit", OceanLimit);
        m_Material.SetColor("OceanColor", OceanColor);
        m_Material.SetFloat("CoastSize", CoastSize);
        m_Material.SetColor("CoastalWaterColor", CoastalWaterColor);
        m_Material.SetFloat("BeachSize", BeachSize);
        m_Material.SetColor("BeachColor", BeachColor);
        m_Material.SetColor("FirstBiomeColor", FirstBiomeColor);
        m_Material.SetColor("SecondBiomeColor", SecondBiomeColor);
        m_Material.SetFloat("BiomeSize", BiomeSize);
        m_Material.SetFloat("BiomeWeight", BiomeWeight);
        m_Material.SetFloat("BiomesLimit", BiomesLimit);
        m_Material.SetColor("MountainColor", MountainColor);
        m_Material.SetFloat("NightLevel", NightLevel);
    }
}

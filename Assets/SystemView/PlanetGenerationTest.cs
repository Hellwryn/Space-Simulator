using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerationTest : MonoBehaviour
{
    public GameObject planet;
    [Range(0.1f, 20.0f)]
    public float Size;
    [Range(0.01f, 1.0f)]
    public float RotationSpeed;
    [Range(0.00f, 1.0f)]
    public float Spectral;
    [Range(0.001f, 12.0f)]
    public float energyReceived;
    [Range(0.00f, 1.0f)]
    public float AtmosphericDensity;
    [Range(0.00f, 1.0f)]
    public float HumidityLevel;
    [Range(0.00f, 1.0f)]
    public float TelluricDensity;

    public float Energy;
    public float avgTemperature;
    public float watertemp;
    public float waterLevel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnValidate()
    {
        if (SystemMap.inst == null)
            return;

        GameObject clouds = planet.transform.GetChild(0).gameObject;
        GameObject atmo = planet.transform.GetChild(1).gameObject;

        // Set the planet orbit
        Orbit planetOrbit = planet.GetComponent<Orbit>();
        planetOrbit.rotationSpeed = RotationSpeed;
        planet.transform.localScale = new Vector3(Size, Size, Size);

        // Get the planet material
        Renderer r_Renderer = planet.GetComponent<Renderer>();
        Material m_Material = new Material(r_Renderer.sharedMaterial);
        r_Renderer.sharedMaterial = m_Material;

        // Get the clouds material
        Renderer r_clouds = clouds.GetComponent<Renderer>();
        Material m_clouds = new Material(r_clouds.sharedMaterial);
        r_clouds.sharedMaterial = m_clouds;

        // Get the atmo material
        Renderer r_atmo = atmo.GetComponent<Renderer>();
        Material m_atmo = new Material(r_atmo.sharedMaterial);
        r_atmo.sharedMaterial = m_atmo;

        Energy = Mathf.Pow(energyReceived, 0.27f)/2f;

        // Choose planet colors
        avgTemperature = (energyReceived + TelluricDensity * TelluricDensity / 2f) * AtmosphericDensity * 2;

        watertemp = 1 / avgTemperature;
        if (watertemp > 1)
            watertemp = 1;
        waterLevel = HumidityLevel;

        Color waterColor = SystemMap.inst.waterGradient.Evaluate(Mathf.Pow(avgTemperature, 0.21f)/2f);
        Color soilColor;
        Color folliageColor;
        if (avgTemperature > 1f)
        {
            soilColor = SystemMap.inst.soilGradient.Evaluate(TelluricDensity);
            folliageColor = SystemMap.inst.folliageGradient.Evaluate(Spectral);
        }
        else
        {
            if (avgTemperature > 0.5f)
            {
                soilColor = SystemMap.inst.soilGradient.Evaluate(TelluricDensity) * (avgTemperature - 0.25f) * 1.33f + new Color(0.7f, 0.8f, 0.9f, 1.0f) * (1 - (avgTemperature - 0.25f) * 1.33f);
                folliageColor = SystemMap.inst.folliageGradient.Evaluate(Spectral) * (avgTemperature - 0.25f) * 1.33f + new Color(0.7f, 0.8f, 0.9f, 1.0f) * (1 - (avgTemperature - 0.25f) * 1.33f);
            }
            else
            {
                soilColor = new Color(0.7f, 0.8f, 0.9f, 1.0f) * avgTemperature * 2f + new Color(0.9f, 0.95f, 1.0f, 1.0f) * (1 - avgTemperature * 2f);
                folliageColor = new Color(0.7f, 0.8f, 0.9f, 1.0f) * avgTemperature * 2f + new Color(0.9f, 0.95f, 1.0f, 1.0f) * (1 - avgTemperature * 2f);
            }
        }

        // Set Planet Colors
        m_Material.SetColor("OceanColor", waterColor);
        m_Material.SetColor("CoastalWaterColor", waterColor * 0.8f + soilColor * 0.2f);
        m_Material.SetColor("BeachColor", soilColor);
        m_Material.SetColor("EquatorColor", soilColor);
        m_Material.SetColor("FirstBiomeColor", folliageColor);
        m_Material.SetColor("SecondBiomeColor", soilColor);
        m_Material.SetColor("MountainColor", Color.white);

        if (avgTemperature < 0.5f)
        {
            m_Material.SetTexture("OceanTexture", SystemMap.inst.textures[3]);
            m_Material.SetTexture("FirstBiomeTexture", SystemMap.inst.textures[5]);
        }
        else
        {
            m_Material.SetTexture("OceanTexture", SystemMap.inst.textures[4]);
            m_Material.SetTexture("FirstBiomeTexture", SystemMap.inst.textures[4]);
        }

        // Calc planet data
        //float OceanLimit = data.planetDatas[i].HumidityLevel * 0.6f;
        float OceanLimit = waterLevel * 0.6f;

        //float BiomeWeight = Mathf.Pow(data.planetDatas[i].HumidityLevel * 2f, 3.5f);
        float BiomeWeight = 1f / (waterLevel * 8f);

        // Set Planet Data
        m_Material.SetFloat("OceanLimit", OceanLimit);
        m_Material.SetFloat("CoastSize", 0.12f);
        m_Material.SetFloat("BeachSize", 0.08f);
        m_Material.SetFloat("BiomesLimit", 0.72f);


        m_Material.SetFloat("BiomeWeight", BiomeWeight);

        // Set Clouds Color and Data
        m_clouds.SetFloat("Fill", Mathf.Pow(AtmosphericDensity, 0.5f) * HumidityLevel * 0.7f);
        m_clouds.SetVector("Speed", new Vector2(RotationSpeed / 5f + 0.03f, 0f));

        // Set Atmo Color and Data
        m_atmo.SetColor("AtmoColor", new Color(TelluricDensity, HumidityLevel, AtmosphericDensity, 1f));
        m_atmo.SetColor("AtmoColorSpread", folliageColor);
        m_atmo.SetFloat("AtmoIntensity", AtmosphericDensity * 2f);
    }
}
